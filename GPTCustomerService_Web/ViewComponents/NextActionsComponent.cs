using GPTCustomerService_Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace GPTCustomerService_Web.ViewComponents
{
    public class NextActionsComponent : ViewComponent
    {
        private readonly CustomResponse _nextActionsResponse;

        public NextActionsComponent([FromServices] CustomResponse nextActionsResponse)
        {
            _nextActionsResponse = nextActionsResponse;
            _nextActionsResponse.SkillName = "TextAnalysisSkill";
            _nextActionsResponse.FunctionName = "NextActions";
        }

        public async Task<IViewComponentResult> InvokeAsync(string input, int actions, string language)
        {

            await _nextActionsResponse.GetAsync("NextActions",
                               new Dictionary<string, string>()
                               {
                    { "input",input },
                    { "actions", actions.ToString() },
                    { "language", language }
                });
            return View();
        }
    }
}
