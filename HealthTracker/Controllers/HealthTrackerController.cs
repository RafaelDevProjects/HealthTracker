using HealthTracker.Services;
using HealthTracker.DTOs;
using HealthTracker.Utils;
using HealthTracker.Enums;

namespace HealthTracker.Controllers
{
    /// <summary>
    /// Controller avançado para gerenciar a interface do usuário
    /// </summary>
    public class HealthTrackerController
    {
        private readonly IHealthTrackerService _healthService;
        private readonly IStatisticsService _statisticsService;
        private readonly IReportService _reportService;
        private readonly IValidationService _validationService;

        public HealthTrackerController(
            IHealthTrackerService healthService,
            IStatisticsService statisticsService,
            IReportService reportService,
            IValidationService validationService)
        {
            _healthService = healthService;
            _statisticsService = statisticsService;
            _reportService = reportService;
            _validationService = validationService;
        }

        public void Run()
        {
            ConsoleHelper.PrintHeader("RASTREADOR AVANÇADO DE ATIVIDADES DE SAÚDE");
            Console.WriteLine("Bem-vindo ao seu diário de saúde pessoal!");

            while (true)
            {
                DisplayMainMenu();
                var option = ConsoleHelper.ReadIntWithPrompt("Escolha uma opção", 1, 9);

                try
                {
                    ProcessMainMenuOption(option);
                }
                catch (Exception ex)
                {
                    ConsoleHelper.PrintError($"Erro inesperado: {ex.Message}");
                    ConsoleHelper.WaitForContinue();
                }

                ConsoleHelper.ClearScreen();
            }
        }

        private void DisplayMainMenu()
        {
            ConsoleHelper.PrintHeader("MENU PRINCIPAL");
            Console.WriteLine("1. Adicionar Registro");
            Console.WriteLine("2. Listar Registros");
            Console.WriteLine("3. Gerenciar Registros");
            Console.WriteLine("4. Estatísticas Detalhadas");
            Console.WriteLine("5. Análise de Tendências");
            Console.WriteLine("6. Relatórios");
            Console.WriteLine("7. Buscar Registros");
            Console.WriteLine("8. Informações do Sistema");
            Console.WriteLine("9. Sair");
        }

        private void ProcessMainMenuOption(int option)
        {
            switch (option)
            {
                case 1:
                    AddActivity();
                    break;
                case 2:
                    ListActivities();
                    break;
                case 3:
                    ManageActivities();
                    break;
                case 4:
                    ShowDetailedStatistics();
                    break;
                case 5:
                    ShowTrendAnalysis();
                    break;
                case 6:
                    GenerateReports();
                    break;
                case 7:
                    SearchActivities();
                    break;
                case 8:
                    ShowSystemInfo();
                    break;
                case 9:
                    ExitApplication();
                    break;
            }
        }

        private void AddActivity()
        {
            ConsoleHelper.PrintHeader("ADICIONAR NOVO REGISTRO");

            // Mostrar exemplo de formato de data
            DateHelper.DisplayDateFormatHelp();

            // Obter tipo de atividade
            var typesResult = _healthService.GetAvailableActivityTypes();
            if (!typesResult.Success)
            {
                ConsoleHelper.PrintError(typesResult.ErrorMessage);
                return;
            }

            Console.WriteLine("Tipos de atividade disponíveis:");
            for (int i = 0; i < typesResult.Data.Count; i++)
            {
                var typeInfo = _healthService.GetActivityTypeInfo(typesResult.Data[i]);
                if (typeInfo.Success)
                {
                    Console.WriteLine($"  {i + 1}. {typeInfo.Data.Name} ({typeInfo.Data.Unit}) - {typeInfo.Data.Description}");
                }
                else
                {
                    Console.WriteLine($"  {i + 1}. {typesResult.Data[i]}");
                }
            }

            // Ler o número e mapear para o nome do tipo
            var typeNumber = ConsoleHelper.ReadIntWithPrompt("Número do tipo de atividade", 1, typesResult.Data.Count);
            var activityType = typesResult.Data[typeNumber - 1];

            // Obter data com formato brasileiro
            var date = ConsoleHelper.ReadDateWithPrompt("Data", true);

            // Obter valor
            var value = ConsoleHelper.ReadDoubleWithPrompt("Valor", 0, 10000);

            // Obter dados opcionais
            var notes = ConsoleHelper.ReadLineWithPrompt("Notas (opcional)");

            Console.WriteLine("Intensidade (1-10, opcional - pressione Enter para pular):");
            var intensityInput = Console.ReadLine()?.Trim();
            var intensity = 5; // valor padrão
            if (!string.IsNullOrEmpty(intensityInput) && int.TryParse(intensityInput, out int parsedIntensity) && parsedIntensity >= 1 && parsedIntensity <= 10)
            {
                intensity = parsedIntensity;
            }

            // Criar DTO
            var activityDTO = new HealthActivityDTO(activityType, date, value, notes, null, intensity);

            // Adicionar atividade
            var result = _healthService.AddActivity(activityDTO);

            if (result.Success)
            {
                ConsoleHelper.PrintSuccess("Registro adicionado com sucesso!");
                if (result.Warnings.Any())
                {
                    foreach (var warning in result.Warnings)
                    {
                        ConsoleHelper.PrintWarning(warning);
                    }
                }
            }
            else
            {
                ConsoleHelper.PrintError(result.ErrorMessage);
            }

            ConsoleHelper.WaitForContinue();
        }

        private void ListActivities()
        {
            ConsoleHelper.PrintHeader("LISTAR REGISTROS");

            var filterOption = ConsoleHelper.ReadIntWithPrompt(
                "Filtrar por:\n1. Todos\n2. Por data\n3. Recentes\n4. Por tipo\nEscolha", 1, 4);

            ServiceResult<List<HealthActivityResponseDTO>> result;

            switch (filterOption)
            {
                case 1:
                    result = _healthService.GetAllActivities();
                    break;
                case 2:
                    var startDate = ConsoleHelper.ReadDateWithPrompt("Data inicial");
                    var endDate = ConsoleHelper.ReadDateWithPrompt("Data final");
                    result = _healthService.GetActivitiesByDateRange(startDate, endDate);
                    break;
                case 3:
                    var count = ConsoleHelper.ReadIntWithPrompt("Quantidade de registros recentes", 1, 100);
                    result = _healthService.GetRecentActivities(count);
                    break;
                case 4:
                    var typesResult = _healthService.GetAvailableActivityTypes();
                    if (!typesResult.Success || !typesResult.Data.Any())
                    {
                        ConsoleHelper.PrintError("Nenhum tipo de atividade disponível.");
                        ConsoleHelper.WaitForContinue();
                        return;
                    }

                    Console.WriteLine("\nTipos de atividade disponíveis:");
                    for (int i = 0; i < typesResult.Data.Count; i++)
                    {
                        Console.WriteLine($"  {i + 1}. {typesResult.Data[i]}");
                    }

                    string activityType;
                    while (true)
                    {
                        var input = ConsoleHelper.ReadLineWithPrompt("Tipo de atividade (digite o número ou nome)", true);

                        // Se o usuário digitou um número
                        if (int.TryParse(input, out int number) && number >= 1 && number <= typesResult.Data.Count)
                        {
                            activityType = typesResult.Data[number - 1];
                            break;
                        }
                        // Se o usuário digitou um nome que existe na lista
                        else if (typesResult.Data.Contains(input, StringComparer.OrdinalIgnoreCase))
                        {
                            activityType = typesResult.Data.First(t =>
                                t.Equals(input, StringComparison.OrdinalIgnoreCase));
                            break;
                        }
                        else
                        {
                            ConsoleHelper.PrintError($"Tipo inválido! Digite um número (1-{typesResult.Data.Count}) ou o nome da atividade.");
                            Console.WriteLine("Tipos disponíveis:");
                            for (int i = 0; i < typesResult.Data.Count; i++)
                            {
                                Console.WriteLine($"  {i + 1}. {typesResult.Data[i]}");
                            }
                        }
                    }

                    // Buscar atividades pelo tipo selecionado
                    var allActivities = _healthService.GetAllActivities();
                    if (allActivities.Success)
                    {
                        result = ServiceResult<List<HealthActivityResponseDTO>>.Ok(
                            allActivities.Data
                                .Where(a => a.ActivityType.Equals(activityType, StringComparison.OrdinalIgnoreCase))
                                .ToList()
                        );
                    }
                    else
                    {
                        result = allActivities;
                    }
                    break;
                default:
                    result = _healthService.GetAllActivities();
                    break;
            }

            if (!result.Success)
            {
                ConsoleHelper.PrintError(result.ErrorMessage);
                ConsoleHelper.WaitForContinue();
                return;
            }

            if (!result.Data.Any())
            {
                ConsoleHelper.PrintInfo("Nenhum registro encontrado.");
                ConsoleHelper.WaitForContinue();
                return;
            }

            DisplayActivitiesTable(result.Data);
            ConsoleHelper.WaitForContinue();
        }

        private void ManageActivities()
        {
            ConsoleHelper.PrintHeader("GERENCIAR REGISTROS");

            var manageOption = ConsoleHelper.ReadIntWithPrompt(
                "Opções:\n1. Editar registro\n2. Excluir registro\n3. Voltar\nEscolha", 1, 3);

            if (manageOption == 3) return;

            var activitiesResult = _healthService.GetAllActivities();
            if (!activitiesResult.Success || !activitiesResult.Data.Any())
            {
                ConsoleHelper.PrintError("Nenhum registro disponível para gerenciar.");
                ConsoleHelper.WaitForContinue();
                return;
            }

            DisplayActivitiesTable(activitiesResult.Data);

            var activityId = ConsoleHelper.ReadIntWithPrompt("ID do registro", 1);

            if (manageOption == 1)
            {
                EditActivity(activityId);
            }
            else if (manageOption == 2)
            {
                DeleteActivity(activityId);
            }
        }

        private void EditActivity(int id)
        {
            var activityResult = _healthService.GetActivity(id);
            if (!activityResult.Success)
            {
                ConsoleHelper.PrintError(activityResult.ErrorMessage);
                return;
            }

            ConsoleHelper.PrintSubHeader($"Editando Registro #{id}");

            // Mostrar dados atuais
            Console.WriteLine($"Dados atuais:");
            Console.WriteLine($"• Tipo: {activityResult.Data.ActivityType}");
            Console.WriteLine($"• Data: {activityResult.Data.Date.ToBrazilianDate()}");
            Console.WriteLine($"• Valor: {activityResult.Data.Value}");
            Console.WriteLine($"• Notas: {activityResult.Data.Notes}");
            Console.WriteLine($"• Intensidade: {activityResult.Data.Intensity}");
            Console.WriteLine();

            // Mostrar tipos disponíveis
            var typesResult = _healthService.GetAvailableActivityTypes();
            if (typesResult.Success && typesResult.Data.Any())
            {
                Console.WriteLine("\nTipos de atividade disponíveis:");
                for (int i = 0; i < typesResult.Data.Count; i++)
                {
                    Console.WriteLine($"  {i + 1}. {typesResult.Data[i]}");
                }
            }

            string newType;
            while (true)
            {
                var input = ConsoleHelper.ReadLineWithPrompt($"Novo tipo de atividade [{activityResult.Data.ActivityType}] (digite número ou nome)")
                           ?? activityResult.Data.ActivityType;

                // Se o usuário digitou um número
                if (int.TryParse(input, out int number) && number >= 1 && number <= typesResult.Data.Count)
                {
                    newType = typesResult.Data[number - 1];
                    break;
                }
                // Se o usuário digitou um nome que existe na lista
                else if (typesResult.Data.Contains(input, StringComparer.OrdinalIgnoreCase))
                {
                    newType = typesResult.Data.First(t =>
                        t.Equals(input, StringComparison.OrdinalIgnoreCase));
                    break;
                }
                else if (input.Equals(activityResult.Data.ActivityType, StringComparison.OrdinalIgnoreCase))
                {
                    newType = activityResult.Data.ActivityType;
                    break;
                }
                else
                {
                    ConsoleHelper.PrintError($"Tipo inválido! Digite um número (1-{typesResult.Data.Count}) ou o nome da atividade.");
                    Console.WriteLine("Tipos disponíveis:");
                    for (int i = 0; i < typesResult.Data.Count; i++)
                    {
                        Console.WriteLine($"  {i + 1}. {typesResult.Data[i]}");
                    }
                }
            }

            var newDate = ConsoleHelper.ReadDateWithPrompt($"Nova data [{activityResult.Data.Date.ToBrazilianDate()}]");
            var newValue = ConsoleHelper.ReadDoubleWithPrompt($"Novo valor [{activityResult.Data.Value}]", 0, 10000);
            var newNotes = ConsoleHelper.ReadLineWithPrompt($"Novas notas [{activityResult.Data.Notes}]")
                          ?? activityResult.Data.Notes;
            var newIntensity = ConsoleHelper.ReadIntWithPrompt($"Nova intensidade [{activityResult.Data.Intensity}]", 1, 10);

            var updateDTO = new HealthActivityDTO(newType, newDate, newValue, newNotes, null, newIntensity);
            var result = _healthService.UpdateActivity(id, updateDTO);

            if (result.Success)
            {
                ConsoleHelper.PrintSuccess("Registro atualizado com sucesso!");
            }
            else
            {
                ConsoleHelper.PrintError(result.ErrorMessage);
            }

            ConsoleHelper.WaitForContinue();
        }

        private void DeleteActivity(int id)
        {
            var activityResult = _healthService.GetActivity(id);
            if (!activityResult.Success)
            {
                ConsoleHelper.PrintError(activityResult.ErrorMessage);
                return;
            }

            ConsoleHelper.PrintSubHeader("CONFIRMAR EXCLUSÃO");
            Console.WriteLine($"Registro a ser excluído:");
            Console.WriteLine($"• ID: {activityResult.Data.Id}");
            Console.WriteLine($"• Tipo: {activityResult.Data.ActivityType}");
            Console.WriteLine($"• Data: {activityResult.Data.Date.ToBrazilianDate()}");
            Console.WriteLine($"• Valor: {activityResult.Data.Value} {activityResult.Data.Unit}");
            Console.WriteLine($"• Notas: {activityResult.Data.Notes}");
            Console.WriteLine();

            var confirm = ConsoleHelper.ReadLineWithPrompt("Tem certeza que deseja excluir este registro? (s/N)");
            if (confirm.Equals("s", StringComparison.OrdinalIgnoreCase))
            {
                var result = _healthService.DeleteActivity(id);
                if (result.Success)
                {
                    ConsoleHelper.PrintSuccess("Registro excluído com sucesso!");
                }
                else
                {
                    ConsoleHelper.PrintError(result.ErrorMessage);
                }
            }
            else
            {
                ConsoleHelper.PrintInfo("Exclusão cancelada.");
            }

            ConsoleHelper.WaitForContinue();
        }

        private void ShowDetailedStatistics()
        {
            ConsoleHelper.PrintHeader("ESTATÍSTICAS DETALHADAS");

            DateHelper.DisplayDateFormatHelp();

            var typesResult = _healthService.GetAvailableActivityTypes();
            if (!typesResult.Success || !typesResult.Data.Any())
            {
                ConsoleHelper.PrintError("Nenhum tipo de atividade disponível.");
                ConsoleHelper.WaitForContinue();
                return;
            }

            Console.WriteLine("Tipos de atividade disponíveis:");
            for (int i = 0; i < typesResult.Data.Count; i++)
            {
                Console.WriteLine($"  {i + 1}. {typesResult.Data[i]}");
            }

            string selectedType;
            while (true)
            {
                var input = ConsoleHelper.ReadLineWithPrompt("Tipo de atividade para análise (digite o número ou nome)", true);

                // Se o usuário digitou um número
                if (int.TryParse(input, out int number) && number >= 1 && number <= typesResult.Data.Count)
                {
                    selectedType = typesResult.Data[number - 1];
                    break;
                }
                // Se o usuário digitou um nome que existe na lista
                else if (typesResult.Data.Contains(input, StringComparer.OrdinalIgnoreCase))
                {
                    selectedType = typesResult.Data.First(t =>
                        t.Equals(input, StringComparison.OrdinalIgnoreCase));
                    break;
                }
                else
                {
                    ConsoleHelper.PrintError($"Tipo inválido! Digite um número (1-{typesResult.Data.Count}) ou o nome da atividade.");
                    Console.WriteLine("Tipos disponíveis:");
                    for (int i = 0; i < typesResult.Data.Count; i++)
                    {
                        Console.WriteLine($"  {i + 1}. {typesResult.Data[i]}");
                    }
                }
            }

            var startDate = ConsoleHelper.ReadDateWithPrompt("Data inicial");
            var endDate = ConsoleHelper.ReadDateWithPrompt("Data final");

            if (!_validationService.IsValidDateRange(startDate, endDate))
            {
                ConsoleHelper.PrintError("Intervalo de datas inválido.");
                ConsoleHelper.WaitForContinue();
                return;
            }

            var summary = _statisticsService.GetActivitySummary(selectedType, startDate, endDate);

            ConsoleHelper.PrintSubHeader($"Estatísticas - {selectedType}");
            Console.WriteLine($"Período: {startDate.ToBrazilianDate()} - {endDate.ToBrazilianDate()}");
            Console.WriteLine($"Total: {summary.Total:F1} {summary.Unit}");
            Console.WriteLine($"Média: {summary.Average:F1} {summary.Unit}");
            Console.WriteLine($"Máximo: {summary.Maximum:F1} {summary.Unit}");
            Console.WriteLine($"Mínimo: {summary.Minimum:F1} {summary.Unit}");
            Console.WriteLine($"Contagem: {summary.Count} registros");
            Console.WriteLine($"Conformidade: {summary.ComplianceRate:F1}%");
            Console.WriteLine($"Tendência: {(summary.Trend > 0 ? "Aumentando" : summary.Trend < 0 ? "Diminuindo" : "Estavel")} {Math.Abs(summary.Trend):F2}");

            // Estatísticas detalhadas por período
            var detailedStats = _statisticsService.GetDetailedStatistics(selectedType, ReportPeriod.Weekly, startDate, endDate);
            if (detailedStats.Any())
            {
                ConsoleHelper.PrintSubHeader("Estatísticas Semanais");
                foreach (var stat in detailedStats.Take(5))
                {
                    Console.WriteLine($"{stat.PeriodStart.ToBrazilianDate()} - {stat.PeriodEnd.ToBrazilianDate()}: {stat.Value:F1} {stat.Unit}");
                }
            }

            // Comparação com recomendações
            var activityTypeInfo = _healthService.GetActivityTypeInfo(selectedType);
            if (activityTypeInfo.Success)
            {
                ConsoleHelper.PrintSubHeader("Recomendações");
                Console.WriteLine($"Faixa recomendada: {activityTypeInfo.Data.RecommendedMin} - {activityTypeInfo.Data.RecommendedMax} {activityTypeInfo.Data.Unit}");
                Console.WriteLine($"{activityTypeInfo.Data.Description}");

                if (summary.Average < activityTypeInfo.Data.RecommendedMin)
                {
                    ConsoleHelper.PrintWarning($"Sua média está abaixo do recomendado!");
                }
                else if (summary.Average > activityTypeInfo.Data.RecommendedMax)
                {
                    ConsoleHelper.PrintWarning($"Sua média está acima do recomendado!");
                }
                else
                {
                    ConsoleHelper.PrintSuccess($"Sua média está dentro da faixa recomendada!");
                }
            }

            ConsoleHelper.WaitForContinue();
        }

        private void ShowTrendAnalysis()
        {
            ConsoleHelper.PrintHeader("ANÁLISE DE TENDÊNCIAS");

            // Mostrar exemplo de formato de data
            DateHelper.DisplayDateFormatHelp();

            var typesResult = _healthService.GetAvailableActivityTypes();
            if (!typesResult.Success || !typesResult.Data.Any())
            {
                ConsoleHelper.PrintError("Nenhum tipo de atividade disponível.");
                ConsoleHelper.WaitForContinue();
                return;
            }

            Console.WriteLine("Tipos de atividade disponíveis:");
            for (int i = 0; i < typesResult.Data.Count; i++)
            {
                Console.WriteLine($"  {i + 1}. {typesResult.Data[i]}");
            }

            string selectedType;
            while (true)
            {
                var input = ConsoleHelper.ReadLineWithPrompt("Tipo de atividade (digite o número ou nome)", true);

                // Se o usuário digitou um número
                if (int.TryParse(input, out int number) && number >= 1 && number <= typesResult.Data.Count)
                {
                    selectedType = typesResult.Data[number - 1];
                    break;
                }
                // Se o usuário digitou um nome que existe na lista
                else if (typesResult.Data.Contains(input, StringComparer.OrdinalIgnoreCase))
                {
                    selectedType = typesResult.Data.First(t =>
                        t.Equals(input, StringComparison.OrdinalIgnoreCase));
                    break;
                }
                else
                {
                    ConsoleHelper.PrintError($"Tipo inválido! Digite um número (1-{typesResult.Data.Count}) ou o nome da atividade.");
                    Console.WriteLine("Tipos disponíveis:");
                    for (int i = 0; i < typesResult.Data.Count; i++)
                    {
                        Console.WriteLine($"  {i + 1}. {typesResult.Data[i]}");
                    }
                }
            }

            var startDate = ConsoleHelper.ReadDateWithPrompt("Data inicial");
            var endDate = ConsoleHelper.ReadDateWithPrompt("Data final");

            var trends = _statisticsService.GetTrendAnalysis(selectedType, startDate, endDate);

            if (!trends.Any())
            {
                ConsoleHelper.PrintInfo("Dados insuficientes para análise de tendências.");
                ConsoleHelper.WaitForContinue();
                return;
            }

            ConsoleHelper.PrintSubHeader($"Tendências - {selectedType}");

            var recentTrends = trends.TakeLast(10).ToList();
            foreach (var trend in recentTrends)
            {
                var trendDirection = trend.Value > 0 ? "Aumento" : trend.Value < 0 ? "Queda" : "Estavel";
                var changeDirection = trend.PercentageChange > 0 ? "acima" : trend.PercentageChange < 0 ? "abaixo" : "estavel";

                Console.WriteLine($"{trendDirection} {trend.PeriodStart.ToBrazilianDate()} para {trend.PeriodEnd.ToBrazilianDate()}: " +
                                $"{trend.Value:+#;-#;0} {trend.Unit} ({changeDirection} {Math.Abs(trend.PercentageChange):F1}%)");
            }

            // Resumo da tendência geral
            var overallTrend = recentTrends.Average(t => t.Value);
            var overallPercentage = recentTrends.Average(t => t.PercentageChange);

            ConsoleHelper.PrintSubHeader("Resumo da Tendência");
            Console.WriteLine($"Tendência geral: {(overallTrend > 0 ? "Aumentando" : overallTrend < 0 ? "Diminuindo" : "Estavel")}");
            Console.WriteLine($"Variação média: {overallTrend:+#;-#;0} {recentTrends.FirstOrDefault()?.Unit ?? "unidades"}");
            Console.WriteLine($"Percentual médio: {overallPercentage:+#;-#;0}%");

            // Correlações
            var otherTypes = typesResult.Data.Where(t => t != selectedType).Take(3).ToList();
            if (otherTypes.Any())
            {
                var correlationTypes = new List<string> { selectedType }.Concat(otherTypes).ToList();
                var correlations = _statisticsService.GetCorrelations(correlationTypes, startDate, endDate);

                if (correlations.Any())
                {
                    ConsoleHelper.PrintSubHeader("Correlações");
                    foreach (var correlation in correlations.Where(c => c.Key.StartsWith(selectedType)))
                    {
                        var strength = Math.Abs(correlation.Value);
                        var strengthDesc = strength > 0.7 ? "Forte" : strength > 0.3 ? "Moderada" : "Fraca";
                        var direction = correlation.Value > 0 ? "positiva" : "negativa";

                        Console.WriteLine($"{correlation.Key}: {strengthDesc} correlação {direction} ({correlation.Value:F2})");
                    }
                }
            }

            ConsoleHelper.WaitForContinue();
        }

        private void GenerateReports()
        {
            ConsoleHelper.PrintHeader("RELATÓRIOS");

            // Mostrar exemplo de formato de data
            DateHelper.DisplayDateFormatHelp();

            var reportOption = ConsoleHelper.ReadIntWithPrompt(
                "Tipo de relatório:\n1. Atividades por período\n2. Resumo geral de saúde\n3. Conformidade\n4. Voltar\nEscolha", 1, 4);

            if (reportOption == 4) return;

            var startDate = ConsoleHelper.ReadDateWithPrompt("Data inicial");
            var endDate = ConsoleHelper.ReadDateWithPrompt("Data final");

            if (!_validationService.IsValidDateRange(startDate, endDate))
            {
                ConsoleHelper.PrintError("Intervalo de datas inválido.");
                ConsoleHelper.WaitForContinue();
                return;
            }

            ActivityReportDTO report;

            switch (reportOption)
            {
                case 1:
                    var period = (ReportPeriod)ConsoleHelper.ReadIntWithPrompt(
                        "Período:\n1. Diário\n2. Semanal\n3. Mensal\n4. Anual\nEscolha", 1, 4);
                    report = _reportService.GenerateActivityReport(period, startDate, endDate);
                    break;
                case 2:
                    report = _reportService.GenerateHealthSummaryReport(startDate, endDate);
                    break;
                case 3:
                    report = _reportService.GenerateComplianceReport(startDate, endDate);
                    break;
                default:
                    return;
            }

            DisplayReport(report);

            // Opção de exportação
            var exportOption = ConsoleHelper.ReadLineWithPrompt("Exportar relatório? (s/N)");
            if (exportOption.Equals("s", StringComparison.OrdinalIgnoreCase))
            {
                ExportReport(report);
            }

            ConsoleHelper.WaitForContinue();
        }

        private void SearchActivities()
        {
            ConsoleHelper.PrintHeader("BUSCAR REGISTROS");

            // Mostrar tipos disponíveis para ajudar na busca
            var typesResult = _healthService.GetAvailableActivityTypes();
            if (typesResult.Success && typesResult.Data.Any())
            {
                Console.WriteLine("\nTipos de atividade disponíveis para busca:");
                for (int i = 0; i < typesResult.Data.Count; i++)
                {
                    Console.WriteLine($"  {i + 1}. {typesResult.Data[i]}");
                }
            }

            var searchTerm = ConsoleHelper.ReadLineWithPrompt("Termo de busca (pode ser número do tipo, nome do tipo ou texto nas notas)", true);

            // Se o usuário digitou um número, converter para o nome do tipo
            if (int.TryParse(searchTerm, out int typeNumber) && typeNumber >= 1 && typeNumber <= typesResult.Data.Count)
            {
                searchTerm = typesResult.Data[typeNumber - 1];
            }

            var result = _healthService.SearchActivities(searchTerm);

            if (!result.Success)
            {
                ConsoleHelper.PrintError(result.ErrorMessage);
                ConsoleHelper.WaitForContinue();
                return;
            }

            if (!result.Data.Any())
            {
                ConsoleHelper.PrintInfo("Nenhum registro encontrado.");
                ConsoleHelper.WaitForContinue();
                return;
            }

            ConsoleHelper.PrintInfo($"Encontrados {result.Data.Count} registros:");
            DisplayActivitiesTable(result.Data);
            ConsoleHelper.WaitForContinue();
        }

        private void ShowSystemInfo()
        {
            ConsoleHelper.PrintHeader("INFORMAÇÕES DO SISTEMA");

            var countResult = _healthService.GetTotalActivitiesCount();
            var typesResult = _healthService.GetAvailableActivityTypes();

            if (countResult.Success)
            {
                Console.WriteLine($"Total de registros: {countResult.Data}");
            }

            if (typesResult.Success)
            {
                Console.WriteLine($"Tipos de atividade: {typesResult.Data.Count}");
                Console.WriteLine($"\nTipos registrados:");
                foreach (var type in typesResult.Data.Take(10))
                {
                    var info = _healthService.GetActivityTypeInfo(type);
                    if (info.Success)
                    {
                        Console.WriteLine($"  • {info.Data.Name} ({info.Data.Unit}): {info.Data.Description}");
                    }
                    else
                    {
                        Console.WriteLine($"  • {type}");
                    }
                }

                if (typesResult.Data.Count > 10)
                {
                    Console.WriteLine($"  • ... e mais {typesResult.Data.Count - 10} tipos");
                }
            }

            // Estatísticas de uso
            var activitiesResult = _healthService.GetAllActivities();
            if (activitiesResult.Success && activitiesResult.Data.Any())
            {
                var firstActivity = activitiesResult.Data.Min(a => a.Date);
                var lastActivity = activitiesResult.Data.Max(a => a.Date);

                Console.WriteLine($"\nPeríodo coberto: {firstActivity.ToBrazilianDate()} - {lastActivity.ToBrazilianDate()}");
                Console.WriteLine($"Dias com registros: {activitiesResult.Data.Select(a => a.Date.Date).Distinct().Count()} dias");
            }

            Console.WriteLine($"\nDesenvolvido com .NET 8");
            Console.WriteLine($"Arquitetura: MVC com Repository Pattern");
            Console.WriteLine($"Última atualização: {DateTime.Now.ToBrazilianDateTime()}");

            ConsoleHelper.WaitForContinue();
        }

        private void ExitApplication()
        {
            ConsoleHelper.PrintHeader("SAIR DO SISTEMA");
            Console.WriteLine("Obrigado por usar o Rastreador Avançado de Saúde!");
            Console.WriteLine("Seus dados foram salvos com segurança.");
            Console.WriteLine();
            Console.WriteLine("Ate logo e continue cuidando da sua saúde!");

            // Pequena pausa para mostrar a mensagem
            Thread.Sleep(2000);
            Environment.Exit(0);
        }

        private void DisplayActivitiesTable(List<HealthActivityResponseDTO> activities)
        {
            ConsoleHelper.PrintSubHeader($"Registros ({activities.Count} encontrados)");

            Console.WriteLine($"{"ID",-4} {"Data",-12} {"Tipo",-15} {"Valor",-8} {"Unid",-6} {"Int",-4} {"Conform",-10} {"Notas"}");
            Console.WriteLine(new string('-', 85));

            foreach (var activity in activities.OrderByDescending(a => a.Date).ThenByDescending(a => a.CreatedAt))
            {
                var conformStatus = activity.IsWithinRecommendation ? "DENTRO" : "FORA";
                var notesPreview = (activity.Notes?.Length > 20 ? activity.Notes.Substring(0, 20) + "..." : activity.Notes) ?? "";
                var formattedDate = activity.Date.ToBrazilianDate();

                Console.WriteLine($"{activity.Id,-4} {formattedDate,-12} {activity.ActivityType,-15} " +
                                $"{activity.Value,-8:F1} {activity.Unit,-6} {activity.Intensity,-4} " +
                                $"{conformStatus,-10} {notesPreview}");
            }

            // Resumo rápido
            var conformCount = activities.Count(a => a.IsWithinRecommendation);
            Console.WriteLine(new string('-', 85));
            Console.WriteLine($"Resumo: {activities.Count} registros | " +
                            $"Media de intensidade: {activities.Average(a => a.Intensity):F1} | " +
                            $"Conformidade: {conformCount}/{activities.Count}");
        }

        private void DisplayReport(ActivityReportDTO report)
        {
            ConsoleHelper.PrintHeader(report.Title);
            Console.WriteLine($"Período: {report.StartDate.ToBrazilianDate()} - {report.EndDate.ToBrazilianDate()}");
            Console.WriteLine($"Gerado em: {report.GeneratedAt.ToBrazilianDateTime()}");
            Console.WriteLine();

            ConsoleHelper.PrintSubHeader("Estatísticas");
            foreach (var stat in report.Statistics)
            {
                Console.WriteLine($"{stat.ActivityType}:");
                Console.WriteLine($"   • Total: {stat.Total:F1} {stat.Unit}");
                Console.WriteLine($"   • Média: {stat.Average:F1} {stat.Unit}");
                Console.WriteLine($"   • Variação: {stat.Minimum:F1} - {stat.Maximum:F1} {stat.Unit}");
                Console.WriteLine($"   • Registros: {stat.Count}");
                Console.WriteLine($"   • Conformidade: {stat.ComplianceRate:F1}%");
                Console.WriteLine();
            }

            if (report.Insights.Any())
            {
                ConsoleHelper.PrintSubHeader("Insights");
                foreach (var insight in report.Insights)
                {
                    Console.WriteLine($"• {insight.Key}: {insight.Value}");
                }
            }

            // Recomendações baseadas nos insights
            if (report.Statistics.Any())
            {
                var bestCompliance = report.Statistics.OrderByDescending(s => s.ComplianceRate).First();
                var worstCompliance = report.Statistics.OrderBy(s => s.ComplianceRate).First();

                if (bestCompliance.ComplianceRate >= 80)
                {
                    ConsoleHelper.PrintSuccess($"Excelente conformidade em: {bestCompliance.ActivityType}");
                }

                if (worstCompliance.ComplianceRate < 50)
                {
                    ConsoleHelper.PrintWarning($"Atenção necessária em: {worstCompliance.ActivityType}");
                }
            }
        }

        private void ExportReport(ActivityReportDTO report)
        {
            var exportOption = ConsoleHelper.ReadIntWithPrompt(
                "Formato de exportação:\n1. CSV\n2. JSON\n3. Cancelar\nEscolha", 1, 3);

            if (exportOption == 3) return;

            var fileName = ConsoleHelper.ReadLineWithPrompt("Nome do arquivo (sem extensão)", true);
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var exportsPath = Path.Combine(basePath, "Exports");

            if (!Directory.Exists(exportsPath))
            {
                Directory.CreateDirectory(exportsPath);
            }

            try
            {
                string filePath;
                if (exportOption == 1)
                {
                    filePath = Path.Combine(exportsPath, $"{fileName}.csv");
                    _reportService.ExportReportToCsv(report, filePath);
                    ConsoleHelper.PrintSuccess($"Relatório exportado como CSV: {filePath}");
                }
                else
                {
                    filePath = Path.Combine(exportsPath, $"{fileName}.json");
                    _reportService.ExportReportToJson(report, filePath);
                    ConsoleHelper.PrintSuccess($"Relatório exportado como JSON: {filePath}");
                }

                // Mostrar informações do arquivo
                var fileInfo = new FileInfo(filePath);
                Console.WriteLine($"Tamanho do arquivo: {fileInfo.Length} bytes");
                Console.WriteLine($"Registros exportados: {report.Statistics.Sum(s => s.Count)}");
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"Erro ao exportar relatório: {ex.Message}");
            }
        }
    }
}