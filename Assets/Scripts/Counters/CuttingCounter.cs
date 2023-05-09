using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
    public static event EventHandler OnAnyCut;

    new public static void ResetStaticData() {
        OnAnyCut = null;
    }

    public event EventHandler<IHasProgress.OnProgressChangedEvenArgs> OnProgressChanged;
    
    public event EventHandler OnCut;
    [SerializeField] private CuttingRecipeSO[] cuttingRecipesSOArray;

    private int cuttingProgress;
    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            //Counter is empty
            if (player.HasKitchenObject()) {
                //Player holding something
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectsSO())) {
                    //Player carrying something that can be cut
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    cuttingProgress = 0;
                    CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectsSO());
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEvenArgs {
                        progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
                    });
                }
            }
            else {
                //Player has nothing
            }
        }
        else {
            if (player.HasKitchenObject()) {
                //Player holding something
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectsSO())) {
                        GetKitchenObject().DestroySelf();
                    }

                }
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
            cuttingProgress++;
            OnCut?.Invoke(this, EventArgs.Empty);
            OnAnyCut?.Invoke(this, EventArgs.Empty);
            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectsSO());
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEvenArgs {
                progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
            });
            if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax) {
                KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectsSO());
                GetKitchenObject().DestroySelf();
                KitchenObject.SpawnKinchenObject(outputKitchenObjectSO, this);
            }
        }
    }
    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputkitchenObjectSO) {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputkitchenObjectSO);
        if(cuttingRecipeSO != null) {
            return cuttingRecipeSO.output;
        }
        else {
            return null;
        }        
    }
    private bool HasRecipeWithInput(KitchenObjectSO inputkitchenObjectSO) {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputkitchenObjectSO);
        return cuttingRecipeSO != null;
    }

    private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO) {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipesSOArray) {
            if (cuttingRecipeSO.input == inputKitchenObjectSO) {
                return cuttingRecipeSO;
            }
        }
        return null;
    }
}
