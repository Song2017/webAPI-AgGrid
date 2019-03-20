using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DataFactory
{
    public class DataServiceFactory : MarshalByRefObject
    {
        public DataTable GetNotes(string userkey, char isShowForAll)
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

        public DataTable GetGridField(string userkey, string gridDataSource, int tabIndex = 0)
        {
            DataAccessMgr daMgr = new DataAccessMgr();
            SqlParameter[] parameters = new SqlParameter[]{
                daMgr.BuildParam("@USERKEY", SqlDbType.NVarChar, 36, userkey),
                daMgr.BuildParam("@SHOWFORALL", SqlDbType.NVarChar, 36, gridDataSource),
                daMgr.BuildParam("@IN_QueryType", SqlDbType.NVarChar, 36, "SELECT"),
                daMgr.BuildParam("@IN_SHOWCDMEAS", SqlDbType.Char, 'F'),
                daMgr.BuildParam("@IN_SHOWCDMFG", SqlDbType.Char,  'F'),
                daMgr.BuildParam("@IN_SHOWCDSHIPPED", SqlDbType.Char,  'F'),
                daMgr.BuildParam("@IN_SHOWCOST", SqlDbType.Char,  'F'),
                daMgr.BuildParam("@IN_IncludeCusFields", SqlDbType.Char,  'F'),
                daMgr.BuildOutParam(AppConstants.CursorParam, SqlDbType.NVarChar, null)
            };

            if (!daMgr.LoadMulDataTable("uspGetGridField", parameters, out DataSet ds))
                return null;

            return ds.Tables[tabIndex];
        }

        public DataTable GetBigDataPage(string userKey, int onlyShowMR, string filterClause, string orderBy, 
            string group, string groupWhere, int pageIndex, int pageSize, ref int pageCount)
        {
            DataAccessMgr daMgr = new DataAccessMgr();

            SqlParameter[] parameters = new SqlParameter[] {
                daMgr.BuildParam("@UserKey", SqlDbType.NVarChar, userKey),
                daMgr.BuildParam("@OnlyShowMR", SqlDbType.Bit, onlyShowMR),
                daMgr.BuildParam("@CVKey", SqlDbType.NVarChar, null),
                daMgr.BuildParam("@PartsReplaced", SqlDbType.Bit, 0),
                daMgr.BuildParam("@PartsToReplace", SqlDbType.Bit, 0),
                daMgr.BuildParam("@PartsOnOrder", SqlDbType.Bit, 0),
                daMgr.BuildParam("@PartsNotGood", SqlDbType.Bit, 0),
                daMgr.BuildParam("@ComingDueType", SqlDbType.Int, 0),
                daMgr.BuildParam("@IncludePastDue", SqlDbType.Bit, 0),
                daMgr.BuildParam("@UsingField", SqlDbType.NVarChar, null),
                daMgr.BuildParam("@DateFrom", SqlDbType.NVarChar, null),
                daMgr.BuildParam("@DateTo", SqlDbType.NVarChar, null),
                daMgr.BuildParam("@AndOr", SqlDbType.NVarChar, " AND "),
                daMgr.BuildParam("@Image", SqlDbType.Bit, 0),
                daMgr.BuildParam("@Generic", SqlDbType.Bit, 0),
                daMgr.BuildParam("@AR", SqlDbType.Bit, 0),
                daMgr.BuildParam("@ToDo", SqlDbType.Bit, 0),
                daMgr.BuildParam("@InPart", SqlDbType.Bit, 0),
                daMgr.BuildParam("@InAR", SqlDbType.Bit, 0),
                daMgr.BuildParam("@InWeld", SqlDbType.Bit, 0),
                daMgr.BuildParam("@InPositioner", SqlDbType.Bit, 0),
                daMgr.BuildParam("@InLocation", SqlDbType.Bit, 0),
                daMgr.BuildParam("@InGeneric", SqlDbType.Bit, 0),
                daMgr.BuildParam("@InToDo", SqlDbType.Bit, 0),
                daMgr.BuildParam("@FilterClause", SqlDbType.NVarChar, filterClause),
                daMgr.BuildParam("@Unit", SqlDbType.NVarChar, null),
                daMgr.BuildParam("@LinkField", SqlDbType.NVarChar, null),
                daMgr.BuildParam("@tarFilter", SqlDbType.NVarChar, null),
                daMgr.BuildParam("@tarFilterLabel", SqlDbType.NVarChar, null),
                daMgr.BuildParam("@group", SqlDbType.NVarChar, group),
                daMgr.BuildParam("@groupWhere", SqlDbType.NVarChar, groupWhere),
                daMgr.BuildParam("@orderby", SqlDbType.NVarChar, orderBy),
                daMgr.BuildParam("@pageIndex", SqlDbType.Int, pageIndex),
                daMgr.BuildParam("@pageSize", SqlDbType.Int, pageSize),
                daMgr.BuildOutParam("@count", SqlDbType.Int, pageCount),
                daMgr.BuildOutParam(AppConstants.CursorParam, SqlDbType.NVarChar, null)
            };

            if (!daMgr.LoadDataTable("USPGETCVLISTBYUSERAGGIRD", parameters, out DataTable dt, out List<int> outParas))
                return null;
            pageCount = outParas[0];
            return dt;
        }
    }
}
