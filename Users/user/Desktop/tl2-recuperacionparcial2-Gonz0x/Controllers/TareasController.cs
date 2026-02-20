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
            if (!(_auth.HasAccessLevel("Admin") || _auth.HasAccessLevel("Cliente"))) return RedirectToAction(nameof(AccesoDenegado));
            return null;
        }

        private IActionResult CheckAdminPermissions() {
            if (!_auth.IsAuthenticated()) return RedirectToAction("Index", "Login");
            if (!_auth.HasAccessLevel("Admin")) return RedirectToAction(nameof(AccesoDenegado));
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
                var tareasVM = tareas.Select(t => new TareaIndexViewModel
                {
                    Id = t.Id,
                    Titulo = t.Titulo,
                    Descripcion = t.Descripcion,
                    Complejidad = t.Complejidad,
                    Estado = t.Estado
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
            var perm = CheckAdminPermissions();
            if (perm != null) return perm;

            var items = Enum.GetValues(typeof(Estado))
            .Cast<Estado>()
            .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() })
            .ToList();

            var vm = new TareaCreateViewModel 
            {
                ListaEstados = new SelectList(items, "Value", "Text")
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
                // VALIDACIÓN: Bloquear suma total > 50
                int sumaTotal = 0;
                var tareas = _tareaRepository.GetAll();
                foreach (var t in tareas)
                {
                    sumaTotal += t.Complejidad;
                }
                //suma total de complejidad de todas las tareas registradas
                if (sumaTotal >= 50)
                {
                    ModelState.AddModelError("Complejidad", "No puedes crear una tarea que haga que la suma complejidad total de las registradas sea > 50.");
                }
                if (!ModelState.IsValid || sumaTotal >= 50)
                {
                    var items = Enum.GetValues(typeof(Estado))
                    .Cast<Estado>()
                    .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() })
                    .ToList();
                    vm.ListaEstados = new SelectList(items, "Value", "Text");
                    return View(vm);
                }

                var tarea = new Tarea()
                {
                    Titulo = vm.Titulo,
                    Descripcion = vm.Descripcion,
                    Complejidad = vm.Complejidad,
                    Estado = vm.Estado
                };

                _tareaRepository.Add(tarea);
                _logger.LogInformation("Tarea '{Titulo}' creada exitosamente.", tarea.Titulo);
                return RedirectToAction("Index");                
            }
            catch (Exception ex)
            {
                // Registro del error serializado
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

            var items = Enum.GetValues(typeof(Estado)).Cast<Estado>().Select(e => new SelectListItem
            { 
                Value = e.ToString(), 
                Text = e.ToString() 
            })
            .ToList();
            var vm = new TareaUpdateViewModel
            {
                Id = tarea.Id,
                Titulo = tarea.Titulo,
                Descripcion = tarea.Descripcion,
                Complejidad = tarea.Complejidad,
                Estado = tarea.Estado,
                //Asignar la lista al ViewModel
                ListaEstados = new SelectList(items, "Value", "Text", tarea.Estado.ToString())

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
                // VALIDACIÓN: Bloquear suma total > 50
                int sumaTotal = 0;
                var tareas = _tareaRepository.GetAll();
                foreach (var t in tareas)
                {
                    sumaTotal += t.Complejidad;
                }
                var tareaVieja = _tareaRepository.GetById(id);
                sumaTotal -= tareaVieja.Complejidad;
                //suma total de complejidad de todas las tareas registradas
                if ((sumaTotal + vm.Complejidad) >= 50)
                {
                    ModelState.AddModelError("Complejidad", "No puedes editar una tarea que haga que la suma complejidad total de las registradas sea > 50.");
                }

                if (!ModelState.IsValid || sumaTotal >= 50)
                {
                    // Si hay error, recargamos la lista de estados para que no falle
                    var items = Enum.GetValues(typeof(Estado))
                        .Cast<Estado>()
                        .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() })
                        .ToList();
                    vm.ListaEstados = new SelectList(items, "Value", "Text", vm.Estado.ToString());
                    
                    return View(vm);
                }

                var tarea = _tareaRepository.GetById(id);
                if (tarea == null) return NotFound();

                tarea.Titulo = vm.Titulo;
                tarea.Descripcion = vm.Descripcion;
                tarea.Complejidad = vm.Complejidad;
                tarea.Estado = vm.Estado;

                _tareaRepository.Update(id, tarea);
                _logger.LogInformation("Tarea ID {Id} editada correctamente. Complejidad: {Complejidad}", id, vm.Complejidad);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                // LOG: Error serializado
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
                _logger.LogInformation("Tarea ID {Id} eliminada.", tarea.Id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                TempData["Error"] = "No se pudo eliminar la tarea.";
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

       