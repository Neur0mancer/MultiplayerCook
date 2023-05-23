using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeliveryManager : MonoBehaviour{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;
    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeListSO recipeListSO;
    
    private List<RecipeSO> waitingRecipeSOList;
    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipesMax = 4;
    private int successfulRecipeAmount = 0;

    private void Awake() {
        Instance = this;
        waitingRecipeSOList = new List<RecipeSO>();
    }
    private void Update() {
        spawnRecipeTimer -= Time.deltaTime;
        if(spawnRecipeTimer < 0 ) {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (GameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipesMax) {
                RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)];
                waitingRecipeSOList.Add(waitingRecipeSO);
                OnRecipeSpawned?.Invoke(this, EventArgs.Empty);                
            }
        }
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
                    successfulRecipeAmount++;
                    waitingRecipeSOList.RemoveAt(i);
                    OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                    OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
                    return;
                }
            
            
        }
        //No matches
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
        Debug.Log("Not correct recipe");

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

    public List<RecipeSO> GetWaitingRecipeSOList() {
        return waitingRecipeSOList;
    }
    public int GetSuccessfulRecipeAmount() {
        return successfulRecipeAmount;
    }
}
