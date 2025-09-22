using System;
using System.Diagnostics;
using UnityEngine;

// @copyright � 2023+ IA Minimax
// @author Author: Jaime D�az Vi�itez
// pamecin@gmail.com
// @date 26/11/2023
// @brief Controls the behaviour of an ai player using the Aspirational algorithm
public class AspiracionalPlayer : AI_Player
{
    //aspirational should be in the same place as negamax
    [SerializeField]
    private NegaMaxPlayer Negamax;

    [SerializeField]
    private int windowRange;

    private bool randomMove = true;
    private float previousScore = 0;

    private Stopwatch timer;
    private long ticksElapsed;

    private void Awake()
    {
        Negamax = GetComponent<NegaMaxPlayer>();
    }

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
            result = AspirationSearch(board, ID, windowRange);
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
            UnityEngine.Debug.LogError("Invalid move in the Aspirational algorithm detected");
            return false;
        }
    }

    private ScoringMove AspirationSearch(GameBoard board, int playerid, int windowRange)
    {
        float alfa, beta;
        ScoringMove move;

        if (previousScore != 0) // Use an appropriate sentinel value
        {
            alfa = previousScore - windowRange;
            beta = previousScore + windowRange;

            while (true)
            {
                print("Loop");
                move = Negamax.Negamax(board, 0, alfa, beta, playerid);
                if (move.score <= alfa) { alfa = Mathf.NegativeInfinity; }
                else if (move.score >= beta) { beta = Mathf.Infinity; }
                else break;
            }

            previousScore = move.score;
        }
        else
        {
            move = Negamax.Negamax(board, 0, Mathf.NegativeInfinity, Mathf.Infinity, playerid);
            previousScore = move.score;
        }

        return move;
    }
}