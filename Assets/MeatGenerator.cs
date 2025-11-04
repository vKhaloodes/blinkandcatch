using System.Collections;
using UnityEngine;

public class MeatGenerator : MonoBehaviour
{
    [Header("Meat prefab to spawn")]
    public GameObject meatPrefab;

    [Header("Spawn settings")]
    public float spawnInterval = 1.5f; // البداية
    private float time = 0f;           // عداد الوقت
    private float intervalDecrease = 0.005f; // مقدار النقص كل 10 ثواني
    private float lastDecreaseTime = 0f;     // آخر مرة نقص فيها interval
    public float y_shift;
    public GameObject cannon;
    GameObject prevcannon;
    Vector3 pspawnpos;
    public BirdScript b;
    void Start()
    {
        Cannon();

        if (meatPrefab == null)
        {
            Debug.LogError("⚠️ Meat prefab not assigned!");
            return;
        }

        
    }

    void Update()
    {
        time += Time.deltaTime;

        // كل 10 ثواني ننقص spawnInterval
        if (time - lastDecreaseTime >= 10f)
        {
            spawnInterval = Mathf.Max(0.01f, spawnInterval - intervalDecrease); // لا نخليها صفر
            lastDecreaseTime = time;
            Debug.Log("Spawn interval decreased: " + spawnInterval);
        }
    }

    public void SpawnM()
    {
        if(b.finish == false)
        {
            StartCoroutine(SpawnMeatCoroutine());
        }
    }
    public void Cannon()
    {
        if(b.finish == false)
        {
            if (prevcannon != null)
            {
                Destroy(prevcannon);
                prevcannon = null;
            }
            float randomX = Random.Range(0 + y_shift, Screen.width);
            float randomY = Screen.height + y_shift;

            Vector3 spawnPos = Camera.main.ScreenToWorldPoint(
        new Vector3(randomX, randomY, 10f)
            );

            GameObject g = Instantiate(cannon, spawnPos, Quaternion.identity);

            prevcannon = g;

            pspawnpos = spawnPos;
        }
    }
    IEnumerator SpawnMeatCoroutine()
    {
        Instantiate(meatPrefab, pspawnpos, Quaternion.identity);

        Cannon();

        yield return new WaitForSeconds(0);
    }
}
