using GPTCustomerService_Web.Models;
using GPTCustomerService_Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace GPTCustomerService_Web.ViewComponents
{
    public class TranscriptComponent : ViewComponent
    {
        
        public IViewComponentResult Invoke(string htmlContent)
        {
            
            return View("Default", htmlContent);
        }

    }
}
