using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed;

    private Camera mainCamera;

    [SerializeField] private Transform gunTransform;

    [SerializeField] private GameObject bulletPrefab;

    [SerializeField] private float bulletSpeed = 5f;

    [SerializeField] private float gunDistanceFromPlayer = 1f;

    void Start()
    {
        mainCamera = Camera.main;
    }
    private void Update()
    {
        if (!IsOwner) return;

        Vector3 moveDir = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) moveDir.y = +1f;
        if (Input.GetKey(KeyCode.S)) moveDir.y = -1f;
        if (Input.GetKey(KeyCode.A)) moveDir.x = -1f;
        if (Input.GetKey(KeyCode.D)) moveDir.x = +1f;

        RequestMoveServerRpc(moveDir, Time.deltaTime);

        AimTowardsMouse();

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }
    
    private void AimTowardsMouse()
    {
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;

        Vector2 direction = (mouseWorldPosition - transform.position).normalized;

        Vector3 gunPosition = transform.position + (Vector3)direction * gunDistanceFromPlayer;

        gunTransform.position = gunPosition;

        gunTransform.right = direction;
    }

    private void Shoot()
    {
        Vector2 shootDirection = gunTransform.right;

        ShootServerRpc(gunTransform.position, gunTransform.rotation, shootDirection);
    }

    [ServerRpc]
    private void ShootServerRpc(Vector3 position, Quaternion rotation, Vector2 shootDirection)
    {
        GameObject bullet = Instantiate(bulletPrefab, position, rotation);

        NetworkObject networkObject = bullet.GetComponent<NetworkObject>();

        networkObject.Spawn();

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = shootDirection * bulletSpeed;
        
        SetBulletVelocityClientRpc(networkObject.NetworkObjectId, shootDirection * bulletSpeed);
    }
    
    [ClientRpc]
    private void SetBulletVelocityClientRpc(ulong bulletNetworkObjectId, Vector2 velocity)
    {
        NetworkObject bulletNetworkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[bulletNetworkObjectId];
        Rigidbody2D rb = bulletNetworkObject.GetComponent<Rigidbody2D>();

        rb.velocity = velocity;
    }

    [ServerRpc]
    private void RequestMoveServerRpc(Vector3 moveDir, float deltaTime)
    {
        MovePlayer(moveDir, deltaTime);
    }

    private void MovePlayer(Vector3 moveDir, float deltaTime)
    {
        transform.position += moveDir.normalized * moveSpeed * deltaTime;
    }
    
}