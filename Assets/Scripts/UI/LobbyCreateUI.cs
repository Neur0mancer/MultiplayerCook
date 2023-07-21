using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour {
    [SerializeField] private Button createPublicButton;
    [SerializeField] private Button createPrivateButton;
    [SerializeField] private Button closeLobbyCreateButton;
    [SerializeField] private TMP_InputField lobbyNameInputField;

    private void Awake() {
        createPublicButton.onClick.AddListener(() => {
            KitchenGameLobby.Instance.CreateLobby(lobbyNameInputField.text, false);
        });
        createPrivateButton.onClick.AddListener(() => {
            KitchenGameLobby.Instance.CreateLobby(lobbyNameInputField.text, true);
        });
        closeLobbyCreateButton.onClick.AddListener(() => {
            Hide();
        });       

        }
    private void Start() {
        Hide();
    }
    public void Show() {
        gameObject.SetActive(true);
        createPublicButton.Select();
    }
    private void Hide() {
        gameObject.SetActive(false);
    }

}
