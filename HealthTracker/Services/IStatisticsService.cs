using HealthTracker.DTOs;
using HealthTracker.Enums;

namespace HealthTracker.Services
{
    /// <summary>
    /// Serviço para cálculos estatísticos avançados
    /// </summary>
    public interface IStatisticsService
    {
        StatisticsSummaryDTO GetActivitySummary(string activityType, DateTime? startDate = null, DateTime? endDate = null);
        List<StatisticsDTO> GetDetailedStatistics(string activityType, ReportPeriod period, DateTime startDate, DateTime endDate);
        Dictionary<string, StatisticsSummaryDTO> GetOverallSummary(DateTime startDate, DateTime endDate);
        List<StatisticsDTO> GetTrendAnalysis(string activityType, DateTime startDate, DateTime endDate);
        double CalculateComplianceRate(string activityType, DateTime startDate, DateTime endDate);
        Dictionary<string, double> GetCorrelations(List<string> activityTypes, DateTime startDate, DateTime endDate);
    }
}