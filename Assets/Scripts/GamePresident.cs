using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePresident : MonoBehaviour
{
    private List<Hand> players;

    public List<Card> Stack;

    private int playerIndex;

    private Hand currentPlayer;

    GamePhase phase = 0;

    private void Start()
    {
        players = GetComponent<CardDealer>().players;
        playerIndex = 0;
        currentPlayer = players[playerIndex];
    }
    private void Update()
    {
        if(phase == 0)
        {

        }
    }

    enum GamePhase
    {
        Deal,
        Play,
        Win
    }


}
