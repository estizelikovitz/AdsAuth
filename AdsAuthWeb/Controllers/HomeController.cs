using AdsAuth;
using AdsAuthWeb.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AdsAuthWeb.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=AdsAuth;Integrated Security=true;";

        public IActionResult Index()
        {
            Class1 c = new(_connectionString);
            AdVM vm = new();
            vm.Ads=c.GetAds();
            vm.IsAuthenticated = User.Identity.IsAuthenticated;
            if(User.Identity.IsAuthenticated)
            {
                string email = User.Identity.Name;
                vm.CurrentUser = c.GetByEmail(email);
            }


            return View(vm);
        }
        public IActionResult AddUser()
        {
         
            return View();
        }
        public IActionResult Login()
        {
            if (TempData["message"] != null)
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }


        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var repo = new Class1(_connectionString);
            var user = repo.Login(email, password);
            if (user == null)
            {
                TempData["message"] = "Invalid login!";
                return RedirectToAction("Login");
            }

            var claims = new List<Claim>
            {

                new Claim("user", email)
            };

            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", "user", "role"))).Wait();

            return Redirect("/home/index");
        }
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/");
        }
        [HttpPost]
        public IActionResult AddUser(User user, string password)
        {
            var repo = new Class1(_connectionString);
            repo.AddUser(user, password);
            var claims = new List<Claim>
            {
                new Claim("user", user.Email)
            };

            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", "user", "role"))).Wait();
            return Redirect("/home/index");
        }
        [Authorize]
        public IActionResult NewAd()
        {
            return View();
        }
        [HttpPost]
        public IActionResult NewAd(Ad ad)
        {
            Class1 repo = new(_connectionString);
            string email = User.Identity.Name;
            User currentUser = repo.GetByEmail(email);
            ad.UserId=currentUser.Id;
            repo.AddAd(ad);
            return Redirect("/home/index");
        }
        public IActionResult Delete(int id)
        {
            Class1 repo = new(_connectionString);
            repo.Delete(id);
            return Redirect("/home/index");
        }
        [Authorize]
        public IActionResult MyAccount()
        {
            Class1 repo = new(_connectionString);
            string email = User.Identity.Name;
            User currentUser = repo.GetByEmail(email);
            return View(repo.GetAdsById(currentUser.Id));
        }

    }
}
