using AgGridApi.Common;
using AgGridApi.Models;
using AgGridApi.Models.Filter;
using AgGridApi.Models.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgGridApi.Services
{
    public class RequestBuilder : IRequestBuilder
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Alias { get; set; } = "r.";

        private List<SortModel> _sortModel;

        private List<String> groupKeys;
        private List<String> rowGroups;
        private Boolean isGrouping;
        private List<ColumnVO> valueColumns;
        private List<ColumnVO> pivotColumns;
        private Dictionary<String, ColumnFilter> filterModel;
        private List<ColumnVO> rowGroupCols;
        private Dictionary<String, List<String>> pivotValues;
        private Boolean isPivotMode;
        private ServerRowsRequest enterpriseGetRowsRequest;
        private Dictionary<String, String> operatorMap = new Dictionary<String, String>()
         {
            {"equals", "="},
            {"notEqual", "<>"},
            {"lessThan", "<"},
            {"lessThanOrEqual", "<="},
            {"greaterThan", ">"},
            {"greaterThanOrEqual", ">="}
        };
        public RequestBuilder() { }
        public RequestBuilder(ServerRowsRequest request)
        {
            enterpriseGetRowsRequest = request;
            AssignRequest(request);
        }

        public RequestBuilder(ServerRowsRequest request, String tableName, Dictionary<String, List<String>> pivotValues)
        {
            enterpriseGetRowsRequest = request;
            this.pivotValues = pivotValues;

            AssignRequest(request);

            // return selectSql() + fromSql(tableName) + whereSql() + groupBySql() + orderBySql() + limitSql();
        }

        private String groupBySql()
        {
            return isGrouping ? string.Join(",", rowGroups) : "";
        }

        private List<String> getRowGroups()
        {
            List<String> rg = new List<string>();
            foreach (var group in rowGroupCols)
            {
                rg.Add(group.Field.ToStringEx());
            }
            return rg;
        }

        public void AssignRequest(ServerRowsRequest request)
        {
            this.valueColumns = request.ValueCols;
            this.pivotColumns = request.PivotCols;
            this.groupKeys = request.GroupKeys;
            this.rowGroupCols = request.RowGroupCols;
            this.isPivotMode = request.IsPivotMode;
            this.filterModel = request.FilterModel;
            this._sortModel = request.SortModel;
            this.PageIndex = request.PageIndex;
            this.PageSize = request.PageSize;
 
            this.rowGroups = getRowGroups();
            this.isGrouping = rowGroups.Count > groupKeys.Count;
        }

        public int GetPageIndex() { 
            return PageIndex + 1;
        }

        public int GetPageSize()
        {
            return PageSize;
        }

        public List<String> GetFilters()
        {
            List<string> filters = new List<string>();
            if (filterModel == null)
                return filters;
            if (filterModel.GetType().Equals("text"))
            {
                
            }

            return new List<string>();
        }

        public String GetSorts()
        {
            if (_sortModel.Count <= 0)
                return string.Empty;

            StringBuilder sorts = new StringBuilder(Constants.whiteSpace);
            foreach (SortModel sort in _sortModel)
            {
                sorts.Append(Constants.whiteSpace + sort.ColId.ToUpper() + Constants.whiteSpace).
                    Append(sort.Sort.ToUpper() + Constants.comma);
            }

            return sorts.ToStringEx().TrimLastCharacter();
        }



    }

}
