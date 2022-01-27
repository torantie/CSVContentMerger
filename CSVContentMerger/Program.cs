using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace CSVContentMerger
{
    class Program
    {
        private static string s_mergeFileName = DefaultArgumentValues.DefaultMergeFileName;

        private static string s_mergeFileDirectoryPath = DefaultArgumentValues.DefaultMergeFileDirectoryPath;

        private static string s_mergeFilePath = "";

        private static SeparatorType s_separatorType = DefaultArgumentValues.DefaultSeparatorType;

        static void Main(string[] args)
        {
            PrintStartMessages();
            ParseArguments(args);

            var csvMergeFileColumnMap = CreateMergeFileColumnMap();
            var csvColumnMaps = MergeCSVFileContent(csvMergeFileColumnMap);
            var linesCollection = CreateLinesToWrite(csvMergeFileColumnMap, csvColumnMaps);
            AppendLinesToMergeFile(linesCollection);
            Console.WriteLine("-----------");
            Console.WriteLine("Finished...");
            Console.WriteLine("-----------");
        }

        private static void PrintStartMessages()
        {
            Console.WriteLine(string.Format("Default csv merge file name <{0}>", DefaultArgumentValues.DefaultMergeFileName));
            Console.WriteLine(string.Format("Default merge file directory path <{0}>", DefaultArgumentValues.DefaultMergeFileDirectoryPath));
            var separatorTypes = Enum.GetValues(typeof(SeparatorType)).Cast<SeparatorType>();
            var separatorTypeStringCollection = separatorTypes.Select(separatorType => string.Format("<{0}>", separatorType.ToString()));
            var separatorTypesString = string.Join(',', separatorTypeStringCollection);

            Console.WriteLine(string.Format("Default separator type <{0}>. Option: {1}",
                DefaultArgumentValues.DefaultSeparatorType,
                separatorTypesString));
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Type --help to see the command names.");
        }

        [DllImport("shell32.dll", SetLastError = true)]
        static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);

        /// <summary>
        /// https://stackoverflow.com/questions/298830/split-string-containing-command-line-parameters-into-string-in-c-sharp
        /// </summary>
        /// <param name="a_commandLine"></param>
        /// <returns></returns>
        public static string[] CommandLineToArgs(string a_commandLine)
        {
            var argv = CommandLineToArgvW(a_commandLine, out int argc);
            if (argv == IntPtr.Zero)
                throw new System.ComponentModel.Win32Exception();
            try
            {
                var args = new string[argc];
                for (var i = 0; i < args.Length; i++)
                {
                    var p = Marshal.ReadIntPtr(argv, i * IntPtr.Size);
                    args[i] = Marshal.PtrToStringUni(p);
                }

                return args;
            }
            finally
            {
                Marshal.FreeHGlobal(argv);
            }
        }

        private static void ParseArguments(string[] a_args)
        {
            while (true)
            {
                var args = a_args;
                if (args.Length == 0)
                {
                    var command = Console.ReadLine();
                    args = CommandLineToArgs(command);
                }

                var parseResult = Parser.Default.ParseArguments<Options>(args).WithParsed(o => HandleParsedArguments(o));
                
                if (parseResult.Tag != ParserResultType.NotParsed)
                    break;
            }
        }

        private static void HandleParsedArguments(Options a_o)
        {
            if (!string.IsNullOrEmpty(a_o.MergeFileName))
            {
                s_mergeFileName = a_o.MergeFileName;
            }
            if (!string.IsNullOrEmpty(a_o.MergeFileDirectoryPath))
            {
                s_mergeFileDirectoryPath = a_o.MergeFileDirectoryPath;
            }
            if (!string.IsNullOrEmpty(a_o.SeparatorType))
            {
                if (!Enum.TryParse(a_o.SeparatorType, true, out s_separatorType))
                    s_separatorType = SeparatorType.NewLineSpace;
            }
            s_mergeFilePath = Path.Combine(s_mergeFileDirectoryPath, s_mergeFileName);
        }

        private static bool TryCreateMergeFileReader(out StreamReader a_streamReader)
        {
            if (!File.Exists(s_mergeFilePath))
            {
                Console.WriteLine(string.Format("File {0} does not exist.", s_mergeFileName));

                if (!TryCreateMergeFile(s_mergeFileDirectoryPath, s_mergeFilePath, out a_streamReader))
                {
                    Console.WriteLine("File was not created. Exiting program ...");
                    Thread.Sleep(3000);
                    return false;
                }
            }
            else
            {
                Console.WriteLine(string.Format("File {0} does exist.", s_mergeFileName));
                a_streamReader = new StreamReader(File.OpenRead(s_mergeFilePath));
            }

            return true;
        }

        private static void AppendLinesToMergeFile(List<string[]> a_linesCollection)
        {
            using var streamWriter = File.AppendText(s_mergeFilePath);

            foreach (var line in a_linesCollection)
            {
                var csvString = string.Join(";", line);
                streamWriter.WriteLine(csvString);
            }
        }

        private static List<string[]> CreateLinesToWrite(Dictionary<string, (int index, List<string> entries)> a_csvMergeFileColumnMap, List<(string fileName, int maxLines, Dictionary<string, List<string>> columnMap)> a_csvColumnMaps)
        {
            List<string[]> linesToWriteCollection = new List<string[]>();
            var offset = 0;

            // add the column names as a line to write if there are none in the merge file
            if (a_csvMergeFileColumnMap.Count == 0)
            {
                int i = 0;
                foreach (var (fileName, maxLines, columnMap) in a_csvColumnMaps)
                {
                    foreach (var (columnName, values) in columnMap)
                    {
                        if (!a_csvMergeFileColumnMap.ContainsKey(columnName))
                        {
                            a_csvMergeFileColumnMap.Add(columnName, (i, values));
                            i++;
                        }
                    }
                }
                var columnNames = a_csvMergeFileColumnMap.Keys.ToArray();
                linesToWriteCollection.Add(columnNames);
                offset = 1;
            }

            foreach (var (fileName, maxLines, columnMap) in a_csvColumnMaps)
            {
                Console.WriteLine("File: {0}", fileName);
                for (int i = 0; i < maxLines; i++)
                {
                    var items = new string[a_csvMergeFileColumnMap.Count];
                    linesToWriteCollection.Add(items);
                }
                foreach (var mapKvp in a_csvMergeFileColumnMap)
                {
                    if (columnMap.TryGetValue(mapKvp.Key, out var columnMapEntry))
                    {
                        for (int i = 0; i < columnMapEntry.Count; i++)
                        {
                            linesToWriteCollection[offset + i][mapKvp.Value.index] = columnMapEntry[i];
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

        private static List<(string fileName, int maxLines, Dictionary<string, List<string>> columnMap)> MergeCSVFileContent(Dictionary<string, (int index, List<string> entries)> csvMergeFileColumnMap)
        {
            var csvColumnMaps = new List<(string fileName, int maxLines, Dictionary<string, List<string>> columnMap)>();
            var directoryInfo = new DirectoryInfo(s_mergeFileDirectoryPath);
            var csvFiles = directoryInfo.GetFiles("*.csv", SearchOption.AllDirectories);
            foreach (var csvFile in csvFiles)
            {
                if (csvFile.Name == s_mergeFileName)
                    continue;

                (string fileName, int maxNumberOfLines, Dictionary<string, List<string>> mappedValues) csvFileColumnMap;
                if (s_separatorType.Equals(SeparatorType.NewLineSpace))
                {
                    csvFileColumnMap = AddLineSeperatedCSVFile(csvMergeFileColumnMap, csvColumnMaps, csvFile);
                }
                else
                {
                    csvFileColumnMap = AddCharacterSeperatedCSVFile(csvMergeFileColumnMap, csvColumnMaps, csvFile, s_separatorType);
                }
                csvColumnMaps.Add(csvFileColumnMap);
            }

            return csvColumnMaps;
        }

        private static (string fileName, int maxNumberOfLines, Dictionary<string, List<string>> columnMap) AddLineSeperatedCSVFile(Dictionary<string, (int index, List<string> entries)> a_csvMergeFileColumnMap, List<(string fileName, int maxLines, Dictionary<string, List<string>> columnMap)> a_csvColumnMaps, FileInfo a_csvFile)
        {
            var maxNumberOfLines = 0;
            var csvFileColumnMap = (a_csvFile.FullName, maxNumberOfLines, new Dictionary<string, List<string>>());
            List<string> columnCache = default;
            foreach (var line in File.ReadLines(a_csvFile.FullName))
            {
                var lineNoWhiteSpace = line.RemoveWhitespace();
                if (a_csvMergeFileColumnMap.TryGetValue(lineNoWhiteSpace, out var column))
                {
                    columnCache = new List<string>();
                    csvFileColumnMap.Item3[lineNoWhiteSpace] = columnCache;
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
                    csvFileColumnMap.maxNumberOfLines = maxNumberOfLines;
                }
            }

            return csvFileColumnMap;
        }

        private static (string fileName, int maxNumberOfLines, Dictionary<string, List<string>> columnMap) AddCharacterSeperatedCSVFile(Dictionary<string, (int index, List<string> entries)> a_csvMergeFileColumnMap, List<(string fileName, int maxLines, Dictionary<string, List<string>> columnMap)> a_csvColumnMaps, FileInfo a_csvFile, SeparatorType a_separatorType)
        {
            var maxNumberOfLines = 0;
            var csvFileColumnMap = (a_csvFile.FullName, maxNumberOfLines, new Dictionary<string, List<string>>());
            var columnCache = new Dictionary<int, string>();
            var lines = File.ReadLines(a_csvFile.FullName).ToList();
            for (int i = 0; i < lines.Count(); i++)
            {
                var line = lines[i];
                string[] columnValues = line.Split(a_separatorType.GetSeperator());

                //first line with column names
                if (i == 0)
                {
                    for (int j = 0; j < columnValues.Length; j++)
                    {
                        var lineNoWhiteSpace = columnValues[j].RemoveWhitespace();
                        csvFileColumnMap.Item3[lineNoWhiteSpace] = new List<string>();
                        columnCache[j] = lineNoWhiteSpace;
                    }
                }
                else
                {
                    for (int j = 0; j < columnValues.Length; j++)
                    {
                        var columnValue = columnValues[j];
                        if (string.IsNullOrWhiteSpace(columnValue) || columnValue == "")
                            continue;

                        csvFileColumnMap.Item3[columnCache[j]].Add(columnValue);
                        maxNumberOfLines = Math.Max(maxNumberOfLines, csvFileColumnMap.Item3[columnCache[j]].Count);
                    }
                }
            }
            csvFileColumnMap.maxNumberOfLines = maxNumberOfLines;

            return csvFileColumnMap;
        }

        private static bool TryCreateMergeFile(string a_path, string a_mergeFilePath, out StreamReader a_streamReader)
        {
            while (true)
            {
                Console.WriteLine(string.Format("Create file at path {0}? [y] | [n]", a_path));
                var answer = Console.ReadLine();
                if (answer.Equals("y"))
                {
                    var fileStream = File.Create(a_mergeFilePath);
                    a_streamReader = new StreamReader(fileStream);
                    return true;
                }
                else if (answer.Equals("n"))
                {
                    a_streamReader = null;
                    return false;
                } 
            }
        }

        /// <summary>
        /// Get the columns of the first line of the csv merge file (which contains the names for the tables columns).
        /// </summary>
        /// <param name="a_mergeFilePath"></param>
        /// <returns></returns>
        private static Dictionary<string, (int index, List<string> entries)> CreateMergeFileColumnMap()
        {
            var csvMergeFileColumnMap = new Dictionary<string, (int index, List<string> entries)>();

            if (!TryCreateMergeFileReader(out var mergeFileReader))
                return csvMergeFileColumnMap;

            using (mergeFileReader)
            {
                try
                {
                    var firstLine = mergeFileReader.ReadLine();
                    if (string.IsNullOrWhiteSpace(firstLine) || firstLine == "")
                    {
                        Console.WriteLine("No column names found in merge file.");
                        return csvMergeFileColumnMap;
                    }

                    var columTexts = firstLine.Split(';');

                    for (int i = 0; i < columTexts.Length; i++)
                    {
                        var columnText = columTexts[i];
                        csvMergeFileColumnMap[columnText.RemoveWhitespace()] = (i, new List<string>());
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            return csvMergeFileColumnMap;
        }

    }

    public static class Extensions
    {
        public static string RemoveWhitespace(this string a_input)
        {
            return new string(a_input.ToCharArray().Where(c => !char.IsWhiteSpace(c)).ToArray());
        }
    }
}
