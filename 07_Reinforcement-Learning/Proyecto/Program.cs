
// Copyright (�) 2024+ - Miguel Rodríguez Gallego
// This code is under public domain
// miguelrodriguezgallego1@gmail.com

using System.Diagnostics;
using System.Globalization;
using System.IO;
using System;

namespace ReinforcementLearning
{
    class Program
    {
        static void Main(string[] args)
        {
            int[,] mazeLayout;
            string fileName = "maze.txt";
            string folderName = "MAZE";
            string filePath = GetMazeFilePath(folderName, fileName);

            // Seleccionar
            string consoleInput;
            do
            {
                Console.WriteLine("Selecciona si utilizaras un mapa personalizado (Archivo .txt en la Carpeta MAZE) o el por defecto: ");
                Console.WriteLine("0 = Si");
                Console.WriteLine("1 = No (Mapa por defecto)");
                Console.WriteLine(" ");
                consoleInput = Console.ReadLine().ToLower();
            } while (consoleInput != "0" && consoleInput != "1");
            Console.Clear();

            if (consoleInput == "0" && File.Exists(filePath))
            {
                Console.WriteLine("");
                Console.WriteLine($"Se ha cogido correctamente el mapa del archivo: {filePath}.");
                Console.WriteLine("");
                mazeLayout = LoadMazeFromFile(filePath);
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine($"El archivo {filePath} no existe o se ha elegido el por defecto.");
                Console.WriteLine("");
                mazeLayout = GetDefaultMaze();
            }

            // Posiciones de inicio y salida del laberinto
            int agentStartX = 1;
            int agentStartY = 1;
            int exitX = 1;
            int exitY = 6;

            /// Punto de inicio del algoritmo
            // Fila 
            Console.WriteLine(" ");
            PrintMap(mazeLayout);
            Console.WriteLine(" ");
            Console.WriteLine("Selecciona la fila de INICIO del algoritmo en el mapa: ");
            Console.WriteLine(" ");
            consoleInput = Console.ReadLine().ToLower();
            agentStartX = int.Parse(consoleInput);
            Console.Clear();
            // Columna
            Console.WriteLine(" ");
            PrintMap(mazeLayout);
            Console.WriteLine(" ");
            Console.WriteLine("Selecciona la columna de INICIO del algoritmo en el mapa: ");
            Console.WriteLine(" ");
            consoleInput = Console.ReadLine().ToLower();
            agentStartY = int.Parse(consoleInput);
            Console.Clear();

            /// Salida del algoritmo
            // Fila 
            Console.WriteLine(" ");
            PrintMap(mazeLayout, agentStartX, agentStartY);
            Console.WriteLine(" ");
            Console.WriteLine("Selecciona la fila de SALIDA del algoritmo en el mapa: ");
            Console.WriteLine(" ");
            consoleInput = Console.ReadLine().ToLower();
            exitX = int.Parse(consoleInput);
            Console.Clear();
            // Columna
            Console.WriteLine(" ");
            PrintMap(mazeLayout, agentStartX, agentStartY);
            Console.WriteLine(" ");
            Console.WriteLine("Selecciona la columna de SALIDA del algoritmo en el mapa: ");
            Console.WriteLine(" ");
            consoleInput = Console.ReadLine().ToLower();
            exitY = int.Parse(consoleInput);
            Console.Clear();

            Console.WriteLine(" ");
            PrintMap(mazeLayout, agentStartX, agentStartY, exitX, exitY);
            Console.WriteLine(" ");

            // Configurar parámetros de aprendizaje
            double learningRate = 0.1;
            double discountRate = 0.9;
            double epsilon = 0.1;
            int rewardGoal = 100;
            int rewardMove = -1;
            int numberOfTrainings = 100; // 100

            /// Escoger parámetros interactivos
            learningRate = GetParameter("Tasa de aprendizaje", 0.1);
            discountRate = GetParameter("Tasa de descuento", 0.9);
            epsilon = GetParameter("Tasa de exploración", 0.1);
            rewardGoal = (int)GetParameter("Recompensa por llegar a la meta", 100);
            rewardMove = (int)GetParameter("Recompensa por movimiento", -1);
            numberOfTrainings = (int)GetParameter("Número de entrenamientos", 100);

            // Seleccionar algoritmo de aprendizaje por refuerzo (SARSA o Q-Learning)
            do
            {
                Console.WriteLine(" ");
                Console.WriteLine(" ");
                Console.WriteLine("Selecciona el algoritmo de aprendizaje por refuerzo (SARSA o Q-Learning): ");
                Console.WriteLine("0 = SARSA");
                Console.WriteLine("1 = Q-Learning");
                Console.WriteLine(" ");
                consoleInput = Console.ReadLine().ToLower();
            } while (consoleInput != "0" && consoleInput != "1");
            Console.Clear();

            // Ejecutar proceso de aprendizaje
            if (consoleInput == "0")
            {
                // Ejecutar código
                Sarsa sarsa = new Sarsa(mazeLayout, learningRate, discountRate, epsilon, rewardGoal, rewardMove);
                sarsa.Train(agentStartX, agentStartY, exitX, exitY, numberOfTrainings);
                Console.ReadLine();
            }
            else if (consoleInput == "1")
            {
                QLearning qLearning = new QLearning(mazeLayout, learningRate, discountRate, epsilon, rewardGoal, rewardMove);
                qLearning.Train(agentStartX, agentStartY, exitX, exitY, numberOfTrainings);
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Cargar el laberinto desde un fichero de texto
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        static int[,] LoadMazeFromFile(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            int rows = lines.Length;
            int cols = lines[0].Split(',').Length;
            int[,] maze = new int[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                string[] cells = lines[i].Split(',');
                for (int j = 0; j < cells.Length; j++)
                {
                    maze[i, j] = int.Parse(cells[j]);
                }
            }

            return maze;
        }

        /// <summary>
        /// Construye la ruta completa al archivo de laberinto.
        /// </summary>
        /// <param name="folderName">El nombre de la carpeta que contiene el archivo.</param>
        /// <param name="fileName">El nombre del archivo.</param>
        /// <returns>La ruta completa al archivo de laberinto.</returns>
        static string GetMazeFilePath(string folderName, string fileName)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string projectDirectory = Directory.GetParent(currentDirectory).Parent.FullName;
            return Path.Combine(projectDirectory, folderName, fileName);
        }
        /// <summary>
        /// Obtiene el laberinto por defecto
        /// </summary>
        /// <returns></returns>
        static int[,] GetDefaultMaze()
        {
            return new int[,]
            {
                {1, 1, 1, 1, 1, 1, 1, 1},
                {1, 0, 0, 0, 0, 1, 0, 1},
                {1, 1, 0, 0, 0, 1, 0, 1},
                {1, 0, 0, 0, 0, 1, 0, 1},
                {1, 0, 1, 1, 0, 0, 0, 1},
                {1, 0, 0, 1, 1, 0, 0, 1},
                {1, 0, 0, 0, 1, 1, 0, 1},
                {1, 0, 0, 0, 0, 1, 0, 1},
                {1, 1, 1, 1, 1, 1, 1, 1}
            };
        }

        /// <summary>
        ///     Imprime por pantalla la recogida de ciertos valores
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        static double GetParameter(string prompt, double defaultValue)
        {
            Console.Write($"Escoge - {prompt} [Recomendación: {defaultValue}]: ");
            string input = Console.ReadLine();
            if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        ///     Imprimir mapa
        /// </summary>
        static void PrintMap(int[,] mazeLayout)
        {
            for (int i = 0; i < mazeLayout.GetLength(0); i++)
            {
                for (int j = 0; j < mazeLayout.GetLength(1); j++)
                {
                    if (mazeLayout[i, j] == 1)
                    {
                        Console.Write(" 1 ");
                    }
                    else
                    {
                        Console.Write(" 0 ");
                    }
                }
                Console.WriteLine();
            }
        }
        /// <summary>
        ///     Imprimir mapa con entrada
        /// </summary>
        static void PrintMap(int[,] mazeLayout, int startX, int startY)
        {
            for (int i = 0; i < mazeLayout.GetLength(0); i++)
            {
                for (int j = 0; j < mazeLayout.GetLength(1); j++)
                {
                    if (i == startX && j == startY)
                    {
                        Console.Write(" S "); // Entrada
                    }
                    else if (mazeLayout[i, j] == 1)
                    {
                        Console.Write(" 1 ");
                    }
                    else
                    {
                        Console.Write(" 0 ");
                    }
                }
                Console.WriteLine();
            }
        }
        /// <summary>
        ///     Imprimir mapa con entrada y salida
        /// </summary>
        static void PrintMap(int[,] mazeLayout, int startX, int startY, int exitX, int exitY)
        {
            for (int i = 0; i < mazeLayout.GetLength(0); i++)
            {
                for (int j = 0; j < mazeLayout.GetLength(1); j++)
                {
                    if (i == startX && j == startY)
                    {
                        Console.Write("S "); // Entrada
                    }
                    else if (i == exitX && j == exitY)
                    {
                        Console.Write("E "); // Salida
                    }
                    else if (mazeLayout[i, j] == 1)
                    {
                        Console.Write("█ ");
                    }
                    else
                    {
                        Console.Write("- ");
                    }
                }
                Console.WriteLine();
            }
        }
    }
}