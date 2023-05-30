using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;

    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;

    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 4f;
    private int platesSpawnedAmount;
    private int platesSpawnAmountMax = 4;

    private void Update() {
        if (!IsServer) { return; }
        spawnPlateTimer += Time.deltaTime;
        if (spawnPlateTimer > spawnPlateTimerMax) {
            spawnPlateTimer = 0;
            if(GameManager.Instance.IsGamePlaying() && platesSpawnedAmount < platesSpawnAmountMax) {
                SpawnPlateServerRpc();
            }
            
        }
    }

    [ServerRpc]
    private void SpawnPlateServerRpc() {
        SpawnPlateClientRpc();
    }
    [ClientRpc]
    private void SpawnPlateClientRpc() {
        platesSpawnedAmount++;
        OnPlateSpawned?.Invoke(this, EventArgs.Empty);
    }

    public override void Interact(Player player) {
        if (!player.HasKitchenObject()) { //Player is not holding an object
            if(platesSpawnedAmount > 0) { //There are plates spawned
                
                KitchenObject.SpawnKinchenObject(plateKitchenObjectSO, player);
                IntercatLogicServerRpc();
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void IntercatLogicServerRpc() {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc() {
        platesSpawnedAmount--;
        OnPlateRemoved?.Invoke(this, EventArgs.Empty);
    }
}
