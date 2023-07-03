using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour {
    private void Start() {
        KitchenGameMultiplayer.Instance.OnTyingToJoinGame += KitchenGameMultiplayer_OnTyingToJoinGame;
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame += KitchenGameMuliplayer_OnFailedToJoinGame;
        Hide();
    }

    private void KitchenGameMuliplayer_OnFailedToJoinGame(object sender, System.EventArgs e) {
        Hide();
    }

    private void KitchenGameMultiplayer_OnTyingToJoinGame(object sender, System.EventArgs e) {
        Show();
    }

    private void Show() {
        gameObject.SetActive(true);
    }
    private void Hide() {
        gameObject.SetActive(false);
    }
    private void OnDestroy() {
        KitchenGameMultiplayer.Instance.OnTyingToJoinGame -= KitchenGameMultiplayer_OnTyingToJoinGame;
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= KitchenGameMuliplayer_OnFailedToJoinGame;
    }
}
