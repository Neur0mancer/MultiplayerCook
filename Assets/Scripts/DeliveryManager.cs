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

            if (waitingRecipeSOList.Count < waitingRecipesMax) {
                RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)];
                waitingRecipeSOList.Add(waitingRecipeSO);
                OnRecipeSpawned?.Invoke(this, EventArgs.Empty);                
            }
        }
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject) {
        for ( int i = 0; i < waitingRecipesMax; i++) {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];
            if(waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count) {
                //Has the same number of ingridients
                bool plateContentsMatchRecipe = true;
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList) {
                    //Cycle all ingredieants in recipe
                    bool ingredientFound = false;
                    foreach (KitchenObjectSO platekitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList()) {
                        //Cycle all ingedients on a plate
                        if(platekitchenObjectSO == recipeKitchenObjectSO) {
                            ingredientFound = true;
                            break;
                        }
                    }
                    if(!ingredientFound) {
                        //Recipe ingedient was not found on plate
                        plateContentsMatchRecipe = false;
                    }
                }
                if (plateContentsMatchRecipe) {
                    //Correct recipe delivered
                    successfulRecipeAmount++;
                    waitingRecipeSOList.RemoveAt(i);
                    OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                    OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }
            
        }
        //No matches
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
        Debug.Log("Not correct recipe");

    }

    public List<RecipeSO> GetWaitingRecipeSOList() {
        return waitingRecipeSOList;
    }
    public int GetSuccessfulRecipeAmount() {
        return successfulRecipeAmount;
    }
}
