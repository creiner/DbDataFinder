using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbDataFinder
{
    public class DataFinder
    {
        private IDatabase database;

        public DataFinder( IDatabase database ) 
        {
            this.database = database;
        }

        public IEnumerable<Result> Find( String search )
        {
            var dbInfo = database.GetDatabaseInformation();
            var result = new List<Result>();
            foreach ( var table in dbInfo )
            {
                result.AddRange( database.ScanTable( search, table ) );
            }
            return result;
        }
    }
}
