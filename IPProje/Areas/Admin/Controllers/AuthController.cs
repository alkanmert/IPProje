using IPProje.Models;
using IPProje.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IPProje.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthController : Controller
    {
        public AuthController()
        {

        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> AccessDenied()
        {
            return View();
        }
    }
}
