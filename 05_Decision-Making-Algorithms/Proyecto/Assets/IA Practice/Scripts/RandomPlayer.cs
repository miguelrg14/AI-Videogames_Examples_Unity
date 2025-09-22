using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// @copyright © 2023+ IA Minimax
// @author Author: Jaime Díaz Viéitez
// pamecin@gmail.com
// @date 14/12/2023
// @brief Controls the behaviour of an ai using random numbers

public class RandomPlayer : AI_Player
{
   
    public override bool MakeMove(GameBoard board)
    {
        ScoringMove result= RandonMove(board);
        if(result!=null)
        {
            board.MakeMove(result.column, ID);
            return true;
        }
        else
        {
            return false;
        }
    }
    private ScoringMove RandonMove(GameBoard board)
    {
        float timeOut=0; 
        while (true)
        {
            ScoringMove randomMove=new ScoringMove(0,0);
            timeOut += Time.deltaTime;
            randomMove.column = Random.Range(0, 7);
            if (board.IsValidMove(randomMove.column))
            {
                return randomMove;
            }
            if(timeOut> 5)
            {
                Debug.LogError("Invalid move in the random algorithm detected");
                return null;
            }

        }

    }
}
