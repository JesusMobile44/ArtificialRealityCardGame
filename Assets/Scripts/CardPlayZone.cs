using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardPlayZone : MonoBehaviourSingleton<CardPlayZone>
{
    private int firstPlayer;
    public int currentPlayer;

    public List<Card> currentStack;

    private CardDealer cardDealer;

    private List<Hand> endOrder;

    void Start()
    {
        firstPlayer = 0;

        cardDealer = GetComponent<CardDealer>();

        foreach(Hand hand in cardDealer.players)
        {
            hand.OnPlayedEvent += NextPlayer;
            hand.OnFinishedHandEvent += FinishHand;
        }

        //EndRound();

        StartRound();
    }

    public void StartRound()
    {
        //cardDealer.players = endOrder;



        currentPlayer = firstPlayer;

        cardDealer.players[firstPlayer].canPlay = true;

        foreach(Hand hand in cardDealer.players)
        {
            hand.playedThisTurn = false; 
        }
    }

    private void NextPlayer(int playerIndex, bool playedACard)
    {
        cardDealer.players[playerIndex].canPlay = false;

        int nextPlayerIndex = (playerIndex + 1) % 4;

        if(nextPlayerIndex == firstPlayer) {

            int lastPlayer = firstPlayer;

            for(int i = 0; i<cardDealer.players.Count; i++)
            {
                lastPlayer = (firstPlayer + i) % 4;
                if (cardDealer.players[lastPlayer].playedThisTurn)
                {
                    firstPlayer = lastPlayer;
                }
            }
            StartRound();
        }

        cardDealer.players[nextPlayerIndex].canPlay = true;

        //make the bots play something or skip
        if (!cardDealer.players[nextPlayerIndex].isPlayer)
        {
            cardDealer.players[nextPlayerIndex].BotPlayCard();
        }

        //currentPlayerDone = true;
    }

    public void ResetStack()
    {
        firstPlayer = currentPlayer;

        foreach(Card card in currentStack)
        {
            Destroy(card.gameObject);
        }

        currentStack.Clear();
    }

    public void AddCard(List<Card> cardsToAdd, int player)
    {
        ResetStack();

        foreach(Card card in cardsToAdd)
        {
            currentStack.Add(Instantiate(card.gameObject).GetComponent<Card>());
        }

        foreach(Card card in currentStack)
        {
            card.GetComponent<FaceCamera>().enabled = false;
            card.GetComponent<Card>().enabled = false;

            card.gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            card.gameObject.transform.rotation = Quaternion.identity;
        }

        //currentStack = cardsToAdd; 
        currentPlayer = player;

        if((player % 4) == ((firstPlayer-1)%4))
        {
            ResetStack();
        }

        ShowCurrentStack();
    }

    public void FinishHand(int player)
    {
        endOrder.Add(cardDealer.players[player]);
    }

    public void EndRound()
    {
        cardDealer.players = endOrder;

        ResetStack();
    }

    public void ShowCurrentStack()
    {
        Debug.Log("how many cards in the stack : " + currentStack.Count);

        Vector3 cardPosition = this.transform.position - new Vector3(0f, 1.2f, 0f);

        float middle = currentStack.Count / 2;
        float cardGap = 0.01f;


        for (int i=0; i < currentStack.Count; i++)
        {
            currentStack[i].gameObject.transform.position = cardPosition;

            currentStack[i].gameObject.transform.position += new Vector3(((i - middle) * cardGap * 10), ((i - middle) * cardGap * 10 / 10), 0f);

            Debug.Log("show current stack : " + currentStack[i].gameObject.name);
        }
    }
}
