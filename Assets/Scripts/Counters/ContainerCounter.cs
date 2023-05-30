using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContainerCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabbedObject;
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    
    public override void Interact(Player player) {
        if (!player.HasKitchenObject()) { //Player is not carrying anything
            KitchenObject.SpawnKinchenObject(kitchenObjectSO, player);
            IntercatLogicServerRpc();


        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void IntercatLogicServerRpc() {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc() {
        OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
    }

}
