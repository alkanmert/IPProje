using AutoMapper;
using IPProje.Extensions;
using IPProje.Extensions.ImageHelpers;
using IPProje.Extensions.UnitOfWorks;
using IPProje.Models;
using IPProje.ViewModels.Articles;
using IPProje.ViewModels.Categories;
using IPProje.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Security.Claims;
using static IPProje.Extensions.Messages.Messages;
using Article = IPProje.Models.Article;


namespace IPProje.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var article = await unitOfWork.GetRepository<Article>().GetAsync(x => !x.IsDeleted && x.Id == id, x => x.Category, i => i.Image, u => u.User);
            var map = mapper.Map<ArticleModel>(article);

            return View(map);
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(UserLoginModel model)
        {

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Geçersiz Kullanıcı Adı veya Parola!");
                return View();
            }
            var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);

            if (signInResult.Succeeded)
            {
                return RedirectToAction("Index");
            }
            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError("", "Kullanıcı Girişi " + user.LockoutEnd + " kadar kısıtlanmıştır!");
                return View();
            }
            ModelState.AddModelError("", "Geçersiz Kullanıcı Adı veya Parola Başarısız Giriş Sayısı :" + await _userManager.GetAccessFailedCountAsync(user) + "/3");
            return View();
        }
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterModel model)
        {


            if (!ModelState.IsValid)
            {
                return View(model);

            }
            var identityResult = await _userManager.CreateAsync(new() { FirstName = model.FirstName, LastName = model.LastName, UserName = model.UserName, Email = model.Email, PhoneNumber = model.PhoneNumber }, model.Password);

            if (!identityResult.Succeeded)
            {
                foreach (var item in identityResult.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }

                return View(model);
            }

            // default olarak Uye rolü ekleme
            var user = await _userManager.FindByNameAsync(model.UserName);
            var roleExist = await _roleManager.RoleExistsAsync("Uye");
            if (!roleExist)
            {
                var role = new AppRole { Name = "Uye" };
                await _roleManager.CreateAsync(role);
            }

            await _userManager.AddToRoleAsync(user, "Uye");

            return RedirectToAction("Login");
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home", new { Area = "" });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
