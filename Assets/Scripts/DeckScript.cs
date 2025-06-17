using UnityEngine;

public class DeckScript : MonoBehaviour
{
    public Sprite[] cardSprites;
    int[] cardValues = new int[53];
    int currentIndex = 0;

    void Start()
    {
        GetCardValues();
    }

    /// <summary>
    /// This method is used to Assign the value to the cards.
    /// </summary>
    void GetCardValues()
    {
        int num = 0;

        //loop to assign values to the cards
        for (int i = 0; i < cardSprites.Length; i++)
        {
            num = i;

            //if there is a remainder after x/13, then remainder
            num %= 13;

            //if num is equal to or over 10 it is a face card and value should be 10
            if (num > 10 || num == 0)
            {
                num = 10;
            }
            cardValues[i] = num++;
        }
    }

    /// <summary>
    /// This method is used to shuffle the deck.
    /// </summary>
    public void Shuffle()
    {
        //standard array data swapping technique
        for (int i = cardSprites.Length - 1; i > 0; --i)
        {
            int j = Mathf.FloorToInt(Random.Range(0.0f, 1.0f) * (cardSprites.Length - 1)) + 1;
            Sprite face = cardSprites[i];
            cardSprites[i] = cardSprites[j];
            cardSprites[j] = face;

            int value = cardValues[i];
            cardValues[i] = cardValues[j];
            cardValues[j] = value;
        }
        currentIndex = 1;
    }

    public int DealCard(CardScript cardScript)
    {
        cardScript.SetSprite(cardSprites[currentIndex]);
        cardScript.SetValue(cardValues[currentIndex]);
        currentIndex++;
        return cardScript.GetValueOfCard();
    }

    public Sprite GetCardBack()
    {
        return cardSprites[0];
    }
}
