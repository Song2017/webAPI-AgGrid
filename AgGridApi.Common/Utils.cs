
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
