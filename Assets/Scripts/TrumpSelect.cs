using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrumpSelect : MonoBehaviour
{
    public GameObject greenSelectBtn;
    public GameObject redSelectBtn;
    public GameObject blackSelectBtn;
    public GameObject yellowSelectBtn;
    public GameObject readyBtn;

    public GameObject infoTxt;

    private CardColor color = CardColor.NONE;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

}
