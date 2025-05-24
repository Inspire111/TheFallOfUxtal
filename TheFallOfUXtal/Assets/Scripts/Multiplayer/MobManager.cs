using System.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class MobManager : NetworkBehaviour
{
    public enum SpawnState { Counting, Spawning, Waiting, End }


    [System.Serializable]
    public class Wave
    {
        public int id;
        public GameObject enemyPrefab; // Must have NetworkObject
        public int count;
        public float rate;

    }

    public Wave[] waves;
    private int nextWave = 0;
    public float timeBetweenWaves = 5f;
    private float waveCountdown;

    public float timeBetweenSearches = 1f;
    private float searchCountdown = 1f;

    public SpawnState state = SpawnState.Counting;
    public string targetTag;

    public GameObject HUD;
    public TMPro.TextMeshProUGUI player1Text;
    public TMPro.TextMeshProUGUI player2Text;

    public Transform[] SpawnPoints;
    

    private void Start()
    {
        if (!IsServer) enabled = false; // Only run on server

        waveCountdown = timeBetweenWaves;
        HUD.SetActive(false);
    }

    private void Update()
    {
        if (!IsServer) return;

        if (state == SpawnState.Waiting)
        {
            if (!EnemyIsAlive())
            {
                WaveCompleted();
            }
            else
            {
                return;
            }
        }

        if (state == SpawnState.Counting)
        {
            if (waveCountdown <= 0f)
            {
                StartCoroutine(SpawnWave(waves[nextWave]));
            }
            else
            {
                waveCountdown -= Time.deltaTime;
            }
        }
    }

    void WaveCompleted()
    {
        Debug.Log("Wave Completed!");

        state = SpawnState.End;
        waveCountdown = timeBetweenWaves;

        if (nextWave + 1 >= waves.Length)
        {
            nextWave = 0;
            EndGame();
        }
        else
        {
            state = SpawnState.Counting;
            nextWave++;
        }
    }

    bool EnemyIsAlive()
    {
        searchCountdown -= Time.deltaTime;

        if (searchCountdown <= 0f)
        {
            searchCountdown = timeBetweenSearches;

            // Find all networked mobs by tag
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(targetTag);

            if (enemies.Length == 0)
                return false;
        }

        return true;
    }

    IEnumerator SpawnWave(Wave wave)
    {
        state = SpawnState.Spawning;

        Debug.Log($"Spawning Wave: {wave.id}");

        for (int i = 0; i < wave.count; i++)
        {
            SpawnEnemy(wave.enemyPrefab);
            yield return new WaitForSeconds(1f / wave.rate);
        }

        state = SpawnState.Waiting;
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {

        int spawnIndex = Random.Range(0, SpawnPoints.Length - 1);
        GameObject go = Instantiate(enemyPrefab, SpawnPoints[spawnIndex].position, Quaternion.identity);
        var netObj = go.GetComponent<NetworkObject>();

        if (netObj != null)
        {
            netObj.Spawn(); // This syncs to all clients
        }
        else
        {
            Debug.LogError("Enemy prefab must have a NetworkObject component!");
        }
    }
    void EndGame()
    {
        Debug.Log("Player number : " + PlayerScore.allPlayers.Count);
        if (PlayerScore.allPlayers.Count >= 2)
        {
            var player1 = PlayerScore.allPlayers[0].gameObject;
            var player2 = PlayerScore.allPlayers[1].gameObject;
            TriggerGameOver(player1, player2);
        }
    }

    public void TriggerGameOver(GameObject player1, GameObject player2)
    {
        if (!IsServer) return;

        var score1 = player1.GetComponent<PlayerScore>()?.score.Value ?? 0;
        var score2 = player2.GetComponent<PlayerScore>()?.score.Value ?? 0;

        ShowFinalScoresClientRpc(score1, score2);
    }

    [ClientRpc]
    private void ShowFinalScoresClientRpc(int score1, int score2)
    {
        HUD.SetActive(true);
        player1Text.text = $"Hote Score: {score1}";
        player2Text.text = $"Client Score: {score2}";
    }

    public void GoToMenu()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.Shutdown(); // Stops server and disconnects clients
            NetworkManager.Singleton.Shutdown(); // End connection
            SceneManager.LoadScene("Menu");
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown(); // Disconnects from server
            NetworkManager.Singleton.Shutdown(); // End connection
            SceneManager.LoadScene("Menu");
        }
        
    }
}
