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
        private List<ColumnFilter> filterModels;
        private List<ColumnVO> rowGroupCols;
        private Dictionary<String, List<String>> pivotValues;
        private Boolean isPivotMode;

        public RequestBuilder() { }

        public RequestBuilder(ServerRowsRequest request, Dictionary<String, List<String>> pivotValues)
        {
            this.pivotValues = pivotValues;

            AssignRequest(request);
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
            this.filterModels = request.FilterModels;
            this._sortModel = request.SortModels;
            this.PageIndex = request.PageIndex;
            this.PageSize = request.PageSize;

            this.rowGroups = getRowGroups();
            this.isGrouping = rowGroups.Count > groupKeys.Count;
        }

        public int GetPageIndex()
        {
            return PageIndex + 1;
        }

        public int GetPageSize()
        {
            return PageSize;
        }

        public String GetFilters()
        {
            if (filterModels.Count <= 0)
                return string.Empty;

            StringBuilder filters = new StringBuilder(" 1=1 ");
            foreach (ColumnFilter filterModel in filterModels)
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
            if (_sortModel.Count <= 0)
                return string.Empty;

            StringBuilder sorts = new StringBuilder(Constants.WHITESPACE);
            foreach (SortModel sort in _sortModel)
            {
                sorts.Append(Constants.WHITESPACE + sort.ColId.ToUpper() + Constants.WHITESPACE).
                    Append(sort.Sort.ToUpper() + Constants.COMMA);
            }

            return sorts.ToStringEx().TrimLastCharacter();
        }
    }

}
