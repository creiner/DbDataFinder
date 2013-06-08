using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace DbDataFinder
{
    public class MSSQLDatabase : IDatabase
    {
        public String DbHost { get; set; }
        public String DbName { get; set; }
        public String DbUser { get; set; }
        public String DbPass { get; set; }

        private String connectionString = String.Empty;
        private SqlConnection sqlConnection;

        public MSSQLDatabase() 
        {
        }

        private void buildConnection()
        {
            if ( String.IsNullOrEmpty( connectionString ) )
            {
                connectionString = String.Format( "Data Source={0};Initial Catalog={1};User Id={2};Password={3};MultipleActiveResultSets=true;",
                                                  DbHost,
                                                  DbName,
                                                  DbUser,
                                                  DbPass );
                sqlConnection = new SqlConnection( connectionString );
            }
        }

        public List<Table> GetDatabaseInformation()
        {
            buildConnection();
            var sql = "SELECT t.TABLE_NAME, C.COLUMN_NAME, C.DATA_TYPE FROM INFORMATION_SCHEMA.TABLES T LEFT JOIN INFORMATION_SCHEMA.COLUMNS C ON T.TABLE_NAME = C.TABLE_NAME WHERE  TABLE_TYPE = 'BASE TABLE' ORDER BY T.TABLE_NAME";
            var sqlCmd = new SqlCommand( sql, sqlConnection );

            var returnList = new List<Table>();
            try
            {
                sqlConnection.Open();
                var reader = sqlCmd.ExecuteReader();

                Table currentTable = null;
                while ( reader.Read() )
                {
                    var tableName = reader["TABLE_NAME"].ToString();
                    if ( currentTable == null || currentTable.TableName != tableName )
                    {
                        currentTable = new Table();
                        currentTable.TableName = tableName;
                        returnList.Add( currentTable );
                    }

                    currentTable.Columns.Add( new Column
                    {
                        ColumName = reader["COLUMN_NAME"].ToString(),
                        Type = ParseColumnType( reader["DATA_TYPE"].ToString() )
                    } );
                }
            }
            catch ( Exception ex )
            {
                Console.WriteLine( "Error while getting the database information. " );
                Console.WriteLine( String.Format( "Exception: {0}", ex.Message ) );
            }
            finally
            {
                sqlConnection.Close();
            }
            return returnList;
        }

        private ColumnType ParseColumnType( String type )
        {
            switch( type )
            {
                case "nchar":
                case "char":
                case "ntext":
                case "nvarchar":
                case "varchar":
                    return ColumnType.Text;
                case "bigint":
                case "float":
                case "smallint":
                case "decimal":
                case "int":
                    return ColumnType.Number;
                case "datetime":
                    return ColumnType.Datetime;
                case "bit":
                    return ColumnType.Bool;
                case "image":
                case "varbinary":
                    return ColumnType.Blob;
            }
            return ColumnType.Text;
        }

        private String GetOperator( ColumnType type )
        {
            switch ( type )
            {
                case ColumnType.Text:
                    return "LIKE";
            }
            return "=";
        }

        private Object GenerateParameter( ColumnType type, String value )
        {
            try
            {
                switch ( type )
                {
                    case ColumnType.Text:
                        return String.Format( "%{0}%", value );
                    case ColumnType.Number:
                        return double.Parse( value );
                    case ColumnType.Datetime:
                        return DateTime.Parse( value );
                    case ColumnType.Bool:
                        return bool.Parse( value );
                }
            }
            catch { }
            return null;
        }

        public List<Result> ScanTable( String search, Table table )
        {
            buildConnection();
            var resultList = new List<Result>();
            foreach ( var column in table.Columns )
            {
                if ( column.Type == ColumnType.Blob )
                {
                    continue;
                }
                var param = GenerateParameter( column.Type, search );
                if ( param == null ) {
                    continue;
                }
                var sql = String.Format( "SELECT COUNT(*) FROM {0} WHERE {1} {2} @val", table.TableName, column.ColumName, GetOperator( column.Type ) );
                var sqlCmd = new SqlCommand( sql, sqlConnection );
                sqlCmd.Parameters.AddWithValue( "@val", param );

                try
                {
                    sqlConnection.Open();
                    var result = (int) sqlCmd.ExecuteScalar();
                    if (result > 0)
                    {
                        resultList.Add( new Result
                        {
                            TableName = table.TableName,
                            ColumnName = column.ColumName,
                        } );
                    }
                }
                catch ( Exception ex )
                {
                    Console.WriteLine( ex.Message );
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
#if DEBUG
            if ( resultList.Count == 0 )
            {
                Console.WriteLine( String.Format( "Didn't found value in {0}", table.TableName ) );
            }
            else
            {
                Console.WriteLine( String.Format( "Found value in {0}", table.TableName ) );
            }
#endif
            return resultList;
        }

    }
}
