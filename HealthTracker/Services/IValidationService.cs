using HealthTracker.Models;

namespace HealthTracker.Services
{
    /// <summary>
    /// Serviço de validação avançado
    /// </summary>
    public interface IValidationService
    {
        ValidationResult ValidateActivity(HealthActivity activity);
        ValidationResult ValidateActivityData(string activityType, string dateInput, string valueInput, string notes = "");
        bool IsValidDateRange(DateTime startDate, DateTime endDate);
        bool IsValidIntensity(int intensity);
        bool IsValidDuration(TimeSpan? duration);
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; }
        public List<string> Warnings { get; set; }

        public ValidationResult()
        {
            Errors = new List<string>();
            Warnings = new List<string>();
        }

        public string GetErrorMessages() => string.Join("; ", Errors);
        public string GetWarningMessages() => string.Join("; ", Warnings);
    }
}