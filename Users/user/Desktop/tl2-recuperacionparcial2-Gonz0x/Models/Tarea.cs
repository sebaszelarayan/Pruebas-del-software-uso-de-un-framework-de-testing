namespace tl2_recuperacionparcial2_Gonz0x.Models
{
    public enum Estado{Pendiente, Iniciada, Realizada};
    public class Tarea
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion{ get; set; }
        public int Complejidad {get; set; }
        public Estado Estado { get; set; } 

    }
}