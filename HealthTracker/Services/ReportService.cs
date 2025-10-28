using HealthTracker.DTOs;
using HealthTracker.Enums;
using HealthTracker.Models;
using HealthTracker.Repository;

namespace HealthTracker.Services
{
    public class ReportService : IReportService
    {
        private readonly IHealthActivityRepository _repository;
        private readonly IStatisticsService _statisticsService;

        public ReportService(IHealthActivityRepository repository, IStatisticsService statisticsService)
        {
            _repository = repository;
            _statisticsService = statisticsService;
        }

        public ActivityReportDTO GenerateActivityReport(ReportPeriod period, DateTime startDate, DateTime endDate, List<string> activityTypes = null)
        {
            var report = new ActivityReportDTO
            {
                Title = $"Relatório de Atividades - {period}",
                Period = period,
                StartDate = startDate,
                EndDate = endDate,
                ActivityTypes = activityTypes ?? _repository.GetActivityTypes()
            };

            foreach (var activityType in report.ActivityTypes)
            {
                var summary = _statisticsService.GetActivitySummary(activityType, startDate, endDate);
                report.Statistics.Add(summary);
            }

            GenerateInsights(report);

            return report;
        }

        public ActivityReportDTO GenerateHealthSummaryReport(DateTime startDate, DateTime endDate)
        {
            var report = new ActivityReportDTO
            {
                Title = "Resumo Geral de Saúde",
                Period = ReportPeriod.Custom,
                StartDate = startDate,
                EndDate = endDate,
                ActivityTypes = _repository.GetActivityTypes()
            };

            var overallSummary = _statisticsService.GetOverallSummary(startDate, endDate);
            report.Statistics.AddRange(overallSummary.Values);

            // Insights específicos para resumo geral
            report.Insights["TotalActivities"] = _repository.GetCount();
            report.Insights["PeriodDays"] = (endDate - startDate).TotalDays;
            report.Insights["ActivitiesPerDay"] = overallSummary.Values.Sum(s => s.Count) / (endDate - startDate).TotalDays;

            return report;
        }

        public ActivityReportDTO GenerateComplianceReport(DateTime startDate, DateTime endDate)
        {
            var report = new ActivityReportDTO
            {
                Title = "Relatório de Conformidade",
                Period = ReportPeriod.Custom,
                StartDate = startDate,
                EndDate = endDate,
                ActivityTypes = _repository.GetActivityTypes()
            };

            foreach (var activityType in report.ActivityTypes)
            {
                var summary = _statisticsService.GetActivitySummary(activityType, startDate, endDate);
                var activityTypeInfo = _repository.GetActivityTypeInfo(activityType);

                if (activityTypeInfo != null)
                {
                    summary.ComplianceRate = _statisticsService.CalculateComplianceRate(activityType, startDate, endDate);
                }

                report.Statistics.Add(summary);
            }

            GenerateComplianceInsights(report);

            return report;
        }

        public void ExportReportToCsv(ActivityReportDTO report, string filePath)
        {
            using var writer = new StreamWriter(filePath);
            
            // Cabeçalho
            writer.WriteLine("Relatório: " + report.Title);
            writer.WriteLine($"Período: {report.StartDate:dd/MM/yyyy} - {report.EndDate:dd/MM/yyyy}");
            writer.WriteLine($"Gerado em: {report.GeneratedAt:dd/MM/yyyy HH:mm}");
            writer.WriteLine();

            // Dados
            writer.WriteLine("Tipo de Atividade,Total,Média,Máximo,Mínimo,Contagem,Conformidade,Unidade");
            foreach (var stat in report.Statistics)
            {
                writer.WriteLine($"\"{stat.ActivityType}\",{stat.Total:F2},{stat.Average:F2},{stat.Maximum:F2},{stat.Minimum:F2},{stat.Count},{stat.ComplianceRate:F1}%,{stat.Unit}");
            }

            // Insights
            if (report.Insights.Any())
            {
                writer.WriteLine();
                writer.WriteLine("Insights:");
                foreach (var insight in report.Insights)
                {
                    writer.WriteLine($"{insight.Key}: {insight.Value}");
                }
            }
        }

        public void ExportReportToJson(ActivityReportDTO report, string filePath)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(report, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            });
            
            File.WriteAllText(filePath, json);
        }

        private void GenerateInsights(ActivityReportDTO report)
        {
            var insights = new Dictionary<string, object>();

            // Encontrar atividade mais frequente
            var mostFrequent = report.Statistics.OrderByDescending(s => s.Count).FirstOrDefault();
            if (mostFrequent != null)
            {
                insights["AtividadeMaisFrequente"] = mostFrequent.ActivityType;
                insights["FrequenciaAtividadeMaisFrequente"] = mostFrequent.Count;
            }

            // Encontrar melhor conformidade
            var bestCompliance = report.Statistics.Where(s => s.ComplianceRate > 0).OrderByDescending(s => s.ComplianceRate).FirstOrDefault();
            if (bestCompliance != null)
            {
                insights["MelhorConformidade"] = bestCompliance.ActivityType;
                insights["TaxaMelhorConformidade"] = $"{bestCompliance.ComplianceRate:F1}%";
            }

            // Calcular consistência geral
            var averageCompliance = report.Statistics.Average(s => s.ComplianceRate);
            insights["ConformidadeMedia"] = $"{averageCompliance:F1}%";

            report.Insights = insights;
        }

        private void GenerateComplianceInsights(ActivityReportDTO report)
        {
            var insights = new Dictionary<string, object>();

            var compliantActivities = report.Statistics.Where(s => s.ComplianceRate >= 80).ToList();
            var nonCompliantActivities = report.Statistics.Where(s => s.ComplianceRate < 80).ToList();

            insights["AtividadesConformes"] = compliantActivities.Count;
            insights["AtividadesNaoConformes"] = nonCompliantActivities.Count;

            if (compliantActivities.Any())
            {
                insights["MelhoresAtividades"] = string.Join(", ", compliantActivities.OrderByDescending(a => a.ComplianceRate).Select(a => a.ActivityType));
            }

            if (nonCompliantActivities.Any())
            {
                insights["AtividadesParaMelhorar"] = string.Join(", ", nonCompliantActivities.OrderBy(a => a.ComplianceRate).Select(a => a.ActivityType));
            }

            report.Insights = insights;
        }
    }
}