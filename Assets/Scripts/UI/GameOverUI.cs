using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipesDeliveredText;
    [SerializeField] private Button replayButton;

    private void Awake() {
        replayButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.GameScene);
        });
    }
    private void Start() {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        Hide();
    }
    private void Update() {
        recipesDeliveredText.text = Mathf.Ceil(DeliveryManager.Instance.GetSuccessfulRecipeAmount()).ToString();
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e) {
        if (GameManager.Instance.IsGameOver()) {
            Show();
        }
        else {
            Hide();
        }
    }
    private void Show() {
        gameObject.SetActive(true);
    }
    private void Hide() {
        gameObject.SetActive(false);
    }
}
