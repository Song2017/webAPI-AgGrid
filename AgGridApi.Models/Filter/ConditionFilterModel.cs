using System;

namespace AgGridApi.Models.Filter
{
    public class ConditionFilterModel
    {
        public string Type { get; set; }

        public string Filter { get; set; }

        public string FilterType { get; set; }

        public string FilterTo { get; set; }

        public ConditionFilterModel()
        {
            this.Type = string.Empty;
            this.Filter = string.Empty;
            this.FilterTo = string.Empty;
            this.FilterType = string.Empty;
        }
    }
}