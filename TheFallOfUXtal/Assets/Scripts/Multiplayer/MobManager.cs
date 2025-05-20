using System;
using System.Collections;
using UnityEngine;

public class MobManager : MonoBehaviour
{
    public enum SpawnState { Counting, Spawning, Wainting }

    [System.Serializable]
    public class Wave
    {
        public int id;
        public GameObject enemy;
        public int count;
        public float rate;
    }

    public Wave[] waves;
    private int nextWave = 0;
    public float timeBetweenWaves = 5f;
    public float waveCountdown;

    public float timeBetweenSearches = 1f;
    public float searchCountdown;
    public SpawnState state;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        waveCountdown = timeBetweenWaves;
        searchCountdown = timeBetweenSearches;
        state = SpawnState.Counting;
    }

    // Update is called once per frame
    void Update()
    {
        if (state is SpawnState.Wainting)
        {
            /*
            if (!EnemyIsAlive())
            {
                // new round
                WaveCompleted();
            }
            else
            {
                return;
            }
            */
            return;
        }
        if (waveCountdown <= 0 && state is SpawnState.Counting)
        {
            StartCoroutine(SpawnWaves(waves[nextWave]));
        }
        else if(state is SpawnState.Counting)
        {
            waveCountdown -= Time.deltaTime;

        }
    }

    void WaveCompleted()
    {
        Debug.Log("Wave Completed !");

        state = SpawnState.Counting;
        waveCountdown = timeBetweenWaves;

        if (nextWave + 1 > waves.Length - 1)
        {
            nextWave = 0;
            Debug.Log("All Waves Complete! Looping...");
        }
        else
        {
            nextWave++;
        }
    }
    bool EnemyIsAlive()
    {
        searchCountdown -= Time.deltaTime;
        if (searchCountdown <= 0)
        {
            searchCountdown = timeBetweenSearches;
            if (GameObject.FindGameObjectWithTag("Enemy") == null)
                return false;
        }
        return true;
    }
    IEnumerator SpawnWaves(Wave _wave)
    {
        state = SpawnState.Spawning;
        Debug.Log("Spawning Wave : " + _wave.id);
        

        for (int i = 0; i < _wave.count; i++)
        {
            SpawnEnemy(_wave.enemy);
            yield return new WaitForSeconds(1f / _wave.rate); // time between each enemies
        }

        state = SpawnState.Wainting;

        yield break;
    }

    void SpawnEnemy(GameObject _enemy)
    {
        //Spawn Enemy
        Debug.Log("Spawning Enemy : " + _enemy.name);
        Instantiate(_enemy, transform.position, transform.rotation);
        
    }
}
