using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbDataFinder
{
    public class Table
    {
        public String TableName { get; set; }
        public List<Column> Columns { get; set; }

        public Table()
        {
            Columns = new List<Column>();
        }
    }
}
