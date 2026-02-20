using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using tl2_recuperacionparcial2_Gonz0x.Models;
using tl2_recuperacionparcial2_Gonz0x.Repositorios;
using tl2_recuperacionparcial2_Gonz0x.ViewModels; 
using tl2_recuperacionparcial2_Gonz0x.Interfaces;

namespace tl2_recuperacionparcial2_Gonz0x
{
    public class TareasController : Controller
    {
        private readonly ITareaRepository _tareaRepository;
        private readonly IAuthenticationService _auth; 
        private readonly ILogger<TareasController> _logger;   
        
        public TareasController(ITareaRepository tareaRepository, IAuthenticationService auth, ILogger<TareasController> logger)
        {
            _tareaRepository = tareaRepository;
            _auth = auth;
            _logger = logger;
        }

        private IActionResult CheckReadPermissions() {
            if (!_auth.IsAuthenticated()) return RedirectToAction("Index", "Login");
            if (!(_auth.HasAccessLevel("Administrador") || _auth.HasAccessLevel("Cliente"))) return RedirectToAction(nameof(AccesoDenegado));
            return null;
        }

        private IActionResult CheckAdminPermissions() {
            if (!_auth.IsAuthenticated()) return RedirectToAction("Index", "Login");
            if (!_auth.HasAccessLevel("Administrador")) return RedirectToAction(nameof(AccesoDenegado));
            return null;
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                var perm = CheckReadPermissions();
                if (perm != null) return perm;
                var tareas = _tareaRepository.GetAll();
                var tareasVM = tareas.Select(p => new TareaIndexViewModel
                {
                    Id = p.Id,
                    Titulo = p.Titulo,
                    Anio = p.Anio,
                    Categoria = p.Categoria
                }).ToList();

                return View(tareasVM);
            }
            catch (Exception ex) {
                _logger.LogError(ex.ToString());
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!_auth.IsAuthenticated())
            return RedirectToAction("Index", "Login");

            if (!_auth.HasAccessLevel("Administrador"))
            return RedirectToAction(nameof(AccesoDenegado));       

            var items = Enum.GetValues(typeof(Categoria))
            .Cast<Categoria>()
            .Select(c => new SelectListItem { Value = c.ToString(), Text = c.ToString() })
            .ToList();

            var vm = new TareaCreateViewModel 
            {
                ListaCategorias = new SelectList(items, "Value", "Text")
            };
            return View(vm);
        }

        [HttpPost]
        public IActionResult Create(TareaCreateViewModel vm)
        {
            var perm = CheckAdminPermissions();
            if (perm != null) return perm;
            try
            {             
                if (!ModelState.IsValid)
                {
                    var items = Enum.GetValues(typeof(Categoria))
                    .Cast<Categoria>()
                    .Select(c => new SelectListItem { Value = c.ToString(), Text = c.ToString() })
                    .ToList();
                    vm.ListaCategorias = new SelectList(items, "Value", "Text");
                    return View(vm);
                }

                var tarea = new Tarea()
                {
                    Titulo = vm.Titulo,
                    Anio = vm.Anio,
                    Categoria = vm.Categoria
                };

                _tareaRepository.Add(tarea);
                _logger.LogInformation("Película '{Titulo}' creada exitosamente.", tarea.Titulo);
                return RedirectToAction("Index");                
            }
            catch (Exception ex)
            {
                // ❌ Registro del error serializado
                _logger.LogError(ex.ToString());
                return RedirectToAction("Error", "Home");
            }
        }
        
        
        [HttpGet]
        public IActionResult Edit(int id)
        {

            var perm = CheckAdminPermissions();
            if (perm != null) return perm;

            var tarea = _tareaRepository.GetById(id);
            if (tarea == null)
                return NotFound();

            var items = Enum.GetValues(typeof(Categoria)).Cast<Categoria>().Select(c => new SelectListItem
            { 
                Value = c.ToString(), 
                Text = c.ToString() 
            })
            .ToList();
            var vm = new TareaUpdateViewModel
            {
                Id = tarea.Id,
                Titulo = tarea.Titulo,
                Anio = tarea.Anio,
                Categoria = tarea.Categoria,
                // 2. Asignar la lista al ViewModel
                ListaCategorias = new SelectList(items, "Value", "Text", tarea.Categoria.ToString())
                // El cuarto argumento (tarea.Categoria.ToString()) selecciona la categoría actual.
            };

            return View(vm);
        }


        [HttpPost]
        public IActionResult Edit(int id, TareaUpdateViewModel vm)
        {

            var perm = CheckAdminPermissions();
            if (perm != null) return perm;            
            try 
            {
                // 🛠️ VALIDACIÓN: Bloquear años futuros
                if (vm.Anio > DateTime.Now.Year)
                {
                    ModelState.AddModelError("Anio", "No puedes editar una película con un año futuro.");
                }

                if (!ModelState.IsValid)
                {
                    // Si hay error, recargamos la lista de categorías para que el combo no falle
                    var items = Enum.GetValues(typeof(Categoria))
                        .Cast<Categoria>()
                        .Select(c => new SelectListItem { Value = c.ToString(), Text = c.ToString() })
                        .ToList();
                    vm.ListaCategorias = new SelectList(items, "Value", "Text", vm.Categoria.ToString());
                    
                    return View(vm);
                }

                var tarea = _tareaRepository.GetById(id);
                if (tarea == null) return NotFound();

                tarea.Titulo = vm.Titulo;
                tarea.Anio = vm.Anio;
                tarea.Categoria = vm.Categoria;

                _tareaRepository.Update(id, tarea);
                _logger.LogInformation("Película ID {Id} editada correctamente. Año: {Anio}", id, vm.Anio);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                // ❌ LOG: Error serializado (Consigna TP 11)
                _logger.LogError(ex.ToString());
                return RedirectToAction("Error", "Home");
            }
        }

        
        [HttpGet]
        public IActionResult Delete(int id)
        {

            var perm = CheckAdminPermissions();
            if (perm != null) return perm;
            var tarea = _tareaRepository.GetById(id);
            if (tarea == null)
                return NotFound();

            return View(tarea);
        }

        [HttpPost]
        public IActionResult Delete(Tarea tarea)
        {            
            var perm = CheckAdminPermissions();
            if (perm != null) return perm;
            
            try
            {
                _tareaRepository.Delete(tarea.Id);
                _logger.LogInformation("Película ID {Id} eliminada.", tarea.Id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                TempData["Error"] = "No se pudo eliminar la película.";
                return RedirectToAction("Index");
            }

        }

        [HttpGet]
        public IActionResult AccesoDenegado()
        {
            return View();
        }
    }


}                    

       