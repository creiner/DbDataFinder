using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbDataFinder
{
    public interface IDatabase
    {
        String DbHost { get; set; }
        String DbName { get; set; }
        String DbUser { get; set; }
        String DbPass { get; set; }

        List<Table> GetDatabaseInformation();
        List<Result> ScanTable( String search, Table table );
    }
}
