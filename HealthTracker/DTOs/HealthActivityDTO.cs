namespace HealthTracker.DTOs
{
    /// <summary>
    /// DTO para criação de atividades de saúde
    /// </summary>
    public class HealthActivityDTO
    {
        public string ActivityType { get; set; }
        public DateTime Date { get; set; }
        public double Value { get; set; }
        public string Notes { get; set; }
        public TimeSpan? Duration { get; set; }
        public int Intensity { get; set; }

        public HealthActivityDTO(string activityType, DateTime date, double value, string notes = "", TimeSpan? duration = null, int intensity = 5)
        {
            ActivityType = activityType;
            Date = date;
            Value = value;
            Notes = notes;
            Duration = duration;
            Intensity = intensity;
        }
    }

    /// <summary>
    /// DTO para resposta de atividades
    /// </summary>
    public class HealthActivityResponseDTO
    {
        public int Id { get; set; }
        public string ActivityType { get; set; }
        public DateTime Date { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }
        public string Notes { get; set; }
        public TimeSpan? Duration { get; set; }
        public int Intensity { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsWithinRecommendation { get; set; }
    }
}