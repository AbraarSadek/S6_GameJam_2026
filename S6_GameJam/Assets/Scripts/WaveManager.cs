using UnityEngine;
using System.Collections;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Settings")]
    public int totalWaves = 3;
    public int baseMobsPerWave = 5;       // wave 1 amount
    public int mobsIncreasePerWave = 3;   // + each wave

    [Header("Timing")]
    public float timeBetweenSpawns = 0.5f;
    public float breakTime = 10f;         // timer AFTER wave is cleared

    [Header("Spawning")]
    public GameObject[] mobPrefabs;       // put your 2 mobs here (size 2)
    public Transform[] spawnPoints;

    [Header("UI")]
    public TextMeshProUGUI timerText;

    private int aliveMobs = 0;

    void Start()
    {
        if (timerText != null) timerText.text = "";
        StartCoroutine(RunWaves());
    }

    IEnumerator RunWaves()
    {
        for (int wave = 1; wave <= totalWaves; wave++)
        {
            int mobsThisWave = baseMobsPerWave + (wave - 1) * mobsIncreasePerWave;

            // Spawn this wave
            for (int i = 0; i < mobsThisWave; i++)
            {
                SpawnMob();
                yield return new WaitForSeconds(timeBetweenSpawns);
            }

            // Wait until all mobs are dead
            yield return new WaitUntil(() => aliveMobs <= 0);

            // Break timer before next wave (not after last wave)
            if (wave < totalWaves)
                yield return StartCoroutine(BreakCountdown());
        }

        if (timerText != null) timerText.text = "All Waves Complete!";
    }

    IEnumerator BreakCountdown()
    {
        float timer = breakTime;

        while (timer > 0)
        {
            if (timerText != null)
                timerText.text = "Next Wave In: " + Mathf.Ceil(timer).ToString();

            yield return new WaitForSeconds(1f);
            timer -= 1f;
        }

        if (timerText != null) timerText.text = "";
    }

    void SpawnMob()
    {
        if (mobPrefabs == null || mobPrefabs.Length == 0) return;
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject randomMobPrefab = mobPrefabs[Random.Range(0, mobPrefabs.Length)];

        GameObject mob = Instantiate(randomMobPrefab, sp.position, sp.rotation);

        aliveMobs++;

        // Give mob a reference so it can report death
        MobDeath death = mob.GetComponent<MobDeath>();
        if (death != null)
            death.manager = this;
    }

    // Called by enemies when they die
    public void MobDied()
    {
        aliveMobs--;
        if (aliveMobs < 0) aliveMobs = 0;
    }
}