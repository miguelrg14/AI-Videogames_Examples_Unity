using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

// @copyright � 2023+ IA Minimax
// @author Author: Martin P�rez Villabrille
// martinx902@gmail.com
// @date 22/11/2023
// @brief Controls the behaviour of an ai player using the MiniMax(NegaMax) algorithm
public class MiniMax_NegaMax : AI_Player
{
    //public int maxDepth = 3;
    private int depth = 0;

    private bool randomMove = true;

    private Stopwatch timer;
    private long ticksElapsed;

    public override bool MakeMove(GameBoard board)
    {
        ScoringMove result = new ScoringMove(0, 0);

        timer = Stopwatch.StartNew();

        if (board.EvaluateBoard(ID) == 0 && randomMove)
        {
            result.column = Random.Range(0, board.columns);
            randomMove = false;
        }
        else
        {
            result = MiniMax(board, 0);
        }

        if (board.IsValidMove(result.column))
        {
            board.MakeMove(result.column, ID);

            timer.Stop();
            ticksElapsed = timer.ElapsedMilliseconds;
            if (GameManager.currentPlayerID == 1)
                DocumentationManager.Player1Documentation.timesList.Add(timer.ElapsedTicks);
            if (GameManager.currentPlayerID == 2)
                DocumentationManager.Player2Documentation.timesList.Add(timer.ElapsedTicks);

            //print("Minimax / Time to make move: " + ticksElapsed + " ms");
            return true;
        }
        else
        {
            Debug.LogError("Invalid move in the Minimax algorithm detected");
            return false;
        }
    }

    public ScoringMove MiniMax(GameBoard board, int depth)
    {
        int bestMove = 0;
        int bestScore = int.MinValue;

        int currentScore;

        ScoringMove scoringMove;

        if (depth == maxDepth || !board.FieldContainsEmptyCell())
        {
            if (depth % 2 == 0)
            {
                scoringMove = new ScoringMove(board.EvaluateBoard(ID), 0);
            }
            else
            {
                scoringMove = new ScoringMove(-board.EvaluateBoard(ID), 0);
            }
        }
        else
        {
            bestScore = int.MinValue;

            foreach (int move in board.GetPossibleMoves())
            {
                GameBoard newBoard = board.CreateTempBoardWithMove(move, ID);

                scoringMove = MiniMax(newBoard, depth + 1);

                currentScore = -scoringMove.score;

                if (currentScore > bestScore)
                {
                    bestScore = currentScore;
                    bestMove = move;
                }
            }

            scoringMove = new ScoringMove(bestScore, bestMove);
        }

        if (GameManager.currentPlayerID == 1)
            DocumentationManager.Player1Documentation.depth += depth;
        if (GameManager.currentPlayerID == 2)
            DocumentationManager.Player2Documentation.depth += depth;
        return scoringMove;
    }
}