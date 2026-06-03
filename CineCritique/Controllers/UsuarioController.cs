using CineCritique.Models;
using CineCritique.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CineCritique.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly ImagenStorage _imagenStorage;
        private readonly IEmailService _emailService;

        public UsuarioController(UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager, ImagenStorage imagenStorage, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _imagenStorage = imagenStorage;
            _emailService = emailService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel user)
        {
            if (ModelState.IsValid)
            {
                var resultado = await _signInManager.PasswordSignInAsync
                    (user.Email, user.Clave, user.Recordarme, lockoutOnFailure: false);

                if (resultado.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Inicio de sesión inválido");
                }
            }

            return View(user);
        }

        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(RegistroViewModel user)
        {
            if (ModelState.IsValid)
            {
                var newUsuario = new Usuario
                {
                    UserName = user.Email,
                    Email = user.Email,
                    Nombre = user.Nombre,
                    Apellido = user.Apellido,
                    ImagenUrlPerfil = "/images/default-avatar.png"
                };

                var resultado = await _userManager.CreateAsync(newUsuario, user.Clave);

                if (resultado.Succeeded)
                {
                    await _signInManager.SignInAsync(newUsuario, isPersistent: false);
                    await _emailService.SendAsync(newUsuario.Email, "Bienvenido a Cine Critique", "<h1>Gracias por registrarte en Cine Critique</h1><p>Esperamos que disfrutes la plataforma</p>");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in resultado.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(user);
        }

        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> MiPerfil()
        {
            var usuarioActual = await _userManager.GetUserAsync(User);

            var miPerfilViewModel = new MiPerfilViewModel
            {
                Nombre = usuarioActual.Nombre,
                Apellido = usuarioActual.Apellido,
                Email = usuarioActual.Email,
                ImagenUrlPerfil = usuarioActual.ImagenUrlPerfil
            };

            return View(miPerfilViewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MiPerfil(MiPerfilViewModel user)
        {
            if (ModelState.IsValid)
            {
                var usuarioActual = await _userManager.GetUserAsync(User);

                try
                {
                    if (user.ImagenPerfil is not null && user.ImagenPerfil.Length > 0)
                    {
                        // opcional: borrar la anterior (si no es placeholder)
                        if (!string.IsNullOrWhiteSpace(usuarioActual.ImagenUrlPerfil))
                            await _imagenStorage.DeleteAsync(usuarioActual.ImagenUrlPerfil);

                        var nuevaRuta = await _imagenStorage.SaveAsync(usuarioActual.Id, user.ImagenPerfil);
                        usuarioActual.ImagenUrlPerfil = nuevaRuta;
                        user.ImagenUrlPerfil = nuevaRuta;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(user);
                }

                usuarioActual.Nombre = user.Nombre;
                usuarioActual.Apellido = user.Apellido;

                var resultado = await _userManager.UpdateAsync(usuarioActual);

                if (resultado.Succeeded)
                {
                    ViewBag.Mensaje = "Perfil actualizado con éxito.";
                    return View(user);
                }
                else
                {
                    foreach (var error in resultado.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(user);
        }
    }
}
