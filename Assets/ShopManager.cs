using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("Player Gold")]
    public int startingGold = 100;
    private int currentGold;

    [Header("UI Text")]
    public TMP_Text goldText;
    public TMP_Text messageText;

    [Header("Item Prices")]
    public int brassKnucklesPrice = 100;
    public int boxingHelmetPrice = 75;
    public int steroidsPrice = 60;
    public int creatinePrice = 1;

    [Header("Buy Buttons")]
    public Button brassKnucklesBuyButton;
    public Button boxingHelmetBuyButton;
    public Button steroidsBuyButton;
    public Button creatineBuyButton;

    // Keeps track of items already purchased
    private HashSet<string> purchasedItems = new HashSet<string>();

    // Controls the message disappearing
    private Coroutine messageClearRoutine;

    void Start()
    {
        currentGold = startingGold;

        UpdateGoldText();
        UpdatePurchaseButtons();

        if (messageText != null)
        {
            messageText.text = "";
        }
    }

    public void BuyItem(string itemID)
    {
        // Prevent invalid item IDs
        if (string.IsNullOrWhiteSpace(itemID))
        {
            ShowMessage("ITEM NOT FOUND!");
            return;
        }

        // Stop the player from buying the same item twice
        if (purchasedItems.Contains(itemID))
        {
            ShowMessage("ALREADY OWNED!");
            return;
        }

        // Find the price of the requested item
        int price = GetItemPrice(itemID);

        if (price < 0)
        {
            ShowMessage("ITEM NOT FOUND!");
            return;
        }

        if (price == 0)
        {
            ShowMessage("INVALID ITEM PRICE!");
            return;
        }

        // Check if the player has enough gold
        if (currentGold < price)
        {
            ShowMessage("NOT ENOUGH GOLD!");
            return;
        }

        // Complete the purchase
        currentGold -= price;
        purchasedItems.Add(itemID);

        // Update the shop
        UpdateGoldText();
        UpdatePurchaseButtons();

        ShowMessage("PURCHASE SUCCESSFUL!");
    }

    int GetItemPrice(string itemID)
    {
        switch (itemID)
        {
            case "brass_knuckles":
                return brassKnucklesPrice;

            case "boxing_helmet":
                return boxingHelmetPrice;

            case "steroids":
                return steroidsPrice;

            case "creatine":
                return creatinePrice;

            default:
                return -1;
        }
    }

    void UpdateGoldText()
    {
        if (goldText != null)
        {
            goldText.text = "$" + currentGold;
        }
    }

    void UpdatePurchaseButtons()
    {
        SetButtonState(brassKnucklesBuyButton, "brass_knuckles");
        SetButtonState(boxingHelmetBuyButton, "boxing_helmet");
        SetButtonState(steroidsBuyButton, "steroids");
        SetButtonState(creatineBuyButton, "creatine");
    }

    void SetButtonState(Button button, string itemID)
    {
        if (button == null)
        {
            return;
        }

        bool alreadyOwned = purchasedItems.Contains(itemID);

        // Owned items cannot be purchased again
        button.interactable = !alreadyOwned;

        if (alreadyOwned)
        {
            TMP_Text buttonLabel =
                button.GetComponentInChildren<TMP_Text>();

            if (buttonLabel != null)
            {
                buttonLabel.text = "OWNED";
            }
        }
    }

    void ShowMessage(string message)
    {
        Debug.Log(message);

        if (messageText == null)
        {
            return;
        }

        // Cancel the previous timer if a new message appears
        if (messageClearRoutine != null)
        {
            StopCoroutine(messageClearRoutine);
        }

        messageText.text = message;

        messageClearRoutine =
            StartCoroutine(ClearMessageAfterDelay());
    }

    IEnumerator ClearMessageAfterDelay()
    {
        // Wait 2.5 seconds before removing the message
        yield return new WaitForSecondsRealtime(2.5f);

        if (messageText != null)
        {
            messageText.text = "";
        }

        messageClearRoutine = null;
    }
}