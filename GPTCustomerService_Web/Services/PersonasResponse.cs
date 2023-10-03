
using GPTCustomerService_Web.Hubs;
using GPTCustomerService_Web.Models;
using GPTCustomerService_Web.Models.Response;
using GPTCustomerService_Web.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.TextCompletion;
using Microsoft.SemanticKernel.Diagnostics;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using Microsoft.SemanticKernel.TemplateEngine.Prompt;
using System.Text.Json;

namespace GPTCustomerService_Web.Services
{
    public class CustomResponse
    {
        
        private readonly ILogger<CustomResponse> _logger;
        private readonly IHubContext<MessageRelayHub> _messageRelayHubContext;
        private readonly IKernel _kernel;
        private string _skillsDirectory = string.Empty;
        private readonly int DELAY = 10;

        public string SkillName { get; set; } = string.Empty;
        public string FunctionName { get; set; } = string.Empty;


        public CustomResponse(ILogger<CustomResponse> logger, [FromServices] IKernel kernel,
        [FromServices] IHubContext<MessageRelayHub> messageRelayHubContext)
        {
            _logger = logger;
            _kernel = kernel;
            _messageRelayHubContext = messageRelayHubContext;
            _skillsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Skills");
        }

        public async Task GetAsync(string whatAbout, Dictionary<string, string> variablesContext)
        {
            // Get the function to invoke
            ISKFunction? function = null;
            try
            {
                function = _kernel.Skills.GetFunction(this.SkillName, this.FunctionName);
            }
            catch (SKException ke)
            {
                this._logger.LogError("Failed to find {0}/{1} on server: {2}", this.SkillName, this.FunctionName, ke);

                throw ke;
            }

            var myContext = _kernel.CreateNewContext();

            foreach (var item in variablesContext)
            {
                myContext.Variables.Set(item.Key, item.Value);
            }

            await InitMessageRequest(Path.Combine(_skillsDirectory, this.SkillName, this.FunctionName), myContext, whatAbout);


        }

        private async Task InitMessageRequest(string path, SKContext context, string whatAbout)
        {
            string? configStream = await ReadSkill.Read(path, SkillFileType.Config);
            string? promptStream = await ReadSkill.Read(path, SkillFileType.Prompt);

            if (configStream != null && promptStream != null)
            {
                PromptConfig? config = JsonSerializer.Deserialize<PromptConfig>(configStream);

                if (config != null)
                {
                    var requestSettings = InitRequestSettings(config);

                    PromptTemplateEngine promptTemplateEngine = new PromptTemplateEngine();
                    string prompt = await promptTemplateEngine.RenderAsync(promptStream, context);

                    await StreamResponseToClient("",whatAbout,prompt, requestSettings);

                }

            }
        }

        private async Task<MessageResponse> StreamResponseToClient(string chatId, string whatAbout, string prompt, CompleteRequestSettings requestSettings)
        {
            ITextCompletion textCompletion = _kernel.GetService<ITextCompletion>();

            MessageResponse messageResponse = new MessageResponse
            {
                State = "Start",
                Content = "",
                WhatAbout = whatAbout
            };
            await this.UpdateMessageOnClient(messageResponse, chatId);
            await Task.Delay(DELAY);

            await foreach (string contentPiece in textCompletion.CompleteStreamAsync(prompt, requestSettings))
            {
                messageResponse.State = "InProgress";
                if (!string.IsNullOrEmpty(contentPiece))
                {                    
                    messageResponse.Content += contentPiece;
                    await this.UpdateMessageOnClient(messageResponse, chatId);
                    await Task.Delay(DELAY);
                }
            }
            messageResponse.State = "End";
            await this.UpdateMessageOnClient(messageResponse, chatId);
            return messageResponse;
        }

        /// <summary>
        /// Update the response on the client.
        /// </summary>
        /// <param name="message">The message</param>
        private async Task UpdateMessageOnClient(MessageResponse message,string chatId)
        {
            //await this._messageRelayHubContext.Clients.Group(chatId).SendAsync("ReceiveMessageUpdate", message);
            await this._messageRelayHubContext.Clients.All.SendAsync("ReceiveMessageUpdate", message);
        }

        private CompleteRequestSettings InitRequestSettings(PromptConfig config)
        {
            return new CompleteRequestSettings()
            {
                MaxTokens = config.completion.max_tokens,
                FrequencyPenalty = config.completion.frequency_penalty,
                PresencePenalty = config.completion.presence_penalty,
                Temperature = config.completion.temperature,
                TopP = config.completion.top_p
            };

        }

    }
}
