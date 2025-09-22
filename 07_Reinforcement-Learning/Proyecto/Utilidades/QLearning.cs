
// Copyright (�) 2024+ - Miguel Rodríguez Gallego
// This code is under public domain
// miguelrodriguezgallego1@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReinforcementLearning
{
    public class QLearning
    {
        private readonly int[,] maze;
        private readonly double alpha;
        private readonly double gamma;
        private readonly double epsilon;
        private readonly double rewardGoal;
        private readonly double rewardMove;
        private readonly double[,,] Q;
        private readonly Random rand = new Random();

        public QLearning(int[,] maze, double alpha, double gamma, double epsilon, double rewardGoal, double rewardMove)
        {
            this.maze = maze;
            this.alpha = alpha;
            this.gamma = gamma;
            this.epsilon = epsilon;
            this.rewardGoal = rewardGoal;
            this.rewardMove = rewardMove;
            int height = maze.GetLength(0);
            int width  = maze.GetLength(1);
            Q = new double[height, width, 4];
        }

        // Temporizador
        Stopwatch stopwatch = new Stopwatch();
        TimeSpan elapsedTime;
        // Contador de acciones realizadas en el proceso de búsqueda
        int actionsTaken = 0;
        int[] actionsTakenTotal;

        /// <summary>
        ///     Entrenar al algoritmo
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="exitX"></param>
        /// <param name="exitY"></param>
        /// <param name="numEpisodes"></param>
        public void Train(int startX, int startY, int exitX, int exitY, int numEpisodes)
        {
            // Crear un temporizador
            stopwatch.Start();
            Array.Resize(ref actionsTakenTotal, numEpisodes);

            for (int episode = 0; episode < numEpisodes; episode++)
            {
                int[] state = { startX, startY };
                actionsTaken = 0;

                while (!IsTerminalState(state, exitX, exitY))
                {
                    int action      = SelectAction(state);
                    int[] newState  = ExecuteAction(state, action);
                    double reward   = GetReward(newState, exitX, exitY);

                    int bestNextAction = SelectBestAction(newState);
                    Q[state[0], state[1], action] += alpha * (reward + gamma * Q[newState[0], newState[1], bestNextAction] - Q[state[0], state[1], action]);

                    state = newState;
                    actionsTaken++;
                }
                actionsTakenTotal[episode] = actionsTaken;
                //PrintPolicy();    // Imprimir cada episodio
            }
            Console.WriteLine();
            PrintPolicy(startX, startY, exitX, exitY);
            Console.WriteLine();
        }

        /// <summary>
        ///     Revisar si encuentra la salida
        /// </summary>
        /// <param name="state"></param>
        /// <param name="exitX"></param>
        /// <param name="exitY"></param>
        /// <returns></returns>
        private bool IsTerminalState(int[] state, int exitX, int exitY) 
            => state[0] == exitX && state[1] == exitY;

        private int SelectAction(int[] state)
        {
            if (rand.NextDouble() < epsilon)
            {
                int action;
                do
                {
                    action = rand.Next(4);
                } while (!IsValidAction(state, action));
                return action;
            }
            return SelectBestAction(state);
        }

        /// <summary>
        ///     Selecciona la mejor acción entre las posibles
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private int SelectBestAction(int[] state)
        {
            double maxQ = double.MinValue;
            int bestAction = 0;
            for (int action = 0; action < 4; action++)
            {
                if (Q[state[0], state[1], action] > maxQ)
                {
                    maxQ = Q[state[0], state[1], action];
                    bestAction = action;
                }
            }
            return bestAction;
        }

        /// <summary>
        ///     Revisar si la próxima acción a realizar es válida
        /// </summary>
        /// <param name="state"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private bool IsValidAction(int[] state, int action)
        {
            int[] newState = ExecuteAction(state, action);
            return maze[newState[0], newState[1]] == 0;
        }

        /// <summary>
        ///     Ejecutar el siguiente movimiento en el laberinto
        /// </summary>
        /// <param name="state"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private int[] ExecuteAction(int[] state, int action)
        {
            int[] newState = (int[])state.Clone();
            switch (action)
            {
                case 0: if (newState[0] > 0)                        newState[0]--; break; // Arriba
                case 1: if (newState[1] < maze.GetLength(1) - 1)    newState[1]++; break; // Derecha
                case 2: if (newState[0] < maze.GetLength(0) - 1)    newState[0]++; break; // Abajo
                case 3: if (newState[1] > 0)                        newState[1]--; break; // Izquierda
            }
            return maze[newState[0], newState[1]] == 1 ? state : newState;
        }

        /// <summary>
        ///     Devuelve si gana o si mueve ficha de nuevo dependiendo si llegó a la meta o no
        /// </summary>
        /// <param name="state"></param>
        /// <param name="exitX"></param>
        /// <param name="exitY"></param>
        /// <returns></returns>
        private double GetReward(int[] state, int exitX, int exitY) 
            => IsTerminalState(state, exitX, exitY) ? rewardGoal : rewardMove;

        /// <summary>
        ///     Imprimir mapa
        /// </summary>
        private void PrintPolicy(int startX, int startY, int exitX, int exitY)
        {
            string[] actions = { "↑", "→", "↓", "←" };
            for (int i = 0; i < maze.GetLength(0); i++)
            {
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    if (i == startX && j == startY)
                    {
                        Console.Write("S "); // Marca la entrada
                    }
                    else if (i == exitX && j == exitY)
                    {
                        Console.Write("E "); // Marca la salida
                    }
                    // Pared
                    else if (maze[i, j] == 1)
                    {
                        Console.Write("█ ");
                    }
                    // Hueco de acción/movimiento
                    else
                    {
                        int bestAction = SelectBestAction(new int[] { i, j });
                        Console.Write(actions[bestAction] + " ");
                    }
                }
                Console.WriteLine();
            }

            // Detener temporizador
            stopwatch.Stop();
            elapsedTime = stopwatch.Elapsed;

            // Imprimir por pantalla los valores de pruebas del algoritmo
            Console.WriteLine($"Tiempo de ejecución: {elapsedTime.TotalMilliseconds} ms");
            Console.WriteLine($"[{string.Join(";", actionsTakenTotal)}]");
        }
    }
}