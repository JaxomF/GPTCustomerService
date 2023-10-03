using GPTCustomerService_Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace GPTCustomerService_Web.ViewComponents
{
    public class TranslateComponent : ViewComponent
    {
        private readonly CustomResponse _translateResponse;

        public TranslateComponent([FromServices] CustomResponse translateResponse)
        {
            _translateResponse = translateResponse;
            _translateResponse.SkillName = "TextAnalysisSkill";
            _translateResponse.FunctionName = "Translate";
        }

        public async Task<IViewComponentResult> InvokeAsync(string input, string language)
        {

            await _translateResponse.GetAsync("Translate", 
                               new Dictionary<string, string>()
                               {
                    { "input",input },
                    { "language", language } 
                });
            return View();
        }


    }
}
