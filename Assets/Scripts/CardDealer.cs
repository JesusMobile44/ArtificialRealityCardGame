using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDealer : MonoBehaviour
{
    public GameObject deckParent;
    public List<GameObject> deck; //list of all cards in deck
    public List<Hand> players;

    void Start()
    {
        PopulateDeck();
        Shuffle();
        Deal();
    }

    private void PopulateDeck()
    {
        int children = deckParent.transform.childCount-1;
        for (int i = 1; i < children; i++)
        {
            deck.Add(deckParent.transform.GetChild(i).gameObject);
        }
    }
    private void Shuffle()
    {
        System.Random random = new System.Random();

        for (int i = 0; i < deck.Count; i++)
        {
            int j = random.Next(i, deck.Count);
            GameObject temporary = deck[i];
            deck[i] = deck[j];
            deck[j] = temporary;
        }
    }

    private void Deal()
    {
        AssignOrder();

        int currentPlayer = 0;
        for (int i = 0; i < deck.Count; i++)
        {
            currentPlayer = currentPlayer % (players.Count);
            players[currentPlayer].prefabHand.Add(deck[i]);

            currentPlayer++;
        }

        for(int i = 0; i < players.Count; i++)
        {
            players[i].OrderHand(players[i].prefabHand);
        }
    }

    private void AssignOrder()
    {
        for (int i = 0;i<players.Count;i++)
        {
            players[i].playerIndex = i;
        }
    }
}
