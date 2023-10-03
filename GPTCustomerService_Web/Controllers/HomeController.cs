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
                Content = "Client : Bonjour, je suis Monsieur Dupond. J’ai commandé chez vous un carton de cocktail sans alcool Tropical Sunrise, mais je me suis trompé de produit. Je voudrais le renvoyer et me faire rembourser.\r\n\r\nSociété : Bonjour, Monsieur Dupond. Je suis Madame Martin, la responsable du service client de Cocktailo. Je suis désolée pour cette erreur. Pouvez-vous me donner votre numéro de commande, votre adresse e-mail, votre adresse postale et votre numéro de mobile s’il vous plaît ?\r\n\r\nClient : Oui, bien sûr. Mon numéro de commande est le 123456789, mon adresse e-mail est dupond@gmail.com, mon adresse postale est 12 rue des Fleurs, 75015 Paris et mon numéro de mobile est le 06 12 34 56 78.\r\n\r\nSociété : Merci. Je viens de vérifier votre commande. Effectivement, vous avez commandé un carton de Tropical Sunrise. Quel était le produit que vous vouliez commander à la place ?\r\n\r\nClient : Je voulais commander un carton de Virgin Mojito.\r\n\r\nSociété : Je comprends. Nous pouvons vous envoyer gratuitement un carton de Virgin Mojito et vous gardez le carton de Tropical Sunrise, ou nous vous remboursons intégralement et vous nous renvoyez le carton de Tropical Sunrise à nos frais.\r\n\r\nClient : Je préfère la deuxième option, car je n’aime pas le Tropical Sunrise.\r\n\r\nSociété : Très bien. Dans ce cas, nous allons vous envoyer par e-mail une étiquette de retour prépayée que vous devrez coller sur le carton. Vous devrez ensuite déposer le colis dans un point relais proche de chez vous dans les 14 jours. Une fois que nous aurons reçu le colis en bon état, nous procéderons au remboursement sur votre compte bancaire.\r\n\r\nClient : D’accord. Merci beaucoup pour votre aide.\r\n\r\nSociété : Je vous en prie. C’est notre devoir de satisfaire nos clients. Nous sommes navrés pour ce désagrément et nous espérons que vous apprécierez le Virgin Mojito. Au revoir et bonne journée.\r\n\r\nClient : Au revoir et merci encore.",
                Language = "fr",
                HtmlContent = "<p><strong>Client</strong> : Bonjour, je suis Monsieur Dupond. J’ai commandé chez vous un carton de cocktail sans alcool Tropical Sunrise, mais je me suis trompé de produit. Je voudrais le renvoyer et me faire rembourser.</p> <p><strong>Société</strong> : Bonjour, Monsieur Dupond. Je suis Madame Martin, la responsable du service client de Cocktailo. Je suis désolée pour cette erreur. Pouvez-vous me donner votre numéro de commande, votre adresse e-mail, votre adresse postale et votre numéro de mobile s’il vous plaît ?</p> <p><strong>Client</strong> : Oui, bien sûr. Mon numéro de commande est le 123456789, mon adresse e-mail est dupond@gmail.com, mon adresse postale est 12 rue des Fleurs, 75015 Paris et mon numéro de mobile est le 06 12 34 56 78.</p> <p><strong>Société</strong> : Merci. Je viens de vérifier votre commande. Effectivement, vous avez commandé un carton de Tropical Sunrise. Quel était le produit que vous vouliez commander à la place ?</p> <p><strong>Client</strong> : Je voulais commander un carton de Virgin Mojito.</p> <p><strong>Société</strong> : Je comprends. Nous pouvons vous envoyer gratuitement un carton de Virgin Mojito et vous gardez le carton de Tropical Sunrise, ou nous vous remboursons intégralement et vous nous renvoyez le carton de Tropical Sunrise à nos frais.</p> <p><strong>Client</strong> : Je préfère la deuxième option, car je n’aime pas le Tropical Sunrise.</p> <p><strong>Société</strong> : Très bien. Dans ce cas, nous allons vous envoyer par e-mail une étiquette de retour prépayée que vous devrez coller sur le carton. Vous devrez ensuite déposer le colis dans un point relais proche de chez vous dans les 14 jours. Une fois que nous aurons reçu le colis en bon état, nous procéderons au remboursement sur votre compte bancaire.</p> <p><strong>Client</strong> : D’accord. Merci beaucoup pour votre aide.</p> <p><strong>Société</strong> : Je vous en prie. C’est notre devoir de satisfaire nos clients. Nous sommes navrés pour ce désagrément et nous espérons que vous apprécierez le Virgin Mojito. Au revoir et bonne journée.</p> <p><strong>Client</strong> : Au revoir et merci encore.</p>"
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


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}