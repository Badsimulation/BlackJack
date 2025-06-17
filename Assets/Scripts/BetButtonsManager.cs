using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BetButtonsManager : MonoBehaviour
{
    public List<Button> betButtons;

    public void EnableBetButtons()
    {
        foreach (Button btn in betButtons)
        {
            btn.gameObject.SetActive(true);
        }
    }
    public void DisableBetBettons()
    {
        foreach (Button btn in betButtons)
        {
            btn.gameObject.SetActive(false);
        }
    }
    
}
