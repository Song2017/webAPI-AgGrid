using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgGridApi.Models.Filter
{
    public class NumberColumnFilter : ColumnFilter
    {
        public String Type { get; set; }
        public int Filter { get; set; }
        public int FilterTo { get; set; }

        public NumberColumnFilter() { }

        public NumberColumnFilter(String type, int filter, int filterTo)
        {
            this.Type = type;
            this.Filter = filter;
            this.FilterTo = filterTo;
        }
    }
}
