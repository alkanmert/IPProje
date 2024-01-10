using IPProje.Roller;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IPProje.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class HomeController : Controller
    {
        [Authorize(Roles = $"{RoleConsts.Superadmin},{RoleConsts.Admin}, {RoleConsts.Yazar}")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
