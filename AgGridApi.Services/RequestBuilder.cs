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
        public string Alias { get; set; } = "r.";

        private ServerRowsRequest _serverRowsRequest;
        private List<SortModel> _sortModels;
        private List<ColumnFilter> _filterModels;

        private Dictionary<String, List<String>> pivotValues;


        public RequestBuilder() { }

        public RequestBuilder(ServerRowsRequest request)
        {
            AssignRequest(request);
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
                    filters.Append(GetFilterCondition(filterModel.Condition[i], filterModel.Head.Field.ToUpper()));
                    if (i + 1 < filterModel.Condition.Count)
                        filters.Append(Constants.WHITESPACE + filterModel.Head.Operate.ToStringEx() + Constants.WHITESPACE);
                }

                filters.Append(") ");
            }

            return filters.ToStringEx();
        }

        private string GetFilterCondition(ConditionFilterModel conditionFilterModel, string field)
        {
            StringBuilder condition = new StringBuilder();
            switch (conditionFilterModel.FilterType)
            {

                case FilterConditionType.TEXT:
                    switch (conditionFilterModel.Type)
                    {
                        case FilterType.EQUALS:
                            condition.Append($" UPPER({field}) = UPPER('{conditionFilterModel.Filter}') ");
                            break;
                        case FilterType.NOT_EQUAL:
                            condition.Append($" UPPER({field}) != UPPER('{conditionFilterModel.Filter}') ");
                            break;
                        case FilterType.STARTS_WITH:
                            condition.Append($" UPPER({field}) LIKE UPPER('{conditionFilterModel.Filter}%') ");
                            break;
                        case FilterType.ENDS_WITH:
                            condition.Append($" UPPER({field}) LIKE UPPER('%{conditionFilterModel.Filter}') ");
                            break;
                        case FilterType.CONTAINS:
                            condition.Append($" INSTR(UPPER({field}), UPPER('{conditionFilterModel.Filter}')) > 0 ");
                            break;
                        case FilterType.NOT_CONTAINS:
                            condition.Append($" INSTR(UPPER({field}), UPPER('{conditionFilterModel.Filter}')) = 0 ");
                            break;
                        default:
                            condition.Append($" INSTR(UPPER({field}), UPPER('{conditionFilterModel.Filter}')) > 0 ");
                            break;
                    }
                    break;
                case FilterConditionType.NUMBER:
                    switch (conditionFilterModel.Type)
                    {
                        case FilterType.EQUALS:
                            condition.Append($" {field} = {conditionFilterModel.Filter} ");
                            break;
                        case FilterType.NOT_EQUAL:
                            condition.Append($" {field} != {conditionFilterModel.Filter} ");
                            break;
                        case FilterType.LESS_THAN:
                            condition.Append($" {field} < {conditionFilterModel.Filter} ");
                            break;
                        case FilterType.LESS_THAN_OR_EQUAL:
                            condition.Append($" {field} <= {conditionFilterModel.Filter} ");
                            break;
                        case FilterType.GREATER_THAN:
                            condition.Append($" {field} > {conditionFilterModel.Filter} ");
                            break;
                        case FilterType.GREATER_THAN_OR_EQUAL:
                            condition.Append($" {field} >= {conditionFilterModel.Filter} ");
                            break;
                        case FilterType.IN_RANGE:
                            condition.Append($" {field} BETWEEN {conditionFilterModel.Filter} " +
                                $"and {conditionFilterModel.FilterTo}");
                            break;
                        default:
                            condition.Append($" {field} = {conditionFilterModel.Filter} ");
                            break;
                    }
                    break;
                case FilterConditionType.DATE:
                    switch (conditionFilterModel.Type)
                    {
                        case FilterType.EQUALS:
                            condition.Append($" {field} = '{conditionFilterModel.DateFrom}' ");
                            break;
                        case FilterType.NOT_EQUAL:
                            condition.Append($" {field} != '{conditionFilterModel.DateFrom}' ");
                            break;
                        case FilterType.LESS_THAN:
                            condition.Append($" {field} < '{conditionFilterModel.DateFrom}' ");
                            break;
                        case FilterType.GREATER_THAN:
                            condition.Append($" {field} > '{conditionFilterModel.DateFrom}' ");
                            break;
                        case FilterType.IN_RANGE:
                            condition.Append($" {field} BETWEEN '{conditionFilterModel.DateFrom}' " +
                                $"and '{conditionFilterModel.DateTo}' ");
                            break;
                        //case FilterType.EQUALS:
                        //    condition.Append($" {field} = TO_DATE('{conditionFilterModel.DateFrom}','{Constants.DATEFORMAT}') ");
                        //    break;
                        //case FilterType.NOT_EQUAL:
                        //    condition.Append($" {field} != TO_DATE('{conditionFilterModel.DateFrom}','{Constants.DATEFORMAT}') ");
                        //    break;
                        //case FilterType.LESS_THAN:
                        //    condition.Append($" {field} < TO_DATE('{conditionFilterModel.DateFrom}','{Constants.DATEFORMAT}') ");
                        //    break;
                        //case FilterType.GREATER_THAN:
                        //    condition.Append($" {field} > TO_DATE('{conditionFilterModel.DateFrom}','{Constants.DATEFORMAT}') ");
                        //    break;
                        //case FilterType.IN_RANGE:
                        //    condition.Append($" {field} BETWEEN TO_DATE('{conditionFilterModel.DateFrom}','{Constants.DATEFORMAT}') " +
                        //        $"and TO_DATE('{conditionFilterModel.DateTo}','{Constants.DATEFORMAT}') ");
                        //    break;
                        default:
                            condition.Append($" {field} = TO_DATE('{conditionFilterModel.Filter}','{Constants.DATEFORMAT}') ");
                            break;
                    }
                    break;
                case FilterConditionType.SET:
                    break;
                default: break;
            }


            return condition.ToStringEx();
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

        public IRequestBuilder GetRequestBuilder(ServerRowsRequest request)
        {
            if (request == null)
                return new RequestBuilder(_serverRowsRequest);
            else
                return new RequestBuilder(request);
        }
    }

}
