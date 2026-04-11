using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public int score = 0;
    public TextMeshProUGUI scoreText;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ✨ يبقى حتى لو تبدلت المشاهد
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ResetScore(); // ✨ يبدأ من 0 ويكتبها فالـ TMP
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = score.ToString(); // ✨ غير الرقم بوحدو
    }
}