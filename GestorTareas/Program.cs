using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GestionTareas
{
    enum TipoTarea
    {
        Persona,
        Trabajo,
        Ocio
    }

    class Tarea
    {
        private static int contadorId = 1;

        public int Id { get; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public TipoTarea Tipo { get; set; }
        public bool Prioridad { get; set; }

        // Constructor para nuevas tareas (ID automático)
        public Tarea(string nombre, string descripcion, TipoTarea tipo, bool prioridad)
        {
            Id = contadorId++;
            Nombre = nombre;
            Descripcion = descripcion;
            Tipo = tipo;
            Prioridad = prioridad;
        }

        // Constructor para importar tareas (ID manual)
        public Tarea(int id, string nombre, string descripcion, TipoTarea tipo, bool prioridad)
        {
            Id = id;
            Nombre = nombre;
            Descripcion = descripcion;
            Tipo = tipo;
            Prioridad = prioridad;

            if (id >= contadorId)
                contadorId = id + 1;
        }

        public override string ToString()
        {
            return $"{Id},{Nombre},{Descripcion},{Tipo},{Prioridad}";
        }

        public static Tarea DesdeTexto(string linea)
        {
            var partes = linea.Split(',');
            return new Tarea(
                int.Parse(partes[0]),
                partes[1],
                partes[2],
                Enum.Parse<TipoTarea>(partes[3]),
                bool.Parse(partes[4])
            );
        }
    }

    class Programa
    {
        static List<Tarea> tareas = new List<Tarea>();

        static void Main()
        {
            int opcion;
            do
            {
                Console.WriteLine("\n--- Menú de Gestión de Tareas ---");
                Console.WriteLine("1. Crear tarea");
                Console.WriteLine("2. Buscar tareas por tipo");
                Console.WriteLine("3. Eliminar tarea por ID");
                Console.WriteLine("4. Exportar tareas");
                Console.WriteLine("5. Importar tareas");
                Console.WriteLine("0. Salir");
                Console.Write("Seleccione una opción: ");
                int.TryParse(Console.ReadLine(), out opcion);

                switch (opcion)
                {
                    case 1: CrearTarea(); break;
                    case 2: BuscarTareas(); break;
                    case 3: EliminarTarea(); break;
                    case 4: ExportarTareas(); break;
                    case 5: ImportarTareas(); break;
                }

            } while (opcion != 0);
        }

        static void CrearTarea()
        {
            Console.Write("Nombre: ");
            string nombre = Console.ReadLine();
            Console.Write("Descripción: ");
            string descripcion = Console.ReadLine();

            Console.Write("Tipo (Persona, Trabajo, Ocio): ");
            if (!Enum.TryParse(Console.ReadLine(), true, out TipoTarea tipo))
            {
                Console.WriteLine("Tipo no válido.");
                return;
            }

            Console.Write("¿Alta prioridad? (Sí/No): ");
            string entradaPrioridad = Console.ReadLine()?.Trim().ToLower();

            bool prioridad;
            if (entradaPrioridad == "sí" || entradaPrioridad == "si")
            {
                prioridad = true;
            }
            else if (entradaPrioridad == "no")
            {
                prioridad = false;
            }
            else
            {
                Console.WriteLine("Valor de prioridad no válido. Escribe 'Sí' o 'No'.");
                return;
            }

            Tarea nueva = new Tarea(nombre, descripcion, tipo, prioridad);
            tareas.Add(nueva);
            Console.WriteLine("Tarea creada con éxito. ID: " + nueva.Id);
        }

        static void BuscarTareas()
        {
            Console.Write("Introduzca tipo de tarea a buscar (Persona, Trabajo, Ocio): ");
            if (Enum.TryParse(Console.ReadLine(), true, out TipoTarea tipo))
            {
                var encontradas = tareas.Where(t => t.Tipo == tipo).ToList();
                if (encontradas.Count == 0)
                {
                    Console.WriteLine("No se encontraron tareas de ese tipo.");
                }
                else
                {
                    foreach (var t in encontradas)
                    {
                        Console.WriteLine(t.ToString());
                    }
                }
            }
            else
            {
                Console.WriteLine("Tipo no válido.");
            }
        }

        static void EliminarTarea()
        {
            Console.Write("Ingrese el ID de la tarea a eliminar: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var tarea = tareas.FirstOrDefault(t => t.Id == id);
                if (tarea != null)
                {
                    tareas.Remove(tarea);
                    Console.WriteLine("Tarea eliminada.");
                }
                else
                {
                    Console.WriteLine("No se encontró una tarea con ese ID.");
                }
            }
        }

        static void ExportarTareas()
        {
            File.WriteAllLines("tareas.txt", tareas.Select(t => t.ToString()));
            Console.WriteLine("Tareas exportadas a 'tareas.txt'.");
        }

        static void ImportarTareas()
        {
            if (!File.Exists("tareas.txt"))
            {
                Console.WriteLine("El archivo 'tareas.txt' no existe.");
                return;
            }

            var lineas = File.ReadAllLines("tareas.txt");
            tareas.Clear(); // Opcional: Limpia tareas anteriores para evitar duplicados

            foreach (var linea in lineas)
            {
                try
                {
                    var tarea = Tarea.DesdeTexto(linea);
                    tareas.Add(tarea);
                }
                catch
                {
                    Console.WriteLine("Error al importar línea: " + linea);
                }
            }

            Console.WriteLine("Tareas importadas correctamente.\n");

            // Mostrar las tareas en formato tabla con bordes
            Console.WriteLine("ID   | Nombre             | Tipo     | Prioridad");
            Console.WriteLine("-----+--------------------+----------+----------");
            
            foreach (var t in tareas)
            {
                string prioridadTexto = t.Prioridad ? "Alta" : "Baja"; // Convertir booleano a texto
                Console.WriteLine("{0,-5} | {1,-18} | {2,-8} | {3,-8}",
                                  t.Id, t.Nombre, t.Tipo, prioridadTexto);
            }
        }
    }
}
