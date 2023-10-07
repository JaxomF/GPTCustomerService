// Copyright (c) Microsoft. All rights reserved.

using System.Reflection;
using GPTCustomerService_Web.Options;
using GPTCustomerService_Web.Services;
using SemanticKernel.Service.Options;

namespace GPTCustomerService_Web.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// Add options and services for Copilot Chat.
/// </summary>
public static class SKServiceExtensions
{
    /// <summary>
    /// Parse configuration into options.
    /// </summary>
    public static IServiceCollection AddOptions(this IServiceCollection services, ConfigurationManager configuration)
    {

        // General configuration
        AddOptions<ServiceOptions>(ServiceOptions.PropertyName);

        // Default AI service configurations for Semantic Kernel
        AddOptions<AIServiceOptions>(AIServiceOptions.PropertyName);

        return services;

        void AddOptions<TOptions>(string propertyName)
            where TOptions : class
        {
            services.AddOptions<TOptions>(configuration.GetSection(propertyName));
        }
    }

    internal static void AddOptions<TOptions>(this IServiceCollection services, IConfigurationSection section)
        where TOptions : class
    {
        services.AddOptions<TOptions>()
            .Bind(section)
            .ValidateDataAnnotations()
            .ValidateOnStart()
            .PostConfigure(TrimStringProperties);
    }
        

    /// <summary>
    /// Add CORS settings.
    /// </summary>
    internal static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        IConfiguration configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        string[] allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
        if (allowedOrigins.Length > 0)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        policy.WithOrigins(allowedOrigins)
                            .WithMethods("GET", "POST", "DELETE", "OPTION")
                            .AllowAnyHeader();
                    });
            });
        }

        return services;
    }


    internal static IServiceCollection AddAIResponses(this IServiceCollection services)
    {        
        return services.AddScoped<CustomResponse>();           
           
    }

    /// <summary>
    /// Add persistent store services.
    /// </summary>
    //    public static IServiceCollection AddPersistentStore(this IServiceCollection services)
    //    {
    //        IStorageContext<ChatSession> chatSessionStorageContext;                


    //        ClientStoreOptions clientStoreConfig = services.BuildServiceProvider().GetRequiredService<IOptions<ClientStoreOptions>>().Value;

    //        switch (clientStoreConfig.Type)
    //        {
    //            case ClientStoreOptions.ChatStoreType.Volatile:
    //                {
    //                    chatSessionStorageContext = new VolatileContext<ChatSession>();                                                            
    //                    break;
    //                }           

    //            case ClientStoreOptions.ChatStoreType.Cosmos:
    //                {
    //                    if (clientStoreConfig.Cosmos == null)
    //                    {
    //                        throw new InvalidOperationException("ChatStore:Cosmos is required when ChatStore:Type is 'Cosmos'");
    //                    }
    //#pragma warning disable CA2000 // Dispose objects before losing scope - objects are singletons for the duration of the process and disposed when the process exits.
    //                    chatSessionStorageContext = new CosmosDbContext<ChatSession>(
    //                        clientStoreConfig.Cosmos.ConnectionString, clientStoreConfig.Cosmos.Database, clientStoreConfig.Cosmos.ClientSessionsContainer);

    //#pragma warning restore CA2000 // Dispose objects before losing scope
    //                    break;
    //                }

    //            default:
    //                {
    //                    throw new InvalidOperationException(
    //                        "Invalid 'ChatStore' setting 'chatStoreConfig.Type'.");
    //                }
    //        }

    //        services.AddSingleton<ChatSessionRepository>(new ChatSessionRepository(chatSessionStorageContext));

    //        return services;
    //    }





    /// <summary>
    /// Trim all string properties, recursively.
    /// </summary>
    private static void TrimStringProperties<T>(T options) where T : class
    {
        Queue<object> targets = new();
        targets.Enqueue(options);

        while (targets.Count > 0)
        {
            object target = targets.Dequeue();
            Type targetType = target.GetType();
            foreach (PropertyInfo property in targetType.GetProperties())
            {
                // Skip enumerations
                if (property.PropertyType.IsEnum)
                {
                    continue;
                }

                // Property is a built-in type, readable, and writable.
                if (property.PropertyType.Namespace == "System" &&
                    property.CanRead &&
                    property.CanWrite)
                {
                    // Property is a non-null string.
                    if (property.PropertyType == typeof(string) &&
                        property.GetValue(target) != null)
                    {
                        property.SetValue(target, property.GetValue(target)!.ToString()!.Trim());
                    }
                }
                else
                {
                    // Property is a non-built-in and non-enum type - queue it for processing.
                    if (property.GetValue(target) != null)
                    {
                        targets.Enqueue(property.GetValue(target)!);
                    }
                }
            }
        }
    }
}