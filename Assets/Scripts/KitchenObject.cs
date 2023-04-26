using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    private IKitchenObjectParent kitchenObjectParent;
    public KitchenObjectSO GetKitchenObjectsSO() { 
        return kitchenObjectSO; 
    }
    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent) { //Moving objects between counters
        if( this.kitchenObjectParent != null) {                    //Clear old counter
            this.kitchenObjectParent.CLearKitchenObject();
        }
        this.kitchenObjectParent = kitchenObjectParent;                   //Assign to new counter
        if (kitchenObjectParent.HasKitchenObject()) {
            Debug.LogError("Parent object already has kitchen object");
        }
        kitchenObjectParent.SetKitchenObject(this);
        transform.parent = kitchenObjectParent.GetKitchenObjectFollowTransform(); //Updating visual
        transform.localPosition = Vector3.zero;
    }
    public IKitchenObjectParent GetKitchenObjectParent() { 
        return kitchenObjectParent; 
    }
    public void DestroySelf() {
        kitchenObjectParent.CLearKitchenObject();
        Destroy(gameObject);
    }
    public static KitchenObject SpawnKinchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent) {
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
        return kitchenObject;
    }
}
