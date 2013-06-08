using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DbDataFinder
{
    class Program
    {
        static void Main( string[] args )
        {
            Console.WriteLine("Welcome to DbDataFinder");
            IDatabase db = new MSSQLDatabase();
            do
            {
                Console.Write( "Please specify the database server: " );
                db.DbHost = Console.ReadLine();
            } while ( String.IsNullOrWhiteSpace( db.DbHost ) );

            do
            {
                Console.Write( "Please specify the database username: " );
                db.DbUser = Console.ReadLine();
            } while ( String.IsNullOrWhiteSpace( db.DbUser ) );
            do
            {
                Console.Write( "Please specify the database password: " );
                db.DbPass = Console.ReadLine();
            } while ( String.IsNullOrWhiteSpace( db.DbPass ) );
            do
            {
                Console.Write( "Please specify the database name: " );
                db.DbName = Console.ReadLine();
            } while ( String.IsNullOrWhiteSpace( db.DbName ) );


            Console.Write( "Please specify a search term: " );
            String search = Console.ReadLine();

            var dataFinder = new DataFinder( db );
            do
            {
                var result = dataFinder.Find( search );

                if ( result.Count() == 0 )
                {
                    Console.WriteLine( "No results found!" );
                }
                else 
                {
                    Console.WriteLine( String.Format( "Found {0} results.", result.Count() ) );
                    foreach ( var hit in result )
                    {
                        Console.WriteLine( String.Format( "{0,-20} - {1,-10}", hit.TableName, hit.ColumnName ) );
                    }
                }
                Console.WriteLine( "Please specify a search term (0 for exit): " );
                search = Console.ReadLine();
            } while ( search != "0" );
        }
    }
}
