using HealthTracker.Enums;

namespace HealthTracker.DTOs
{
    /// <summary>
    /// DTO para estatísticas detalhadas
    /// </summary>
    public class StatisticsDTO
    {
        public string ActivityType { get; set; }
        public StatisticType Type { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public int DataPoints { get; set; }
        public double PercentageChange { get; set; }
        public Dictionary<string, double> AdditionalMetrics { get; set; }

        public StatisticsDTO()
        {
            AdditionalMetrics = new Dictionary<string, double>();
        }
    }

    /// <summary>
    /// DTO para resumo estatístico
    /// </summary>
    public class StatisticsSummaryDTO
    {
        public string ActivityType { get; set; }
        public double Total { get; set; }
        public double Average { get; set; }
        public double Maximum { get; set; }
        public double Minimum { get; set; }
        public int Count { get; set; }
        public double Trend { get; set; }
        public double ComplianceRate { get; set; }
        public string Unit { get; set; }
    }
}