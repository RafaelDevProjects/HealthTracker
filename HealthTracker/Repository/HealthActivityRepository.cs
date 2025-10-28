using HealthTracker.Models;
using HealthTracker.Enums;

namespace HealthTracker.Repository
{
    /// <summary>
    /// Implementação avançada do repositório com funcionalidades extendidas
    /// </summary>
    public class HealthActivityRepository : IHealthActivityRepository
    {
        private readonly List<HealthActivity> _activities = new List<HealthActivity>();
        private int _nextId = 1;
        private readonly object _lock = new object();

        // Tipos de atividade predefinidos
        private readonly Dictionary<string, ActivityType> _predefinedTypes = new Dictionary<string, ActivityType>
        {
            { "Exercício", new ActivityType("Exercício", "minutos", "Atividade física", 30, 180, ActivityCategory.Exercise) },
            { "Água", new ActivityType("Água", "litros", "Hidratação", 2, 4, ActivityCategory.Hydration) },
            { "Sono", new ActivityType("Sono", "horas", "Descanso noturno", 6, 9, ActivityCategory.Sleep) },
            { "Meditação", new ActivityType("Meditação", "minutos", "Prática mindfulness", 5, 60, ActivityCategory.MentalHealth) },
            { "Caminhada", new ActivityType("Caminhada", "minutos", "Caminhada leve a moderada", 20, 120, ActivityCategory.Exercise) },
            { "Alongamento", new ActivityType("Alongamento", "minutos", "Exercícios de flexibilidade", 10, 30, ActivityCategory.Exercise) },
            { "Yoga", new ActivityType("Yoga", "minutos", "Prática de yoga", 15, 90, ActivityCategory.Exercise) },
            { "Natação", new ActivityType("Natação", "minutos", "Natação recreativa ou competitiva", 20, 120, ActivityCategory.Exercise) },
            { "Corrida", new ActivityType("Corrida", "minutos", "Corrida leve a intensa", 15, 60, ActivityCategory.Exercise) },
            { "Ciclismo", new ActivityType("Ciclismo", "minutos", "Ciclismo recreativo", 30, 120, ActivityCategory.Exercise) }
        };

        public HealthActivity GetById(int id)
        {
            lock (_lock)
            {
                return _activities.FirstOrDefault(a => a.Id == id);
            }
        }

        public List<HealthActivity> GetAll()
        {
            lock (_lock)
            {
                return new List<HealthActivity>(_activities);
            }
        }

        public void Add(HealthActivity activity)
        {
            lock (_lock)
            {
                activity.Id = _nextId++;
                _activities.Add(activity);
            }
        }

        public bool Update(HealthActivity activity)
        {
            lock (_lock)
            {
                var existing = _activities.FirstOrDefault(a => a.Id == activity.Id);
                if (existing != null)
                {
                    existing.ActivityType = activity.ActivityType;
                    existing.Date = activity.Date;
                    existing.Value = activity.Value;
                    existing.Notes = activity.Notes;
                    existing.Duration = activity.Duration;
                    existing.Intensity = activity.Intensity;
                    existing.UpdatedAt = DateTime.Now;
                    return true;
                }
                return false;
            }
        }

        public bool Delete(int id)
        {
            lock (_lock)
            {
                var activity = _activities.FirstOrDefault(a => a.Id == id);
                if (activity != null)
                {
                    return _activities.Remove(activity);
                }
                return false;
            }
        }

        public List<HealthActivity> GetByActivityType(string activityType)
        {
            lock (_lock)
            {
                return _activities
                    .Where(a => a.ActivityType.Equals(activityType, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
        }

        public List<HealthActivity> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            lock (_lock)
            {
                return _activities
                    .Where(a => a.Date >= startDate && a.Date <= endDate)
                    .ToList();
            }
        }

        public List<HealthActivity> GetByCategory(ActivityCategory category)
        {
            lock (_lock)
            {
                var typesInCategory = _predefinedTypes
                    .Where(t => t.Value.Category == category)
                    .Select(t => t.Key)
                    .ToList();

                // Também incluir tipos personalizados que possam pertencer à categoria
                var customTypesInCategory = _activities
                    .Select(a => a.ActivityType)
                    .Distinct()
                    .Where(type => !_predefinedTypes.ContainsKey(type))
                    .Where(type => InferCategoryFromType(type) == category)
                    .ToList();

                var allTypesInCategory = typesInCategory.Concat(customTypesInCategory).ToList();

                return _activities
                    .Where(a => allTypesInCategory.Contains(a.ActivityType, StringComparer.OrdinalIgnoreCase))
                    .ToList();
            }
        }

        public List<HealthActivity> GetRecent(int count)
        {
            lock (_lock)
            {
                return _activities
                    .OrderByDescending(a => a.Date)
                    .ThenByDescending(a => a.CreatedAt)
                    .Take(count)
                    .ToList();
            }
        }

        public List<string> GetActivityTypes()
        {
            var predefinedTypes = _predefinedTypes.Keys.ToList();
            var customTypes = _activities
                .Select(a => a.ActivityType)
                .Distinct()
                .Where(type => !_predefinedTypes.ContainsKey(type))
                .ToList();

            return predefinedTypes.Concat(customTypes).Distinct().ToList();
        }

        public List<HealthActivity> Search(string searchTerm)
        {
            lock (_lock)
            {
                return _activities
                    .Where(a => a.ActivityType.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                               a.Notes.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
        }

        public Dictionary<DateTime, double> GetDailyTotals(string activityType, DateTime startDate, DateTime endDate)
        {
            lock (_lock)
            {
                return _activities
                    .Where(a => a.ActivityType.Equals(activityType, StringComparison.OrdinalIgnoreCase) &&
                               a.Date >= startDate && a.Date <= endDate)
                    .GroupBy(a => a.Date.Date)
                    .ToDictionary(g => g.Key, g => g.Sum(a => a.Value));
            }
        }

        public int GetNextId()
        {
            lock (_lock)
            {
                return _nextId;
            }
        }

        public bool Exists(int id)
        {
            lock (_lock)
            {
                return _activities.Any(a => a.Id == id);
            }
        }

        public int GetCount()
        {
            lock (_lock)
            {
                return _activities.Count;
            }
        }

        /// <summary>
        /// Obtém informações detalhadas sobre um tipo de atividade
        /// </summary>
        public ActivityType GetActivityTypeInfo(string activityType)
        {
            if (_predefinedTypes.ContainsKey(activityType))
            {
                return _predefinedTypes[activityType];
            }

            // Para tipos personalizados, criar um ActivityType básico
            var unit = GetUnitForCustomActivityType(activityType);
            var category = InferCategoryFromType(activityType);
            return new ActivityType(activityType, unit, "Atividade personalizada", 0, 100, category);
        }

        /// <summary>
        /// Obtém todos os tipos de atividade predefinidos
        /// </summary>
        public List<ActivityType> GetAllActivityTypes()
        {
            return _predefinedTypes.Values.ToList();
        }

        /// <summary>
        /// Determina a unidade para tipos de atividade personalizados
        /// </summary>
        private string GetUnitForCustomActivityType(string activityType)
        {
            var lowerType = activityType.ToLower();

            if (lowerType.Contains("exercício") || lowerType.Contains("exercicio") ||
                lowerType.Contains("corrida") || lowerType.Contains("caminhada") ||
                lowerType.Contains("yoga") || lowerType.Contains("musculação") ||
                lowerType.Contains("natação") || lowerType.Contains("natacao") ||
                lowerType.Contains("academia") || lowerType.Contains("fitness"))
                return "minutos";

            if (lowerType.Contains("água") || lowerType.Contains("agua") ||
                lowerType.Contains("líquido") || lowerType.Contains("liquido") ||
                lowerType.Contains("bebida") || lowerType.Contains("hidratação"))
                return "litros";

            if (lowerType.Contains("sono") || lowerType.Contains("dormir") ||
                lowerType.Contains("descanso") || lowerType.Contains("repouso"))
                return "horas";

            if (lowerType.Contains("peso") || lowerType.Contains("balança"))
                return "kg";

            if (lowerType.Contains("pressão") || lowerType.Contains("sanguínea"))
                return "mmHg";

            if (lowerType.Contains("glicose") || lowerType.Contains("açúcar") || lowerType.Contains("acucar"))
                return "mg/dL";

            return "unidades";
        }

        /// <summary>
        /// Infere a categoria de um tipo de atividade personalizado
        /// </summary>
        private ActivityCategory InferCategoryFromType(string activityType)
        {
            var lowerType = activityType.ToLower();

            if (lowerType.Contains("exercício") || lowerType.Contains("exercicio") ||
                lowerType.Contains("corrida") || lowerType.Contains("caminhada") ||
                lowerType.Contains("yoga") || lowerType.Contains("musculação") ||
                lowerType.Contains("natação") || lowerType.Contains("natacao") ||
                lowerType.Contains("academia") || lowerType.Contains("fitness"))
                return ActivityCategory.Exercise;

            if (lowerType.Contains("água") || lowerType.Contains("agua") ||
                lowerType.Contains("líquido") || lowerType.Contains("liquido") ||
                lowerType.Contains("bebida") || lowerType.Contains("hidratação"))
                return ActivityCategory.Hydration;

            if (lowerType.Contains("sono") || lowerType.Contains("dormir") ||
                lowerType.Contains("descanso") || lowerType.Contains("repouso"))
                return ActivityCategory.Sleep;

            if (lowerType.Contains("meditação") || lowerType.Contains("meditacao") ||
                lowerType.Contains("mindfulness") || lowerType.Contains("relaxamento") ||
                lowerType.Contains("respiração") || lowerType.Contains("respiracao"))
                return ActivityCategory.MentalHealth;

            if (lowerType.Contains("comida") || lowerType.Contains("alimento") ||
                lowerType.Contains("refeição") || lowerType.Contains("refeicao") ||
                lowerType.Contains("dieta") || lowerType.Contains("caloria"))
                return ActivityCategory.Nutrition;

            if (lowerType.Contains("pressão") || lowerType.Contains("sanguínea") ||
                lowerType.Contains("glicose") || lowerType.Contains("açúcar") ||
                lowerType.Contains("acucar") || lowerType.Contains("medicamento") ||
                lowerType.Contains("vitamina") || lowerType.Contains("suplemento"))
                return ActivityCategory.Medical;

            return ActivityCategory.Lifestyle;
        }

        /// <summary>
        /// Obtém a unidade apropriada para um tipo de atividade
        /// </summary>
        public string GetUnitForActivityType(string activityType)
        {
            var info = GetActivityTypeInfo(activityType);
            return info?.Unit ?? "unidades";
        }
    }
}