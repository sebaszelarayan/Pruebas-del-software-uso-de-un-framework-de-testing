using Microsoft.AspNetCore.Mvc;
using tl2_recuperacionparcial2_Gonz0x.Interfaces;
using tl2_recuperacionparcial2_Gonz0x.ViewModels;

namespace tl2_recuperacionparcial2_Gonz0x.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<LoginController> _logger;  
        public LoginController(IAuthenticationService authenticationService, ILogger<LoginController> logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;
        }

        // [HttpGet] Muestra la vista de login
        [HttpGet]
        public IActionResult Index()
        {
            return View(new LoginViewModel());
        }

        // [HttpPost] Procesa el login
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
                {
                    model.ErrorMessage = "Debe ingresar usuario y contraseña.";
                    return View("Index", model);
                }

                if (_authenticationService.Login(model.Username, model.Password))
                {
                    _logger.LogInformation("El usuario {Usuario} ingresó correctamente", model.Username);
                    return RedirectToAction("Index", "Home");
                }
                _logger.LogWarning("Intento de acceso inválido + Usuario: {Usuario} + Clave ingresada: {Clave}", model.Username, model.Password);
                model.ErrorMessage = "Credenciales inválidas.";
                return View("Index", model);
            }
            catch (Exception ex)
            {
                // ❌ Error en ejecución (Consigna 1.3)
                _logger.LogError(ex.ToString());
                model.ErrorMessage = "Ocurrió un error en el sistema.";
                return View("Index", model);
            }

        }

        // [HttpGet] Cierra sesión
        public IActionResult Logout()
        {
            _authenticationService.Logout();
            _logger.LogInformation("Usuario cerró sesión");
            return RedirectToAction("Index");
        }
    }
}
