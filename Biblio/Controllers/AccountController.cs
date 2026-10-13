using Biblio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Biblio.Controllers
{
    public class AccountController : Controller
    {

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager; 
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]

        public async Task<IActionResult> Login(LoginModel login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            var user = await _userManager.FindByNameAsync(login.UserNameOrEmail) ?? await _userManager.FindByEmailAsync(login.UserNameOrEmail);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, login.Password, login.RememberMe, lockoutOnFailure: true);


                if(result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(login.ReturnUrl) && Url.IsLocalUrl(login.ReturnUrl))
                    {
                        return Redirect(login.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "La cuenta esta bloqueada temporalmente por intentos fallidos");
                }
            }
            ModelState.AddModelError(string.Empty, "Credenciales no validas.");
            return View(login);
        }

        [HttpGet]
        [AllowAnonymous]

        public IActionResult Register()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]

        public async Task<IActionResult> Register(RegisterModel register)
        {
            if (!ModelState.IsValid)
            {
                return View(register);
            }

            var user = new IdentityUser
            {
                UserName = register.Username,
                Email = register.Email
            };

            var result = await _userManager.CreateAsync(
                user,
                register.Password
            );

            if (result.Succeeded)
            {

                var resultadoRol = await _userManager
                .AddToRoleAsync(user, "Lector");

                if (!resultadoRol.Succeeded)
                {
                    foreach (var error in resultadoRol.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }



                await _signInManager.SignInAsync(
                    user,
                    isPersistent: false
                );


                return RedirectToAction("Index", "Home");
            }

            foreach(var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
                

            }

            ModelState.AddModelError(
                string.Empty,
                "Error al crear el usuario."
            );

            return View(register);
        }


        [HttpGet]
        public ActionResult AccessDenied()
        {
            return View();
        }

        


    }
}
