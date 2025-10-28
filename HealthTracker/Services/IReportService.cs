using HealthTracker.DTOs;
using HealthTracker.Enums;

namespace HealthTracker.Services
{
    /// <summary>
    /// Serviço para geração de relatórios
    /// </summary>
    public interface IReportService
    {
        ActivityReportDTO GenerateActivityReport(ReportPeriod period, DateTime startDate, DateTime endDate, List<string> activityTypes = null);
        ActivityReportDTO GenerateHealthSummaryReport(DateTime startDate, DateTime endDate);
        ActivityReportDTO GenerateComplianceReport(DateTime startDate, DateTime endDate);
        void ExportReportToCsv(ActivityReportDTO report, string filePath);
        void ExportReportToJson(ActivityReportDTO report, string filePath);
    }
}