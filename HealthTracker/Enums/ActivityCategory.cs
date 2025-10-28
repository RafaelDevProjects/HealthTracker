namespace HealthTracker.Enums
{
    /// <summary>
    /// Categorias de atividades de saúde
    /// </summary>
    public enum ActivityCategory
    {
        Exercise,
        Nutrition,
        Sleep,
        MentalHealth,
        Hydration,
        Medical,
        Lifestyle
    }

    /// <summary>
    /// Períodos para relatórios
    /// </summary>
    public enum ReportPeriod
    {
        Daily,
        Weekly,
        Monthly,
        Yearly,
        Custom
    }

    /// <summary>
    /// Tipos de estatísticas
    /// </summary>
    public enum StatisticType
    {
        Total,
        Average,
        Maximum,
        Minimum,
        Trend,
        Compliance
    }
}