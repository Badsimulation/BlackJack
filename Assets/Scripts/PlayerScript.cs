using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // --- This script is for BOTH player and dealer

    //get other scripts
    public CardScript cardScript;
    public DeckScript deckScript;

    //Total value of player/dealer hand
    public int handValue = 0;

    //Betting money
    private int money = 1000;

    //Array of card objects on table
    public GameObject[] hand;

    //index of next card to be turned over
    public int cardIndex = 0;

    //tracking aces for 1 to 11 conversions
    List<CardScript> aceList = new List<CardScript>();

    public void StartHand()
    {
        GetCard();
        GetCard();
    }

    //add a hand to the player/dealer hand
    public int GetCard()
    {
        //Get a card, use deal card to assign sprite and value to card on table
        int cardValue = deckScript.DealCard(hand[cardIndex].GetComponent<CardScript>());

        //show card on game screen
        hand[cardIndex].GetComponent<Renderer>().enabled = true;

        //add card value to running total of the hand
        handValue += cardValue;

        //if value is 1, it is an ace
        if (cardValue == 1)
        {
            aceList.Add(hand[cardIndex].GetComponent<CardScript>());
        }
        //check if we should use an 11 instead of a 1
        AceCheck();
        cardIndex++;
        return handValue;
    }

    //if value would be over 21 drops value of Ace from 11 to 1
    public void AceCheck()
    {
        foreach (CardScript ace in aceList) 
        {
            if (handValue + 10 < 22 && ace.GetValueOfCard() == 1)
            {
                //if converting, adjust card object value and hand
                ace.SetValue(11);
                handValue += 10;
            }
            else if (handValue > 21 && ace.GetValueOfCard() == 11)
            {
                //if converting, adjust gameobject value and hand value
                ace.SetValue(1);
                handValue -= 10;
            }
        }
    }

    //add or subtract from money, for bets
    public void AdjustMoney(int amount)
    {
        money += amount;
    }

    //output players current money amount
    public int GetMoney()
    {
        return money;
    }

    public int GetFirstCardValue()
    {
        return GameObject.Find("D_Card1").GetComponent<CardScript>().GetValueOfCard();
    }

    //hides all cards, resets the needed variables
    public void ResetHand()
    {
        for (int i = 0; i < hand.Length; i++)
        {
            hand[i].GetComponent<CardScript>().ResetCard();
            hand[i].GetComponent<Renderer>().enabled = false;
        }
        cardIndex = 0;
        handValue = 0;
        aceList = new List<CardScript>();
    }
}
