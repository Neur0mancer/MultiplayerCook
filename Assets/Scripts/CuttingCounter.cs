using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter
{
    [SerializeField] private CuttingRecipeSO[] cuttingRecipesSOArray;
    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            //Counter is empty
            if (player.HasKitchenObject()) {
                //Player holding something
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectsSO())) {
                    //Player carrying something that can be cut
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                }
            }
            else {
                //Player has nothing
            }
        }
        else {
            if (player.HasKitchenObject()) {
                //Player holding something
            }
            else {
                //Player is not holding anything
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
    public override void InteractAlternate(Player player) {
        if(HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectsSO())) {
            //There is an object on counter and it can be cut
            KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectsSO());
            GetKitchenObject().DestroySelf();
            KitchenObject.SpawnKinchenObject(outputKitchenObjectSO, this);
        }
    }
    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputkitchenObjectSO) {
        foreach(CuttingRecipeSO cuttingRecipeSO in cuttingRecipesSOArray) {
            if(cuttingRecipeSO.input ==  inputkitchenObjectSO) {
                return cuttingRecipeSO.output;
            }
        }
        return null;
    }
    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO) {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipesSOArray) {
            if (cuttingRecipeSO.input == inputKitchenObjectSO) {
                return true;
            }
        }
        return false;
    }
}
