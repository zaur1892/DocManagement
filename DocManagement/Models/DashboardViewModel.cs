using System;
using System.Collections.Generic;

namespace DocManagement.Models
{
    public class DashboardViewModel
    {
        public int TotalJobs { get; set; }
        public int CompletedJobs { get; set; }

        public Dictionary<string, int> JobsByStatus { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> JobsByEquipment { get; set; } = new Dictionary<string, int>();

        // Filter parameters
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string GroupBy { get; set; } // "Year", "Month", "Day"
    }
}
