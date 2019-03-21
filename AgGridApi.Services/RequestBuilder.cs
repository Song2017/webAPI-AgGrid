using AgGridApi.Common;
using AgGridApi.Models;
using AgGridApi.Models.Filter;
using AgGridApi.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgGridApi.Services
{
    public class RequestBuilder : IRequestBuilder
    {
        private ServerRowsRequest _serverRowsRequest;
        private List<SortModel> _sortModels;
        private List<ColumnFilter> _filterModels;

        public RequestBuilder() { }

        public RequestBuilder(ServerRowsRequest request)
        {
            AssignRequest(request);
        }
        public IRequestBuilder GetRequestBuilder(ServerRowsRequest request)
        {
            if (request == null)
                return new RequestBuilder(_serverRowsRequest);
            else
                return new RequestBuilder(request);
        }

        public void AssignRequest(ServerRowsRequest request)
        {
            _serverRowsRequest = request;
            _filterModels = request.FilterModels;
            _sortModels = request.SortModels;
        }


        public int GetPageIndex()
        {
            return _serverRowsRequest.PageIndex + 1;
        }

        public int GetPageSize()
        {
            return _serverRowsRequest.PageSize;
        }

        public String GetFilters()
        {
            if (_filterModels.Count <= 0)
                return string.Empty;

            StringBuilder filters = new StringBuilder(" 1=1 ");
            foreach (ColumnFilter filterModel in _filterModels)
            {
                filters.Append(" AND (");
                for (int i = 0; i < filterModel.Condition.Count; i++)
                {
                    filters.Append(filterModel.Condition[i].GetFilterCondition(filterModel.Head.Field.ToUpper()));
                    if (i + 1 < filterModel.Condition.Count)
                        filters.Append(Constants.WHITESPACE + filterModel.Head.Operate.ToStringEx() + Constants.WHITESPACE);
                }

                filters.Append(") ");
            }

            return filters.ToStringEx();
        }

        public String GetSorts()
        {
            if (_sortModels.Count <= 0)
                return string.Empty;

            StringBuilder sorts = new StringBuilder(Constants.WHITESPACE);
            foreach (SortModel sort in _sortModels)
            {
                sorts.Append(Constants.WHITESPACE + sort.ColId.ToUpper() + Constants.WHITESPACE).
                    Append(sort.Sort.ToUpper() + Constants.COMMA);
            }

            return sorts.ToStringEx().TrimLastCharacter();
        }

        public string GetGroups()
        {
            if (_serverRowsRequest.RowGroupCols.Count == 0 ||
                _serverRowsRequest.GroupKeys.Count > 0)
                return string.Empty;

            return string.Join(",", _serverRowsRequest.RowGroupCols
                .Select(s => s.Id));
        }

        public string GetGroupWheres()
        {
            if (_serverRowsRequest.RowGroupCols.Count == 0 ||
                _serverRowsRequest.GroupKeys.Count == 0)
                return string.Empty;
            string where = string.Empty;
            for (int i = 0; i < _serverRowsRequest.GroupKeys.Count; i++)
            {
                where += " and " + _serverRowsRequest.RowGroupCols[i].Id +
                    " = '" + _serverRowsRequest.GroupKeys[i] + "'";
            }

            return where.ToStringEx();
        }

    }

}
