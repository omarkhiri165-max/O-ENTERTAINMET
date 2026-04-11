using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ZombieSpawnerTMP : MonoBehaviour
{
    [Header("Zombie Prefabs")]
    public GameObject zombieType1;
    public GameObject zombieType2;
    public GameObject zombieType3;
    public GameObject zombieType4;
    public GameObject zombieType5;

    [Header("Spawn Settings")]
    public Transform[] spawnPoints;
    public float spawnInterval = 3f;
    public int maxZombiesOnScreen = 7; // ✨ الحد الأقصى للزومبيات

    [Header("Score Settings")]
    public int score = 0;
    public TMP_Text scoreTMP;

    private float timer = 0f;
    private List<GameObject> activeZombies = new List<GameObject>();
    private bool zombieKilled = false;

    void Start()
    {
        UpdateScoreUI();
        SpawnZombie(zombieType1, 1);
    }

    void Update()
    {
        timer += Time.deltaTime;

        // ✨ الشرط دابا فيه جوج حوايج:
        // 1. خاص يدوز الوقت (spawnInterval)
        // 2. خاص يكون مات واحد على الأقل (zombieKilled)
        // 3. والجديد: خاص عدد الزومبيات اللي حيين يكون قل من 7 (maxZombiesOnScreen)
        if (timer >= spawnInterval && zombieKilled && activeZombies.Count < maxZombiesOnScreen)
        {
            HandleSpawning();
            timer = 0f;
            zombieKilled = false; 
        }
    }

    void HandleSpawning()
    {
        // فحص إضافي قبل الولادة باش نضمنو ما نفوتوش 7
        if (activeZombies.Count >= maxZombiesOnScreen) return;

        if (score >= 0 && score < 10)
        {
            SpawnZombie(zombieType1, 1);
        }
        else if (score >= 10 && score < 20)
        {
            SpawnZombie(zombieType2, 2);
        }
        else if (score >= 20 && score < 30)
        {
            SpawnZombie(zombieType3, 3);
        }
        else if (score >= 30 && score < 40)
        {
            SpawnZombie(zombieType4, 4);
        }
        else if (score >= 50)
        {
            // هنا الفوج فيه 5، غنولدوهم غير إلا كان كاين اتساع
            for (int i = 0; i < 5; i++)
            {
                if (activeZombies.Count < maxZombiesOnScreen)
                {
                    int rand = Random.Range(1, 5);
                    GameObject type = (rand == 1) ? zombieType1 :
                                     (rand == 2) ? zombieType2 :
                                     (rand == 3) ? zombieType3 : zombieType4;
                    SpawnZombie(type, 1);
                }
            }
        }
    }

    void SpawnZombie(GameObject prefab, int count)
    {
        if (prefab == null || spawnPoints.Length == 0) return;

        for (int i = 0; i < count; i++)
        {
            // تأكد مرة أخرى وسط اللوب
            if (activeZombies.Count >= maxZombiesOnScreen) break;

            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject zombie = Instantiate(prefab, point.position, point.rotation);

            activeZombies.Add(zombie);

            PLAER playerScript = FindObjectOfType<PLAER>();
            EnemyAI enemyAI = zombie.GetComponent<EnemyAI>();
            if (enemyAI != null && playerScript != null)
            {
                enemyAI.spawner = this;
                enemyAI.player = playerScript.transform;
            }

            DestroyTracker tracker = zombie.AddComponent<DestroyTracker>();
            tracker.spawner = this;
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreTMP != null)
            scoreTMP.text = score.ToString();
    }

    public void ZombieDied(GameObject zombie)
    {
        if (activeZombies.Contains(zombie))
        {
            activeZombies.Remove(zombie);
            AddScore(1);
            zombieKilled = true;
        }
    }
}

public class DestroyTracker : MonoBehaviour
{
    public ZombieSpawnerTMP spawner;

    void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.ZombieDied(gameObject);
        }
    }
}