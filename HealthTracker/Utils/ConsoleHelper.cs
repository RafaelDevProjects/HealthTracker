using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace HealthTracker.Utils
{
    /// <summary>
    /// Utilitários para interface console com formatação avançada
    /// </summary>
    public static class ConsoleHelper
    {
        /// <summary>
        /// Cores disponíveis para o console
        /// </summary>
        public static class Colors
        {
            public static ConsoleColor Success = ConsoleColor.Green;
            public static ConsoleColor Error = ConsoleColor.Red;
            public static ConsoleColor Warning = ConsoleColor.Yellow;
            public static ConsoleColor Info = ConsoleColor.Blue;
            public static ConsoleColor Header = ConsoleColor.Cyan;
            public static ConsoleColor SubHeader = ConsoleColor.Magenta;
            public static ConsoleColor Input = ConsoleColor.White;
            public static ConsoleColor Highlight = ConsoleColor.Yellow;
            public static ConsoleColor Muted = ConsoleColor.DarkGray;
        }

        /// <summary>
        /// Exibe uma mensagem de sucesso
        /// </summary>
        public static void PrintSuccess(string message)
        {
            Console.ForegroundColor = Colors.Success;
            Console.WriteLine($"✅ {message}");
            Console.ResetColor();
        }

        /// <summary>
        /// Exibe uma mensagem de erro
        /// </summary>
        public static void PrintError(string message)
        {
            Console.ForegroundColor = Colors.Error;
            Console.WriteLine($"❌ {message}");
            Console.ResetColor();
        }

        /// <summary>
        /// Exibe uma mensagem de aviso
        /// </summary>
        public static void PrintWarning(string message)
        {
            Console.ForegroundColor = Colors.Warning;
            Console.WriteLine($"⚠️ {message}");
            Console.ResetColor();
        }

        /// <summary>
        /// Exibe uma mensagem informativa
        /// </summary>
        public static void PrintInfo(string message)
        {
            Console.ForegroundColor = Colors.Info;
            Console.WriteLine($"ℹ️ {message}");
            Console.ResetColor();
        }

        /// <summary>
        /// Exibe um cabeçalho formatado
        /// </summary>
        public static void PrintHeader(string title)
        {
            Console.WriteLine();
            Console.ForegroundColor = Colors.Header;
            Console.WriteLine(new string('=', 60));
            Console.WriteLine($" {title.ToUpper()}".PadRight(59) + " ");
            Console.WriteLine(new string('=', 60));
            Console.ResetColor();
            Console.WriteLine();
        }

        /// <summary>
        /// Exibe um subcabeçalho formatado
        /// </summary>
        public static void PrintSubHeader(string title)
        {
            Console.ForegroundColor = Colors.SubHeader;
            Console.WriteLine($"\n{new string('-', 50)}");
            Console.WriteLine($" 🎯 {title}");
            Console.WriteLine(new string('-', 50));
            Console.ResetColor();
        }

        /// <summary>
        /// Exibe um título de seção
        /// </summary>
        public static void PrintSectionTitle(string title)
        {
            Console.ForegroundColor = Colors.Info;
            Console.WriteLine($"\n📁 {title}");
            Console.WriteLine(new string('─', title.Length + 4));
            Console.ResetColor();
        }

        /// <summary>
        /// Exibe uma linha de destaque
        /// </summary>
        public static void PrintHighlight(string message)
        {
            Console.ForegroundColor = Colors.Highlight;
            Console.WriteLine($"✨ {message}");
            Console.ResetColor();
        }

        /// <summary>
        /// Exibe texto com cor muted
        /// </summary>
        public static void PrintMuted(string message)
        {
            Console.ForegroundColor = Colors.Muted;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        /// <summary>
        /// Lê uma linha do console com prompt personalizado
        /// </summary>
        public static string ReadLineWithPrompt(string prompt, bool required = false, string defaultValue = "")
        {
            Console.ForegroundColor = Colors.Input;
            Console.Write($"{prompt}");

            if (!string.IsNullOrEmpty(defaultValue))
            {
                Console.Write($" [{defaultValue}]");
            }

            Console.Write(": ");
            Console.ResetColor();

            var input = Console.ReadLine()?.Trim() ?? "";

            // Se vazio e tem valor padrão, usa o padrão
            if (string.IsNullOrEmpty(input) && !string.IsNullOrEmpty(defaultValue))
            {
                return defaultValue;
            }

            // Validação de campo obrigatório
            while (required && string.IsNullOrWhiteSpace(input))
            {
                PrintWarning("Este campo é obrigatório.");
                Console.ForegroundColor = Colors.Input;
                Console.Write($"{prompt}: ");
                Console.ResetColor();
                input = Console.ReadLine()?.Trim() ?? "";
            }

            return input;
        }

        /// <summary>
        /// Lê um número inteiro do console com validação
        /// </summary>
        public static int ReadIntWithPrompt(string prompt, int min = int.MinValue, int max = int.MaxValue, int? defaultValue = null)
        {
            while (true)
            {
                Console.ForegroundColor = Colors.Input;
                Console.Write($"{prompt}");

                if (defaultValue.HasValue)
                {
                    Console.Write($" [{defaultValue}]");
                }

                Console.Write(": ");
                Console.ResetColor();

                var input = Console.ReadLine()?.Trim();

                // Se vazio e tem valor padrão, usa o padrão
                if (string.IsNullOrEmpty(input) && defaultValue.HasValue)
                {
                    return defaultValue.Value;
                }

                if (int.TryParse(input, out int result))
                {
                    if (result >= min && result <= max)
                    {
                        return result;
                    }
                    PrintWarning($"Por favor, insira um número entre {min} e {max}.");
                }
                else
                {
                    PrintWarning("Por favor, insira um número inteiro válido.");
                }
            }
        }

        /// <summary>
        /// Lê um número decimal do console com validação
        /// </summary>
        public static double ReadDoubleWithPrompt(string prompt, double min = double.MinValue, double max = double.MaxValue, double? defaultValue = null)
        {
            while (true)
            {
                Console.ForegroundColor = Colors.Input;
                Console.Write($"{prompt}");

                if (defaultValue.HasValue)
                {
                    Console.Write($" [{defaultValue}]");
                }

                Console.Write(": ");
                Console.ResetColor();

                var input = Console.ReadLine()?.Trim();

                // Se vazio e tem valor padrão, usa o padrão
                if (string.IsNullOrEmpty(input) && defaultValue.HasValue)
                {
                    return defaultValue.Value;
                }

                if (double.TryParse(input, out double result))
                {
                    if (result >= min && result <= max)
                    {
                        return result;
                    }
                    PrintWarning($"Por favor, insira um número entre {min} e {max}.");
                }
                else
                {
                    PrintWarning("Por favor, insira um número válido.");
                }
            }
        }

        /// <summary>
        /// Lê uma data do console com validação e formato brasileiro
        /// </summary>
        public static DateTime ReadDateWithPrompt(string prompt, bool allowFuture = false, DateTime? defaultValue = null)
        {
            while (true)
            {
                Console.ForegroundColor = Colors.Input;
                Console.Write($"{prompt}");

                if (defaultValue.HasValue)
                {
                    Console.Write($" [{defaultValue.Value:dd/MM/yyyy}]");
                }

                Console.Write(" (dd/MM/aaaa): ");
                Console.ResetColor();

                var input = Console.ReadLine()?.Trim();

                // Se vazio e tem valor padrão, usa o padrão
                if (string.IsNullOrEmpty(input) && defaultValue.HasValue)
                {
                    return defaultValue.Value;
                }

                // Tentar parse com formato brasileiro
                if (DateHelper.TryParseBrazilianDate(input, out DateTime result))
                {
                    if (!allowFuture && result > DateTime.Now)
                    {
                        PrintWarning("Data não pode ser no futuro.");
                        continue;
                    }
                    return result;
                }

                // Tentar parse com formato padrão como fallback
                if (DateTime.TryParse(input, out result))
                {
                    if (!allowFuture && result > DateTime.Now)
                    {
                        PrintWarning("Data não pode ser no futuro.");
                        continue;
                    }
                    PrintInfo($"📅 Data interpretada como: {result:dd/MM/yyyy}");
                    return result;
                }

                PrintWarning("Data inválida. Use o formato dd/MM/aaaa (ex: 15/11/2024).");
            }
        }

        /// <summary>
        /// Lê um horário do console
        /// </summary>
        public static TimeSpan ReadTimeWithPrompt(string prompt, TimeSpan? defaultValue = null)
        {
            while (true)
            {
                Console.ForegroundColor = Colors.Input;
                Console.Write($"{prompt}");

                if (defaultValue.HasValue)
                {
                    Console.Write($" [{defaultValue.Value:hh\\:mm}]");
                }

                Console.Write(" (hh:mm): ");
                Console.ResetColor();

                var input = Console.ReadLine()?.Trim();

                // Se vazio e tem valor padrão, usa o padrão
                if (string.IsNullOrEmpty(input) && defaultValue.HasValue)
                {
                    return defaultValue.Value;
                }

                if (TimeSpan.TryParse(input, out TimeSpan result))
                {
                    return result;
                }

                PrintWarning("Horário inválido. Use o formato hh:mm (ex: 14:30).");
            }
        }

        /// <summary>
        /// Exibe uma pergunta de sim/não
        /// </summary>
        public static bool ReadYesNoWithPrompt(string prompt, bool defaultValue = false)
        {
            var defaultText = defaultValue ? "S" : "N";
            var options = defaultValue ? " (S/n)" : " (s/N)";

            while (true)
            {
                Console.ForegroundColor = Colors.Input;
                Console.Write($"{prompt}{options}: ");
                Console.ResetColor();

                var input = Console.ReadLine()?.Trim().ToLower();

                if (string.IsNullOrEmpty(input))
                {
                    return defaultValue;
                }

                if (input == "s" || input == "sim" || input == "y" || input == "yes")
                {
                    return true;
                }
                else if (input == "n" || input == "não" || input == "nao" || input == "no")
                {
                    return false;
                }
                else
                {
                    PrintWarning("Por favor, responda com 's' para sim ou 'n' para não.");
                }
            }
        }

        /// <summary>
        /// Exibe um menu de opções e retorna a seleção
        /// </summary>
        public static int ReadMenuOption(string prompt, List<string> options, int defaultValue = 1)
        {
            Console.WriteLine();
            PrintSectionTitle(prompt);

            for (int i = 0; i < options.Count; i++)
            {
                Console.WriteLine($"  {i + 1}. {options[i]}");
            }

            Console.WriteLine();
            return ReadIntWithPrompt("Escolha uma opção", 1, options.Count, defaultValue);
        }

        /// <summary>
        /// Formata uma data no formato brasileiro
        /// </summary>
        public static string FormatDate(DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// Formata uma data com hora no formato brasileiro
        /// </summary>
        public static string FormatDateTime(DateTime date)
        {
            return date.ToString("dd/MM/yyyy HH:mm");
        }

        /// <summary>
        /// Formata uma data no formato brasileiro para exibição em tabelas
        /// </summary>
        public static string FormatDateForTable(DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// Formata um número como porcentagem
        /// </summary>
        public static string FormatPercent(double value)
        {
            return $"{value:F1}%";
        }

        /// <summary>
        /// Formata um valor numérico
        /// </summary>
        public static string FormatNumber(double value, int decimals = 1)
        {
            return value.ToString($"F{decimals}");
        }

        /// <summary>
        /// Exibe um exemplo de formato de data
        /// </summary>
        public static void ShowDateFormatExample()
        {
            PrintInfo("📅 Formato de data: dd/MM/aaaa (ex: 15/11/2024)");
        }

        /// <summary>
        /// Aguarda o usuário pressionar uma tecla para continuar
        /// </summary>
        public static void WaitForContinue(string message = "Pressione qualquer tecla para continuar...")
        {
            Console.WriteLine();
            PrintMuted(message);
            Console.ReadKey();
        }

        /// <summary>
        /// Aguarda alguns segundos
        /// </summary>
        public static void WaitSeconds(int seconds)
        {
            for (int i = seconds; i > 0; i--)
            {
                Console.Write($"\rAguarde {i} segundos...");
                Thread.Sleep(1000);
            }
            Console.Write("\r" + new string(' ', 30) + "\r");
        }

        /// <summary>
        /// Limpa a tela do console
        /// </summary>
        public static void ClearScreen()
        {
            Console.Clear();
        }

        /// <summary>
        /// Exibe uma barra de progresso
        /// </summary>
        public static void ShowProgressBar(int current, int total, string message = "Processando")
        {
            var percentage = (double)current / total;
            var progressBarWidth = 30;
            var progress = (int)(percentage * progressBarWidth);

            var progressBar = new string('█', progress) + new string('░', progressBarWidth - progress);

            Console.Write($"\r{message}: [{progressBar}] {percentage:P1} ({current}/{total})");

            if (current == total)
            {
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Exibe uma tabela de dados
        /// </summary>
        public static void DisplayTable<T>(List<T> data, List<TableColumn<T>> columns, string title = "")
        {
            if (!string.IsNullOrEmpty(title))
            {
                PrintSubHeader(title);
            }

            if (!data.Any())
            {
                PrintInfo("Nenhum dado para exibir.");
                return;
            }

            // Calcular larguras das colunas
            var columnWidths = columns.Select(col =>
            {
                var headerWidth = col.Header.Length;
                var dataWidth = data.Any() ? data.Max(item => col.GetValue(item).Length) : 0;
                return Math.Max(headerWidth, dataWidth) + 2; // +2 para padding
            }).ToArray();

            // Exibir cabeçalho
            Console.ForegroundColor = Colors.Header;
            for (int i = 0; i < columns.Count; i++)
            {
                Console.Write(columns[i].Header.PadRight(columnWidths[i]));
            }
            Console.WriteLine();
            Console.ResetColor();

            // Linha separadora
            Console.WriteLine(new string('-', columnWidths.Sum()));

            // Exibir dados
            foreach (var item in data)
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    var value = columns[i].GetValue(item);
                    Console.Write(value.PadRight(columnWidths[i]));
                }
                Console.WriteLine();
            }

            // Linha final
            Console.WriteLine(new string('-', columnWidths.Sum()));
            Console.WriteLine($"Total: {data.Count} registros");
        }

        /// <summary>
        /// Exibe uma lista de itens com numeração
        /// </summary>
        public static void DisplayNumberedList<T>(List<T> items, string title = "", Func<T, string> formatter = null)
        {
            if (!string.IsNullOrEmpty(title))
            {
                PrintSectionTitle(title);
            }

            if (!items.Any())
            {
                PrintInfo("Nenhum item para exibir.");
                return;
            }

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var displayText = formatter != null ? formatter(item) : item?.ToString() ?? "";
                Console.WriteLine($"  {i + 1}. {displayText}");
            }
        }

        /// <summary>
        /// Exibe um resumo estatístico
        /// </summary>
        public static void DisplayStatsSummary(Dictionary<string, double> stats, string title = "Resumo Estatístico")
        {
            PrintSubHeader(title);

            foreach (var stat in stats)
            {
                Console.WriteLine($"• {stat.Key}: {stat.Value:F2}");
            }
        }

        /// <summary>
        /// Exibe uma mensagem de carregamento
        /// </summary>
        public static void ShowLoading(string message = "Carregando", int dots = 3, int delay = 500)
        {
            Console.Write($"\r{message}");
            for (int i = 0; i < dots; i++)
            {
                Console.Write(".");
                Thread.Sleep(delay);
            }
            Console.Write("\r" + new string(' ', message.Length + dots) + "\r");
        }

        /// <summary>
        /// Exibe uma mensagem de conclusão
        /// </summary>
        public static void ShowCompletion(string message = "Concluído!")
        {
            Console.Write($"\r{message}");
            Thread.Sleep(1000);
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Define uma coluna para exibição em tabela
    /// </summary>
    public class TableColumn<T>
    {
        public string Header { get; set; }
        public Func<T, string> GetValue { get; set; }

        public TableColumn(string header, Func<T, string> getValue)
        {
            Header = header;
            GetValue = getValue;
        }
    }

    /// <summary>
    /// Utilitários para animações no console
    /// </summary>
    public static class ConsoleAnimations
    {
        /// <summary>
        /// Exibe uma animação de spinner
        /// </summary>
        public static void ShowSpinner(int durationMs = 2000, string message = "Processando")
        {
            var spinner = new[] { "|", "/", "-", "\\" };
            var startTime = DateTime.Now;
            var spinnerIndex = 0;

            Console.Write($"{message} ");

            while ((DateTime.Now - startTime).TotalMilliseconds < durationMs)
            {
                Console.Write($"\r{message} {spinner[spinnerIndex]}");
                spinnerIndex = (spinnerIndex + 1) % spinner.Length;
                Thread.Sleep(100);
            }

            Console.Write($"\r{message} ✅\n");
        }

        /// <summary>
        /// Exibe uma animação de digitação
        /// </summary>
        public static void TypeWriter(string text, int delayMs = 50)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(delayMs);
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Exibe uma contagem regressiva
        /// </summary>
        public static void Countdown(int seconds, string message = "Iniciando em")
        {
            for (int i = seconds; i > 0; i--)
            {
                Console.Write($"\r{message} {i}...");
                Thread.Sleep(1000);
            }
            Console.Write("\r" + new string(' ', message.Length + seconds.ToString().Length + 5) + "\r");
        }
    }
}