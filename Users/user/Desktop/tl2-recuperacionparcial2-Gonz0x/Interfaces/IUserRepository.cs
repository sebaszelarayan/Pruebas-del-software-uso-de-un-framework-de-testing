using tl2_recuperacionparcial2_Gonz0x.Models;

namespace tl2_recuperacionparcial2_Gonz0x.Interfaces
{
    public interface IUserRepository
    {
        Usuario? GetUser(string username, string password);
        Usuario? GetById(int id);
    }
}
