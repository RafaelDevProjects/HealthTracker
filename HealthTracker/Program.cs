using HealthTracker.Repository;
using HealthTracker.Services;
using HealthTracker.Controllers;

namespace HealthTracker
{
    /// <summary>
    /// Classe principal do programa Rastreador de Saúde Avançado
    /// Configura a injeção de dependência e inicia a aplicação
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Configuração da injeção de dependência
                var repository = new HealthActivityRepository();
                var validationService = new ValidationService(repository);
                var statisticsService = new StatisticsService(repository);
                var reportService = new ReportService(repository, statisticsService);
                var healthService = new HealthTrackerService(repository, validationService);

                var controller = new HealthTrackerController(
                    healthService,
                    statisticsService,
                    reportService,
                    validationService
                );

                // Execução da aplicação
                controller.Run();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"💥 Ocorreu um erro crítico: {ex.Message}");
                Console.WriteLine($"Detalhes: {ex}");
                Console.ResetColor();
                Console.WriteLine("Pressione qualquer tecla para sair...");
                Console.ReadKey();
            }
        }
    }
}