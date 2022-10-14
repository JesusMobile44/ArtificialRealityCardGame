using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class Hand : MonoBehaviour
{
    #region Events
    public delegate void PlayedHandler(int index, bool playedACard);
    public event PlayedHandler OnPlayedEvent;

    public delegate void FinishedHandHandler(int index);
    public event FinishedHandHandler OnFinishedHandEvent;
    #endregion

    public List<GameObject> prefabHand;
    public List<GameObject> instanceHand;

    private Transform handPosition;

    private Transform cam;

    private float cardGap = 0.01f;

    private CardPlayZone cardPlayZone;

    private Vector3 rotationV3;

    public int playerIndex;

    public bool canPlay;

    public bool playedThisTurn;

    public bool isPlayer; // do this when you get back

    public void Awake()
    {
        cam = Camera.main.transform;
        cardPlayZone = CardPlayZone.Instance;
        rotationV3 = new Vector3(0, 0, 0);

        handPosition = cam.Find("Hand_Position");
    }
    void Start()
    {
        canPlay = false;
        playedThisTurn = false;
        isPlayer = false;
        checkIfPlayer();
    }

    public void OrderHand(List<GameObject> hand)
    {
        for(int i = 0; i < hand.Count; i++)
        {
            if (hand[i].name.Contains("Joker") || hand[i].name.Equals(""))
            {
                hand[i].name = "14";
            }
            else
            {
                hand[i].name = Regex.Replace(hand[i].name, "[^0-9]", "");
            }
        }
        prefabHand.Sort(SortByName);
        checkIfPlayer();
        UpdateHand();
    }

    private static int SortByName(GameObject o1, GameObject o2)
    {
        return o1.name.CompareTo(o2.name);
    }


    public void checkIfPlayer()
    {
        if (transform.parent.name == "Player")
        {
            isPlayer = true;
        }
    }
    public void UpdateHand()
    {
        if (isPlayer)
        {
            UpdatePlayerHand();
        }
        else
        {
            UpdateBotHand();
        }
    }

    public void UpdatePlayerHand()
    {
        foreach( Transform child in handPosition)
        {
            GameObject.Destroy(child.gameObject);
        }

        instanceHand.Clear();

        for (int i = 0; i < prefabHand.Count; i++)
        {
            instanceHand.Add(Instantiate(prefabHand[i], new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(-90f, 0, 0))));
            instanceHand[i].GetComponent<FaceCamera>().enabled = true;
            instanceHand[i].name = Regex.Replace(instanceHand[i].name, "[^0-9]", "");
        }

        PlaceCardsAroundPoint(handPosition);
    }

    public void UpdateBotHand()
    {
        Transform tf = this.transform;

        foreach (Transform child in tf)
        {
            GameObject.Destroy(child.gameObject);
        }

        instanceHand.Clear();

        //tf.position = new Vector3(0f, -0.5f, 1f);

        for (int i = 0; i < prefabHand.Count; i++)
        {
            instanceHand.Add(Instantiate(prefabHand[i], new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(rotationV3.x,rotationV3.y,rotationV3.z))));
            instanceHand[i].name = Regex.Replace(instanceHand[i].name, "[^0-9]", "");
        }

        float middle = instanceHand.Count / 2;
        float totalTwist = 40f;
        // 20f for example, try various values
        int numberOfCards = instanceHand.Count;
        float twistPerCard = totalTwist / numberOfCards;
        float startTwist = -1f * (totalTwist / 2f);

        float scalingFactor = 0.007f;
        // that should be roughly one-tenth the height of one
        // of your cards, just experiment until it works well


        for (int i = 0; i < instanceHand.Count; i++)
        {
            float twistForThisCard = startTwist + (i * twistPerCard);
            float nudgeThisCard = Mathf.Abs(twistForThisCard);
            nudgeThisCard *= scalingFactor;

            instanceHand[i].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            instanceHand[i].transform.position = tf.position;
            instanceHand[i].transform.Rotate(0f, twistForThisCard, 0f);
            instanceHand[i].transform.position += new Vector3(((i - middle) * cardGap * 10), ((i - middle) * cardGap * 10 / 10), 0f);

            instanceHand[i].transform.Translate(0f, 0f, -nudgeThisCard);

            instanceHand[i].transform.SetParent(this.transform);
        }
        
        if(rotationV3 == new Vector3(0,0,0))
        {
            tf.Rotate(-80, 0, 0);

            float rotation = 90f * playerIndex;

            transform.parent.transform.Rotate(0f, rotation, 0f);

            Debug.Log("Rotation for index : " + playerIndex + " is : " + rotation);

            rotationV3 = new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z);
        }
        else
        {
            tf.Rotate(-80, 0, 0);

            float rotation = 90f * playerIndex;

            tf.Rotate(0f, rotation, 0f);

            //transform.parent.transform.rotation.SetEulerAngles(rotationV3.x, rotationV3.y, rotationV3.z);


            //les cartes reset leur orientations quand tu les instantiates
        }
        
    }

    public void PlaceCardsAroundPoint(Transform middle)
    {
        float numberOfCards = instanceHand.Count;
        float distanceFromMiddle = 0.25f;
        float ratio = numberOfCards / 14;

        //float maxAngle = 80f;
        //float cardAngle = maxAngle / numberOfCards;

        for (int i = 0; i < instanceHand.Count; i++)
        {
            float radians = ((i + 0.5f) * (Mathf.PI) * ratio / numberOfCards) + 
                ( (15 - instanceHand.Count)/2 * (Mathf.PI) * ratio / numberOfCards) ; //gotta change the hard numbers

            //Debug.Log(radians);

            var vertical = Mathf.Sin(radians);
            var horizontal = Mathf.Cos(radians);

            var spawnDir = new Vector3(horizontal, 0, vertical);

            var spawnPos = middle.position + spawnDir * distanceFromMiddle;

            instanceHand[i].transform.position = spawnPos;
            instanceHand[i].transform.localScale = new Vector3(0.007f, 0.007f, 0.007f);
            //instanceHand[i].transform.LookAt(middle);
            instanceHand[i].transform.SetParent(handPosition);

        }
    }

    public void SelectCard(Card card)
    {
        if (!card.isSelected)
        {
            Debug.Log("Select Card");
            card.Select();
            CheckIfSelected(card.gameObject.name);
        }
        else
        {
            card.Deselect();
        }
    }
    private void CheckIfSelected(string cardName)
    {
        for(int i = 0; i < instanceHand.Count; i++)
        {
            if(instanceHand[i].name != cardName)
            {
                instanceHand[i].GetComponent<Card>().Deselect();
            }
        }
    }
    public void PlayCard()
    {
        List<int> playedIndex = new List<int>();
        List<Card> playedCards = new List<Card>();

        for(int i = 0; i < instanceHand.Count; i++)
        {
            if (instanceHand[i].GetComponent<Card>().isSelected)
            {
                instanceHand[i].GetComponent<Card>().PlayCard();

                playedIndex.Add(i);
                playedCards.Add(instanceHand[i].GetComponent<Card>());
                //RemoveCard(i);
            }
        }
        cardPlayZone.AddCard(playedCards, playerIndex);

        RemoveCardList(playedIndex);

        playedThisTurn = true;

        OnPlayedEvent.Invoke(playerIndex, playedThisTurn);
    }

    public void BotPlayCard()
    {
        StartCoroutine(WaitALil(2));

        if (canPlay)
        {
            bool pass = true;


            for(int i = 0; i < instanceHand.Count; i++)
            {
                //int nbOfSame = 1;

                SelectCard(instanceHand[i].GetComponent<Card>());

                Debug.Log(this.transform.parent.name + " selected this card : " + instanceHand[i].name);

                if (CheckIfPlayable())
                {
                    pass = false;

                    Debug.Log(this.transform.parent.name + " will try playing this card : " + instanceHand[i].name);

                    PlayCard();

                    return;
                }

                /*
                for(int j = i + 1; j < instanceHand.Count; j++)
                {
                    SelectCard(instanceHand[i].GetComponent<Card>());
                    
                    if (instanceHand[j].name.Equals(instanceHand[i].name))
                    {
                        SelectCard(instanceHand[j].GetComponent<Card>());

                        nbOfSame++;
                    }
                    if (CheckIfPlayable())
                    {
                        pass = false;

                        PlayCard();
                        return;
                    }
                }*/
            }
            if (pass)
            {
                Debug.Log(this.transform.parent.name + " passed his turn");

                PassTurn();
            }
        }
    }

    public bool CheckIfPlayable()
    {
        List<GameObject> playable = new List<GameObject>();

        if(cardPlayZone.currentStack.Count == 0)
        {
            return true;
        }
        else
        {
            foreach (GameObject gameObject in instanceHand)
            {
                if (gameObject.GetComponent<Card>().isSelected)
                {
                    playable.Add(gameObject);
                }
            }
            //&& int.Parse(playable[0].name) > int.Parse(cardPlayZone.currentStack[0].name)
            if (playable.Count == cardPlayZone.currentStack.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }

    private void RemoveCardList(List<int> indexs)
    {
        for(int i = 0; i<indexs.Count; i++)
        {
            RemoveCard(indexs[0]);
        }
    }
    private void RemoveCard(int index)
    {
        Debug.Log("Removing : " + instanceHand[index].name);

        instanceHand.Remove(instanceHand[index]);
        prefabHand.Remove(prefabHand[index]);

        if(instanceHand.Count == 0)
        {
            finishHand();
        }

        UpdateHand();
    }
    private void finishHand()
    {
        OnFinishedHandEvent.Invoke(playerIndex);
    }

    public void PassTurn()
    {
        playedThisTurn = false;

        OnPlayedEvent.Invoke(playerIndex, playedThisTurn);
    }

    IEnumerator WaitALil(int seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}
