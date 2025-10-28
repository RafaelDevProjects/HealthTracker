using HealthTracker.Models;
using HealthTracker.Repository;
using HealthTracker.Utils;

namespace HealthTracker.Services
{
    public class ValidationService : IValidationService
    {
        private readonly IHealthActivityRepository _repository;

        public ValidationService(IHealthActivityRepository repository)
        {
            _repository = repository;
        }

        public ValidationResult ValidateActivity(HealthActivity activity)
        {
            var result = new ValidationResult();

            if (activity == null)
            {
                result.Errors.Add("Atividade não pode ser nula");
                return result;
            }

            // Validar tipo de atividade
            if (string.IsNullOrWhiteSpace(activity.ActivityType))
            {
                result.Errors.Add("Tipo de atividade é obrigatório");
            }

            // Validar data
            if (activity.Date > DateTime.Now.AddDays(1)) // Permite registrar para o próximo dia
            {
                result.Errors.Add("Data não pode ser no futuro distante");
            }

            if (activity.Date < DateTime.Now.AddYears(-10))
            {
                result.Errors.Add("Data muito antiga");
            }

            // Validar valor
            if (activity.Value < 0)
            {
                result.Errors.Add("Valor não pode ser negativo");
            }

            if (activity.Value > 10000) // Limite razoável
            {
                result.Errors.Add("Valor muito alto");
            }

            // Validar intensidade
            if (!IsValidIntensity(activity.Intensity))
            {
                result.Errors.Add("Intensidade deve estar entre 1 e 10");
            }

            // Validar notas
            if (activity.Notes?.Length > 500)
            {
                result.Errors.Add("Notas muito longas (máximo 500 caracteres)");
            }

            // Verificações de aviso
            var activityTypeInfo = _repository.GetActivityTypeInfo(activity.ActivityType);
            if (activityTypeInfo != null && !activity.IsWithinRecommendedRange(activityTypeInfo))
            {
                result.Warnings.Add($"Valor fora da faixa recomendada ({activityTypeInfo.RecommendedMin}-{activityTypeInfo.RecommendedMax} {activityTypeInfo.Unit})");
            }

            if (activity.Value == 0)
            {
                result.Warnings.Add("Valor zero registrado");
            }

            result.IsValid = !result.Errors.Any();
            return result;
        }

        public ValidationResult ValidateActivityData(string activityType, string dateInput, string valueInput, string notes = "")
        {
            var result = new ValidationResult();

            // Validar tipo de atividade
            if (string.IsNullOrWhiteSpace(activityType))
            {
                result.Errors.Add("Tipo de atividade é obrigatório");
            }

            // Validar data com formato brasileiro
            if (!DateHelper.TryParseBrazilianDate(dateInput, out DateTime date))
            {
                // Tentar parse normal como fallback
                if (!DateTime.TryParse(dateInput, out date))
                {
                    result.Errors.Add("Data inválida. Use o formato dd/MM/aaaa (ex: 15/11/2024)");
                }
                else
                {
                    // Data foi parseada mas não no formato brasileiro - adicionar aviso
                    result.Warnings.Add($"Data interpretada como: {date:dd/MM/yyyy}");
                }
            }

            if (date > DateTime.Now.AddDays(1))
            {
                result.Errors.Add("Data não pode ser no futuro distante");
            }

            if (date < DateTime.Now.AddYears(-10))
            {
                result.Errors.Add("Data muito antiga");
            }

            // Validar valor
            if (!double.TryParse(valueInput, out double value))
            {
                result.Errors.Add("Valor deve ser um número válido");
            }
            else if (value < 0)
            {
                result.Errors.Add("Valor não pode ser negativo");
            }

            // Validar notas
            if (notes?.Length > 500)
            {
                result.Errors.Add("Notas muito longas (máximo 500 caracteres)");
            }

            result.IsValid = !result.Errors.Any();
            return result;
        }
        public bool IsValidDateRange(DateTime startDate, DateTime endDate)
        {
            return startDate <= endDate && endDate <= DateTime.Now && startDate >= DateTime.Now.AddYears(-10);
        }

        public bool IsValidIntensity(int intensity)
        {
            return intensity >= 1 && intensity <= 10;
        }

        public bool IsValidDuration(TimeSpan? duration)
        {
            return duration == null || (duration.Value.TotalHours >= 0 && duration.Value.TotalHours <= 24);
        }
    }
}