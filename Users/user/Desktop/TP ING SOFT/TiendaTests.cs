using System;
using System.Collections.Generic;
using Xunit;
using Moq; // Librería para los Test Doubles

public class TiendaTests : IDisposable
{
    private readonly Tienda _tiendaBase;

    // ==========================================
    // PUNTO 4: FIXTURES (Setup)
    // Se ejecuta antes de cada método de prueba [Fact]
    // ==========================================
    public TiendaTests()
    {
        _tiendaBase = new Tienda();
        _tiendaBase.AgregarProducto(new Producto("Laptop", 1000m, "Electrónica"));
        _tiendaBase.AgregarProducto(new Producto("Mouse", 50m, "Electrónica"));
        _tiendaBase.AgregarProducto(new Producto("Teclado", 100m, "Electrónica"));
    }

    // Teardown: Se ejecuta al terminar cada prueba para limpiar
    public void Dispose()
    {
        _tiendaBase.Inventario.Clear(); 
    }

    // ==========================================
    // PUNTO 1: PRUEBAS BÁSICAS
    // ==========================================
    [Fact]
    public void Test_AgregarProducto()
    {
        var tienda = new Tienda();
        var prod = new Producto("Monitor", 200m, "Hardware");
        
        tienda.AgregarProducto(prod);
        
        Assert.Contains(prod, tienda.Inventario);
    }

    [Fact]
    public void Test_BuscarProducto_Existente()
    {
        // Usamos el fixture (la tienda que armó el constructor)
        var prod = _tiendaBase.BuscarProducto("Laptop");
        
        Assert.Equal("Laptop", prod.Nombre);
        Assert.Equal(1000m, prod.Precio);
    }

    // ==========================================
    // PUNTO 2: PRUEBAS CON EXCEPCIONES
    // ==========================================
    [Fact]
    public void Test_BuscarProducto_NoExistente_LanzaExcepcion()
    {
        var ex = Assert.Throws<KeyNotFoundException>(() => _tiendaBase.BuscarProducto("Auriculares"));
        Assert.Equal("Producto no encontrado", ex.Message);
    }

    [Fact]
    public void Test_EliminarProducto_Existente()
    {
        bool resultado = _tiendaBase.EliminarProducto("Mouse");
        
        Assert.True(resultado);
        // Verificamos que ya no está buscando de nuevo (debe fallar)
        Assert.Throws<KeyNotFoundException>(() => _tiendaBase.BuscarProducto("Mouse"));
    }

    [Fact]
    public void Test_ActualizarPrecio_Negativo_LanzaExcepcion()
    {
        var prod = new Producto("Tablet", 300m, "Electrónica");
        
        var ex = Assert.Throws<ArgumentException>(() => prod.ActualizarPrecio(-50m));
        Assert.Equal("El precio no puede ser negativo", ex.Message);
    }

    // ==========================================
    // PUNTO 3: USO DE DOBLES (MOCKS)
    // ==========================================
    [Fact]
    public void Test_AplicarDescuento_ConMock()
    {
        var tienda = new Tienda();
        
        // 1. Creamos el 'Test Double' (Mock) usando la librería Moq
        var mockProducto = new Mock<Producto>("Celular", 1000m, "Electrónica");
        
        // 2. Agregamos el objeto falso (.Object extrae la instancia simulada)
        tienda.AgregarProducto(mockProducto.Object);
        
        // 3. Ejecutamos el método
        tienda.AplicarDescuento("Celular", 20m); // 20% de 1000 = 200. Precio final: 800.
        
        // 4. Verificamos que Tienda llamó a ActualizarPrecio pasándole 800m exactamente 1 vez
        mockProducto.Verify(p => p.ActualizarPrecio(800m), Times.Once);
    }

    // ==========================================
    // PUNTO 5: PRUEBAS DE INTEGRACIÓN
    // ==========================================
    [Fact]
    public void Test_CalcularTotalCarrito()
    {
        var carrito = new List<string> { "Laptop", "Teclado" }; // 1000 + 100
        
        decimal total = _tiendaBase.CalcularTotalCarrito(carrito);
        
        Assert.Equal(1100m, total);
    }

    [Fact]
    public void Test_CalcularTotalCarrito_ConDescuentoPrevio()
    {
        // Integramos el descuento y luego sumamos el carrito
        _tiendaBase.AplicarDescuento("Laptop", 10m); // 10% descuento -> 900
        
        var carrito = new List<string> { "Laptop", "Mouse" }; // 900 + 50
        
        decimal total = _tiendaBase.CalcularTotalCarrito(carrito);
        
        Assert.Equal(950m, total);
    }
}