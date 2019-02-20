using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.IO;

namespace DataFactory
{
    public class DataFactory
    {

        #region properties
        public static string ConnectionString { get; set; }
        public static int SqlCommandTimeout { get; set; }
        #endregion


        public DataFactory(string connectionString, int? sqlCommandTimeout)
        {
            ConnectionString = connectionString ?? "";
            SqlCommandTimeout = sqlCommandTimeout ?? 600;
        }

        public static IDbConnection CreateConnection(string connectionString)
        {
            return new OracleConnection(connectionString);
        }




    }
}
