using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** <summary>
 * Connects the watering can's drag-and-drop behavior and the plant script.
 * </summary>
 */

public class WateringCan : MonoBehaviour, IDraggable
{

    private IEnumerator rotateGraduallyIE;
    private IEnumerator rotateBackGraduallyIE;
    private Quaternion initialRotation;

    private void Start() {
        rotateGraduallyIE = RotateGradually();
        rotateBackGraduallyIE = RotateBackGradually();
        initialRotation = transform.rotation;

    }

    /// <summary>
    /// Waters the plant the watering can is dropped onto if there is one
    /// </summary>
    public void Drop(GameObject onto) {
        transform.Rotate(new Vector3(0.0f, 0.0f, 60.0f));
        StopCoroutine(rotateBackGraduallyIE);
        StartCoroutine(rotateGraduallyIE);

        Plant plantToWater = onto.GetComponent<PlantSpot>().currentFlower;
        if (plantToWater != null) {
            plantToWater.StartWatering();
        }
    }

    public void Lift(GameObject from) {
        transform.rotation = initialRotation;
        //StopCoroutine(rotateGraduallyIE);
        //StartCoroutine(rotateBackGraduallyIE);

        Plant plantToWater = from.GetComponent<PlantSpot>().currentFlower;
        if (plantToWater != null) {
            plantToWater.StopWatering();
        }
    }

    private IEnumerator RotateGradually() {
        while (transform.rotation.eulerAngles.z < 60.0f && transform.rotation.eulerAngles.z >= -180.0f) {
            yield return null;
            transform.Rotate(new Vector3(0.0f, 0.0f, 100.0f * Time.deltaTime));
        }
    }

    private IEnumerator RotateBackGradually() {
        while (transform.rotation.eulerAngles.z > 0.0f && transform.rotation.eulerAngles.z <= 180.0f) {
            yield return null;
            transform.Rotate(new Vector3(0.0f, 0.0f, -100.0f * Time.deltaTime));
        }
    }
}
