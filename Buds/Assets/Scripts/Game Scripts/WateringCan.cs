using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WateringCan : MonoBehaviour, IDraggable
{
    public void Drop(GameObject onto) {
        Plant plantToWater = onto.GetComponent<FlowerPot>().currentFlower;
        if (plantToWater != null) {
            plantToWater.Water();
        }
    }
}
