using HealthTracker.Enums;

namespace HealthTracker.DTOs
{
    /// <summary>
    /// DTO para relatórios personalizados
    /// </summary>
    public class ActivityReportDTO
    {
        public string Title { get; set; }
        public ReportPeriod Period { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> ActivityTypes { get; set; }
        public List<StatisticsSummaryDTO> Statistics { get; set; }
        public Dictionary<string, object> Insights { get; set; }
        public DateTime GeneratedAt { get; set; }

        public ActivityReportDTO()
        {
            ActivityTypes = new List<string>();
            Statistics = new List<StatisticsSummaryDTO>();
            Insights = new Dictionary<string, object>();
            GeneratedAt = DateTime.Now;
        }
    }
}