# Trabajo Práctico: Pruebas del Software (Gestión de Tienda)
**Materia:** Ingeniería de Software 1 / 2 (PU / LI / II)
**Lenguaje utilizado:** C# (.NET 8.0)
**Framework de pruebas:** xUnit + Moq

---

## 1. Pasos para la creación y ejecución del proyecto

**Requisitos Previos:**
* SDK de .NET 8.0 instalado.
* Visual Studio Code (con la extensión *C# Dev Kit*).

**Instrucciones de configuración (Terminal):**
1. Creamos una carpeta para el proyecto (`TP_ING_SOFT`) y la abrimos en VS Code.
2. Abrimos la terminal integrada y ejecutamos el siguiente comando para inicializar un proyecto de pruebas con xUnit:
   `dotnet new xunit`
3. Instalamos la librería **Moq** (necesaria para los dobles de prueba en el Punto 3) ejecutando:
   `dotnet add package Moq`
4. Borramos el archivo `UnitTest1.cs` que se genera por defecto.
5. Creamos los archivos `Producto.cs`, `Tienda.cs` y `TiendaTests.cs` con el código que se detalla en la sección 2.
6. Ejecutamos las pruebas tipeando en la consola:
   `dotnet test`

---