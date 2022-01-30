using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace CSVContentMerger
{
    public class Merger
    {
        private CommandLineHandler m_commandLineHandler;

        private CsvFileIO m_csvFileIO;

        public Merger(CommandLineHandler a_commandLineHandler)
        {
            m_commandLineHandler = a_commandLineHandler;
            m_csvFileIO = new CsvFileIO(a_commandLineHandler);
        }

        public bool TryGetLinesToWrite(out List<string[]> a_linesToWrite)
        {
            a_linesToWrite = default;

            try
            {
                var csvMergeFileColumnMap = CreateMergeFileColumnMap();
                var csvColumnMaps = MergeCSVFileContent(csvMergeFileColumnMap);
                a_linesToWrite = CreateLinesToWrite(csvMergeFileColumnMap, csvColumnMaps);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return false;
        }

        private List<string[]> CreateLinesToWrite(ColumnMap a_csvMergeFileColumnMap, ColumnMapCollection a_csvColumnMaps)
        {
            List<string[]> linesToWriteCollection = new List<string[]>();
            var offset = 0;

            // add the column names as a line to write if there are none in the merge file
            if (a_csvMergeFileColumnMap.Count == 0 && a_csvColumnMaps.Count != 0)
            {
                int i = 0;
                foreach (var csvColumnMap in a_csvColumnMaps)
                {
                    foreach (var (columnKey, columnInfo) in csvColumnMap)
                    {
                        if (!a_csvMergeFileColumnMap.ContainsKey(columnKey))
                        {
                            a_csvMergeFileColumnMap.Add(columnKey, new ColumnInfo(i, columnInfo.ColumnName, columnInfo.Values));
                            i++;
                        }
                    }
                }
                var columnNames = a_csvMergeFileColumnMap.Values.Select((columnInfo) => columnInfo.ColumnName).ToArray();
                linesToWriteCollection.Add(columnNames);
                offset = 1;
            }

            foreach (var csvColumnMap in a_csvColumnMaps)
            {
                Console.WriteLine("File: {0}", csvColumnMap.FileName);
                for (int i = 0; i < csvColumnMap.MaxLines; i++)
                {
                    var items = new string[a_csvMergeFileColumnMap.Count];
                    linesToWriteCollection.Add(items);
                }
                foreach (var mapKvp in a_csvMergeFileColumnMap)
                {
                    if (csvColumnMap.TryGetValue(mapKvp.Key, out var columnMapEntry))
                    {
                        for (int i = 0; i < columnMapEntry.Values.Count; i++)
                        {
                            linesToWriteCollection[offset + i][mapKvp.Value.ColumnIndex] = columnMapEntry.Values[i];
                        }
                    }
                    else
                    {
                        Console.WriteLine("Key not found: {0}", mapKvp.Key);
                    }
                }
                offset = linesToWriteCollection.Count;
            }

            return linesToWriteCollection;
        }

        private ColumnMapCollection MergeCSVFileContent(ColumnMap a_csvMergeFileColumnMap)
        {
            var csvColumnMaps = new ColumnMapCollection();
            FileInfo[] csvFiles = m_csvFileIO.GetCsvFiles();
            foreach (var csvFile in csvFiles)
            {
                var lines = m_csvFileIO.GetFileLines(csvFile);
                var fileName = csvFile.Name;

                if (fileName.Equals(m_commandLineHandler.MergeFileName, StringComparison.InvariantCultureIgnoreCase))
                    continue;

                ColumnMap csvFileColumnMap;
                if (m_commandLineHandler.SeparatorType.Equals(SeparatorType.NewLineSpace))
                {
                    csvFileColumnMap = AddLineSeparatedCSVFile(a_csvMergeFileColumnMap, lines, fileName);
                }
                else
                {
                    csvFileColumnMap = AddCharacterSeparatedCSVFile(m_commandLineHandler.SeparatorType, lines, fileName);
                }

                if (csvFileColumnMap.Count != 0)
                    csvColumnMaps.Add(csvFileColumnMap);
            }

            return csvColumnMaps;
        }

        private ColumnMap AddLineSeparatedCSVFile(ColumnMap a_csvMergeFileColumnMap, IEnumerable<string> a_lines, string a_fileName)
        {
            var maxNumberOfLines = 0;
            var csvFileColumnMap = new ColumnMap(a_fileName, maxNumberOfLines);
            List<string> columnCache = default;
            int i = 0;
            foreach (var line in a_lines)
            {
                var lineNoWhiteSpace = line.RemoveWhitespace();
                if (a_csvMergeFileColumnMap.TryGetValue(lineNoWhiteSpace, out var column))
                {
                    columnCache = new List<string>();
                    // TODO test index i
                    csvFileColumnMap[lineNoWhiteSpace] = new ColumnInfo(i, line, columnCache);

                    if (csvFileColumnMap.FileName == "")
                        csvFileColumnMap.FileName = line;

                    i++;
                }
                // "column name" for line separated file begins
                //if (columnCache == default && !csvFileColumnMap.Item3.ContainsKey(lineNoWhiteSpace))
                //{
                //    columnCache = new List<string>();
                //    csvFileColumnMap.Item3[lineNoWhiteSpace] = columnCache;
                //}
                // "column name" values end
                else if (line == "" || string.IsNullOrWhiteSpace(line))
                {
                    columnCache = default;
                }
                //add values to the "column name"
                else if (columnCache != default)
                {
                    columnCache.Add(line);
                    maxNumberOfLines = Math.Max(maxNumberOfLines, columnCache.Count);
                    csvFileColumnMap.MaxLines = maxNumberOfLines;
                }
            }

            return csvFileColumnMap;
        }

        private ColumnMap AddCharacterSeparatedCSVFile(SeparatorType a_separatorType, IEnumerable<string> a_lines, string a_fileName)
        {
            var maxNumberOfLines = 0;
            var csvFileColumnMap = new ColumnMap(a_fileName, maxNumberOfLines);
            var columnCache = new Dictionary<int, string>();
            var lines = a_lines.ToList();
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                string[] columnValues = line.Split(a_separatorType.GetSeperator());

                //first line with column names
                if (i == 0)
                {
                    for (int j = 0; j < columnValues.Length; j++)
                    {
                        var lineNoWhiteSpace = columnValues[j].RemoveWhitespace();
                        csvFileColumnMap[lineNoWhiteSpace] = new ColumnInfo(j, columnValues[j], new List<string>());
                        columnCache[j] = lineNoWhiteSpace;

                        if (csvFileColumnMap.FileName == "")
                            csvFileColumnMap.FileName = columnValues[j];
                    }
                }
                else
                {
                    for (int j = 0; j < columnValues.Length; j++)
                    {
                        var columnValue = columnValues[j];
                        if (string.IsNullOrWhiteSpace(columnValue) || columnValue == "")
                            continue;

                        csvFileColumnMap[columnCache[j]].Values.Add(columnValue);
                        maxNumberOfLines = Math.Max(maxNumberOfLines, csvFileColumnMap[columnCache[j]].Values.Count);
                    }
                }
            }
            csvFileColumnMap.MaxLines = maxNumberOfLines;

            return csvFileColumnMap;
        }

        /// <summary>
        /// Get the columns of the first line of the csv merge file (which contains the names for the tables columns).
        /// </summary>
        /// <returns></returns>
        private ColumnMap CreateMergeFileColumnMap()
        {
            var columnMap = new ColumnMap("", 0);

            if (!m_csvFileIO.TryCreateMergeFileReader(out var mergeFileReader))
                return columnMap;

            using (mergeFileReader)
            {
                var firstLine = mergeFileReader.ReadLine();
                if (string.IsNullOrWhiteSpace(firstLine) || firstLine == "")
                {
                    Console.WriteLine("No column names found in merge file.");
                    return columnMap;
                }

                var columTexts = firstLine.Split(';');
                for (int i = 0; i < columTexts.Length; i++)
                {
                    var columnText = columTexts[i];
                    columnMap[columnText.RemoveWhitespace()] = new ColumnInfo(i, columnText, new List<string>());
                }
            }

            return columnMap;
        }
    }
}
