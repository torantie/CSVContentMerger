using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace CSVContentMerger
{
    public class CsvFileIO
    {
        private CommandLineHandler m_commandLineHandler;

        public CsvFileIO(CommandLineHandler a_commandLineHandler)
        {
            m_commandLineHandler = a_commandLineHandler;
        }

        public void AppendLinesToMergeFile(List<string[]> a_linesCollection)
        {
            var text = File.ReadAllText(m_commandLineHandler.MergeFilePath);
            var textCount = text.Count();
            using var streamWriter = File.AppendText(m_commandLineHandler.MergeFilePath);

            // in case there is no new line after the first line of column names
            if (textCount > 2 && text[^2] != '\r' && text[^1] != '\n')
                streamWriter.Write(Environment.NewLine);

            foreach (var line in a_linesCollection)
            {
                var csvString = string.Join(SeparatorTypeExtension.GetSeperator(DefaultArgumentValues.MergeFileSeparatorType), line);
                streamWriter.WriteLine(csvString);
            }
        }

        public bool TryCreateMergeFileReader(out StreamReader a_streamReader)
        {
            a_streamReader = default;
            if (!File.Exists(m_commandLineHandler.MergeFilePath))
            {
                Console.WriteLine(string.Format("File {0} does not exist.", m_commandLineHandler.MergeFileName));
                return false;
            }
            else
            {
                Console.WriteLine(string.Format("File {0} does exist.", m_commandLineHandler.MergeFileName));
                a_streamReader = new StreamReader(File.OpenRead(m_commandLineHandler.MergeFilePath));
                return true;
            }
        }

        public FileInfo[] GetCsvFiles()
        {
            var directoryInfo = new DirectoryInfo(m_commandLineHandler.MergeFileDirectoryPath);
            var csvFiles = directoryInfo.GetFiles("*.csv", SearchOption.AllDirectories);
            return csvFiles;
        }


        public IEnumerable<string> GetFileLines(FileInfo a_csvFile)
        {
            return File.ReadLines(a_csvFile.FullName);
        }

        private StreamReader CreateMergeFile()
        {
            var fileStream = File.Create(m_commandLineHandler.MergeFilePath);
            return new StreamReader(fileStream);
        }
    }
}
