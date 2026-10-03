using System;

public class Producto
{
    public string Nombre { get; set; }
    public decimal Precio { get; set; }
    public string Categoria { get; set; }

    public Producto(string nombre, decimal precio, string categoria)
    {
        Nombre = nombre;
        Precio = precio;
        Categoria = categoria;
    }

    // PUNTO 2: Lanza excepción si es negativo
    // Marcamos el método como 'virtual' para que Moq pueda sobrescribirlo (Test Double) en el Punto 3
    public virtual void ActualizarPrecio(decimal nuevoPrecio) 
    {
        if (nuevoPrecio < 0)
        {
            throw new ArgumentException("El precio no puede ser negativo");
        }
        Precio = nuevoPrecio;
    }
}