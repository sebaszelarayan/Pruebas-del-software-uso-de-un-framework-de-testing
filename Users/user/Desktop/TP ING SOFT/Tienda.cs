using System;
using System.Collections.Generic;
using System.Linq;

public class Tienda
{
    public List<Producto> Inventario { get; private set; }

    public Tienda()
    {
        Inventario = new List<Producto>();
    }

    public void AgregarProducto(Producto producto)
    {
        Inventario.Add(producto);
    }

    public Producto BuscarProducto(string nombre)
    {
        // Usa LINQ para buscar. Si no lo encuentra, queda en null.
        var producto = Inventario.FirstOrDefault(p => p.Nombre == nombre);
        
        // PUNTO 2: Lanza excepción si no se encuentra
        if (producto == null)
        {
            throw new KeyNotFoundException("Producto no encontrado");
        }
        return producto;
    }

    public bool EliminarProducto(string nombre)
    {
        // Reutilizamos la búsqueda. Si no existe, BuscarProducto lanzará la excepción sola.
        var producto = BuscarProducto(nombre); 
        Inventario.Remove(producto);
        return true;
    }

    // PUNTO 3: Busca el producto, calcula y aplica el descuento
    public void AplicarDescuento(string nombre, decimal porcentaje)
    {
        var producto = BuscarProducto(nombre);
        decimal descuento = producto.Precio * (porcentaje / 100m);
        decimal nuevoPrecio = producto.Precio - descuento;
        producto.ActualizarPrecio(nuevoPrecio);
    }

    // PUNTO 5: Integra la búsqueda y suma los precios
    public decimal CalcularTotalCarrito(List<string> listaNombres)
    {
        decimal total = 0;
        foreach (var nombre in listaNombres)
        {
            var producto = BuscarProducto(nombre);
            total += producto.Precio;
        }
        return total;
    }
}