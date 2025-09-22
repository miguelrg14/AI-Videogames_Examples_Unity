using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @copyright © 2023+ IA Minimax 
// @author Author: Martin Pérez Villabrille
// martinx902@gmail.com
// @date 16/11/2023
// @brief Abstract base class for players 

public abstract class Player : MonoBehaviour
{
    [HideInInspector]
    public int ID;

    public abstract bool MakeMove(GameBoard board);
}
