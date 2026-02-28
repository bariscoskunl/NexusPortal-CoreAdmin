using App.Mvc.Data;
using App.Mvc.Data.Entities;
using App.Mvc.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace App.Mvc.Controllers
{
    public class AuthController : Controller
    {

        private readonly AppDbContext _dbContext;

        public AuthController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Invalid input.";
                return View();
            }
        

            var user = await _dbContext.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.Email == loginViewModel.Email);
            if (user == null || user.Password != loginViewModel.Password)
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }
            if (user.IsApproved == false)
            { 
                ViewBag.Error = "Your account is not approved yet.";
                return View();
            }

            await DoLoginOperations(user);

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register([FromForm] RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Invalid input.";
                return View();
            }        

            var excitingUser = _dbContext.Users.Any(x => x.Email == registerViewModel.Email);
            if (excitingUser)
            {
                ViewBag.Error = "Email is already in use.";
                return View();
            }

            var userRole = _dbContext.Roles.FirstOrDefault(x => x.Name == "User");

            var user = new UserEntity
            {
                UserName = registerViewModel.UserName,
                Email = registerViewModel.Email,
                Password = registerViewModel.Password,
                RoleId = userRole.Id, 
                IsApproved = false,
                IsRejected = false
            };

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            return RedirectToAction("Login");
        }



        private Task DoLoginOperations(UserEntity user)
        {

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.Name),
                new Claim("Server-Time", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                IsPersistent = true,
            };
            return HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Auth");
        }

        public IActionResult AccessDenied()
        {
            return View();

        }

    }
}
