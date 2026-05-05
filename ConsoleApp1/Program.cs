using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookWorldSystem
{
    // ==========================================
    // ACA VIENE LA CAPA DE MODELOS (POO)
    // ==========================================
    public abstract class EntidadBase
    {
        public int Id { get; set; }
    }

    public class Libro : EntidadBase
    {
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public bool Disponible { get; set; } = true;

        public override string ToString() =>
            $"[ID: {Id:D3}] {Titulo.PadRight(20)} | Autor: {Autor.PadRight(15)} | Status: {(Disponible ? "DISPONIBLE" : "PRESTADO")}";
    }

    public class Usuario : EntidadBase
    {
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public List<Libro> LibrosPrestados { get; set; } = new List<Libro>();
    }

    // ==========================================
    // ACA LA CAPA DE DATOS (O EL Singleton)
    // ==========================================
    public class BibliotecaContext
    {
        private static BibliotecaContext? _instancia;
        public List<Usuario> Usuarios { get; set; } = new List<Usuario>();
        public List<Libro> Libros { get; set; } = new List<Libro>();

        private BibliotecaContext() { }

        public static BibliotecaContext Instancia => _instancia ??= new BibliotecaContext();

        public void CargarDatos()
        {
            if (Libros.Count == 0)
            {
                Libros.Add(new Libro { Id = 101, Titulo = "C# Avanzado", Autor = "M. Stephens" });
                Libros.Add(new Libro { Id = 102, Titulo = "Patrones de Diseño", Autor = "E. Gamma" });
                Libros.Add(new Libro { Id = 103, Titulo = "Clean Code", Autor = "R. Martin" });
            }
        }
    }

    // ==========================
    // DE LA INTERFAZ Y LÓGICA
    // ==========================
    class Program
    {
        static BibliotecaContext db = BibliotecaContext.Instancia;

        static void Main(string[] args)
        {
            // Esta parte soluciona el problema de los caracteres extraños que tengamos como input
            Console.OutputEncoding = Encoding.UTF8;
            db.CargarDatos();
            bool salir = false;

            while (!salir)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔══════════════════════════════════════════════════════╗");
                Console.WriteLine("║            SISTEMA BIBLIOTECARIO BOOKWORLD           ║");
                Console.WriteLine("╚══════════════════════════════════════════════════════╝");
                Console.ResetColor();
                Console.WriteLine(" 1. Registrar Estudiante");
                Console.WriteLine(" 2. Ver Estudiantes Registrados");
                Console.WriteLine(" 3. Ver Catálogo de Libros");
                Console.WriteLine(" 4. Realizar Préstamo");
                Console.WriteLine(" 5. Devolución de Libro");
                Console.WriteLine(" 6. Reporte de Préstamos Activos");
                Console.WriteLine(" 7. Salir");
                Console.Write("\n > Seleccione una opción: ");

                switch (Console.ReadLine())
                {
                    case "1": RegistrarEstudiante(); break;
                    case "2": ListarEstudiantes(); break;
                    case "3": ListarLibros(); break;
                    case "4": RealizarPrestamo(); break;
                    case "5": DevolverLibro(); break;
                    case "6": MostrarReporte(); break;
                    case "7": salir = true; break;
                }
            }
        }

        // ALGUNOS  MÉTODOS DE APOYO 

        static void RegistrarEstudiante()
        {
            Console.Write("\nNombre completo: ");
            string nombre = Console.ReadLine() ?? "";
            Console.Write("Correo: ");
            string correo = Console.ReadLine() ?? "";

            int id = db.Usuarios.Count + 1;
            db.Usuarios.Add(new Usuario { Id = id, Nombre = nombre, Correo = correo });
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n[EXITO] Estudiante guardado.");
            Console.ResetColor();
            Console.ReadKey();
        }

        static void ListarEstudiantes()
        {
            Console.Clear();
            Console.WriteLine("--- LISTADO DE ESTUDIANTES ---");
            if (!db.Usuarios.Any()) Console.WriteLine("No hay registros.");
            else db.Usuarios.ForEach(u => Console.WriteLine($"ID: {u.Id:D3} | {u.Nombre} ({u.Correo})"));
            Console.ReadKey();
        }

        static void ListarLibros()
        {
            Console.Clear();
            Console.WriteLine("--- CATALOGO DE LIBROS ---");
            db.Libros.ForEach(l => Console.WriteLine(l.ToString()));
            Console.ReadKey();
        }

        static void RealizarPrestamo()
        {
            Console.Write("\nID Estudiante: ");
            if (int.TryParse(Console.ReadLine(), out int uId))
            {
                var user = db.Usuarios.FirstOrDefault(u => u.Id == uId);
                if (user == null) { Console.WriteLine("Estudiante no encontrado."); Console.ReadKey(); return; }

                Console.Write("ID Libro: ");
                if (int.TryParse(Console.ReadLine(), out int lId))
                {
                    var libro = db.Libros.FirstOrDefault(l => l.Id == lId);
                    if (libro != null && libro.Disponible && user.LibrosPrestados.Count < 3)
                    {
                        libro.Disponible = false;
                        user.LibrosPrestados.Add(libro);
                        Console.WriteLine("Préstamo registrado.");
                    }
                    else Console.WriteLine("No se puede realizar el préstamo (Límite o disponibilidad).");
                }
            }
            Console.ReadKey();
        }

        static void DevolverLibro()
        {
            Console.Write("\nID Estudiante: ");
            if (int.TryParse(Console.ReadLine(), out int uId))
            {
                var user = db.Usuarios.FirstOrDefault(u => u.Id == uId);
                if (user != null && user.LibrosPrestados.Any())
                {
                    var libro = user.LibrosPrestados.First();
                    libro.Disponible = true;
                    user.LibrosPrestados.Remove(libro);
                    Console.WriteLine($"Libro '{libro.Titulo}' devuelto.");
                }
                else Console.WriteLine("No hay libros para devolver.");
            }
            Console.ReadKey();
        }

        static void MostrarReporte()
        {
            Console.Clear();
            Console.WriteLine("--- REPORTE DE PRÉSTAMOS ACTIVOS ---");
            var activos = db.Usuarios.Where(u => u.LibrosPrestados.Any());
            foreach (var u in activos)
            {
                Console.WriteLine($"\nEstudiante: {u.Nombre}");
                u.LibrosPrestados.ForEach(l => Console.WriteLine($" - {l.Titulo}"));
            }
            Console.ReadKey();
        }
    }
}