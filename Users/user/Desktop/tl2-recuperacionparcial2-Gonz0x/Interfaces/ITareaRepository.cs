using tl2_recuperacionparcial2_Gonz0x.Models;
namespace tl2_recuperacionparcial2_Gonz0x.Interfaces
{
    public interface ITareaRepository
    {
        List<Tarea> GetAll();
        Tarea? GetById(int id);
        void Add(Tarea t);
        void Update(int id, Tarea t);
        void Delete(int id);
    }
}