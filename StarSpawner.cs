using System.Collections.Generic;
using UnityEngine;

public class StarSpawner : MonoBehaviour
{
    [Header("Star & Spawn Settings")]
    public GameObject starPrefab;         // 🌟 حط هنا الـ Prefab ديال النجمة
    public Transform[] spawnPoints;       // 📍 حط هنا جميع الأماكن الممكنة فـ الخريطة
    public int starsToSpawn = 5;          // 🔢 عدد النجمات اللي بغينا ننزلوها (5)

    void Start()
    {
        SpawnStarsOnce();
    }

    void SpawnStarsOnce()
    {
        // 1. التأكد من وجود الـ Prefab والأماكن
        if (starPrefab == null)
        {
            Debug.LogError("⚠️ حط Star Prefab فـ Inspector!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("⚠️ حط قائمة الأماكن Spawn Points فـ Inspector!");
            return;
        }

        // 2. نسخ قائمة الأماكن فـ List باش نقدرو نحيدو البلاصة اللي تخترات وما نتكرروش
        List<Transform> availablePoints = new List<Transform>(spawnPoints);

        // 3. تحديد العدد النهائي (بحيث ما يتعداش عدد البلايص المتاحة)
        int spawnCount = Mathf.Min(starsToSpawn, availablePoints.Count);

        // 4. اختيار 5 بلايص عشوائية وإنشاء النجمات
        for (int i = 0; i < spawnCount; i++)
        {
            // اختيار index عشوائي من القائمة المتاحة
            int randomIndex = Random.Range(0, availablePoints.Count);
            Transform selectedPoint = availablePoints[randomIndex];

            // إنشاء النجمة فـ البلاصة المختارة
            Instantiate(starPrefab, selectedPoint.position, selectedPoint.rotation);

            // مسح البلاصة من القائمة باش ما تنزلش فيها نجمة ثانية
            availablePoints.RemoveAt(randomIndex);
        }

        // 5. تعطيل السكريبت بعد التنفيذ للحيطة والحذر
        this.enabled = false;
    }
}