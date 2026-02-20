using tl2_recuperacionparcial2_Gonz0x.Interfaces;
using tl2_recuperacionparcial2_Gonz0x.Models;
using Microsoft.Data.Sqlite;

namespace tl2_recuperacionparcial2_Gonz0x.Repositorios
{
    public class TareaRepository : ITareaRepository
    {
        private string cadenaConexion;

        public TareaRepository(string cadenaConexion)
        {
            this.cadenaConexion = cadenaConexion;
        }
        
        public void Add(Tarea tarea)
        {
            using var conexion = new SqliteConnection(cadenaConexion);
            conexion.Open();
            string sql = "INSERT INTO Tareas(Titulo, Descripcion, Complejidad, Estado) VALUES(@Titulo, @Descripcion, @Complejidad, @Estado)";
            using var comando = new SqliteCommand(sql, conexion);
            comando.Parameters.Add(new SqliteParameter("@Titulo", tarea.Titulo));
            comando.Parameters.Add(new SqliteParameter("@Descripcion", tarea.Descripcion));
            comando.Parameters.Add(new SqliteParameter("@Complejidad", tarea.Complejidad));            
            comando.Parameters.Add(new SqliteParameter("@Estado", tarea.Estado.ToString()));
            int filas = comando.ExecuteNonQuery();
            if (filas == 0)
                throw new Exception("No se pudo crear la Tarea");
        }
        
        public void Update(int id, Tarea tarea)
        {
            using var conexion = new SqliteConnection(cadenaConexion);
            conexion.Open();
            string sql = "UPDATE Tareas SET Titulo = @Titulo, Descripcion = @Descripcion, Complejidad = @Complejidad, Estado = @Estado WHERE Id = @Id";
            using var comando = new SqliteCommand(sql, conexion);
            comando.Parameters.Add(new SqliteParameter("@Id", id));
            comando.Parameters.Add(new SqliteParameter("@Titulo", tarea.Titulo));
            comando.Parameters.Add(new SqliteParameter("@Descripcion", tarea.Descripcion));
            comando.Parameters.Add(new SqliteParameter("@Complejidad", tarea.Complejidad));            
            comando.Parameters.Add(new SqliteParameter("@Estado", tarea.Estado.ToString()));
            int filas = comando.ExecuteNonQuery();
            if (filas == 0)
                throw new Exception("No se pudo modificar la tarea");
        }

        public List<Tarea> GetAll()
        {
            var tareas = new List<Tarea>();
            using var conexion = new SqliteConnection(cadenaConexion);
            conexion.Open();
            string sql = "SELECT * FROM Tareas";
            using var comando = new SqliteCommand(sql, conexion);
            using SqliteDataReader reader = comando.ExecuteReader();
            while (reader.Read())
            {
                var tarea = new Tarea
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Titulo = reader["Titulo"].ToString(), 
                    Descripcion = reader["Descripcion"].ToString(),
                    Complejidad = Convert.ToInt32(reader["Complejidad"]),
                    Estado = Enum.Parse<Estado>(reader["Estado"].ToString())
                };
                tareas.Add(tarea);
            }
            if (tareas.Count == 0)
            throw new Exception("No existen tareas cargadas");
            return tareas;
        }

        public Tarea GetById(int id)
        {
            using var conexion = new SqliteConnection(cadenaConexion);
            conexion.Open();
            string sql = "SELECT * FROM Tareas WHERE Id = @Id";
            using var comando = new SqliteCommand(sql, conexion);
            comando.Parameters.Add(new SqliteParameter("@Id", id));
            using var reader = comando.ExecuteReader();
            if(reader.Read())
            {
                return new Tarea
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Titulo = reader["Titulo"].ToString(), 
                    Descripcion = reader["Descripcion"].ToString(),
                    Complejidad = Convert.ToInt32(reader["Complejidad"]),
                    Estado = Enum.Parse<Estado>(reader["Estado"].ToString())
                };
            }
            throw new Exception($"La Tarea con ID {id} no existe en la base de datos.");
        }  

        public void Delete(int id)
        {
            using var conexion = new SqliteConnection(cadenaConexion);
            conexion.Open();
            string sql = "DELETE FROM Tareas WHERE Id = @Id";
            using var comando = new SqliteCommand(sql, conexion);
            comando.Parameters.Add(new SqliteParameter("@Id", id));
            int filas = comando.ExecuteNonQuery();
            if (filas == 0)
                throw new Exception($"No se pudo eliminar. Tarea {id} inexistente");
        }
    }
}
