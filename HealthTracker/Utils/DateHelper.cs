namespace HealthTracker.Utils
{
    /// <summary>
    /// Utilitários para manipulação e formatação de datas
    /// </summary>
    public static class DateHelper
    {
        /// <summary>
        /// CultureInfo para formatação brasileira
        /// </summary>
        public static readonly System.Globalization.CultureInfo BrazilianCulture =
            new System.Globalization.CultureInfo("pt-BR");

        /// <summary>
        /// Formata data no padrão brasileiro
        /// </summary>
        public static string ToBrazilianDate(this DateTime date)
        {
            return date.ToString("dd/MM/yyyy", BrazilianCulture);
        }

        /// <summary>
        /// Formata data e hora no padrão brasileiro
        /// </summary>
        public static string ToBrazilianDateTime(this DateTime date)
        {
            return date.ToString("dd/MM/yyyy HH:mm", BrazilianCulture);
        }

        /// <summary>
        /// Formata data para exibição compacta
        /// </summary>
        public static string ToShortBrazilianDate(this DateTime date)
        {
            return date.ToString("dd/MM/yy", BrazilianCulture);
        }

        /// <summary>
        /// Tenta converter string para DateTime no formato brasileiro
        /// </summary>
        public static bool TryParseBrazilianDate(string input, out DateTime result)
        {
            return DateTime.TryParseExact(input, "dd/MM/yyyy",
                BrazilianCulture,
                System.Globalization.DateTimeStyles.None,
                out result);
        }

        /// <summary>
        /// Obtém o primeiro dia do mês
        /// </summary>
        public static DateTime GetFirstDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        /// <summary>
        /// Obtém o último dia do mês
        /// </summary>
        public static DateTime GetLastDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// Verifica se a data está dentro de um intervalo
        /// </summary>
        public static bool IsInRange(this DateTime date, DateTime start, DateTime end)
        {
            return date >= start && date <= end;
        }

        /// <summary>
        /// Exibe exemplo de formato de data
        /// </summary>
        public static void DisplayDateFormatHelp()
        {
            Console.WriteLine("📅 FORMATO DE DATA: dd/MM/aaaa");
            Console.WriteLine("   Exemplos:");
            Console.WriteLine("   • 15/11/2024 - 15 de Novembro de 2024");
            Console.WriteLine("   • 01/01/2024 - 1 de Janeiro de 2024");
            Console.WriteLine("   • 25/12/2024 - 25 de Dezembro de 2024");
            Console.WriteLine();
        }
    }
}