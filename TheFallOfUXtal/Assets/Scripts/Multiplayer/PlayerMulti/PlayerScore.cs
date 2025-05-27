using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class PlayerScore : NetworkBehaviour
{
    public TMP_Text scoreTextHost;
    public int scoreHost;
    public TMP_Text scoreTextClient;
    public int scoreClient;
    // Server-controlled, initially only owner can read

    public static List<PlayerScore> allPlayers = new List<PlayerScore>();


    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            allPlayers.Remove(this);
        }
    }


    public void AddScore(int amount)
    {
        if (IsHost)
        {
            scoreHost += amount;
            allPlayers[1].scoreHost = scoreHost;
            Debug.Log("score Host : " + scoreHost);
        }
        else
        {
            scoreClient += amount;
            allPlayers[0].scoreClient = scoreClient;
            Debug.Log("score Client : " + scoreClient);
        }
    }


    private void Update()
    {
        if (allPlayers.Count >= 2)
        {
            if (IsHost)
            {
                scoreClient = allPlayers[1].scoreClient;
            }
            else
            {
                scoreHost = allPlayers[0].scoreHost;
            }
        }
        scoreTextHost.text = scoreHost.ToString();
        scoreTextClient.text = scoreClient.ToString();
    }
    public override void OnNetworkSpawn()
    {
        allPlayers.Add(this);
    }
}
