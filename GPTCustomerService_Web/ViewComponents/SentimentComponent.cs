using GPTCustomerService_Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace GPTCustomerService_Web.ViewComponents
{
    public class SentimentComponent : ViewComponent
    {
        private readonly CustomResponse _sentimentResponse;

        public SentimentComponent([FromServices] CustomResponse sentimentResponse)
        {
            _sentimentResponse = sentimentResponse;
            _sentimentResponse.SkillName = "TextAnalysisSkill";
            _sentimentResponse.FunctionName = "Sentiment";
        }

        public async Task<IViewComponentResult> InvokeAsync(string input, string language)
        {

            await _sentimentResponse.GetAsync("Sentiment", 
                               new Dictionary<string, string>()
                               {
                    { "input",input },
                    { "language", language } 
                });
            return View();
        }


    }
}
