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
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    //Player Holding plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectsSO())) {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());                        
                    }
                }
                else {
                    //Player holding not plate, something else
                    if (GetKitchenObject().TryGetPlate(out plateKitchenObject)) {
                        //Plate is on the counter
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectsSO())) {
                            KitchenObject.DestroyKitchenObject(player.GetKitchenObject());                            
                        }
                    }
                }
            } else {
                //Player is not holding anything
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

}
