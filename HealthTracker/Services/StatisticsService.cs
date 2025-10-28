using HealthTracker.DTOs;
using HealthTracker.Enums;
using HealthTracker.Models;
using HealthTracker.Repository;

namespace HealthTracker.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IHealthActivityRepository _repository;

        public StatisticsService(IHealthActivityRepository repository)
        {
            _repository = repository;
        }

        public StatisticsSummaryDTO GetActivitySummary(string activityType, DateTime? startDate = null, DateTime? endDate = null)
        {
            var activities = GetFilteredActivities(activityType, startDate, endDate);
            var activityTypeInfo = _repository.GetActivityTypeInfo(activityType);

            if (!activities.Any())
            {
                return new StatisticsSummaryDTO
                {
                    ActivityType = activityType,
                    Unit = activityTypeInfo?.Unit ?? "unidades"
                };
            }

            var values = activities.Select(a => a.Value).ToList();
            var trend = CalculateTrend(activities);

            return new StatisticsSummaryDTO
            {
                ActivityType = activityType,
                Total = values.Sum(),
                Average = values.Average(),
                Maximum = values.Max(),
                Minimum = values.Min(),
                Count = activities.Count,
                Trend = trend,
                ComplianceRate = CalculateComplianceRate(activityType, startDate ?? activities.Min(a => a.Date), endDate ?? DateTime.Now),
                Unit = activityTypeInfo?.Unit ?? "unidades"
            };
        }

        public List<StatisticsDTO> GetDetailedStatistics(string activityType, ReportPeriod period, DateTime startDate, DateTime endDate)
        {
            var statistics = new List<StatisticsDTO>();
            var activities = _repository.GetByActivityType(activityType)
                .Where(a => a.Date >= startDate && a.Date <= endDate)
                .ToList();

            if (!activities.Any()) return statistics;

            var activityTypeInfo = _repository.GetActivityTypeInfo(activityType);

            // Estatísticas por período
            var groupedActivities = period switch
            {
                ReportPeriod.Daily => activities.GroupBy(a => a.Date.Date),
                ReportPeriod.Weekly => activities.GroupBy(a => GetWeekStart(a.Date)),
                ReportPeriod.Monthly => activities.GroupBy(a => new DateTime(a.Date.Year, a.Date.Month, 1)),
                ReportPeriod.Yearly => activities.GroupBy(a => new DateTime(a.Date.Year, 1, 1)),
                _ => activities.GroupBy(a => a.Date.Date)
            };

            foreach (var group in groupedActivities)
            {
                var groupActivities = group.ToList();
                var groupValues = groupActivities.Select(a => a.Value).ToList();

                statistics.Add(new StatisticsDTO
                {
                    ActivityType = activityType,
                    Type = StatisticType.Total,
                    Value = groupValues.Sum(),
                    Unit = activityTypeInfo?.Unit ?? "unidades",
                    PeriodStart = group.Key,
                    PeriodEnd = GetPeriodEnd(group.Key, period),
                    DataPoints = groupActivities.Count,
                    AdditionalMetrics = new Dictionary<string, double>
                    {
                        ["average_intensity"] = groupActivities.Average(a => a.Intensity)
                    }
                });
            }

            return statistics;
        }

        public Dictionary<string, StatisticsSummaryDTO> GetOverallSummary(DateTime startDate, DateTime endDate)
        {
            var summary = new Dictionary<string, StatisticsSummaryDTO>();
            var activityTypes = _repository.GetActivityTypes();

            foreach (var activityType in activityTypes)
            {
                summary[activityType] = GetActivitySummary(activityType, startDate, endDate);
            }

            return summary;
        }

        public List<StatisticsDTO> GetTrendAnalysis(string activityType, DateTime startDate, DateTime endDate)
        {
            var trends = new List<StatisticsDTO>();
            var dailyTotals = _repository.GetDailyTotals(activityType, startDate, endDate);
            var activityTypeInfo = _repository.GetActivityTypeInfo(activityType);

            if (dailyTotals.Count < 2) return trends;

            var values = dailyTotals.Values.ToArray();
            var dates = dailyTotals.Keys.ToArray();

            for (int i = 1; i < values.Length; i++)
            {
                var change = values[i] - values[i - 1];
                var percentageChange = values[i - 1] != 0 ? (change / values[i - 1]) * 100 : 0;

                trends.Add(new StatisticsDTO
                {
                    ActivityType = activityType,
                    Type = StatisticType.Trend,
                    Value = change,
                    Unit = activityTypeInfo?.Unit ?? "unidades",
                    PeriodStart = dates[i - 1],
                    PeriodEnd = dates[i],
                    PercentageChange = percentageChange,
                    DataPoints = 2
                });
            }

            return trends;
        }

        public double CalculateComplianceRate(string activityType, DateTime startDate, DateTime endDate)
        {
            var activityTypeInfo = _repository.GetActivityTypeInfo(activityType);
            if (activityTypeInfo == null) return 0;

            var activities = GetFilteredActivities(activityType, startDate, endDate);
            if (!activities.Any()) return 0;

            var compliantDays = activities.Count(a => a.IsWithinRecommendedRange(activityTypeInfo));
            return (double)compliantDays / activities.Count * 100;
        }

        public Dictionary<string, double> GetCorrelations(List<string> activityTypes, DateTime startDate, DateTime endDate)
        {
            var correlations = new Dictionary<string, double>();
            var dailyData = new Dictionary<DateTime, Dictionary<string, double>>();

            // Coletar dados diários
            foreach (var activityType in activityTypes)
            {
                var dailyTotals = _repository.GetDailyTotals(activityType, startDate, endDate);
                foreach (var (date, value) in dailyTotals)
                {
                    if (!dailyData.ContainsKey(date))
                        dailyData[date] = new Dictionary<string, double>();

                    dailyData[date][activityType] = value;
                }
            }

            // Calcular correlações entre pares
            for (int i = 0; i < activityTypes.Count; i++)
            {
                for (int j = i + 1; j < activityTypes.Count; j++)
                {
                    var type1 = activityTypes[i];
                    var type2 = activityTypes[j];

                    var commonDates = dailyData.Where(d => d.Value.ContainsKey(type1) && d.Value.ContainsKey(type2))
                                              .Select(d => (d.Value[type1], d.Value[type2]))
                                              .ToList();

                    if (commonDates.Count >= 2)
                    {
                        var correlation = CalculateCorrelation(
                            commonDates.Select(x => x.Item1).ToArray(),
                            commonDates.Select(x => x.Item2).ToArray()
                        );
                        correlations[$"{type1}-{type2}"] = correlation;
                    }
                }
            }

            return correlations;
        }

        private List<HealthActivity> GetFilteredActivities(string activityType, DateTime? startDate, DateTime? endDate)
        {
            var activities = _repository.GetByActivityType(activityType);

            if (startDate.HasValue)
                activities = activities.Where(a => a.Date >= startDate.Value).ToList();

            if (endDate.HasValue)
                activities = activities.Where(a => a.Date <= endDate.Value).ToList();

            return activities;
        }

        private double CalculateTrend(List<HealthActivity> activities)
        {
            if (activities.Count < 2) return 0;

            var orderedActivities = activities.OrderBy(a => a.Date).ToList();
            var values = orderedActivities.Select(a => a.Value).ToArray();

            // Regressão linear simples para tendência
            double xAvg = 0, yAvg = values.Average();
            double numerator = 0, denominator = 0;

            for (int i = 0; i < values.Length; i++)
            {
                numerator += (i - xAvg) * (values[i] - yAvg);
                denominator += (i - xAvg) * (i - xAvg);
            }

            return denominator != 0 ? numerator / denominator : 0;
        }

        private DateTime GetWeekStart(DateTime date)
        {
            var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-1 * diff).Date;
        }

        private DateTime GetPeriodEnd(DateTime periodStart, ReportPeriod period)
        {
            return period switch
            {
                ReportPeriod.Daily => periodStart.AddDays(1).AddSeconds(-1),
                ReportPeriod.Weekly => periodStart.AddDays(7).AddSeconds(-1),
                ReportPeriod.Monthly => periodStart.AddMonths(1).AddSeconds(-1),
                ReportPeriod.Yearly => periodStart.AddYears(1).AddSeconds(-1),
                _ => periodStart.AddDays(1).AddSeconds(-1)
            };
        }

        private double CalculateCorrelation(double[] x, double[] y)
        {
            if (x.Length != y.Length || x.Length < 2)
                return 0;

            double xMean = x.Average();
            double yMean = y.Average();
            double numerator = 0, xDenom = 0, yDenom = 0;

            for (int i = 0; i < x.Length; i++)
            {
                numerator += (x[i] - xMean) * (y[i] - yMean);
                xDenom += Math.Pow(x[i] - xMean, 2);
                yDenom += Math.Pow(y[i] - yMean, 2);
            }

            return xDenom > 0 && yDenom > 0 ? numerator / Math.Sqrt(xDenom * yDenom) : 0;
        }
    }
}