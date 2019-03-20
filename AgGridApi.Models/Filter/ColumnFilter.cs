using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgGridApi.Models.Filter
{
    public class ColumnFilter
    {
        public HeadFilterModel Head { get; set; }

        public List<ConditionFilterModel> Condition { get; set; }

        public ColumnFilter()
        {
            Head = new HeadFilterModel();
            Condition = new List<ConditionFilterModel>();
        }
    }
}
