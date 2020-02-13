using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** <summary>
 * Connects the watering can's drag-and-drop behavior and the plant script.
 * </summary>
 */

public class WateringCan : MonoBehaviour, IDraggable
{

    public float tiltTime;
    private float currentTiltTime;

    private Quaternion initialRotation;
    private Quaternion wateringRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 60.0f));
    private Quaternion negativeWateringRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));

    private bool watering;

    private void Start() {
        initialRotation = transform.rotation;
    }

    /// <summary>
    /// Waters the plant the watering can is dropped onto if there is one
    /// </summary>
    public void Drop(GameObject onto) {
        //transform.rotation = wateringRotation;


        Plant plantToWater = onto.GetComponent<PlantSpot>().currentFlower;
        if (plantToWater != null) {
            plantToWater.StartWatering();
            watering = true;

            StopCoroutine("RotateGradually");
            StartCoroutine("RotateGradually");
        }
    }

    public void Lift(GameObject from) {
        //transform.rotation = initialRotation;


        if (watering) {
            Plant plantToWater = from.GetComponent<PlantSpot>().currentFlower;
            if (plantToWater != null) {
                plantToWater.StopWatering();
                watering = false;

                StopCoroutine("RotateGradually");
                StartCoroutine("RotateGradually");
            }
        }
    }

    private IEnumerator RotateGradually() {
        Quaternion sourceRotation = !watering ? wateringRotation : initialRotation;
        Quaternion targetRotation = watering ? wateringRotation : initialRotation;
        Quaternion negativeSourceRotation = !watering ? negativeWateringRotation : initialRotation;
        Quaternion negativeTargetRotation = watering ? negativeWateringRotation : initialRotation;

        currentTiltTime = (currentTiltTime > 0.0f) ? tiltTime - currentTiltTime : currentTiltTime;

        while (currentTiltTime <= tiltTime) {
            yield return null;

            currentTiltTime += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(sourceRotation, targetRotation, currentTiltTime / tiltTime);
            transform.GetChild(0).rotation = Quaternion.Slerp(negativeSourceRotation, negativeTargetRotation, currentTiltTime / tiltTime);
        }
    }
}
