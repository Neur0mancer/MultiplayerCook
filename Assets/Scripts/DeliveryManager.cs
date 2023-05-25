using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeliveryManager : NetworkBehaviour {

    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;
    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeListSO recipeListSO;
    
    private List<RecipeSO> waitingRecipeSOList;
    private float spawnRecipeTimer = 4f;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipesMax = 4;
    private int successfulRecipeAmount = 0;

    private void Awake() {
        Instance = this;
        waitingRecipeSOList = new List<RecipeSO>();
    }
    private void Update() {
        if (!IsServer) { return; }
        spawnRecipeTimer -= Time.deltaTime;
        if(spawnRecipeTimer < 0 ) {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (GameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipesMax) {
                int waitingRecipeSOIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);
                
                SpawnNewWaitinRecipeClientRpc(waitingRecipeSOIndex);                           
            }
        }
    }
    [ClientRpc]
    private void SpawnNewWaitinRecipeClientRpc(int waitingRecipeSOIndex) {
        RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[waitingRecipeSOIndex];
        waitingRecipeSOList.Add(waitingRecipeSO);
        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject) {
        for (int i = 0; i < waitingRecipeSOList.Count; i++) {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];
            if (waitingRecipeSO.kitchenObjectSOList.Count != plateKitchenObject.GetKitchenObjectSOList().Count) {
                //Does not have the same number of ingridients
                continue;
            }
                bool plateContentsMatchRecipe = DoPlateContetntsMatchRecipe(plateKitchenObject, waitingRecipeSO);
                
                if (plateContentsMatchRecipe) {
                //Correct recipe delivered
                DeliverCorrectRecipeServerRpc(i);
                    return;
                }            
        }
        //No matches
        DeliverIncorrectRecipeServerRpc();

    }
    private bool DoPlateContetntsMatchRecipe(PlateKitchenObject plateKitchenObject, RecipeSO recipeSO) {
        foreach (KitchenObjectSO recipeKitchenObjectSO in recipeSO.kitchenObjectSOList) {
            bool ingridientFound = DoesPlateHaveIngridient(plateKitchenObject, recipeKitchenObjectSO);
            if(!ingridientFound) {
                return false;
            }
        }
        return true;

        }
    private bool DoesPlateHaveIngridient(PlateKitchenObject plateKitchenObject, KitchenObjectSO ingridientKitchenObjectSO) {
        foreach( KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList()) {
            //Check all ingridients on plate
            if(plateKitchenObjectSO == ingridientKitchenObjectSO) {
                return true;
            }
        }
        return false;
    }
    [ServerRpc(RequireOwnership = false)]
    private void DeliverIncorrectRecipeServerRpc() {
        DeliverIncorrectRecipeClientRpc();
    }
    [ClientRpc]
    private void DeliverIncorrectRecipeClientRpc() {
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverCorrectRecipeServerRpc(int waitingRecipeSOListIndex) {
        DeliverCorrectRecipeClientRpc(waitingRecipeSOListIndex);
    }
    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(int waitingRecipeSOListIndex) {
        successfulRecipeAmount++;
        waitingRecipeSOList.RemoveAt(waitingRecipeSOListIndex);
        OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
        OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
    }
    public List<RecipeSO> GetWaitingRecipeSOList() {
        return waitingRecipeSOList;
    }
    public int GetSuccessfulRecipeAmount() {
        return successfulRecipeAmount;
    }
}
