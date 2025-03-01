using Unity.Netcode;
using UnityEngine;

public class PlayerDebug : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        Debug.Log($"Player Spawned - IsOwner: {IsOwner}, IsClient: {IsClient}"); 
    }
}

