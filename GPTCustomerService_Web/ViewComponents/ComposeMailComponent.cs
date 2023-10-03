using GPTCustomerService_Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace GPTCustomerService_Web.ViewComponents
{
    public class ComposeMailComponent : ViewComponent
    {
        private readonly CustomResponse _composeMailResponse;

        public ComposeMailComponent([FromServices] CustomResponse composeMailResponse)
        {
            _composeMailResponse = composeMailResponse;
            _composeMailResponse.SkillName = "EmailSkill";
            _composeMailResponse.FunctionName = "ComposeMail";
        }

        public async Task<IViewComponentResult> InvokeAsync(string input, string language)
        {

            await _composeMailResponse.GetAsync("ComposeMail",
                new Dictionary<string, string>()
                {
                    { "input",input },
                    { "language", language }
                });
            return View();
        }
    }
}
