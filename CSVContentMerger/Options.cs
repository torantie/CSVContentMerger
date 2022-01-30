using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSVContentMerger
{
    class Options
    {
        [Option('n', "mfn", HelpText = "Name of csv file that should contain the merged files.")]
        public string MergeFileName { get; set; }

        [Option('p', "mfp", HelpText = "Directory path where the csv data files are located.")]
        public string MergeFileDirectoryPath { get; set; }

        [Option('s', "st", HelpText = "Separator used in the csv data files that should be merged.")]
        public string SeparatorType { get; set; }

    }
}
