using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            //Counter is empty
            if (player.HasKitchenObject()) {
                //Player holding something
                player.GetKitchenObject().SetKitchenObjectParent(this);
            } else {
                //Player has nothing
            }
        } else {
            if(player.HasKitchenObject()) {
                //Player holding something
            } else {
                //Player is not holding anything
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

}
