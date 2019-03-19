using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgGridApi.Models.Filter
{
    public class SetColumnFilter : ColumnFilter
    {
        public String Type { get; set; }
        public int FilterTo { get; set; }

        private List<String> values;

        public SetColumnFilter() { }

        public SetColumnFilter(List<String> values)
        {
            this.values = values;
        }
        public List<String> getValues()
        {
            return values;
        }
    }
}
