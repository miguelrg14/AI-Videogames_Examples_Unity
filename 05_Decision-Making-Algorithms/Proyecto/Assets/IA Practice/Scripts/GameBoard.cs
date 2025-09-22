using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

// @copyright © 2023+ IA Minimax
// @author Author: Martin Pérez Villabrille
// martinx902@gmail.com
// @date 16/11/2023
// @brief Creates and controls the board of the game

public class GameBoard : MonoBehaviour
{
    private enum Piece
    {
        Empty = 0,
        Blue = 1,
        Red = 2
    }

    public int columns = 7;
    public int rows = 6;

    public int[,] board;

    // Board for evaluation function
    public int[,] boardSpaces;

    [SerializeField]
    private bool debug = false;

    [SerializeField]
    private float dropTime = 5f;

    [SerializeField]
    private int numPiecesToWin = 4;

    // Game Objects
    public GameObject pieceRed;

    public GameObject pieceBlue;
    public GameObject pieceField;

    private bool isLoading = true;
    private bool gameOver = false;

    private GameObject gameObjectField;

    private bool isDropping = false;

    public GameManager gameManager;

    public GameBoard()
    { }

    public GameBoard(GameBoard other)
    {
        this.board = other.board;
    }

    private void Awake()
    {
        board = new int[columns, rows];
        boardSpaces = new int[columns, rows];

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        int max = Mathf.Max(rows, columns);

        if (numPiecesToWin > max)
            numPiecesToWin = max;

        CreateField();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.D)) DebugBoardEvaluation();
    }

    private void CreateField()
    {
        isLoading = true;

        //Delete the last field if existing

        gameObjectField = GameObject.Find("Field");
        if (gameObjectField != null)
        {
            DestroyImmediate(gameObjectField);
        }

        //Create a new one
        gameObjectField = new GameObject("Field");
        gameObjectField.tag = "GameBoard";

        // create an empty board and instantiate the cells
        board = new int[columns, rows];
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                board[x, y] = (int)Piece.Empty;
                GameObject g = Instantiate(pieceField, new Vector3(x, y * -1, -1), Quaternion.identity) as GameObject;
                g.tag = "GameBoard";
                g.transform.parent = gameObjectField.transform;
            }
        }

        isLoading = false;
        gameOver = false;

        // center camera
        Camera.main.transform.position = new Vector3(
            (columns - 1) / 2.0f, -((rows - 1) / 2.0f), Camera.main.transform.position.z);
    }

    // Gets all the possibles moves.
    public List<int> GetPossibleMoves()
    {
        List<int> possibleMoves = new List<int>();
        for (int x = 0; x < columns; x++)
        {
            for (int y = rows - 1; y >= 0; y--)
            {
                if (board[x, y] == (int)Piece.Empty)
                {
                    possibleMoves.Add(x);
                    break;
                }
            }
        }
        return possibleMoves;
    }

    public void MakeMove(int column, int playerID, bool tempMove = false)
    {
        //Debug.Log("Player with ID: " + playerID + " has placed a piece in column: " + column);

        if (!isDropping)
            StartCoroutine(dropPiece(column, playerID));
    }

    #region TemporalBoard

    public GameBoard CreateTempBoardWithMove(int column, int playerID)
    {
        GameBoard tempBoard = new GameBoard();

        tempBoard.board = new int[columns, rows];

        tempBoard.gameManager = gameManager;

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                tempBoard.board[i, j] = board[i, j];
            }
        }

        MakeTemporalMove(tempBoard, column, playerID);

        return tempBoard;
    }

    private void MakeTemporalMove(GameBoard tempBoard, int column, int playerID)
    {
        for (int i = rows - 1; i >= 0; i--)
        {
            if (tempBoard.board[column, i] == 0)
            {
                tempBoard.board[column, i] = playerID == 1 ? (int)Piece.Blue : (int)Piece.Red;
                break;
            }
        }
    }

    #endregion TemporalBoard

    private IEnumerator dropPiece(int dropColumn, int playerID)
    {
        isDropping = true;

        //Find the place for the new piece in the board

        Vector3 startPosition = new Vector3(dropColumn, gameObjectField.transform.position.y + 1, 0);
        Vector3 endPosition = new Vector3();

        bool foundFreeCell = false;
        for (int i = rows - 1; i >= 0; i--)
        {
            if (board[dropColumn, i] == 0)
            {
                foundFreeCell = true;
                board[dropColumn, i] = playerID == 1 ? (int)Piece.Blue : (int)Piece.Red;
                endPosition = new Vector3(dropColumn, i * -1, startPosition.z);

                break;
            }
        }

        //Drop Piece Animation

        if (foundFreeCell)
        {
            GameObject newPiece = Instantiate(playerID == 1 ? pieceBlue : pieceRed);

            float distance = Vector3.Distance(startPosition, endPosition);

            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * dropTime * ((rows - distance) + 1);

                newPiece.transform.position = Vector3.Lerp(startPosition, endPosition, t);
                yield return null;
            }

            newPiece.transform.parent = gameObjectField.transform;
        }

        isDropping = false;

        yield return 0;
    }

    //public int Evaluate(int playerID)
    //{
    //    int maxHeight = Math.Min(rows, columns);
    //    int total = 0;

    //    for (int row = 0; row < rows; row++)
    //    {
    //        for (int col = 0; col < columns; col++)
    //        {
    //            if (board[col, row] == playerID)
    //            {
    //                int minHeight = Math.Min(maxHeight, row + 1);

    //                for (int height = minHeight - 1; height >= 0; height--)
    //                {
    //                    if (board[col, row - height] == playerID)
    //                    {
    //                        total += 1 << height;
    //                    }
    //                    else
    //                    {
    //                        break;
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    return total;
    //}

    #region CheckWin

    public GameState CheckWin(int playerID)
    {
        GameState state = GameState.Playing;

        if (CheckHorizontal(playerID))
            return state = GameState.Win;

        if (CheckVertical(playerID))
            return state = GameState.Win;

        if (CheckDiagonal(playerID))
            return state = GameState.Win;

        if (!FieldContainsEmptyCell())
            return state = GameState.Draw;

        return state;
    }

    private bool CheckHorizontal(int playerID)
    {
        for (int columns = 0; columns < 7; columns++)
        {
            for (int rows = 0; rows < 3; rows++)
            {
                if (board[columns, rows] == playerID &&
                    board[columns, rows + 1] == playerID &&
                    board[columns, rows + 2] == playerID &&
                    board[columns, rows + 3] == playerID)
                {
                    // Found four consecutive pieces horizontally
                    return true;
                }
            }
        }
        return false;
    }

    private bool CheckVertical(int playerID)
    {
        for (int rows = 0; rows < 6; rows++)
        {
            for (int columns = 0; columns < 4; columns++)
            {
                if (board[columns, rows] == playerID &&
                    board[columns + 1, rows] == playerID &&
                    board[columns + 2, rows] == playerID &&
                    board[columns + 3, rows] == playerID)
                {
                    // Found four consecutive pieces vertically
                    return true;
                }
            }
        }
        return false;
    }

    private bool CheckDiagonal(int playerID)
    {
        // Check diagonally from bottom-left to top-right
        for (int rows = 0; rows < 3; rows++)
        {
            for (int columns = 0; columns < 4; columns++)
            {
                if (board[columns, rows] == playerID &&
                    board[columns + 1, rows + 1] == playerID &&
                    board[columns + 2, rows + 2] == playerID &&
                    board[columns + 3, rows + 3] == playerID)
                {
                    // Found four consecutive pieces diagonally
                    return true;
                }
            }
        }

        // Check diagonally from top-left to bottom-right
        for (int rows = 3; rows < 6; rows++)
        {
            for (int columns = 0; columns < 4; columns++)
            {
                if (board[columns, rows] == playerID &&
                    board[columns + 1, rows - 1] == playerID &&
                    board[columns + 2, rows - 2] == playerID &&
                    board[columns + 3, rows - 3] == playerID)
                {
                    // Found four consecutive pieces diagonally
                    return true;
                }
            }
        }

        return false;
    }

    #endregion CheckWin

    #region Board Comprobations

    public bool IsValidMove(int column)
    {
        // If the top cell is empty, the move is valid
        return board[column, 0] == 0;
    }

    public bool FieldContainsEmptyCell()
    {
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (board[x, y] == (int)Piece.Empty)
                    return true;
            }
        }
        return false;
    }

    public int GetColumnFromHitPoint(Vector3 hitPoint)
    {
        float cellWidth = GetCellWidth();

        float xOffset = hitPoint.x - gameObjectField.transform.position.x + cellWidth * 0.5f;

        int column = (int)(xOffset / cellWidth);

        column = Mathf.Clamp(column, 0, columns);

        return column;
    }

    private float GetCellWidth()
    {
        return pieceField.transform.localScale.x;
    }

    #endregion Board Comprobations

    //#region Board Evaluation

    //public int EvaluateBoard()
    //{
    //    int score = 0;
    //    int piece = 0;

    //    int currentPlayer = gameManager.currentPlayerID;

    //    int rownum = board.GetLength(0), column = board.GetLength(1);

    //    for (int j = 0; j < column; j++)
    //    {
    //        for (int i = 0; i < rownum; i++)
    //        {
    //            piece = board[i, j];

    //            // Score the board based on different directions
    //            score =
    //                (
    //                    ScoreCenterColumn(score, piece) +
    //                    ScoreHorizontal  (score, piece) +
    //                    ScoreVertical    (score, piece) +
    //                    ScoreDiagonals   (score, piece)
    //                );
    //        }
    //    }

    //    Debug.Log("Player [ " + currentPlayer + " ] => Board ev.: " + score);

    //    return score;
    //}

    //int ScoreCenterColumn(int score, int piece)
    //{
    //    int centerColumn = columns / 2;
    //    for (int i = 0; i < rows; i++)
    //    {
    //        if (board[i, centerColumn] == piece)
    //        {
    //            score += 3; // Increase the score for each piece in the center column
    //        }
    //    }

    //    return score;
    //}
    //int ScoreHorizontal(int score, int piece)
    //{
    //    for (int r = 0; r < rows; r++)
    //    {
    //        int[] rowArray = new int[columns];
    //        for (int c = 0; c < columns - 1; c++)
    //        {
    //            rowArray[c] = board[r, c]; // Extract values from each row
    //        }
    //        for (int c = 0; c < columns - 3; c++)
    //        {
    //            int[] window = ExtractWindow(rowArray, c, numPiecesToWin);
    //            score += EvaluateWindow(window, piece);
    //        }
    //    }

    //    return score;
    //}
    //int ScoreVertical(int score, int piece)
    //{
    //    for (int c = 0; c < columns - 1; c++)
    //    {
    //        int[] colArray = new int[rows];
    //        for (int r = 0; r < rows; r++)
    //        {
    //            colArray[r] = board[r, c]; // Extract values from each column
    //        }
    //        for (int r = 0; r < rows - 3; r++)
    //        {
    //            int[] window = ExtractWindow(colArray, r, numPiecesToWin);
    //            score += EvaluateWindow(window, piece);
    //        }
    //    }
    //    return score;
    //}
    //int ScoreDiagonals(int score, int piece)
    //{
    //    // Score positive sloped diagonal
    //    for (int r = 0; r < rows - 4; r++)
    //    {
    //        for (int c = 0; c < columns - 4; c++)
    //        {
    //            int[] window = new int[numPiecesToWin];
    //            for (int i = 0; i < numPiecesToWin; i++)
    //            {
    //                window[i] = board[r + i, c + i];
    //            }
    //            score += EvaluateWindow(window, piece);
    //        }
    //    }

    //    for (int r = 0; r < rows - 4; r++)
    //    {
    //        for (int c = 0; c < columns - 4; c++)
    //        {
    //            int[] window = new int[numPiecesToWin];
    //            for (int i = 0; i < numPiecesToWin; i++)
    //            {
    //                window[i] = board[r + 3 - i, c + i];
    //            }
    //            score += EvaluateWindow(window, piece);
    //        }
    //    }
    //    return score;
    //}

    //// Evaluate the given window for the current player's pieces
    //private int EvaluateWindow(int[] window, int piece)
    //{
    //    int score = 0;
    //    int opponentPiece = (piece == gameManager.player1.ID) ? gameManager.player2.ID : gameManager.player1.ID;

    //    // Depending on turn vantaje
    //    if(gameManager.currentPlayerID == gameManager.player1.ID)
    //    {
    //        score += 1;
    //    }
    //    if (gameManager.currentPlayerID == gameManager.player2.ID)
    //    {
    //        score -= 1;
    //    }

    //    // Depending on positions
    //    int playerCount = CountPieces(window, piece);   // Player pieces in next spaces
    //    int opponentCount = CountPieces(window, opponentPiece); // Opponent pieces in next spaces
    //    int emptyCount = CountPieces(window, 0);    // Empty Spaces

    //    if (playerCount == 4)
    //    {
    //        score += 100;
    //    }
    //    else if (playerCount == 3 && emptyCount == 1)
    //    {
    //        score += 5;
    //    }
    //    else if (playerCount == 2 && emptyCount == 2)
    //    {
    //        score += 2;
    //    }

    //    if (opponentCount == 3 && emptyCount == 1)
    //    {
    //        score -= 4;
    //    }
    //    if (opponentCount == 4)
    //    {
    //        score -= 100;
    //    }

    //    return score;
    //}

    //// -------------------- UTILITES --------------------
    //// Extract a window of pieces from the array
    //private int[] ExtractWindow(int[] array, int start, int length)
    //{
    //    int[] window = new int[length];
    //    for (int i = 0; i < length; i++)
    //    {
    //        window[i] = array[start + i];
    //    }
    //    return window;
    //}
    //// Count the occurrences of a specific value in the array
    //private int CountPieces(int[] array, int value)
    //{
    //    int count = 0;
    //    foreach (int element in array)
    //    {
    //        if (element == value)
    //        {
    //            count++;
    //        }
    //    }
    //    return count;
    //}
    //void DebugBoardEvaluation()
    //{
    //    int scorePlayer1 = 0;
    //    int scorePlayer2 = 0;
    //    int player1 = gameManager.player1.ID;
    //    int player2 = gameManager.player2.ID;

    //    int piecePlayer1 = 0;
    //    int piecePlayer2 = 0;

    //    // Score the board based on different directions
    //    int rownumP1 = board.GetLength(0), columnP1 = board.GetLength(1);

    //    for (int j = 0; j < columnP1; j++)
    //    {
    //        for (int i = 0; i < rownumP1; i++)
    //        {
    //            piecePlayer1 = board[i, j];

    //            // Score the board based on different directions
    //            scorePlayer1 =
    //                (
    //                    ScoreCenterColumn(scorePlayer1, piecePlayer1) +
    //                    ScoreHorizontal(scorePlayer1, piecePlayer1) +
    //                    ScoreVertical(scorePlayer1, piecePlayer1) +
    //                    ScoreDiagonals(scorePlayer1, piecePlayer1)
    //                );
    //        }
    //    }
    //    // Score the board based on different directions
    //    int rownumP2 = board.GetLength(0), columnP2 = board.GetLength(1);

    //    for (int j = 0; j < columnP2; j++)
    //    {
    //        for (int i = 0; i < rownumP2; i++)
    //        {
    //            piecePlayer2 = board[i, j];

    //            // Score the board based on different directions
    //            scorePlayer2 =
    //                (
    //                    ScoreCenterColumn(scorePlayer2, piecePlayer2) +
    //                    ScoreHorizontal(scorePlayer2, piecePlayer2) +
    //                    ScoreVertical(scorePlayer2, piecePlayer2) +
    //                    ScoreDiagonals(scorePlayer2, piecePlayer2)
    //                );
    //        }
    //    }

    //    Debug.Log("Player [ " + player1 + " ] => Board ev.: " + scorePlayer1);
    //    Debug.Log("Player [ " + player2 + " ] => Board ev.: " + scorePlayer2);
    //}
    //// --------------------------------------------------

    //#endregion

    #region Board Evaluation


    public int EvaluateBoard(int ID)
    {
       
        int oponent;
        if(ID==1)
        {
            oponent = 2;
        }
        else
        {
            oponent = 1;
        }

        int myFours = CheckForStreak(board, ID, 4);
        int myThrees = CheckForStreak(board, ID, 3);
        int myTwos = CheckForStreak(board, ID, 2);
        int compFours = CheckForStreak(board, oponent, 4);
        int compThrees = CheckForStreak(board, oponent, 3);
        int compTwos = CheckForStreak(board, oponent, 2);

        if (compThrees >= 1)
        {
            Debug.LogWarning(" Oponent near 3 win! " + oponent);
        }
        if (compFours >= 1)
        {
            Debug.LogWarning(" Oponent near win! " + +oponent);
        }
        //int score = (myFours * 40 + myThrees * 60 + myTwos * 20) - (compFours * 10 + compThrees * 5 + compTwos * 2);
        int score = (myFours * 40 + myThrees * 6 + myTwos * 1) - (compFours * 90 + compThrees * 8 + compTwos * 2);
        return (-score);
    }

    public int CheckForStreak(int[,] state, int Id, int streak)
    {
        int count = 0;
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                if (state[i, j] == Id)
                {
                    count += VerticalStreak(j, i, state, streak);
                    count += HorizontalStreak(j, i, state, streak);
                    count += DiagonalCheck(j, i, state, streak);
                }
            }
        }
        return count;
    }

    public int VerticalStreak(int row, int column, int[,] state, int streak)
    {
        int consecutiveCount = 0;
        for (int i = row; i < rows; i++)
        {
            if (state[column, i] == state[column, row])
            {
                consecutiveCount += 1;
            }
            else
            {
                break;
            }
        }
        return consecutiveCount >= streak ? 1 : 0;
    }

    public int HorizontalStreak(int row, int column, int[,] state, int streak)
    {
        int count = 0;
        for (int j = column; j < columns; j++)
        {
            if (state[j, row] == state[column, row])
            {
                count += 1;
            }
            else
            {
                break;
            }
        }
        return count >= streak ? 1 : 0;
    }

    public int DiagonalCheck(int row, int column, int[,] state, int streak)
    {
        int total = 0;
        int count = 0;
        int j = column;

        for (int i = row; i < rows; i++)
        {
            if (j >= columns)
            {
                break;
            }
            else if (state[j,i] == state[column, row])
            {
                count += 1;
            }
            else
            {
                break;
            }
            j += 1;
        }

        if (count >= streak)
        {
            total += 1;
        }

        count = 0;
        j = column;

        for (int i = row; i >= 0; i--)
        {
            if (j >= columns)
            {
                break;
            }


            else if (state[j, i] == state[column, row])
            {
                count += 1;
            }
            else
            {
                break;
            }
            j += 1;
        }

        if (count >= streak)
        {
            total += 1;
        }

        return total;
    }

    #endregion Board Evaluation

    #region Documentation Register => Google Sheets

    private string sheetId = "YOUR_GOOGLE_SHEET_ID";
    private string apiKey = "YOUR_GOOGLE_API_KEY";

    private IEnumerator UpdateGoogleSheet()
    {
        // Replace with your Google Sheet URL
        string sheetUrl = $"https://docs.google.com/spreadsheets/d/1KSqnRYv54v-2739mBQqqobDk_xkWYXbTE3E4BJ5Fmmk/edit?usp=sharing{sheetId}/values/Sheet1?key={apiKey}";

        // Construct the JSON payload
        string jsonData = "{ \"values\": [[\"Player Score\", \"AI Score\"], [100, 80]] }";

        // Create and send a POST request to update values
        UnityWebRequest www = UnityWebRequest.Put(sheetUrl, jsonData);
        www.method = UnityWebRequest.kHttpVerbPOST;
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error updating Google Sheet: {www.error}");
        }
        else
        {
            Debug.Log("Google Sheet updated successfully");
        }
    }

    #endregion Documentation Register => Google Sheets

    #region Utilities

    // Debugs complete 2 dimension array visually by rows and columns
    private void Debug_2DimensionArray(int[,] array)
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

    private void Debug_1LineArray(int[] array)
    {
        // Log the entire array in one line
        Debug.Log("Array elements: " + string.Join(", ", array));
    }

    #endregion Utilities
}