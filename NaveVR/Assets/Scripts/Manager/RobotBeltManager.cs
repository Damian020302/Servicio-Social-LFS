using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotBeltManager : MonoBehaviour
{
    [Header("Robot Belt Settings")]
    [SerializeField] private GameObject[] robotPrefabs;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnInterval = 2.0f;
    [SerializeField] private int copiesPerPrefab = 5;
    [Header("Scale Settings")]
    [SerializeField] private float scaleMultiplier = 1.25f;
    [SerializeField] private float scaleDuration = 0.25f;

    private readonly List<GameObject> robotPool = new List<GameObject>();
    private readonly Dictionary<GameObject, Vector3> baseScales = new Dictionary<GameObject, Vector3>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (GameObject prefab in robotPrefabs)
        {
            for (int i = 0; i < copiesPerPrefab; i++)
            {
                GameObject bot = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
                baseScales[bot] = bot.transform.localScale;
                bot.SetActive(false);
                robotPool.Add(bot);
            }
        }
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(spawnInterval);
        while (true)
        {
            SpawnRandomRobot();
            yield return wait;
        }
    }

    void SpawnRandomRobot()
    {
        List<GameObject> inactiveRobots = robotPool.FindAll(bot => !bot.activeInHierarchy);
        if (inactiveRobots.Count == 0)
        {
            Debug.LogWarning("No inactive robots available. Cannot spawn robot.");
            return;
        }
        GameObject chosen = inactiveRobots[Random.Range(0, inactiveRobots.Count)];
        chosen.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
        chosen.SetActive(true);
        if(chosen.TryGetComponent(out Rigidbody rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        Vector3 targetScale = baseScales[chosen] * scaleMultiplier;
        StartCoroutine(ScaleUpRoutine(chosen.transform, targetScale));
    }

    private IEnumerator ScaleUpRoutine(Transform target, Vector3 targetScale)
    {
        if(scaleDuration <= 0f)
        {
            target.localScale = targetScale;
            yield break;
        }
        float elapsedTime = 0f;
        Vector3 initialScale = Vector3.zero;
        target.localScale = initialScale;
        while (elapsedTime < scaleDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / scaleDuration);
            target.localScale = Vector3.Lerp(initialScale, targetScale, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        target.localScale = targetScale;
    }
}