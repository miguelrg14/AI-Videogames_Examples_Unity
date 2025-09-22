using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

// @copyright � 2023+ IA Minimax
// @author Author: Miguel Rodriguez gallego
// miguelrodriguezgallego1@gmail.com
// @date 21/12/2023
// @controls documentation generation
public class DocumentationManager : MonoBehaviour
{
    // Ruta del archivo de texto
    private string filePath;

    public UIManager uiManager;

    public string[,] documentationData;

    Stopwatch timer;

    public class Player1Documentation
    {
        public static string algorithm;
        public static int depth;
        public static List<float> timesList;
        public static float averageTime;
        public static float effectiveness;
    }
    public class Player2Documentation
    {
        public static string algorithm;
        public static int depth;
        public static List<float> timesList;
        public static float averageTime;
        public static float effectiveness;
    }

    Player1Documentation player1Documentation;
    Player2Documentation player2Documentation;

    void InitializePlayer1Documentation()
    {
        Player1Documentation.timesList = new List<float>();
    }
    void InitializePlayer2Documentation()
    {
        Player2Documentation.timesList = new List<float>();
    }

    void Start()
    {
        documentationData = new string[5, 4];

        InitializePlayer1Documentation();
        InitializePlayer2Documentation();

        FillFirstArrayRow("Algorithm" ,"Expanded Nodes", "Average Time", "Effectiveness");
        PrintArray();
    }

    public void DocumentationInfoUpdate()   // Función principal a llamar
    {
        CaculateEffectiveness();
        FillDocumentationDataNewRow_Player1Documentation();
        FillDocumentationDataNewRow_Player2Documentation();
        PrintArray();

        Player1Documentation.timesList.Clear();
        Player2Documentation.timesList.Clear();
    }
    void PrintArray()
    {
        // Muestra el contenido del array en la consola en un solo mensaje
        string arrayContent = "Documentation results:\n";

        for (int i = 0; i < documentationData.GetLength(0); i++)
        {
            for (int j = 0; j < documentationData.GetLength(1); j++)
            {
                //arrayContent += $"{documentationData[i, j]}\t";
                arrayContent += $"{documentationData[i, j]}\t";
            }
            arrayContent += "\n";
        }

        Debug.Log(arrayContent);
        //// Muestra el contenido del array en la consola en un solo mensaje
        //string arrayContent = "Documentation results:\n";

        //for (int i = 0; i < documentationData.GetLength(0); i++)
        //{
        //    for (int j = 0; j < documentationData.GetLength(1); j++)
        //    {
        //        //arrayContent += $"{documentationData[i, j]}\t";
        //        arrayContent += $"[{i},{j}] = {documentationData[i, j]}\t";
        //    }
        //    arrayContent += "\n";
        //}

        //Debug.Log(arrayContent);
    }
    public void CaculateEffectiveness()
    {
        Player1Documentation.effectiveness = Player1Documentation.depth / CalculateAverage(Player1Documentation.timesList);
        Player2Documentation.effectiveness = Player2Documentation.depth / CalculateAverage(Player2Documentation.timesList);
    }



    void FillFirstArrayRow(string column0Info, string column1Info, string column2Info, string column3Info)
    {
        // Obtiene la primera dimensión del array (número de filas)
        int numRows = documentationData.GetLength(0);

        // Encuentra la primera fila vacía (donde la primera columna está vacía)
        int newRow = 0;
        while (newRow < numRows && !string.IsNullOrEmpty(documentationData[newRow, 0]))
        {
            newRow++;
        }

        // Verifica si encontró una fila vacía
        if (newRow < numRows)
        {
            // Llena la nueva fila con los valores proporcionados
            documentationData[newRow, 0] = column0Info;
            documentationData[newRow, 1] = column1Info;
            documentationData[newRow, 2] = column2Info;
            documentationData[newRow, 3] = column3Info;
        }
        else
        {
            Debug.LogWarning("El array está lleno, no se pudo agregar una nueva fila.");
        }
    }
    public void FillDocumentationDataNewRow_Player1Documentation()
    {
        // Obtengo los valores para las columnas
        string column0Info = uiManager.player1txt.text.ToString();
        string column1Info = Player1Documentation.depth.ToString();
        string column2Info = CalculateAverage(Player1Documentation.timesList).ToString();
        string column3Info = Player1Documentation.effectiveness.ToString();

        // Obtengo las dimensiones actuales del array
        int numRows = documentationData.GetLength(0);
        int numCols = documentationData.GetLength(1);

        // Busco la primera fila vacía (donde la primera columna está vacía)
        int emptyRow = 0;
        while (emptyRow < numRows && !string.IsNullOrEmpty(documentationData[emptyRow, 0]))
        {
            emptyRow++;
        }

        // Si no se encuentra una fila vacía, agrego una nueva fila
        if (emptyRow == numRows)
        {
            // Creo un nuevo array con una fila adicional
            string[,] newArray = new string[numRows + 1, numCols];

            // Copio los elementos existentes al nuevo array
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    newArray[i, j] = documentationData[i, j];
                }
            }

            // Actualizo el número de filas
            numRows++;

            // Asigno el nuevo array al antiguo
            documentationData = newArray;
        }

        // Llena la fila vacía con los valores proporcionados
        documentationData[emptyRow, 0] = column0Info;
        documentationData[emptyRow, 1] = column1Info;
        documentationData[emptyRow, 2] = column2Info;
        documentationData[emptyRow, 3] = column3Info;
    }

    public void FillDocumentationDataNewRow_Player2Documentation()
    {
        // Obtengo los valores para las columnas
        string column0Info = uiManager.player2txt.text.ToString();
        string column1Info = Player2Documentation.depth.ToString();
        string column2Info = CalculateAverage(Player2Documentation.timesList).ToString();
        string column3Info = Player2Documentation.effectiveness.ToString();

        // Obtengo las dimensiones actuales del array
        int numRows = documentationData.GetLength(0);
        int numCols = documentationData.GetLength(1);

        // Busco la primera fila vacía (donde la primera columna está vacía)
        int emptyRow = 0;
        while (emptyRow < numRows && !string.IsNullOrEmpty(documentationData[emptyRow, 0]))
        {
            emptyRow++;
        }

        // Si no se encuentra una fila vacía, agrego una nueva fila
        if (emptyRow == numRows)
        {
            // Creo un nuevo array con una fila adicional
            string[,] newArray = new string[numRows + 1, numCols];

            // Copio los elementos existentes al nuevo array
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    newArray[i, j] = documentationData[i, j];
                }
            }

            // Actualizo el número de filas
            numRows++;

            // Asigno el nuevo array al antiguo
            documentationData = newArray;
        }

        // Llena la fila vacía con los valores proporcionados
        documentationData[emptyRow, 0] = column0Info;
        documentationData[emptyRow, 1] = column1Info;
        documentationData[emptyRow, 2] = column2Info;
        documentationData[emptyRow, 3] = column3Info;
    }


    float CalculateAverage(List<float> array)
    {
        if (array.Count == 0)
        {
            return 0; // Evitar división por cero
        }

        int sum = 0;

        foreach (int value in array)
        {
            sum += value;
        }

        float average = (float)sum / array.Count;          

        return average;
    }










    static string[,] FillStringArray(int rows, int cols)
    {
        // Initialize a 2D array
        string[,] stringArray = new string[rows, cols];

        // Prompt the user to input values for each element
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Console.Write($"Enter value for element at position ({i + 1}, {j + 1}): ");
                stringArray[i, j] = Console.ReadLine();
            }
        }

        return stringArray;
    }

    void FillDocumentationData(string[,] array)
    {
        int rownum = array.GetLength(0), colnum = array.GetLength(1);
        string arrayOutput = "";

        for (int j = 0; j < colnum; j++)
        {
            for (int i = 0; i < rownum; i++)
            {
                arrayOutput += array[i, j].ToString() + "\t"; // Use "\t" for tab spacing
            }

            arrayOutput += "\n"; // Newline after each row
        }
    }
    void Debug_2DimensionArray(string[,] array)
    {
        int rownum = array.GetLength(0), colnum = array.GetLength(1);
        string arrayOutput = "";

        for (int j = 0; j < colnum; j++)
        {
            for (int i = 0; i < rownum; i++)
            {
                arrayOutput += array[i, j].ToString() + "\t"; // Use "\t" for tab spacing
            }

            arrayOutput += "\n"; // Newline after each row
        }

        Debug.Log("Array values:\n" + arrayOutput);
    }

}
