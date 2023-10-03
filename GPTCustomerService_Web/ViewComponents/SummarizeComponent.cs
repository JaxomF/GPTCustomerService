using GPTCustomerService_Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace GPTCustomerService_Web.ViewComponents
{
    public class SummarizeComponent : ViewComponent
    {
        private readonly CustomResponse _summarizeResponse;

        public SummarizeComponent([FromServices] CustomResponse summarizeResponse)
        {
            _summarizeResponse = summarizeResponse;
            _summarizeResponse.SkillName = "TextAnalysisSkill";
            _summarizeResponse.FunctionName = "Summarize";
        }

        public async Task<IViewComponentResult> InvokeAsync(string input, string language)
        {

            await _summarizeResponse.GetAsync("Summarize", 
                new Dictionary<string, string>() 
                { 
                    { "input",input }, 
                    { "language", language } 
                });
            return View();
        }

    }
}
