using GPTCustomerService_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Diagnostics;

namespace GPTCustomerService_Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private const string LANGUAGE = "French";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            
            
            return View("index");
        }

        public IActionResult Transcript()
        {
            Conversation conversation = new Conversation()
            {
                ID = "1",
                Content = "Store: Hello, you have reached Luxor, the luxury watch store. My name is Lisa, how can I help you? Customer: Hello, my name is Jane Smith. I bought a watch from you six months ago and I accidentally broke it yesterday. Store: Oh, I’m sorry to hear that. What is the model of your watch? Customer: It’s a Luxor Chrono 3000, in rose gold with a black leather strap. Store: I see. And what is broken exactly? Customer: Well, the glass is cracked and the second hand doesn’t move anymore. Store: I understand. Fortunately, your watch is still under warranty. In order to determine whether the damages are eligible for coverage, we'll need to examine the watch. You can mail it to us free of charge or drop it off at the store. Customer: Ah, that’s good news! I prefer to come to the store, it’s safer. Store: Very well. We are open from Monday to Saturday, from 10 am to 7 pm. Our address is 12 rue du Faubourg Saint-Honoré, 75008 Paris. Can you give me your phone number and email address so that I can confirm the appointment? Customer: Yes, of course. My number is 06 12 34 56 78 and my email is jane.smith@gmail.com. Store: Perfect. I’ll send you a text message and an email confirmation. When do you think you can come to the store? Customer: I think I can come tomorrow afternoon, around 3 pm. Store: Okay. I’ll book a slot for you tomorrow at 3 pm. We look forward to seeing you. Do you have any other questions? Customer: No, that’s all. Thank you very much for your help and kindness. Store: You’re welcome. Thank you for contacting us. Goodbye and see you tomorrow. Customer: Goodbye and thank you again.",
                Language = "en",
                HtmlContent = "<p><strong>Store:</strong> Hello, you have reached Luxor, the luxury watch store. My name is Lisa, how can I help you?</p>\r\n<p><strong>Customer:</strong> Hello, my name is Jane Smith. I bought a watch from you six months ago and I accidentally broke it yesterday.</p>\r\n<p><strong>Store:</strong> Oh, I’m sorry to hear that. What is the model of your watch?</p>\r\n<p><strong>Customer:</strong> It’s a Luxor Chrono 3000, in rose gold with a black leather strap.</p>\r\n<p><strong>Store:</strong> I see. And what is broken exactly?</p>\r\n<p><strong>Customer:</strong> Well, the glass is cracked and the second hand doesn’t move anymore.</p>\r\n<p><strong>Store:</strong> I understand. Fortunately, your watch is still under warranty. In order to determine whether the damages are eligible for coverage, we'll need to examine the watch. You can mail it to us free of charge or drop it off at the store.</p>\r\n<p><strong>Customer:</strong> Ah, that’s good news! I prefer to come to the store, it’s safer.</p>\r\n<p><strong>Store:</strong> Very well. We are open from Monday to Saturday, from 10 am to 7 pm. Our address is 12 rue du Faubourg Saint-Honoré, 75008 Paris. Can you give me your phone number and email address so that I can confirm the appointment?</p>\r\n<p><strong>Customer:</strong> Yes, of course. My number is 06 12 34 56 78 and my email is jane.smith@gmail.com.</p>\r\n<p><strong>Store:</strong> Perfect. I’ll send you a text message and an email confirmation. When do you think you can come to the store?</p>\r\n<p><strong>Customer:</strong> I think I can come tomorrow afternoon, around 3 pm.</p>\r\n<p><strong>Store:</strong> Okay. I’ll book a slot for you tomorrow at 3 pm. We look forward to seeing you. Do you have any other questions?</p>\r\n<p><strong>Customer:</strong> No, that’s all. Thank you very much for your help and kindness.</p>\r\n<p><strong>Store:</strong> You’re welcome. Thank you for contacting us. Goodbye and see you tomorrow.</p>\r\n<p><strong>Customer:</strong> Goodbye and thank you again.</p>"
            };

            return ViewComponent("TranscriptComponent", new { htmlContent = conversation.HtmlContent});
        }

        //Add GetSummarize
        [AcceptVerbs("POST")]
        public IActionResult GetSummarize(string texte)
        {
            this._logger.LogDebug("Summarize receive request.");

            return ViewComponent("SummarizeComponent", new { input = texte, language = LANGUAGE });
        }

        //Add GetSentiment
        [AcceptVerbs("POST")]
        public IActionResult GetSentiment(string texte)
        {
            this._logger.LogDebug("Sentiment receive request.");

            return ViewComponent("SentimentComponent", new { input = texte, language = LANGUAGE });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}