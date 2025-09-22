using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @copyright © 2023+ IA Minimax
// @author Author: Jaime Díaz Viéitez / Martin Pérez Villabrille
// pamecin@gmail.com / martinx902@gmail.com
// @date 18/11/2023
// @brief Holds the information for the moves during the calculations
public class ScoringMove
{
    public int score;
    public int column;

    public ScoringMove(int score, int column)
    {
        this.score = score;
        this.column = column;
    }
}