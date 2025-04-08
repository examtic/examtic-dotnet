namespace Examtic;

public static class Extensions
{
    public static IServiceCollection AddExamtic(this IServiceCollection services)
    {
        return services.AddScoped<ExamticService>();
    }
}
