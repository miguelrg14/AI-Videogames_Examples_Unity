using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 

public class UIManager : MonoBehaviour
{
    private int temp1;
    private int temp2;

    public GameManager gamaManager; 

    public List<PlayerData> players; 

    public TextMeshProUGUI player1txt;
    public TextMeshProUGUI player2txt;

    public TextMeshProUGUI winner;

    public GameObject restartButton;

    public void CallWinner(string _winner)
    {
        winner.gameObject.SetActive(true);
        winner.text = _winner;
        restartButton.SetActive(true);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ChangePlayer1(int _type)
    {
        PlayerTypes type = (PlayerTypes)_type;

        temp1 = _type;

        ChangeName1(type);
    }

    public void ChangePlayer2(int _type)
    {
        PlayerTypes type = (PlayerTypes)_type;

        temp2 = _type;

        ChangeName2(type);
    }

    public void ChangeName1(PlayerTypes type)
    {
        switch(type)
        {
            case PlayerTypes.Human:
            player1txt.text = "HUMAN";
            break; 

            case PlayerTypes.Minimax:
            player1txt.text = "MINIMAX";
            break; 

            case PlayerTypes.Negamax:
            player1txt.text = "NEGAMAX";
            break; 

            case PlayerTypes.Aspirational:
            player1txt.text = "ASPIRATIONAL";
            break; 
        }
    }

    public void ChangeName2(PlayerTypes type)
    {
        switch(type)
        {
            case PlayerTypes.Human:
            player2txt.text = "HUMAN";
            break; 

            case PlayerTypes.Minimax:
            player2txt.text = "MINIMAX";
            break; 

            case PlayerTypes.Negamax:
            player2txt.text = "NEGAMAX";
            break; 

            case PlayerTypes.Aspirational:
            player2txt.text = "ASPIRATIONAL";
            break; 
        }
    }

    public void StartGame()
    {
        InstantiatePlayer1();
        InstantiatePlayer2();
        gamaManager.StartGame();
    }

    public void InstantiatePlayer1()
    {
        GameObject player1 = null;

        switch((PlayerTypes)temp1)
        {
            case PlayerTypes.Human:
             player1 = Instantiate(players[0].player);
            gamaManager.player1 = player1.GetComponent<Player>();
            break; 

            case PlayerTypes.Minimax:
             player1 = Instantiate(players[1].player);
            gamaManager.player1 = player1.GetComponent<Player>();
            break; 

            case PlayerTypes.Negamax:
             player1 = Instantiate(players[2].player);
            gamaManager.player1 = player1.GetComponent<Player>();
            break; 

            case PlayerTypes.Aspirational:
             player1 = Instantiate(players[3].player);
            gamaManager.player1 = player1.GetComponent<Player>();
            break; 
        }
    }

    public void InstantiatePlayer2()
    {
        GameObject player2 = null;

        switch((PlayerTypes)temp2)
        {
            case PlayerTypes.Human:
            player2 = Instantiate(players[0].player);
            gamaManager.player2 = player2.GetComponent<Player>();
            break; 

            case PlayerTypes.Minimax:
            player2 = Instantiate(players[1].player);
            gamaManager.player2 = player2.GetComponent<Player>();
            break; 

            case PlayerTypes.Negamax:
            player2 = Instantiate(players[2].player);
            gamaManager.player2 = player2.GetComponent<Player>();
            break; 

            case PlayerTypes.Aspirational:
            player2 = Instantiate(players[3].player);
            gamaManager.player2 = player2.GetComponent<Player>();
            break; 
        }
    }

}

[System.Serializable]
public class PlayerData
{
    public GameObject player;
    public PlayerTypes type;
}

public enum PlayerTypes
{
    Human = 0,
    Minimax,
    Negamax,
    Aspirational
}
