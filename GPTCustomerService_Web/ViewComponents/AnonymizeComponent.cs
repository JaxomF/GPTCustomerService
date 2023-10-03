using GPTCustomerService_Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace GPTCustomerService_Web.ViewComponents
{
    public class AnonymizeComponent : ViewComponent
    {
        private readonly CustomResponse _anonymizeResponse;

        public AnonymizeComponent([FromServices] CustomResponse anonymizeResponse)
        {
            _anonymizeResponse = anonymizeResponse;
            _anonymizeResponse.SkillName = "TextAnalysisSkill";
            _anonymizeResponse.FunctionName = "Anonymize";
        }

        public async Task<IViewComponentResult> InvokeAsync(string input)
        {

            await _anonymizeResponse.GetAsync("Anonymize",
                new Dictionary<string, string>()
                {
                    { "input",input }
                });
            return View();
        }
    }
}
