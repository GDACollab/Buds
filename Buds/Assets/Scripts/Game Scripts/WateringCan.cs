using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** <summary>
 * Connects the watering can's drag-and-drop behavior and the plant script.
 * </summary>
 */

public class WateringCan : MonoBehaviour, IDraggable
{

    /// <summary>
    /// Waters the plant the watering can is dropped onto if there is one
    /// </summary>
    public void Drop(GameObject onto) {
        Plant plantToWater = onto.GetComponent<PlantSpot>().currentFlower;
        if (plantToWater != null) {
            plantToWater.StartWatering();
        }

    }

    public void Lift(GameObject from) {
        Plant plantToWater = from.GetComponent<PlantSpot>().currentFlower;
        if (plantToWater != null) {
            plantToWater.StopWatering();
        }
    }
}
