using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    [SerializeField] private KitchenObjectsSO kitchenObjectsSO;
    private IKitchenObjectParent kitchenObjectParent;
    public KitchenObjectsSO GetKitchenObjectsSO() { 
        return kitchenObjectsSO; 
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
}
