using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public interface IKitchenObjectParent {
    public Transform GetKitchenObjectFollowTransform();
    
    public void SetKitchenObject(KitchenObject kitchenObject);

    public KitchenObject GetKitchenObject();
    public void CLearKitchenObject();
    public bool HasKitchenObject();
    public NetworkObject GetNetworkObject();
}
