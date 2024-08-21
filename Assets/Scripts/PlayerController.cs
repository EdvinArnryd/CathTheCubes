using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    private float moveSpeed;

    private void Update()
    {
        if (!IsOwner) return;

        Vector3 moveDir = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) moveDir.y = +1f;
        if (Input.GetKey(KeyCode.S)) moveDir.y = -1f;
        if (Input.GetKey(KeyCode.A)) moveDir.x = -1f;
        if (Input.GetKey(KeyCode.D)) moveDir.x = +1f;

        // Send movement request to the server with client time delta
        RequestMoveServerRpc(moveDir, Time.deltaTime);
    }

    [ServerRpc]
    private void RequestMoveServerRpc(Vector3 moveDir, float deltaTime)
    {
        // Server-side authoritative movement
        MovePlayer(moveDir, deltaTime);

        // Optionally, broadcast the movement to all clients
        // MovePlayerClientRpc(moveDir, deltaTime);
    }

    private void MovePlayer(Vector3 moveDir, float deltaTime)
    {
        // Apply movement on the server
        transform.position += moveDir.normalized * moveSpeed * deltaTime;
    }

    // Uncomment this if you want to broadcast the new position to all clients
    /*
    [ClientRpc]
    private void MovePlayerClientRpc(Vector3 moveDir, float deltaTime)
    {
        // Sync position on clients
        if (!IsOwner)
        {
            transform.position += moveDir.normalized * moveSpeed * deltaTime;
        }
    }
    */
}