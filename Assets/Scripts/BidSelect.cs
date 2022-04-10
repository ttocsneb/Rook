using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BidSelect : MonoBehaviour
{
    public GameObject bidInterface;
    public Button bidBtn;
    public Button passBtn;
    public Button increaseBtn;
    public Button decreaseBtn;
    public TextMeshProUGUI bidTxt;
    public TextMeshProUGUI playerBidTxt;
    public TextMeshProUGUI enemyBid1BidTxt;
    public TextMeshProUGUI enemyBid2BidTxt;
    public TextMeshProUGUI enemyBid3BidTxt;

    private int maxBid = 120;
    private int minBid = 70;

    private int bid = 70;

    public delegate void BidUpdated(int bid);
    public delegate void Pass();

    private List<BidUpdated> bidCallbacks = new List<BidUpdated>();
    private List<Pass> passCallbacks = new List<Pass>();

    // Start is called before the first frame update
    void Start()
    {
        updateText();
    }

    public void OnPass()
    {
        Debug.Log("Passed");
        foreach (Pass callback in passCallbacks) {
            callback();
        }
    }

    public void OnBid()
    {
        Debug.Log("Bid " + bid);
        foreach (BidUpdated callback in bidCallbacks) {
            callback(bid);
        }
    }

    public void AddBidCallback(BidUpdated callback)
    {
        bidCallbacks.Add(callback);
    }

    public void AddPassCallback(Pass callback)
    {
        passCallbacks.Add(callback);
    }

    public void CanBid(bool canBid)
    {
        bidBtn.interactable = canBid;
        passBtn.interactable = canBid;
    }


    private void updateText()
    {
        bidTxt.text = "" + bid;
    }

    public void IncreaseBid()
    {
        bid += 5;
        if (bid > maxBid) {
            bid = maxBid;
        }
        decreaseBtn.interactable = true;
        if (bid == maxBid) {
            increaseBtn.interactable = false;
        }
        updateText();
    }

    public void DecreaseBid()
    {
        bid -= 5;
        if (bid < minBid) {
            bid = minBid;
        }
        increaseBtn.interactable = true;
        if (bid == minBid) {
            decreaseBtn.interactable = false;
        }
        updateText();
    }

    public void SetMinimumBid(int min)
    {
        minBid = min;
        if (bid <= minBid) {
            bid = minBid;
            decreaseBtn.interactable = false;
            updateText();
        } else {
            decreaseBtn.interactable = true;
        }
    }

    public void SetMaximumBid(int max)
    {
        maxBid = max;
        if (bid >= maxBid) {
            bid = maxBid;
            increaseBtn.interactable = false;
            updateText();
        } else {
            increaseBtn.interactable = true;
        }
    }

    public void ShowInterface(bool show)
    {
        bidInterface.SetActive(show);
        playerBidTxt.gameObject.SetActive(!show);
    }
}
