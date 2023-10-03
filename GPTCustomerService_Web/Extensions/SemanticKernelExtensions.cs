using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using GPTCustomerService_Web.Options;
using Microsoft.SemanticKernel.AI.Embeddings;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI.TextEmbedding;
using SemanticKernel.Service.Options;
using Microsoft.SemanticKernel.Diagnostics;

namespace GPTCustomerService_Web.Extensions
{
    internal static class SemanticKernelExtensions
    {
        /// <summary>
        /// Delegate to register skills with a Semantic Kernel
        /// </summary>
        public delegate Task RegisterSkillsWithKernel(IServiceProvider sp, IKernel kernel);

        /// <summary>
        /// Add Semantic Kernel services
        /// </summary>
        internal static IServiceCollection AddSemanticKernelServices(this IServiceCollection services)
        {
            // Semantic Kernel
            services.AddScoped<IKernel>(sp =>
            {
                IKernel kernel = Kernel.Builder
                    .WithLoggerFactory(sp.GetRequiredService<ILoggerFactory>())
                    //.WithMemory(sp.GetRequiredService<ISemanticTextMemory>())                    
                    .WithCompletionBackend(sp.GetRequiredService<IOptions<AIServiceOptions>>().Value)
                    //.WithEmbeddingBackend(sp.GetRequiredService<IOptions<AIServiceOptions>>().Value)
                    .Build();

                sp.GetRequiredService<RegisterSkillsWithKernel>()(sp, kernel);
                return kernel;
            });


            // Register skills
            services.AddScoped<RegisterSkillsWithKernel>(sp => RegisterSkillsAsync);

            return services;

        }

        /// <summary>
        /// Register the skills with the kernel.
        /// </summary>
        private static Task RegisterSkillsAsync(IServiceProvider sp, IKernel kernel)
        {
            // Semantic skills
            ServiceOptions options = sp.GetRequiredService<IOptions<ServiceOptions>>().Value;
            if (!string.IsNullOrWhiteSpace(options.SemanticSkillsDirectory))
            {
                foreach (string subDir in Directory.GetDirectories(options.SemanticSkillsDirectory))
                {
                    try
                    {
                        kernel.ImportSemanticSkillFromDirectory(options.SemanticSkillsDirectory, Path.GetFileName(subDir)!);
                    }
                    catch (SKException e)
                    {
                        var logger = kernel.LoggerFactory.CreateLogger(nameof(Kernel));
                        logger.LogError("Could not load skill from {Directory}: {Message}", subDir, e.Message);
                    }
                }
            }

            return Task.CompletedTask;
        }

        ///<summary>
        /// Add the completion backend to the kernel config.
        /// </summary>
        private static KernelBuilder WithCompletionBackend(this KernelBuilder kernelBuilder, AIServiceOptions options)
        {
            return options.Type switch
            {
                AIServiceOptions.AIServiceType.AzureOpenAI
                    => kernelBuilder.WithAzureChatCompletionService(options.Models.Completion, options.Endpoint, options.Key),
                AIServiceOptions.AIServiceType.OpenAI
                    => kernelBuilder.WithOpenAIChatCompletionService(options.Models.Completion, options.Key),
                _
                    => throw new ArgumentException($"Invalid {nameof(options.Type)} value in '{AIServiceOptions.PropertyName}' settings."),
            };
        }

        /// <summary>
        /// Add the embedding backend to the kernel config
        /// </summary>
        private static KernelBuilder WithEmbeddingBackend(this KernelBuilder kernelBuilder, AIServiceOptions options)
        {
            return options.Type switch
            {
                AIServiceOptions.AIServiceType.AzureOpenAI
                    => kernelBuilder.WithAzureTextEmbeddingGenerationService(options.Models.Embedding, options.Endpoint, options.Key),
                AIServiceOptions.AIServiceType.OpenAI
                    => kernelBuilder.WithOpenAITextEmbeddingGenerationService(options.Models.Embedding, options.Key),
                _
                    => throw new ArgumentException($"Invalid {nameof(options.Type)} value in '{AIServiceOptions.PropertyName}' settings."),
            };
        }

        /// <summary>
        /// Add the completion backend to the kernel config for the planner.
        /// </summary>
        private static KernelBuilder WithPlannerBackend(this KernelBuilder kernelBuilder, AIServiceOptions options)
        {
            return options.Type switch
            {
                AIServiceOptions.AIServiceType.AzureOpenAI => kernelBuilder.WithAzureChatCompletionService(options.Models.Planner, options.Endpoint, options.Key),
                AIServiceOptions.AIServiceType.OpenAI => kernelBuilder.WithOpenAIChatCompletionService(options.Models.Planner, options.Key),
                _ => throw new ArgumentException($"Invalid {nameof(options.Type)} value in '{AIServiceOptions.PropertyName}' settings."),
            };
        }

        /// <summary>
        /// Construct IEmbeddingGeneration from <see cref="AIServiceOptions"/>
        /// </summary>
        /// <param name="options">The service configuration</param>
        /// <param name="httpClient">Custom <see cref="HttpClient"/> for HTTP requests.</param>
        /// <param name="logger">Application logger</param>
        private static ITextEmbeddingGeneration ToTextEmbeddingsService(this AIServiceOptions options,
            HttpClient? httpClient = null,
            ILoggerFactory? loggerFactory = null)
        {
            return options.Type switch
            {
                AIServiceOptions.AIServiceType.AzureOpenAI
                    => new AzureTextEmbeddingGeneration(options.Models.Embedding, options.Endpoint, options.Key, httpClient: httpClient, loggerFactory: loggerFactory),
                AIServiceOptions.AIServiceType.OpenAI
                    => new OpenAITextEmbeddingGeneration(options.Models.Embedding, options.Key, httpClient: httpClient, loggerFactory: loggerFactory),
                _
                    => throw new ArgumentException("Invalid AIService value in embeddings backend settings"),
            };
        }


    }
}