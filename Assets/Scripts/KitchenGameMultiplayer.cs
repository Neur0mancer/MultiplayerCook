using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenGameMultiplayer : NetworkBehaviour
{
    public const int MAX_PLAYER_AMOUNT = 4;
    private const string PLAYER_PREFS_PLAYER_NAME = "PlayerName";
    public static KitchenGameMultiplayer Instance { get; private set; }

    public event EventHandler OnTyingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDataNetworkListChanged;

    [SerializeField] private KitchenObjectsListSO kitchenObjectsListSO;
    [SerializeField] private List<UnityEngine.Color> playerColorList;

    private NetworkList<PlayerData> playerDataNetworkList;
    private string playerName;
    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME, "PlayerName" + UnityEngine.Random.Range(10, 100));
        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent) {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost() {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId) {
        for (int i = 0; i < playerDataNetworkList.Count; i++) {
            PlayerData playerData = playerDataNetworkList[i];
            if(playerData.clientId == clientId) {
                //Disconnected player found
                playerDataNetworkList.RemoveAt(i);
            }
        }
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId) {
        playerDataNetworkList.Add(new PlayerData {
            clientId = clientId,
            colorId = GetFirstUnusedColorId(),
        });
        SetPlayerNameServerRpc(GetPlayerName());
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse) {
        if(SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectionScene.ToString()) {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game has already started";
            return;
        }
        if(NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT) {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is full";
            return;
        }
        connectionApprovalResponse.Approved = true;
    }

    public void StartClient() {
        OnTyingToJoinGame?.Invoke(this, EventArgs.Empty);
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Server_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_Server_OnClientConnectedCallback(ulong clientId) {
        SetPlayerNameServerRpc(GetPlayerName());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default) {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = playerDataNetworkList[playerDataIndex];
        playerData.playerName = playerName;
        playerDataNetworkList[playerDataIndex] = playerData;
    }
    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId) {
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }

    public void SpawnKinchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent) {
        SpawnKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());


    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference) {
        KitchenObjectSO kitchenObjectSO = GetKitchenObjectFromIndex(kitchenObjectSOIndex);
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
        NetworkObject kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
        kitchenObjectNetworkObject.Spawn(true);
        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject); //Geting parent object from network object
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
    } 

    public int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO) {
        return kitchenObjectsListSO.kitchenObjectSOList.IndexOf(kitchenObjectSO);
    }
    public KitchenObjectSO GetKitchenObjectFromIndex(int kitchenObjectSOIndex) {
        return kitchenObjectsListSO.kitchenObjectSOList[kitchenObjectSOIndex];
    }
    public void DestroyKitchenObject(KitchenObject kitchenObject) {
        DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
    }
    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference) {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
        ClearKitchenObjectObParentClientRpc(kitchenObjectNetworkObjectReference);
        kitchenObject.DestroySelf();
    }
    [ClientRpc]
    private void ClearKitchenObjectObParentClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference) {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
        kitchenObject.ClearKitchenObjectOnParent();
    }

    public bool IsPlayerIndexConnected(int playerIndex) {
        return playerIndex < playerDataNetworkList.Count;
    }
    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex) {
        return playerDataNetworkList[playerIndex];
    }
    public UnityEngine.Color GetPlayerColor(int colorId) {
        return playerColorList[colorId];
    }
    public PlayerData GetPlayerDataFromClientId(ulong clientId) {
        foreach(PlayerData playerData in playerDataNetworkList) {
            if(playerData.clientId == clientId) {
                return playerData;
            }
        }
        return default;
    }
    public int GetPlayerDataIndexFromClientId(ulong clientId) {
        for(int i = 0; i < playerDataNetworkList.Count; i++) {
            if (playerDataNetworkList[i].clientId == clientId) {
                return i;
            }
        }
        return -1;        
    }
    public PlayerData GetPlayerData() {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }
    public void ChangePlayerColor(int colorId) {
        ChangePlayerColorServerRpc(colorId);
    }
    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default) {
        if(!IsColorAvailable(colorId)) {
            return;
        }
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = playerDataNetworkList[playerDataIndex];
        playerData.colorId = colorId;
        playerDataNetworkList[playerDataIndex] = playerData;
    }

    private bool IsColorAvailable(int colorId) {
        foreach(PlayerData playerData in playerDataNetworkList) {
            if(playerData.colorId == colorId) {
                return false;
            }
        }
        return true;
    }
    private int GetFirstUnusedColorId() {
        for(int i = 0; playerColorList.Count > 0; i++) {
            if(IsColorAvailable(i)) {
                return i;
            }
        }
        return -1;
    }
    public void KickPlayer(ulong  clientId) {
        NetworkManager.Singleton.DisconnectClient(clientId);
        NetworkManager_Server_OnClientDisconnectCallback(clientId);
    }
    public string GetPlayerName() {
        return playerName;
    }
    public void SetPlayerName(string playerName) {
        this.playerName = playerName;
        PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME, playerName);
    }
}
