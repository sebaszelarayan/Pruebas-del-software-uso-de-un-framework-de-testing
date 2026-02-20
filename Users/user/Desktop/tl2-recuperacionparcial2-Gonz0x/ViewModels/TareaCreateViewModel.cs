using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using tl2_recuperacionparcial2_Gonz0x.Models;

namespace tl2_recuperacionparcial2_Gonz0x.ViewModels
{
    public class TareaCreateViewModel
    {
        [Display(Name = "Título de la Tarea")]
        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El título debe tener entre 3 y 100 caracteres.")]
        public string Titulo { get; set; }        
        
        [Display(Name = "Complejidad")]
        [Required(ErrorMessage = "La complejidad es obligatorio.")]
        [Range(1, 10, ErrorMessage = "La complejidad debe ser entre 1 y 10.")]
        public int Complejidad { get; set; }

        [Display(Name = "Estado de la Tarea")]
        [Required(ErrorMessage = "El Estado es obligatorio.")]
        public Estado Estado { get; set; }

        [ValidateNever]
        public SelectList ListaTareas { get; set; }
    }
}