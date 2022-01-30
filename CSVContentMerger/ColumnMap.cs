using System;
using System.Collections.Generic;
using System.Text;

namespace CSVContentMerger
{
    public class ColumnMapCollection : List<ColumnMap>
    {

    }

    public class ColumnMap : Dictionary<string, ColumnInfo>
    {
        public ColumnMap(string a_fileName, int a_maxLines)
        {
            FileName = a_fileName;
            MaxLines = a_maxLines;
        }

        public string FileName { get; set; }

        public int MaxLines { get; set; }
    }

    public class ColumnInfo
    {
        public ColumnInfo(int a_columnIndex, string a_columnName, List<string> a_values)
        {
            ColumnIndex = a_columnIndex;
            ColumnName = a_columnName;
            Values = a_values;
        }

        public int ColumnIndex { get; set; }

        public string ColumnName { get; set; }

        public List<string> Values { get; set; }
    }
}
