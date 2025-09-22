using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @copyright © 2023+ IA Minimax 
// @author Author: Martin Pérez Villabrille
// martinx902@gmail.com
// @date 16/11/2023
// @brief Controls the flow of the game, who's turn it is and resests

public enum GameState
{
    Playing = 0,
    Win,
    Draw,
    Choosing
}

public class GameManager : MonoBehaviour
{
    public GameBoard gameBoard;
    public DocumentationManager documentationManager;
    
    public Player player1;
    public Player player2;

    public static int currentPlayerID = 1;

    public float waitTime = 0.1f;

    public UIManager uIManager;

    private GameState gameState = GameState.Playing;

    public void StartGame()
    {
        player1.ID = 1;
        player2.ID = 2;

        currentPlayerID = player1.ID;

        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        while(gameState == GameState.Playing)
        {
            if (currentPlayerID == 1)
            {
                if (player1.MakeMove(gameBoard))
                {
                    gameState = gameBoard.CheckWin(currentPlayerID);
                    CheckEnd(currentPlayerID, gameState);
                    SwitchPlayer();
                    yield return new WaitForSeconds(waitTime);
                }
            }
            else if (currentPlayerID == 2)
            {
                if (player2.MakeMove(gameBoard))
                {
                    gameState = gameBoard.CheckWin(currentPlayerID);
                    CheckEnd(currentPlayerID, gameState);
                    SwitchPlayer();
                    yield return new WaitForSeconds(waitTime);
                }
            }

            yield return null;
        }

    }

    void CheckEnd(int playerID, GameState state)
    {
        if(state == GameState.Playing)
            return;
        
        switch (state)
        {
            case GameState.Win:
                uIManager.CallWinner($"Player{playerID} wins!!");
                documentationManager.DocumentationInfoUpdate();
                break;
            
            case GameState.Draw:
                uIManager.CallWinner("It's a draw");
                documentationManager.DocumentationInfoUpdate();
                break;
            
            default:
                Debug.Log("Error with the End statement in the GameManager");
                break;
        }
    }
    
    void SwitchPlayer()
    {
        currentPlayerID = 3 - currentPlayerID;
    }
}
