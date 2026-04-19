using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    [Header("Scene Settings")]
    public string gameplaySceneName = "LevelPlay"; // سمية الـ Scene ديال اللعب عندك

    // رجعتها public int باش تبان ليك ف الـ Inspector وتقدر تتحكم فيها
    public int bestScore;

    [Header("UI Data Display")]
    public TMP_Text highScoreText;
    public TMP_Text totalCoinsText;

    // الخانة ديال الـ Settings Panel غاتبقى هنا باش نتحكمو ف الظهور ديالها
    [Header("Panels Configuration")]
    public GameObject settingsPanel;

    private void Awake()
    {
        // باش تغبر السيتينغ غير يبدا اللعب
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    private void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        // كنخدو السكور الحقيقي من الـ Bridge
        bestScore = GameDataBridge.GetHighScore();
        int coins = GameDataBridge.GetTotalCoins();

        if (highScoreText != null) highScoreText.text = bestScore.ToString();
        if (totalCoinsText != null) totalCoinsText.text = coins.ToString();
    }

    public void PlayGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            // Pause إذا كنا ف وسط اللعب
            if (SceneManager.GetActiveScene().name == gameplaySceneName) Time.timeScale = 0f;
        }
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}