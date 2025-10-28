using HealthTracker.DTOs;
using HealthTracker.Models;

namespace HealthTracker.Services
{
    /// <summary>
    /// Serviço principal do rastreador de saúde
    /// </summary>
    public interface IHealthTrackerService
    {
        // Operações básicas
        ServiceResult<HealthActivityResponseDTO> AddActivity(HealthActivityDTO activityDTO);
        ServiceResult<HealthActivityResponseDTO> UpdateActivity(int id, HealthActivityDTO activityDTO);
        ServiceResult<bool> DeleteActivity(int id);
        ServiceResult<HealthActivityResponseDTO> GetActivity(int id);
        ServiceResult<List<HealthActivityResponseDTO>> GetAllActivities();

        // Operações avançadas
        ServiceResult<List<HealthActivityResponseDTO>> GetActivitiesByDateRange(DateTime startDate, DateTime endDate);
        ServiceResult<List<HealthActivityResponseDTO>> SearchActivities(string searchTerm);
        ServiceResult<List<HealthActivityResponseDTO>> GetRecentActivities(int count);

        // Métodos de utilidade
        ServiceResult<List<string>> GetAvailableActivityTypes();
        ServiceResult<ActivityType> GetActivityTypeInfo(string activityType);
        ServiceResult<int> GetTotalActivitiesCount();
    }

    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string ErrorMessage { get; set; }
        public List<string> Warnings { get; set; }

        public ServiceResult()
        {
            Warnings = new List<string>();
        }

        public static ServiceResult<T> Ok(T data) => new ServiceResult<T> { Success = true, Data = data };
        public static ServiceResult<T> Fail(string error) => new ServiceResult<T> { Success = false, ErrorMessage = error };
    }
}