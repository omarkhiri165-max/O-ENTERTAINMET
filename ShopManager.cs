using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class SkinData
{
    public int id;
    public int price;

    [HideInInspector]
    public bool isBought;
}

/// <summary>
/// ShopManager: manages multiple skins using a single shared UI for purchase/select actions.
/// - Keeps a list of skins (id, price)
/// - Uses PlayerPrefs to persist purchases and selected skin
/// - Updates a single action button and a coins text element based on current skin
/// </summary>
public class ShopManager : MonoBehaviour
{
    [Header("Skins")]
    [Tooltip("List of available skins. Use unique IDs for each skin.")]
    public List<SkinData> skins = new List<SkinData>();

    [Header("UI References")]
    [Tooltip("Text element that shows the player's total coins.")]
    public TextMeshProUGUI coinsText;

    [Tooltip("Button used for Buy/Select action for the currently inspected skin.")]
    public Button actionButton;

    [Tooltip("Text element inside the action button (Buy: X / Select).")]
    public TextMeshProUGUI actionButtonText;

    [Header("Settings")]
    [Tooltip("Starting coins if none are saved in PlayerPrefs.")]
    public int defaultCoins = 1000;

    private int currentSkinIndex = 0; // index into skins list for the currently inspected skin

    const string CoinsKey = "TotalCoins";
    const string SelectedSkinKey = "SelectedSkinID";

    void Awake()
    {
        // Ensure coins exist
        if (!PlayerPrefs.HasKey(CoinsKey))
            PlayerPrefs.SetInt(CoinsKey, defaultCoins);

        // Load bought state for each skin
        for (int i = 0; i < skins.Count; i++)
        {
            skins[i].isBought = PlayerPrefs.GetInt(GetSkinBoughtKey(skins[i].id), 0) == 1;
        }

        // Ensure action button has no duplicate listeners
        if (actionButton != null)
            actionButton.onClick.RemoveAllListeners();

        UpdateUI();
    }

    // Call this to inspect a different skin (e.g. when the user navigates skins in the UI)
    public void InspectSkinByIndex(int index)
    {
        if (index < 0 || index >= skins.Count) return;
        currentSkinIndex = index;
        UpdateUI();
    }

    public void InspectSkinByID(int skinID)
    {
        int idx = skins.FindIndex(s => s.id == skinID);
        if (idx >= 0) InspectSkinByIndex(idx);
    }

    // ProcessPurchase: attempts to buy the skin with the provided ID.
    // If successful, deducts coins, marks skin as bought in PlayerPrefs, and updates UI.
    public void ProcessPurchase(int skinID)
    {
        SkinData skin = GetSkinByID(skinID);
        if (skin == null) return;

        int coins = PlayerPrefs.GetInt(CoinsKey, defaultCoins);
        if (coins < skin.price)
        {
            Debug.Log("ShopManager: Not enough coins to purchase skin " + skinID);
            return; // insufficient funds
        }

        coins -= skin.price;
        PlayerPrefs.SetInt(CoinsKey, coins);

        // Mark bought
        PlayerPrefs.SetInt(GetSkinBoughtKey(skinID), 1);
        PlayerPrefs.Save();

        skin.isBought = true;

        // If the purchased skin is the currently inspected skin, update button to Select
        UpdateUI();

        Debug.Log("ShopManager: Purchased skin " + skinID + " for " + skin.price + " coins.");
    }

    // Selects the skin (sets as equipped)
    public void SelectSkin(int skinID)
    {
        SkinData skin = GetSkinByID(skinID);
        if (skin == null) return;
        if (!skin.isBought)
        {
            Debug.Log("ShopManager: Can't select skin that is not bought: " + skinID);
            return;
        }

        PlayerPrefs.SetInt(SelectedSkinKey, skinID);
        PlayerPrefs.Save();

        Debug.Log("ShopManager: Selected skin " + skinID);
        UpdateUI();
    }

    // Update the shared UI (coins text and action button) based on the currently inspected skin
    public void UpdateUI()
    {
        if (skins.Count == 0) return;
        if (currentSkinIndex < 0 || currentSkinIndex >= skins.Count) currentSkinIndex = 0;

        SkinData current = skins[currentSkinIndex];

        int coins = PlayerPrefs.GetInt(CoinsKey, defaultCoins);
        if (coinsText != null) coinsText.text = "Coins: " + coins;

        if (actionButton == null || actionButtonText == null) return;

        actionButton.onClick.RemoveAllListeners();

        bool isOwned = PlayerPrefs.GetInt(GetSkinBoughtKey(current.id), 0) == 1;
        current.isBought = isOwned;

        int selectedID = PlayerPrefs.GetInt(SelectedSkinKey, -1);
        bool isSelected = selectedID == current.id;

        if (isOwned)
        {
            if (isSelected)
            {
                actionButtonText.text = "Selected";
                actionButton.interactable = false;
            }
            else
            {
                actionButtonText.text = "Select";
                actionButton.interactable = true;
                actionButton.onClick.AddListener(() => SelectSkin(current.id));
            }
        }
        else
        {
            actionButtonText.text = "Buy: " + current.price;
            actionButton.interactable = coins >= current.price;
            actionButton.onClick.AddListener(() => ProcessPurchase(current.id));
        }
    }

    private SkinData GetSkinByID(int id)
    {
        return skins.Find(s => s.id == id);
    }

    private string GetSkinBoughtKey(int skinID)
    {
        return "Skin_" + skinID + "_Bought";
    }
}