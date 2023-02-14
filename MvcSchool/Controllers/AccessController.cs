using Microsoft.AspNetCore.Mvc;

//Authentication
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MvcSchool.Models.Domain;
using MvcSchool.Data;
using Microsoft.EntityFrameworkCore;

namespace MvcSchool.Controllers
{
    public class AccessController : Controller
    {
        private readonly SchoolDbContext schoolDbContext;
        public AccessController(SchoolDbContext schoolDbContext)
        {
            this.schoolDbContext = schoolDbContext;
        }
        public IActionResult Login()
        {
            //Check is user is Already Logged In?
            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "School");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login modelLogin)
        {
            var account = schoolDbContext.Loginaccount.FirstOrDefault(x=> x.Email==modelLogin.Email);

            if (modelLogin.Password == account.Password)
            {
                account.KeepLoggedIn = modelLogin.KeepLoggedIn;
                await schoolDbContext.SaveChangesAsync();

                List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, modelLogin.Email)
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                AuthenticationProperties authenticationProperties = new AuthenticationProperties()
                {
                    AllowRefresh = false,
                    IsPersistent = modelLogin.KeepLoggedIn,
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), authenticationProperties);

                return RedirectToAction("Index", "School");
            }

            //int id = (from l in schoolDbContext.Loginaccount where l.AccountId == modelLogin.AccountId select l.AccountId);

            //var account = await schoolDbContext.Loginaccount.FindAsync(id);
            //if(account != null)
            //{
                //if (modelLogin.Password == account.Password) 
                //{
                    
                //}
            //}

            //if(modelLogin.Email == "zk@gmail.com" && modelLogin.Password == "password") 
            //{
                
           // }
            ViewData["ValidateMessage"] = "user not found";
            return View();
        }
    }
}
