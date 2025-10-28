using HealthTracker.Enums;
using HealthTracker.Models;

namespace HealthTracker.Repository
{
    /// <summary>
    /// Interface do repositório com operações avançadas
    /// </summary>
    public interface IHealthActivityRepository
    {
        // Operações básicas
        HealthActivity GetById(int id);
        List<HealthActivity> GetAll();
        void Add(HealthActivity activity);
        bool Update(HealthActivity activity);
        bool Delete(int id);

        // Consultas avançadas
        List<HealthActivity> GetByActivityType(string activityType);
        List<HealthActivity> GetByDateRange(DateTime startDate, DateTime endDate);
        List<HealthActivity> GetByCategory(ActivityCategory category);
        List<HealthActivity> GetRecent(int count);

        // Agregações
        List<string> GetActivityTypes();
        List<HealthActivity> Search(string searchTerm);
        Dictionary<DateTime, double> GetDailyTotals(string activityType, DateTime startDate, DateTime endDate);

        // Métodos de utilidade
        int GetNextId();
        bool Exists(int id);
        int GetCount();

        // Novos métodos adicionados
        ActivityType GetActivityTypeInfo(string activityType);
        List<ActivityType> GetAllActivityTypes();
    }
}