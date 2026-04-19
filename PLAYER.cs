using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PLAYER : MonoBehaviour
{
    // ... (نفس المتغيرات القديمة خليها كيف هي) ...
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float speedMultiplier = 2f;
    public float speedDuration = 10f;
    public float cooldownDuration = 5f;

    [Header("Gravity Settings")]
    public float gravity = -9.81f;
    private Vector3 velocity;
    private bool isGrounded;

    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public Image healthFill;
    public GameObject gameOverUI;

    [Header("Regeneration Settings")]
    public float regenDelay = 20f;
    private float regenTimer = 0f;
    private bool needsRegen = false;
    private float healthBeforeDamage;
    public float fillSpeed = 20f;

    [Header("UI References")]
    public Button shootButton;
    public Button speedButton;
    public RectTransform crosshair;
    public analog analog;

    [Header("Shooting Settings")]
    public GameObject hitEffect;
    public int playerDamage = 20;

    [Header("Speed Effect Settings")]
    public GameObject speedEffectPrefab;
    public Transform effectSpawnPoint;
    private GameObject activeEffect;

    private Animator animator;
    private CharacterController controller;
    private float baseSpeed;
    private bool isSpeedActive = false;

    internal int health;
    public int coins; // ✨ هادو هما الكوينز اللي غانحفظو

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        currentHealth = maxHealth;
        healthBeforeDamage = currentHealth;
        UpdateHealthUI();
        gameOverUI.SetActive(false);

        if (crosshair != null) crosshair.gameObject.SetActive(true);
        if (shootButton != null) shootButton.onClick.AddListener(Shoot);
        if (speedButton != null) speedButton.onClick.AddListener(() => { if (!isSpeedActive) StartCoroutine(SpeedBoost()); });

        baseSpeed = moveSpeed;
    }

    void Update()
    {
        if (currentHealth <= 0) return;
        HandleMovement();
        HandleRegeneration();
    }

    // ... (HandleMovement, HandleRegeneration, SmoothHeal خليهم كيف هما) ...
    void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0) velocity.y = -2f;
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0f; right.y = 0f;
        forward.Normalize(); right.Normalize();
        float moveX = analog.Horizontal;
        float moveZ = analog.Vertical;
        Vector3 move = (forward * moveZ + right * moveX);
        controller.Move(move * moveSpeed * Time.deltaTime);
        animator.SetBool("isRunning", moveX != 0 || moveZ != 0);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleRegeneration()
    {
        if (currentHealth < maxHealth && needsRegen)
        {
            regenTimer += Time.deltaTime;
            if (regenTimer >= regenDelay)
            {
                float targetHealth = currentHealth;
                if (currentHealth >= 50f) targetHealth += 25f;
                else targetHealth += 50f;
                targetHealth = Mathf.Clamp(targetHealth, 0, maxHealth);
                StartCoroutine(SmoothHeal(targetHealth));
                regenTimer = 0f;
                needsRegen = false;
            }
        }
    }

    IEnumerator SmoothHeal(float target)
    {
        while (currentHealth < target)
        {
            currentHealth += fillSpeed * Time.deltaTime;
            if (currentHealth > target) currentHealth = target;
            UpdateHealthUI();
            yield return null;
        }
        healthBeforeDamage = currentHealth;
    }

    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return;
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
        if (healthBeforeDamage - currentHealth >= 20f) { needsRegen = true; regenTimer = 0f; }
        if (currentHealth <= 0) Die();
        else animator.SetTrigger("Damage");
    }

    // --- ✨ التعديل المهم هنا فـ دالة Die ✨ ---
    void Die()
    {
        animator.SetTrigger("Die");

        // 1. حفظ السكور العالي (عن طريق استدعاء الدالة من spawner)
        ZombieSpawnerTMP spawner = FindObjectOfType<ZombieSpawnerTMP>();
        if (spawner != null)
        {
            spawner.SaveFinalData();
        }

        // 2. حفظ الكوينز وزيادتهم على القدام
        int totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        PlayerPrefs.SetInt("TotalCoins", totalCoins + coins);
        PlayerPrefs.Save();

        Debug.Log("Game Over: Saved Score and added " + coins + " coins.");

        StartCoroutine(ShowGameOverDelay());
    }

    public void RevivePlayer()
    {
        currentHealth = maxHealth;
        healthBeforeDamage = maxHealth;
        needsRegen = false;
        UpdateHealthUI();
        gameOverUI.SetActive(false);
        Time.timeScale = 1f;
        animator.Play("Idle");
    }

    void Shoot()
    {
        if (currentHealth <= 0) return;
        animator.SetTrigger("Shoot");
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hitEffect != null) Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();
            if (enemy != null) enemy.TakeDamage(playerDamage);
        }
        StartCoroutine(ResetShootTrigger());
    }

    IEnumerator ResetShootTrigger() { yield return new WaitForSeconds(0.5f); animator.ResetTrigger("Shoot"); }
    void UpdateHealthUI() { if (healthFill != null) healthFill.fillAmount = currentHealth / maxHealth; }
    IEnumerator ShowGameOverDelay() { yield return new WaitForSecondsRealtime(1.5f); Time.timeScale = 0f; gameOverUI.SetActive(true); }
    public void RestartGame() { Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }

    private IEnumerator SpeedBoost()
    {
        isSpeedActive = true;
        moveSpeed = baseSpeed * speedMultiplier;
        if (speedEffectPrefab != null && effectSpawnPoint != null)
        {
            activeEffect = Instantiate(speedEffectPrefab, effectSpawnPoint.position, Quaternion.identity);
            activeEffect.transform.SetParent(transform);
        }
        float timer = speedDuration;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            if (activeEffect != null) activeEffect.SetActive(controller.velocity.magnitude > 0.1f);
            yield return null;
        }
        moveSpeed = baseSpeed;
        if (activeEffect != null) Destroy(activeEffect);
        yield return new WaitForSeconds(cooldownDuration);
        isSpeedActive = false;
    }
}
