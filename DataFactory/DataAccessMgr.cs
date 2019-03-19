using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;

namespace DataFactory
{
    public class DataAccessMgr : MarshalByRefObject
    {
        public SqlParameter BuildParam(string name, SqlDbType type, int? size, object paramValue)
        {
            SqlParameter param;

            param = ((size != null)) ? new SqlParameter(name, type, (int)size) : new SqlParameter(name, type);
            param.Direction = ParameterDirection.Input;

            if (paramValue == null)
            {
                param.Value = DBNull.Value;
            }
            else
            {
                if (paramValue is DateTime val)
                    if (val == DateTime.MinValue)
                        paramValue = DBNull.Value;

                param.Value = paramValue;
            }

            return param;
        }
        public SqlParameter BuildParam(string name, SqlDbType type, object paramValue)
        {
            return BuildParam(name, type, null, paramValue);
        }

        public SqlParameter BuildOutParam(string name, SqlDbType type, object paramValue)
        {
            return BuildOutParam(name, type, null, paramValue);
        }
        public SqlParameter BuildOutParam(string name, SqlDbType type, int? size, object paramValue)
        {
            SqlParameter param;

            param = size != null ? new SqlParameter(name, type, (int)size) : new SqlParameter(name, type);
            param.Direction = ParameterDirection.Output;

            if (paramValue == null)
            {
                param.Value = DBNull.Value;
            }
            else
            {
                if (paramValue is DateTime val)
                    if (val == DateTime.MinValue)
                        paramValue = DBNull.Value;

                param.Value = paramValue;
            }

            return param;
        }

        private OracleCommand LoadDataCommandType(string cmdText, SqlParameter[] parameters,
            CommandType commandType = CommandType.Text)
        {
            OracleCommand cmd = new OracleCommand();
            try
            {
                AddParams(cmd, parameters);
                cmd.CommandType = commandType;
                cmd.CommandTimeout = DataFactory.SqlCommandTimeout;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return cmd;
        }


        // return datatable
        public bool LoadDataTable(string procedureName, SqlParameter[] parameters, out DataTable dt, out List<int> outParas)
        {
            dt = new DataTable();
            outParas = new List<int>();
            DataSet dataSet = new DataSet();
            OracleConnection conn = new OracleConnection(DataFactory.ConnectionString);
            try
            {
                using (OracleCommand cmd = new OracleCommand(procedureName))
                {
                    AddParams(cmd, parameters, false);
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 30;

                    if (conn.State == ConnectionState.Closed || conn.State == ConnectionState.Broken)
                    {
                        conn.Open();
                    }

                    using (DbDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    //handle output paras
                    for (short i = 0; i < cmd.Parameters.Count; i++)
                    {
                        IDataParameter param = (IDataParameter)cmd.Parameters[i];

                        if (param.Direction == ParameterDirection.InputOutput ||
                            param.Direction == ParameterDirection.Output)
                        {
                            if (int.TryParse(param.Value.ToString(), out int outPara))
                                outParas.Add(outPara);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }

            return true;

        }
        public bool LoadDataTable(string procedureName, SqlParameter[] parameters, out DataTable dt)
        {
            dt = new DataTable();
            DataSet dataSet = new DataSet();
            OracleConnection conn = new OracleConnection(DataFactory.ConnectionString);
            try
            {
                using (OracleCommand cmd = new OracleCommand(procedureName))
                {
                    AddParams(cmd, parameters);
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 30;

                    if (conn.State == ConnectionState.Closed || conn.State == ConnectionState.Broken)
                    {
                        conn.Open();
                    }

                    using (DbDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }

            return true;
        }
        public DataTable LoadDataTable(OracleConnection oracleConnection, CommandType commandType, string text, SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            try
            {

                using (OracleCommand cmd = new OracleCommand(text))
                {
                    AddParams(cmd, parameters);
                    cmd.Connection = oracleConnection;
                    cmd.CommandType = commandType;
                    cmd.CommandTimeout = DataFactory.SqlCommandTimeout;

                    if (oracleConnection.State == ConnectionState.Closed || oracleConnection.State == ConnectionState.Broken)
                    {
                        oracleConnection.Open();
                    }

                    using (DbDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }

            }
            catch (Exception ex)
            {
                return null;

                throw ex;
            }

            return dt;
        }
        public bool LoadMulDataTable(string procedureName, SqlParameter[] parameters, out DataSet ds)
        {
            DataSet dataSetMain = new DataSet();
            ds = new DataSet();
            OracleConnection conn = new OracleConnection(DataFactory.ConnectionString);
            try
            {

                using (OracleCommand cmd = new OracleCommand(procedureName))
                {
                    AddParams(cmd, parameters);
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = DataFactory.SqlCommandTimeout;
                    conn.Open();

                    using (DbDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        da.Fill(dataSetMain);
                    }
                }

                DataRow dr = dataSetMain.Tables[0].Rows[0];
                for (int i = 1; i < dataSetMain.Tables[0].Columns.Count; i++)
                {
                    DataTable dataTable = LoadDataTable(conn, CommandType.Text, dr[i].ToString(), null);
                    dataTable.TableName = "Table" + (i - 1);
                    ds.Tables.Add(dataTable);
                }


            }
            catch (Exception ex)
            {
                return false;

                throw ex;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }

            return true;
        }

        public bool LoadDataSet(string procedureName, SqlParameter[] parameters, out DataSet ds)
        {
            ds = new DataSet();
            DataSet dataSet = new DataSet();
            try
            {
                using (OracleConnection conn = new OracleConnection(DataFactory.ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand(procedureName))
                    {
                        AddParams(cmd, parameters);
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = DataFactory.SqlCommandTimeout;

                        conn.OpenAsync();

                        using (DbDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(ds);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return false;

                throw ex;
            }

            return true;
        }

        private void AddParams(IDbCommand cmd, SqlParameter[] parameters, bool processSQLInject = true)
        {
            if (processSQLInject)
                ProcessSqlInjection(cmd.CommandText, parameters);

            // Add each of the parameters specified
            if (parameters == null)
                return;

            for (short i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterName == AppConstants.CursorParam)
                {
                    cmd.Parameters.Add(new OracleParameter(AppConstants.CursorParam, OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.Output
                    });
                }
                else
                {
                    cmd.Parameters.Add(FromSqlParamToOraParam(parameters[i]));
                }
            }

        }

        public static IDataParameter FromSqlParamToOraParam(SqlParameter param)
        {
            OracleDbType oraDbType = OracleDbType.Varchar2;
            object oraValue = param.Value;
            string paramName = param.ParameterName;

            if (param.Direction == ParameterDirection.Input || param.Direction == ParameterDirection.InputOutput)
            {
                paramName = paramName.Insert(1, "IN_");
            }

            switch (param.SqlDbType)
            {
                case SqlDbType.BigInt:
                    oraDbType = OracleDbType.Int64;
                    break;
                case SqlDbType.Binary:
                    oraDbType = OracleDbType.Blob;
                    break;
                case SqlDbType.Bit:
                    oraDbType = OracleDbType.Char;
                    if (param.Value != DBNull.Value)
                    {
                        oraValue = Convert.ToBoolean(param.Value) ? "T" : "F";
                    }
                    break;
                case SqlDbType.Char:
                    oraDbType = OracleDbType.Char;
                    break;
                case SqlDbType.Date:
                    oraDbType = OracleDbType.Date;
                    break;
                case SqlDbType.DateTime:
                case SqlDbType.DateTime2:
                case SqlDbType.DateTimeOffset:
                    oraDbType = OracleDbType.TimeStamp;
                    break;
                case SqlDbType.Decimal:
                    oraDbType = OracleDbType.Decimal;
                    break;
                case SqlDbType.Float:
                    oraDbType = OracleDbType.Single;
                    break;
                case SqlDbType.Image:
                    oraDbType = OracleDbType.Blob;
                    break;
                case SqlDbType.Int:
                    oraDbType = OracleDbType.Int32;
                    break;
                case SqlDbType.Money:
                    oraDbType = OracleDbType.Decimal;
                    break;
                case SqlDbType.NChar:
                    oraDbType = OracleDbType.NChar;
                    break;
                case SqlDbType.NText:
                    oraDbType = OracleDbType.NClob;
                    break;
                case SqlDbType.NVarChar:
                    oraDbType = OracleDbType.NVarchar2;
                    break;
                case SqlDbType.Real:
                    oraDbType = OracleDbType.Double;
                    break;
                case SqlDbType.SmallDateTime:
                    oraDbType = OracleDbType.Date;
                    break;
                case SqlDbType.SmallInt:
                    oraDbType = OracleDbType.Int16;
                    break;
                case SqlDbType.SmallMoney:
                    oraDbType = OracleDbType.Decimal;
                    break;
                case SqlDbType.Structured:
                    break;
                case SqlDbType.Text:
                    oraDbType = OracleDbType.Clob;
                    break;
                case SqlDbType.Time:
                case SqlDbType.Timestamp:
                    oraDbType = OracleDbType.TimeStamp;
                    break;
                case SqlDbType.TinyInt:
                    oraDbType = OracleDbType.Byte;
                    break;
                case SqlDbType.Udt:
                    break;
                case SqlDbType.UniqueIdentifier:
                    oraDbType = OracleDbType.Varchar2;
                    break;
                case SqlDbType.VarBinary:
                    oraDbType = OracleDbType.Blob;
                    break;
                case SqlDbType.VarChar:
                    oraDbType = OracleDbType.Varchar2;
                    break;
                case SqlDbType.Variant:
                    oraDbType = OracleDbType.Clob;
                    break;
                case SqlDbType.Xml:
                    oraDbType = OracleDbType.XmlType;
                    break;
                default:
                    break;
            }

            IDataParameter oraParam = new OracleParameter(paramName, oraDbType, param.Size)
            {
                Value = oraValue,
                Direction = param.Direction
            };

            return oraParam;
        }

        private void ProcessSqlInjection(string cmdText, SqlParameter[] parameters)
        {
            if (!(cmdText.ToUpper().StartsWith("USPUPDATE") || cmdText.ToUpper().StartsWith("USPSAVE") 
                || cmdText.ToUpper().StartsWith("USPINSERT")))
            {
                string strSqlInjectionContent = GetSqlInjectionStr(parameters);
                if (!string.IsNullOrEmpty(strSqlInjectionContent))
                {
                    throw new ArgumentException(strSqlInjectionContent);
                }
            }
        }
        private string GetSqlInjectionStr(SqlParameter[] parameters)
        {
            StringBuilder sbSqlInjectionContent = new StringBuilder();
            var keyWord = AppConstants.SqlInjectionKeyword;
            if (parameters != null)
            {
                for (short i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].ParameterName == AppConstants.CursorParam || null == parameters[i].Value)
                        continue;

                    if (Regex.IsMatch(parameters[i].Value.ToString(), keyWord, RegexOptions.IgnoreCase))
                    {
                        string word = Regex.Match(parameters[i].Value.ToString(), keyWord, RegexOptions.IgnoreCase).Value;
                        //deal with gridview column head filter, like user input:drop table, the alert message shows:'drop table
                        if (word[0] == '\'')
                            word = word.Substring(1);
                        if (word[word.Length - 1] == '\'')
                            word = word.Substring(0, word.Length - 1);

                        sbSqlInjectionContent.Append(word);
                        sbSqlInjectionContent.AppendLine();
                    }
                }
            }
            return sbSqlInjectionContent.ToString();
        }

    }
}