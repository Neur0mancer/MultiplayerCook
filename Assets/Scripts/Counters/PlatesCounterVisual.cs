using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour
{
    [SerializeField] private PlatesCounter plateCounter;
    [SerializeField] private Transform counterTopPoint;
    [SerializeField] private Transform plateVisualPrefab;
    private List<GameObject> platesVisualGameObject;

    private void Awake() {
        platesVisualGameObject = new List<GameObject>();
    }
    private void Start() {
        plateCounter.OnPlateSpawned += PlateCounter_OnPlateSpawned;
        plateCounter.OnPlateRemoved += PlateCounter_OnPlateRemoved;
    }

    private void PlateCounter_OnPlateRemoved(object sender, System.EventArgs e) {
        GameObject plateGameObject = platesVisualGameObject[platesVisualGameObject.Count - 1];
        platesVisualGameObject.Remove(plateGameObject);
        Destroy(plateGameObject);
    }

    private void PlateCounter_OnPlateSpawned(object sender, System.EventArgs e) {
        Transform plateVisualTransform =  Instantiate(plateVisualPrefab, counterTopPoint);
        float plateOffsetY = 0.1f;
        plateVisualTransform.localPosition = new Vector3(0, plateOffsetY * platesVisualGameObject.Count, 0);
        platesVisualGameObject.Add(plateVisualTransform.gameObject);
    }

    
}
