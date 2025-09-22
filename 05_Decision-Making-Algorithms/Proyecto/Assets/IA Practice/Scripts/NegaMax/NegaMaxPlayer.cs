using System;
using System.Diagnostics;
using UnityEngine;

// @copyright © 2023+ IA Minimax
// @author Author: Jaime Díaz Viéitez
// pamecin@gmail.com
// @date 18/11/2023
// @brief Controls the behaviour of an ai playerusing the NegaMax algorithm with prune
public class NegaMaxPlayer : AI_Player
{
    private bool randomMove = true;

    private Stopwatch timer;
    private long ticksElapsed;

    public override bool MakeMove(GameBoard board)
    {
        ScoringMove result = new ScoringMove(0, 0);

        timer = Stopwatch.StartNew();

        if (board.EvaluateBoard(ID) == 0 && randomMove)
        {
            result.column = UnityEngine.Random.Range(0, board.columns);
            randomMove = false;
        }
        else
        {
            result = Negamax(board, 0, -Mathf.Infinity, Mathf.Infinity, ID);
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
            UnityEngine.Debug.LogError("Invalid move in the NegaMax algorithm detected");
            return false;
        }
    }

    /// @brief using the megamax algorith (with prune) determines the best move in this moment
    /// @param board, the cuarrent status of the gameboard
    /// @param depth how deep will the mini max explore it's options
    /// @param alfa the cut value for max (should be minus infinite)
    /// @param beta the cut value for min (should be infinite)
    /// @return int the numeric value of the best column move
    public ScoringMove Negamax(GameBoard board, int depth, float alfa, float beta, int playerid)
    {
        byte bestMove = 0;

        int bestScore = 0;

        int currentScore;

        ScoringMove scoringMove = null;

        int newMaxDepth = maxDepth;

        GameBoard newBoard;

        if (depth == newMaxDepth || !board.FieldContainsEmptyCell() /* CheckEnd(playerid, board.CheckWin(playerid))*/)//it continues no matter what
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

            //byte[] possibleMoves;

            int[] possibleMoves;
            possibleMoves = board.GetPossibleMoves().ToArray();
            foreach (byte move in possibleMoves)

            {
                newBoard = board.CreateTempBoardWithMove(move, playerid);

                scoringMove = Negamax(newBoard, depth + 1, -beta, -Math.Max(alfa, bestScore), playerid);

                currentScore = -scoringMove.score;

                if (currentScore > bestScore)

                {
                    bestScore = currentScore;

                    bestMove = move;
                }

                if (bestScore >= beta)

                {
                    scoringMove = new ScoringMove(bestScore, bestMove);

                    return scoringMove;
                }
            }
            scoringMove = new ScoringMove(bestScore, bestMove);
            //Debug.LogError(scoringMove.score +" Score");
        }

        if (GameManager.currentPlayerID == 1)
            DocumentationManager.Player1Documentation.depth += depth;
        if (GameManager.currentPlayerID == 2)
            DocumentationManager.Player2Documentation.depth += depth;

        return scoringMove;
    }

    private bool CheckEnd(int playerID, GameState state)
    {
        if (state == GameState.Playing)
            return false;

        switch (state)
        {
            case GameState.Win:
                print("Player: " + playerID + " wins!!");
                break;

            case GameState.Draw:
                print("It's a draw!");
                break;

            default:
                print("Error with the End statement in the GameManager");
                break;
        }
        return true;
    }
}