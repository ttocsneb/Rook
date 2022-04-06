using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrumpSelect : MonoBehaviour
{
    public GameObject greenSelectBtn;
    public GameObject redSelectBtn;
    public GameObject blackSelectBtn;
    public GameObject yellowSelectBtn;
    public GameObject readyBtn;


    public delegate void TrumpUpdated(CardColor cardColor, bool isReady);
    private List<TrumpUpdated> callbacks = new List<TrumpUpdated>();

    public GameObject infoTxt;

    private CardColor color = CardColor.NONE;
    private int toDiscard = 5;

    public void Start() 
    {
        AddCallback(OnTrumpUpdated);
        updateHelp();
    }

    private void resetButtons()
    {
        greenSelectBtn.GetComponent<Button>().interactable = true;
        redSelectBtn.GetComponent<Button>().interactable = true;
        blackSelectBtn.GetComponent<Button>().interactable = true;
        yellowSelectBtn.GetComponent<Button>().interactable = true;
    }

    private void disableButton(GameObject btn)
    {
        resetButtons();
        btn.GetComponent<Button>().interactable = false;
        updateHelp();
        if (toDiscard == 0) {
            readyBtn.GetComponent<Button>().interactable = true;
        }
        call(false);
    }

    private void call(bool ready)
    {
        foreach (TrumpUpdated callback in callbacks) {
            callback(color, ready);
        }
    }

    public void AddCallback(TrumpUpdated callback)
    {
        callbacks.Add(callback);
    }

    public void UpdateToDiscard(int cardsLeft)
    {
        toDiscard = cardsLeft < 0 ? 0 : cardsLeft;
        if (toDiscard == 0 && color != CardColor.NONE) {
            readyBtn.GetComponent<Button>().interactable = true;
        }
        updateHelp();
    }

    public void DiscardOne()
    {
        UpdateToDiscard(toDiscard - 1);
    }

    private void updateHelp()
    {
        string text = "";

        if (color == CardColor.NONE) {
            text += "Select a trump color";
        } else {
            text += "You have selected ";
            switch (color) {
                case CardColor.GREEN: text += "green";
                    break;
                case CardColor.RED: text += "red";
                    break;
                case CardColor.BLACK: text += "black";
                    break;
                case CardColor.YELLOW: text += "yellow";
                    break;
                default: text += "no";
                    break;
            }
            text += " trump";
        }

        if (toDiscard > 0) {
            text += " and discard " + toDiscard + " more cards";
        }

        infoTxt.GetComponent<TextMeshProUGUI>().text = text;
    }

    public void OnTrumpUpdated(CardColor trump, bool isReady)
    {
        Debug.Log("Callback Called: Is ready: " + isReady);
    }

    public void OnGreenSelect() 
    {
        color = CardColor.GREEN;
        disableButton(greenSelectBtn);
        Debug.Log("Green selected");
    }
    
    public void OnRedSelect() 
    {
        color = CardColor.RED;
        disableButton(redSelectBtn);
        Debug.Log("Red selected");
    }

    public void OnBlackSelect() 
    {
        color = CardColor.BLACK;
        disableButton(blackSelectBtn);
        Debug.Log("Black selected");
    }

    public void OnYellowSelect() 
    {
        color = CardColor.YELLOW;
        disableButton(yellowSelectBtn);
        Debug.Log("Yellow selected");
    }

    public void OnReady()
    {
        if (color == CardColor.NONE) {
            return;
        }
        if (toDiscard > 0) {
            return;
        }
        call(true);
    }

}
