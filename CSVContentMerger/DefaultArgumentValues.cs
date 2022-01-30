using System;
using System.Collections.Generic;
using System.Text;

namespace CSVContentMerger
{
    public class DefaultArgumentValues
    {
        public const SeparatorType DefaultSeparatorType = SeparatorType.NewLineSpace;

        public const SeparatorType MergeFileSeparatorType = SeparatorType.SemiColon;

        public const string DefaultMergeFileDirectoryPath = @".\";

        public const string DefaultMergeFileName = "All_Data.csv";
    }
}
