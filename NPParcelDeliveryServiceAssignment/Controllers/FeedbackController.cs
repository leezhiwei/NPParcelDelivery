using Microsoft.AspNetCore.Mvc;

namespace NPParcelDeliveryServiceAssignment.Controllers
{
    public class FeedbackController : Controller
    {
        public IActionResult MakeFeedback()
        {
            return View();
        }
        
    }
}
