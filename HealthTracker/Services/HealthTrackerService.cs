using HealthTracker.DTOs;
using HealthTracker.Models;
using HealthTracker.Repository;

namespace HealthTracker.Services
{
    public class HealthTrackerService : IHealthTrackerService
    {
        private readonly IHealthActivityRepository _repository;
        private readonly IValidationService _validationService;

        public HealthTrackerService(IHealthActivityRepository repository, IValidationService validationService)
        {
            _repository = repository;
            _validationService = validationService;
        }

        public ServiceResult<HealthActivityResponseDTO> AddActivity(HealthActivityDTO activityDTO)
        {
            try
            {
                // Validar dados básicos
                var validationResult = _validationService.ValidateActivityData(
                    activityDTO.ActivityType,
                    activityDTO.Date.ToString("dd/MM/yyyy"),
                    activityDTO.Value.ToString(),
                    activityDTO.Notes
                );

                if (!validationResult.IsValid)
                {
                    return ServiceResult<HealthActivityResponseDTO>.Fail(validationResult.GetErrorMessages());
                }

                // Criar atividade
                var activity = new HealthActivity(
                    _repository.GetNextId(),
                    activityDTO.ActivityType,
                    activityDTO.Date,
                    activityDTO.Value,
                    activityDTO.Notes,
                    activityDTO.Duration,
                    activityDTO.Intensity
                );

                // Validar atividade completa
                var activityValidation = _validationService.ValidateActivity(activity);
                if (!activityValidation.IsValid)
                {
                    return ServiceResult<HealthActivityResponseDTO>.Fail(activityValidation.GetErrorMessages());
                }

                // Adicionar ao repositório
                _repository.Add(activity);

                // Criar resposta
                var response = MapToResponseDTO(activity);
                var result = ServiceResult<HealthActivityResponseDTO>.Ok(response);
                result.Warnings.AddRange(activityValidation.Warnings);

                return result;
            }
            catch (Exception ex)
            {
                return ServiceResult<HealthActivityResponseDTO>.Fail($"Erro ao adicionar atividade: {ex.Message}");
            }
        }

        public ServiceResult<HealthActivityResponseDTO> UpdateActivity(int id, HealthActivityDTO activityDTO)
        {
            try
            {
                var existingActivity = _repository.GetById(id);
                if (existingActivity == null)
                {
                    return ServiceResult<HealthActivityResponseDTO>.Fail("Atividade não encontrada");
                }

                // Validar dados
                var validationResult = _validationService.ValidateActivityData(
                    activityDTO.ActivityType,
                    activityDTO.Date.ToString("dd/MM/yyyy"),
                    activityDTO.Value.ToString(),
                    activityDTO.Notes
                );

                if (!validationResult.IsValid)
                {
                    return ServiceResult<HealthActivityResponseDTO>.Fail(validationResult.GetErrorMessages());
                }

                // Atualizar atividade
                var updatedActivity = new HealthActivity(
                    id,
                    activityDTO.ActivityType,
                    activityDTO.Date,
                    activityDTO.Value,
                    activityDTO.Notes,
                    activityDTO.Duration,
                    activityDTO.Intensity
                )
                {
                    CreatedAt = existingActivity.CreatedAt,
                    UpdatedAt = DateTime.Now
                };

                // Validar atividade atualizada
                var activityValidation = _validationService.ValidateActivity(updatedActivity);
                if (!activityValidation.IsValid)
                {
                    return ServiceResult<HealthActivityResponseDTO>.Fail(activityValidation.GetErrorMessages());
                }

                if (!_repository.Update(updatedActivity))
                {
                    return ServiceResult<HealthActivityResponseDTO>.Fail("Erro ao atualizar atividade");
                }

                var response = MapToResponseDTO(updatedActivity);
                var result = ServiceResult<HealthActivityResponseDTO>.Ok(response);
                result.Warnings.AddRange(activityValidation.Warnings);

                return result;
            }
            catch (Exception ex)
            {
                return ServiceResult<HealthActivityResponseDTO>.Fail($"Erro ao atualizar atividade: {ex.Message}");
            }
        }

        public ServiceResult<bool> DeleteActivity(int id)
        {
            try
            {
                if (!_repository.Exists(id))
                {
                    return ServiceResult<bool>.Fail("Atividade não encontrada");
                }

                var success = _repository.Delete(id);
                return success ?
                    ServiceResult<bool>.Ok(true) :
                    ServiceResult<bool>.Fail("Erro ao excluir atividade");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail($"Erro ao excluir atividade: {ex.Message}");
            }
        }

        public ServiceResult<HealthActivityResponseDTO> GetActivity(int id)
        {
            try
            {
                var activity = _repository.GetById(id);
                if (activity == null)
                {
                    return ServiceResult<HealthActivityResponseDTO>.Fail("Atividade não encontrada");
                }

                var response = MapToResponseDTO(activity);
                return ServiceResult<HealthActivityResponseDTO>.Ok(response);
            }
            catch (Exception ex)
            {
                return ServiceResult<HealthActivityResponseDTO>.Fail($"Erro ao buscar atividade: {ex.Message}");
            }
        }

        public ServiceResult<List<HealthActivityResponseDTO>> GetAllActivities()
        {
            try
            {
                var activities = _repository.GetAll();
                var response = activities.Select(MapToResponseDTO).ToList();
                return ServiceResult<List<HealthActivityResponseDTO>>.Ok(response);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<HealthActivityResponseDTO>>.Fail($"Erro ao buscar atividades: {ex.Message}");
            }
        }

        public ServiceResult<List<HealthActivityResponseDTO>> GetActivitiesByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (!_validationService.IsValidDateRange(startDate, endDate))
                {
                    return ServiceResult<List<HealthActivityResponseDTO>>.Fail("Intervalo de datas inválido");
                }

                var activities = _repository.GetByDateRange(startDate, endDate);
                var response = activities.Select(MapToResponseDTO).ToList();
                return ServiceResult<List<HealthActivityResponseDTO>>.Ok(response);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<HealthActivityResponseDTO>>.Fail($"Erro ao buscar atividades: {ex.Message}");
            }
        }

        public ServiceResult<List<HealthActivityResponseDTO>> SearchActivities(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return ServiceResult<List<HealthActivityResponseDTO>>.Fail("Termo de busca não pode estar vazio");
                }

                var activities = _repository.Search(searchTerm);
                var response = activities.Select(MapToResponseDTO).ToList();
                return ServiceResult<List<HealthActivityResponseDTO>>.Ok(response);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<HealthActivityResponseDTO>>.Fail($"Erro ao buscar atividades: {ex.Message}");
            }
        }

        public ServiceResult<List<HealthActivityResponseDTO>> GetRecentActivities(int count)
        {
            try
            {
                if (count <= 0 || count > 1000)
                {
                    return ServiceResult<List<HealthActivityResponseDTO>>.Fail("Contagem deve estar entre 1 e 1000");
                }

                var activities = _repository.GetRecent(count);
                var response = activities.Select(MapToResponseDTO).ToList();
                return ServiceResult<List<HealthActivityResponseDTO>>.Ok(response);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<HealthActivityResponseDTO>>.Fail($"Erro ao buscar atividades recentes: {ex.Message}");
            }
        }

        public ServiceResult<List<string>> GetAvailableActivityTypes()
        {
            try
            {
                var types = _repository.GetActivityTypes();
                return ServiceResult<List<string>>.Ok(types);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<string>>.Fail($"Erro ao buscar tipos de atividade: {ex.Message}");
            }
        }

        public ServiceResult<ActivityType> GetActivityTypeInfo(string activityType)
        {
            try
            {
                var info = _repository.GetActivityTypeInfo(activityType);
                return info != null ?
                    ServiceResult<ActivityType>.Ok(info) :
                    ServiceResult<ActivityType>.Fail("Tipo de atividade não encontrado");
            }
            catch (Exception ex)
            {
                return ServiceResult<ActivityType>.Fail($"Erro ao buscar informações do tipo de atividade: {ex.Message}");
            }
        }

        public ServiceResult<int> GetTotalActivitiesCount()
        {
            try
            {
                var count = _repository.GetCount();
                return ServiceResult<int>.Ok(count);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail($"Erro ao contar atividades: {ex.Message}");
            }
        }

        private HealthActivityResponseDTO MapToResponseDTO(HealthActivity activity)
        {
            var activityTypeInfo = _repository.GetActivityTypeInfo(activity.ActivityType);

            // CORREÇÃO: Calcular corretamente se está dentro da recomendação
            bool isWithinRecommendation = false;

            if (activityTypeInfo != null)
            {
                isWithinRecommendation = activity.Value >= activityTypeInfo.RecommendedMin &&
                                        activity.Value <= activityTypeInfo.RecommendedMax;
            }

            return new HealthActivityResponseDTO
            {
                Id = activity.Id,
                ActivityType = activity.ActivityType,
                Date = activity.Date,
                Value = activity.Value,
                Unit = activityTypeInfo?.Unit ?? "unidades",
                Notes = activity.Notes ?? "",
                Duration = activity.Duration,
                Intensity = activity.Intensity,
                CreatedAt = activity.CreatedAt,
                IsWithinRecommendation = isWithinRecommendation // CORREÇÃO AQUI
            };
        }
    }
}