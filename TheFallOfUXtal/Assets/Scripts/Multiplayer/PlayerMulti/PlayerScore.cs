using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerScore : NetworkBehaviour
{
    public TMP_Text scoreText;
    // Server-controlled, initially only owner can read

    public static List<PlayerScore> allPlayers = new List<PlayerScore>();


    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            allPlayers.Remove(this);
        }
    }
    public NetworkVariable<int> score = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Owner,
        NetworkVariableWritePermission.Server
    );

    public void AddScore(int amount)
    {
        if (IsServer)
        {
            score.Value += amount;
            Debug.Log("score : " + score.Value);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            allPlayers.Add(this);
        }
        if (IsOwner)
        {
            score.OnValueChanged += (oldVal, newVal) => UpdateUI(newVal);
            UpdateUI(score.Value);
        }
    }

    private void UpdateUI(int value)
    {
        scoreText.text = value.ToString();
        // Hook this to your HUD
        Debug.Log($"My Score: {value}");
    }
}
