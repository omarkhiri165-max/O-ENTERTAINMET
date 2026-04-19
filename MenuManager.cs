using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject settingsPanel; // الـ Canvas أو الـ Panel ديال السيتينغ
    public GameObject mainLobbyPanel; // الـ Panel الرئيسي اللي فيه الزر ديال Play و Store

    void Start()
    {
        // فاش كتحل اللعبة، كنتأكدو بلي السيتينغ مخفية واللوبي باينة
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (mainLobbyPanel != null) mainLobbyPanel.SetActive(true);
    }

    // --- دالة لإظهار السيتينغ وإخفاء اللوبي ---
    public void OpenSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
        if (mainLobbyPanel != null) mainLobbyPanel.SetActive(false);
        Debug.Log("Settings Opened");
    }

    // --- دالة لإغلاق السيتينغ والرجوع للوبي ---
    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (mainLobbyPanel != null) mainLobbyPanel.SetActive(true);
        Debug.Log("Settings Closed");
    }
}