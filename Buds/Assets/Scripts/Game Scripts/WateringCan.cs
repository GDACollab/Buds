using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** <summary>
 * Connects the watering can's drag-and-drop behavior and the plant script.
 * </summary>
 */

public class WateringCan : MonoBehaviour, IDraggable
{

    private Quaternion initialRotation;

    private void Start() {
        initialRotation = transform.rotation;
    }

    /// <summary>
    /// Waters the plant the watering can is dropped onto if there is one
    /// </summary>
    public void Drop(GameObject onto) {
        transform.Rotate(new Vector3(0.0f, 0.0f, 60.0f));

        Plant plantToWater = onto.GetComponent<PlantSpot>().currentFlower;
        if (plantToWater != null) {
            plantToWater.StartWatering();
        }
    }

    public void Lift(GameObject from) {
        transform.rotation = initialRotation;

        Plant plantToWater = from.GetComponent<PlantSpot>().currentFlower;
        if (plantToWater != null) {
            plantToWater.StopWatering();
        }
    }
}
