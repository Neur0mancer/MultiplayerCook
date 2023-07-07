using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour {
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private TextMeshProUGUI lobyyNameText;
    [SerializeField] private TextMeshProUGUI lobyyCodeText;

    private void Awake() {
        mainMenuButton.onClick.AddListener(() => {
            NetworkManager.Singleton.Shutdown(); //Shutdown connection before loading Main menu
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        readyButton.onClick.AddListener(() => {
            CharacterSelectReady.Instance.SetPlayerReady();
        });
    }
    private void Start() {
        Lobby lobby = KitchenGameLobby.Instance.GetLobby();
        lobyyNameText.text = "Lobby Name: " +  lobby.Name;
        lobyyCodeText.text = "Lobby Code: " + lobby.LobbyCode;
    }
}
