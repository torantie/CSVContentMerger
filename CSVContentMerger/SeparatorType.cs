using System;
using System.Collections.Generic;
using System.Text;

namespace CSVContentMerger
{
    public enum SeparatorType
    {
        NewLineSpace, Comma, SemiColon
    }

    public static class SeparatorTypeExtension
    {
        public static string GetSeperator(this SeparatorType a_separatorType)
        {
            return a_separatorType switch
            {
                SeparatorType.NewLineSpace => Environment.NewLine,
                SeparatorType.Comma => ",",
                SeparatorType.SemiColon => ";",
                _ => "",
            };
        }
    }
}
