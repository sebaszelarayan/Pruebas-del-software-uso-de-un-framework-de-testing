using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using tl2_recuperacionparcial2_Gonz0x.Models;

namespace tl2_recuperacionparcial2_Gonz0x.ViewModels
{
    public class TareaIndexViewModel
    {
        [Display(Name = "Id")]
        public int Id { get; set; }
        
        [Display(Name = "Título de la Tarea")]
        public string Titulo { get; set; }        
        
        [Display(Name = "Descripcion de la Tarea")]
        public string Descripcion { get; set; } 

        [Display(Name = "Complejidad")]
        public int Complejidad { get; set; }

        [Display(Name = "Estado de la Tarea")]
        public Estado Estado { get; set; }

    }
}
