namespace Quizor;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHostedSingleton<T>(this IServiceCollection services) 
        where T : class, IHostedService
    {
        services.AddSingleton<T>();
        services.AddSingleton<IHostedService>(sp => sp.GetRequiredService<T>());
        return services;
    }
}