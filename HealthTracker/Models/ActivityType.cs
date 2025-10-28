using HealthTracker.Enums;

namespace HealthTracker.Models
{
    /// <summary>
    /// Tipo de atividade com metadados e configurações
    /// </summary>
    public class ActivityType
    {
        public string Name { get; set; }
        public string Unit { get; set; }
        public string Description { get; set; }
        public double RecommendedMin { get; set; }
        public double RecommendedMax { get; set; }
        public ActivityCategory Category { get; set; }

        public ActivityType(string name, string unit, string description, double recommendedMin, double recommendedMax, ActivityCategory category)
        {
            Name = name;
            Unit = unit;
            Description = description;
            RecommendedMin = recommendedMin;
            RecommendedMax = recommendedMax;
            Category = category;
        }
    }
}