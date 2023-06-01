using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenObject : NetworkBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private IKitchenObjectParent kitchenObjectParent;
    private FollowTransform followTransform;

    protected virtual void Awake() {
        followTransform =  GetComponent<FollowTransform>();
    }
    public KitchenObjectSO GetKitchenObjectsSO() { 
        return kitchenObjectSO; 
    }
    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent) { //Moving objects between counters
        SetKitchenObjectParentServerRpc(kitchenObjectParent.GetNetworkObject());


    }
    [ServerRpc(RequireOwnership = false)]

    private void SetKitchenObjectParentServerRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference) {
        SetKitchenObjectParentClientRpc(kitchenObjectParentNetworkObjectReference);
    }

    [ClientRpc]
    private void SetKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference) {
        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject); //Geting parent object from network object
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

        if (this.kitchenObjectParent != null) {                    //Clear old counter
            this.kitchenObjectParent.CLearKitchenObject();
        }
        this.kitchenObjectParent = kitchenObjectParent;                   //Assign to new counter
        if (kitchenObjectParent.HasKitchenObject()) {
            Debug.LogError("Parent object already has kitchen object");
        }
        kitchenObjectParent.SetKitchenObject(this);

        followTransform.SetTargetTransform(kitchenObjectParent.GetKitchenObjectFollowTransform());  //Updating visual
    }
    public IKitchenObjectParent GetKitchenObjectParent() { 
        return kitchenObjectParent; 
    }
    public void DestroySelf() {
        
        Destroy(gameObject);
    }
    public void ClearKitchenObjectOnParent() {
        kitchenObjectParent.CLearKitchenObject();
    }
    public static void SpawnKinchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent) {
        KitchenGameMultiplayer.Instance.SpawnKinchenObject(kitchenObjectSO, kitchenObjectParent);  //Spawn kitchen object on server
    }
    public static void DestroyKitchenObject(KitchenObject kitchenObject) {
        KitchenGameMultiplayer.Instance.DestroyKitchenObject(kitchenObject);
    }
    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject) {
        if(this is PlateKitchenObject) {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        } else { 
            plateKitchenObject = null;
            return false; 
        }
    }
}
