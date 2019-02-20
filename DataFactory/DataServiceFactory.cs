using System;
using System.Data;
using System.Data.SqlClient;

namespace DataFactory
{
    public class DataServiceFactory : MarshalByRefObject
    {
        public DataTable GetNotesByOwnerPlant(string userkey, char isShowForAll)
        {
            DataAccessMgr daMgr = new DataAccessMgr();
            SqlParameter[] parameters = new SqlParameter[]{
                daMgr.BuildParam("@USERKEY", SqlDbType.NVarChar, 36, userkey),
                daMgr.BuildParam("@SHOWFORALL", SqlDbType.Char, isShowForAll),
                daMgr.BuildOutParam(AppConstants.CursorParam, SqlDbType.NVarChar, null)
            };

            if (!daMgr.LoadDataTable("uspGetNotesByOwnerPlant", parameters, out DataTable dt))
                return null;

            return dt;
        }
    }
}
