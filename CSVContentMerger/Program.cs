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
        static void Main(string[] a_args)
        {
            while (true)
            {
                CommandLineHandler commandLineHandler = new CommandLineHandler();
                commandLineHandler.HandleProgramStart(a_args);

                CsvFileIO csvFileIO = new CsvFileIO(commandLineHandler);
                Merger merger = new Merger(commandLineHandler);

                if (merger.TryGetLinesToWrite(out var linesToWrite))
                    csvFileIO.AppendLinesToMergeFile(linesToWrite);
                else
                    continue;

                commandLineHandler.HandleProgramEnd(); 
            }
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
