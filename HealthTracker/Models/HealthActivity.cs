namespace HealthTracker.Models
{
    /// <summary>
    /// Modelo que representa uma atividade de saúde com metadados avançados
    /// </summary>
    public class HealthActivity
    {
        public int Id { get; set; }
        public string ActivityType { get; set; }
        public DateTime Date { get; set; }
        public double Value { get; set; }
        public string Notes { get; set; }
        public TimeSpan? Duration { get; set; }
        public int Intensity { get; set; } // 1-10 scale
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public HealthActivity(int id, string activityType, DateTime date, double value, string notes = "", TimeSpan? duration = null, int intensity = 5)
        {
            Id = id;
            ActivityType = activityType;
            Date = date;
            Value = value;
            Notes = notes;
            Duration = duration;
            Intensity = intensity;
            CreatedAt = DateTime.Now;
        }

        public bool IsWithinRecommendedRange(ActivityType activityType)
        {
            return Value >= activityType.RecommendedMin && Value <= activityType.RecommendedMax;
        }
    }
}