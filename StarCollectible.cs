using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class StarCollectible : MonoBehaviour
{
    [Header("Targets & Settings")]
    [SerializeField] private GameObject doorObject;
    [SerializeField] private TMP_Text starText;
    [SerializeField] private int starsToOpenDoor = 5;

    [Header("Effects")]
    [SerializeField] private GameObject collectEffect;
    [SerializeField] private AudioClip collectSound;

    public static int totalCollected = 0;
    private bool isCollected = false;

    private void OnEnable()
    {
        // الاشتراك فـ حدث تبديل الـ Scene باش يتصفر العداد أوتوماتيكياً
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // تصفير العداد فوراً فاش كتحل الماب
        totalCollected = 0;
    }

    private void Awake()
    {
        // تصفير إضافي مباشر مع بداية تشغيل النجمة
        totalCollected = 0;
        isCollected = false;
    }

    private void Start()
    {
        // إيجاد الـ UI والباب تلقائياً
        if (starText == null)
        {
            GameObject textObj = GameObject.Find("StarText");
            if (textObj != null) starText = textObj.GetComponent<TMP_Text>();
        }

        if (doorObject == null)
        {
            doorObject = GameObject.FindWithTag("Door");
        }

        UpdateUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;

        bool isPlayer = false;

        // 1. فحص عن طريق التاغ
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            isPlayer = true;
        }
        // 2. فحص عن طريق سكريبت PlayerController
        else if (other.GetComponentInParent<PlayerController>() != null || other.GetComponent<PlayerController>() != null)
        {
            isPlayer = true;
        }
        // 3. فحص عن طريق CharacterController
        else if (other.GetComponentInParent<CharacterController>() != null)
        {
            isPlayer = true;
        }

        if (isPlayer)
        {
            Collect();
        }
    }

    private void Collect()
    {
        isCollected = true;
        totalCollected++;

        UpdateUI();

        // تشغيل صوت التجميع
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        // تشغيل المؤثر البصري
        if (collectEffect != null)
        {
            GameObject effectInstance = Instantiate(collectEffect, transform.position, Quaternion.identity);
            Destroy(effectInstance, 2.5f);
        }

        // فتح الباب
        if (totalCollected >= starsToOpenDoor && doorObject != null)
        {
            doorObject.SetActive(false);
        }

        Destroy(gameObject);
    }

    private void UpdateUI()
    {
        if (starText != null)
        {
            starText.text = totalCollected.ToString() + "/" + starsToOpenDoor.ToString();
        }
    }
}