using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class RightClickBetHandler : MonoBehaviour, IPointerClickHandler
{
    public GameManager gameManager;
    public Button betButton;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            gameManager.BetRightClicked(betButton);
        }
    }
}