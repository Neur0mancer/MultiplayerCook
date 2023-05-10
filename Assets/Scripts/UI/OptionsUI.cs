using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour {

    [SerializeField] private Button soundEffectsButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private TextMeshProUGUI soundEffectText;
    [SerializeField] private TextMeshProUGUI musicText;


    private void Awake() {
        soundEffectsButton.onClick.AddListener(() => {
            SoundManager.Instance.ChangeVolume();
            UpdateVisual();
        });
        musicButton.onClick.AddListener(() => {
            MusicManager.Instance.ChangeVolume();
            UpdateVisual();
        });
    }
    private void Start() {
        UpdateVisual();
    }
    private void UpdateVisual() {
        musicText.text = "MUSIC: " + Mathf.Round(MusicManager.Instance.GetVolume() * 10f);
        soundEffectText.text = "SOUND EFFECTS: " + Mathf.Round(SoundManager.Instance.GetVolume() * 10f);
    }
}
