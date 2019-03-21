
using AgGridApi.Models.Filter;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Text;
using System.Web;

namespace AgGridApi.Common
{
    public static class Utils
    {
        // Grid Customized
        // Aim to customize column with database
        /// <summary>
        /// set grid header
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="indexField"></param>
        /// <param name="indexCaption"></param>
        /// <param name="isVisble"></param>
        /// <returns></returns>
        public static string GetHeader(this DataTable dt, int indexField, int indexCaption, bool isVisble = true)
        {
            DataRow[] dataRows;
            if (isVisble)
            {
                dataRows = dt.Select("VISIBLE=1");
            }
            else
            {
                dataRows = dt.Select("VISIBLE!=1");
            }
            StringBuilder stringBuilder = new StringBuilder("[");
            foreach (DataRow dr in dataRows)
            {
                stringBuilder.Append(string.Format("{{\"headerName\":\"{0}\",\"field\":\"{1}\"",
                    dr[indexCaption].ToStringEx(),
                    dr[indexField].ToStringEx().ToLower()));
                if (dr["FIELDTYPE"].Equals("B"))
                {
                    stringBuilder.Append(",\"cellRenderer\": \"booleanCellRenderer\"");
                }
                stringBuilder.Append("},");
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1).Append("]");

            return stringBuilder.ToStringEx();
        }
        // transfer ag-grid filter condition to sql
        public static string GetFilterCondition(this ConditionFilterModel conditionFilterModel, string field)
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
                            condition.Append($" TO_NUMBER(NVL({field},0)) = {conditionFilterModel.Filter} ");
                            break;
                        case FilterType.NOT_EQUAL:
                            condition.Append($" TO_NUMBER(NVL({field},0)) != {conditionFilterModel.Filter} ");
                            break;
                        case FilterType.LESS_THAN:
                            condition.Append($" TO_NUMBER(NVL({field},0)) < {conditionFilterModel.Filter} ");
                            break;
                        case FilterType.LESS_THAN_OR_EQUAL:
                            condition.Append($" TO_NUMBER(NVL({field},0)) <= {conditionFilterModel.Filter} ");
                            break;
                        case FilterType.GREATER_THAN:
                            condition.Append($" TO_NUMBER(NVL({field},0)) > {conditionFilterModel.Filter} ");
                            break;
                        case FilterType.GREATER_THAN_OR_EQUAL:
                            condition.Append($" TO_NUMBER(NVL({field},0)) >= {conditionFilterModel.Filter} ");
                            break;
                        case FilterType.IN_RANGE:
                            condition.Append($" TO_NUMBER(NVL({field},0)) BETWEEN {conditionFilterModel.Filter} " +
                                $"and {conditionFilterModel.FilterTo}");
                            break;
                        default:
                            condition.Append($" TO_NUMBER(NVL({field},0)) = {conditionFilterModel.Filter} ");
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


        // Json Extension
        public static string SerializeToJSON(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T DeserializeFromJSON<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json.ToStringUD());
        }


        // String Extension
        public static string ToStringEx(this object obj)
        {
            if (obj == null)
                return string.Empty;
            else
                return obj.ToString();
        }

        public static string ToStringUD(this object obj)
        {
            if (obj == null)
                return string.Empty;
            else
                return HttpUtility.UrlDecode(obj.ToString());
        }

        public static string TrimLastCharacter(this String str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return str;
            }
            else
            {
                return str.TrimEnd(str[str.Length - 1]);
            }
        }

        public static bool IsNullOrEmptyOrSpace(this String str)
        {
            return string.IsNullOrEmpty(str) ?
                true : string.IsNullOrWhiteSpace(str);
        }
    }

}
