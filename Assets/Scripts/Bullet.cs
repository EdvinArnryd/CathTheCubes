using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RequestPlayerHitServerRpc(other.GetComponent<NetworkObject>().NetworkObjectId);
        }

        if (other.CompareTag("Wall"))
        {
            RequestBulletHitServerRpc();
        }
    }

    [ServerRpc]
    private void RequestPlayerHitServerRpc(ulong playerNetworkObjectId)
    {
        if (IsServer)
        {
            NetworkObject playerNetworkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerNetworkObjectId];
            
            Debug.Log("Player hit! Client ID: " + playerNetworkObject.OwnerClientId);
            
            NetworkObject.Despawn();
        }
    }
    
    [ServerRpc]
    private void RequestBulletHitServerRpc()
    {
        if (IsServer)
        {
            NetworkObject.Despawn();
        }
    }
}
