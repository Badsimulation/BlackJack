using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Button dealBtn;
    public Button hitBtn;
    public Button standBtn;
    public Button dblDownBtn;

    //bet buttons
    public Button betBtn10;
    public Button betBtn50;
    public Button betBtn100;
    public Button betBtn500;

    //game over buttons
    public Button quitButton;
    public Button restartButton;

    //access to the player and dealer hands and the bet button manager
    public PlayerScript playerScript;
    public PlayerScript dealerScript;
    public BetButtonsManager betButtonsManager;

    //boolean to track blackjack and double down
    private bool blackJack = false;
    private bool doubleDown = false;

    //access to text fields
    public TextMeshProUGUI cashText;
    public TextMeshProUGUI betText;
    public TextMeshProUGUI playerHandText;
    public TextMeshProUGUI dealerHandText;
    public TextMeshProUGUI mainText;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI betInfoText;

    //hiding dealers card access
    public GameObject hideCard;

    //access to the game over panel
    public GameObject gameOverCanvas;

    //how much is bet
    [SerializeField]
    int pot = 0;
    [SerializeField]
    int bet = 0;

    void Start()
    {
        // Add on click listeners to the buttons
        standBtn.onClick.AddListener(() => StandClicked());
        hitBtn.onClick.AddListener(() => HitClicked());
        dealBtn.onClick.AddListener(() => DealClicked());
        dblDownBtn.onClick.AddListener(() => DoubleDownClicked());

        //add bet button on click listeners
        betBtn10.onClick.AddListener(() => BetClicked(betBtn10));
        betBtn50.onClick.AddListener(() => BetClicked(betBtn50));
        betBtn100.onClick.AddListener(() => BetClicked(betBtn100));
        betBtn500.onClick.AddListener(() => BetClicked(betBtn500));

        //add game over button listeners
        restartButton.onClick.AddListener(() => RestartClicked());
        quitButton.onClick.AddListener(() => QuitClicked());

        //disable stand, hit, and double down buttons
        standBtn.gameObject.SetActive(false);
        hitBtn.gameObject.SetActive(false);
        dblDownBtn.gameObject.SetActive(false);

    }

    private void StandClicked()
    {
        //hide buttons
        hitBtn.gameObject.SetActive(false);
        dealBtn.gameObject.SetActive(false);
        standBtn.gameObject.SetActive(false);
        dblDownBtn.gameObject.SetActive(false);

        StartCoroutine(HitDealer());

        //play sound effect
        AudioManager.instance.PlaySFX(AudioManager.instance.buttonPress);
    }

    private void HitClicked()
    {
        //check that there is still room on the table
        if (playerScript.cardIndex <= 11)
        {
            playerScript.GetCard();
            AudioManager.instance.PlaySFX(AudioManager.instance.cardPlaced);
        }

        //hide double down button
        dblDownBtn.gameObject.SetActive(false);

        //check if player busted or has blackjack
        if (playerScript.handValue > 20) RoundOver();

        //update player hand text
        playerHandText.SetText(GameStrings.YourHand + playerScript.handValue.ToString());

        //play sound effect
        AudioManager.instance.PlaySFX(AudioManager.instance.buttonPress);

    }

    private void DealClicked()
    {
        if ((bet > 0) && (bet <= playerScript.GetMoney()))
        {
            //Reset round, hide text, prep for new round
            playerScript.ResetHand();
            dealerScript.ResetHand();

            //make sure cash text color is white
            cashText.color = Color.white;

            //hide dealer hand score at the start of deal TODO:change this to only show value of revealed cards
            mainText.gameObject.SetActive(false);
            GameObject.Find("Deck").GetComponent<DeckScript>().Shuffle();
            playerScript.StartHand();
            dealerScript.StartHand();

            //update the score displayed
            playerHandText.SetText(GameStrings.YourHand + playerScript.handValue.ToString());
            dealerHandText.SetText(GameStrings.DealerHand + dealerScript.GetFirstCardValue().ToString());

            //hide dealer 2nd card
            hideCard.GetComponent<Renderer>().enabled = true;

            //adjust button visibility
            dealBtn.gameObject.SetActive(false);
            hitBtn.gameObject.SetActive(true);
            standBtn.gameObject.SetActive(true);
            dblDownBtn.gameObject.SetActive(true);
            betButtonsManager.DisableBetBettons();

            //hide bet info text
            betInfoText.gameObject.SetActive(false);

            //get player selected pot and bet size
            betText.SetText(GameStrings.BetPrefix + bet.ToString());
            playerScript.AdjustMoney(-bet);
            cashText.SetText(GameStrings.CashPrefix + playerScript.GetMoney().ToString());

            //play sound effect
            AudioManager.instance.PlaySFX(AudioManager.instance.cardShuffle);

            if ((playerScript.handValue == 21) || (dealerScript.handValue == 21))
            {
                blackJack = true;
                RoundOver();
            }


        }
        else
        {
            if (bet <= 0)
            {
                mainText.SetText(GameStrings.MustPlaceBet);
                betText.color = Color.red;
            }
            else if (bet > playerScript.GetMoney())
            {
                mainText.SetText(GameStrings.BetTooHigh);
                cashText.color = Color.red;

            }

            //play sound effect
            AudioManager.instance.PlaySFX(AudioManager.instance.buttonPress);
        }
    }

    private IEnumerator HitDealer()
    {
        hideCard.GetComponent<Renderer>().enabled = false;
        AudioManager.instance.PlaySFX(AudioManager.instance.cardPlaced);
        yield return new WaitForSeconds(1f);

        while (dealerScript.handValue < 17 && dealerScript.cardIndex < 11)
        {
            dealerScript.GetCard();
            AudioManager.instance.PlaySFX(AudioManager.instance.cardPlaced);
            dealerHandText.SetText(GameStrings.DealerHand + dealerScript.handValue.ToString());
            yield return new WaitForSeconds(1f); // half-second delay between each card
        }

        RoundOver();
    }

    //check for winner and loser, hand is over
    private void RoundOver()
    {
        bool playerBust = playerScript.handValue > 21;
        bool dealerBust = dealerScript.handValue > 21;
        bool player21 = playerScript.handValue == 21;
        bool dealer21 = dealerScript.handValue == 21;

        //make sure the text is up to date with correct scores
        dealerHandText.SetText(GameStrings.DealerHand + dealerScript.handValue.ToString());
        playerHandText.SetText(GameStrings.YourHand + playerScript.handValue.ToString());

        //both have blackjack, push
        if ((playerScript.handValue == 21) && (dealerScript.handValue == 21) && (blackJack == true))
        {
            mainText.SetText(GameStrings.TieMessage);
        }

        //player has blackjack only
        else if ((playerScript.handValue == 21) && (blackJack == true))
        {
            mainText.SetText(GameStrings.BackjackWin);
            playerScript.AdjustMoney((bet * 5) / 2); //multiplies winnings by 1.5
        }

        //dealer has blackjack only
        else if ((dealerScript.handValue == 21) && (blackJack == true))
        {
            mainText.SetText(GameStrings.DealBlackJack);
        }

        else if (playerBust)
        {
            mainText.SetText(GameStrings.PlayerBust);
        }

        //player busted or dealer won
        else if (!dealerBust && dealerScript.handValue > playerScript.handValue)
        {
            mainText.SetText(GameStrings.DealerWins);
        }

        //dealer busted
        else if (dealerBust)
        {
            mainText.SetText(GameStrings.DealerBust);
            playerScript.AdjustMoney(pot);
        }

        //player won
        else if (playerScript.handValue > dealerScript.handValue)
        {
            mainText.SetText(GameStrings.PlayerWins);
            playerScript.AdjustMoney(pot);
        }

        //player and dealer tied, push
        else if (playerScript.handValue == dealerScript.handValue)
        {
            mainText.SetText(GameStrings.TieMessage);
            playerScript.AdjustMoney(pot / 2);
        }

        //set up for next round
        hitBtn.gameObject.SetActive(false);
        standBtn.gameObject.SetActive(false);
        dblDownBtn.gameObject.SetActive(false);
        dealBtn.gameObject.SetActive(true);
        betButtonsManager.EnableBetButtons();
        betInfoText.gameObject.SetActive(true);
        mainText.gameObject.SetActive(true);
        dealerHandText.gameObject.SetActive(true);
        hideCard.GetComponent<Renderer>().enabled = false;
        cashText.SetText(GameStrings.CashPrefix + playerScript.GetMoney().ToString());
        betText.color = Color.white;
        cashText.color = Color.white;
        blackJack = false;

        //reset bet to what it was before if double down was done
        if (doubleDown)
        {
            doubleDown = false;
            bet = (bet / 2);
            pot = (pot / 2);
        }

        //if player doesn't have enough money left to make a bet, show game over screen
        if (playerScript.GetMoney() < 10)
        {
            StartCoroutine(GameOver());
        }


    }

    //add money to pot if bet clicked
    private void BetClicked(Button betBtn)
    {
        TextMeshProUGUI newBet = betBtn.GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
        int intBet = int.Parse(newBet.text.ToString());
        cashText.SetText(GameStrings.CashPrefix + playerScript.GetMoney().ToString());
        pot += (intBet * 2);
        bet += intBet;
        betText.SetText(GameStrings.BetPrefix + bet.ToString());

        //play sound effect
        AudioManager.instance.PlaySFX(AudioManager.instance.betButtonPress);

        //if bet is over $0, turn text white
        if (bet > 0)
        {
            betText.color = Color.white;
        }
    }

    public void BetRightClicked(Button betBtn)
    {
        TextMeshProUGUI removeBet = betBtn.GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
        int intBet = int.Parse(removeBet.text.ToString());
        cashText.SetText(GameStrings.CashPrefix + playerScript.GetMoney().ToString());
        pot -= (intBet * 2);
        bet -= intBet;
        betText.SetText(GameStrings.BetPrefix + bet.ToString());

        //if bet is $0 or less, turn text red
        if (bet <= 0)
        {
            betText.color = Color.red;
        }

        //play sound effect
        AudioManager.instance.PlaySFX(AudioManager.instance.betButtonPress);
    }

    private void DoubleDownClicked()
    {
        
        //check that there is still room on the table
        if (playerScript.cardIndex <= 11)
        {
            //save the original bet amount before doubling
            int additionalBet = bet;

            playerScript.GetCard();
            AudioManager.instance.PlaySFX(AudioManager.instance.cardPlaced);

            //double the bet
            bet = (bet * 2);
            betText.SetText(GameStrings.BetPrefix + bet.ToString());

            //double the pot
            pot = (pot * 2);
            doubleDown = true;

            //subtract the *additional* amount from the player's money immediately
            playerScript.AdjustMoney(-additionalBet);
            cashText.SetText(GameStrings.CashPrefix + playerScript.GetMoney().ToString());
        }

        //play sound effect
        AudioManager.instance.PlaySFX(AudioManager.instance.buttonPress);

        //hide buttons
        dealBtn.gameObject.SetActive(false);
        standBtn.gameObject.SetActive(false);
        hitBtn.gameObject.SetActive(false);
        dblDownBtn.gameObject.SetActive(false);

        //update player hand text
        playerHandText.SetText(GameStrings.YourHand + playerScript.handValue.ToString());

        //let the dealer finish their hand
        StartCoroutine(HitDealer());
    }

    //opens the game over panel
    private IEnumerator GameOver()
    {
        //stop current music
        AudioManager.instance.StopMusic();

        //play game over music
        AudioManager.instance.PlayMusic(AudioManager.instance.gameOverMusic);

        yield return new WaitForSeconds(3.5f);

        gameOverText.SetText(GameStrings.GameOverMessage);
        gameOverCanvas.SetActive(true);
    }

    //restarts the game
    private void RestartClicked()
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.buttonPress);
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioManager.instance.PlayMusic(AudioManager.instance.regularMusic);
        SceneManager.sceneLoaded -= OnSceneLoaded; // remove listener to prevent duplicate calls
    }

    //quits the game
    private void QuitClicked()
    {
        //play sound effect
        AudioManager.instance.PlaySFX(AudioManager.instance.buttonPress);

        Application.Quit();    
    }
}
