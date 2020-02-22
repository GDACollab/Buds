using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** <summary>
 * Connects the watering can's drag-and-drop behavior and the plant script.
 * </summary>
 */

public class WateringCan : MonoBehaviour
{

    public float tiltTime;
    private float currentTiltTime;

    private Quaternion initialRotation;
    private Quaternion wateringRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 60.0f));
    private Quaternion negativeWateringRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));

    private bool watering;
    private GameObject currentPlant;
    private GameObject startShadow;
    private bool onStartShadow;

    private ParticleSystem water;

    private void Start() {
        initialRotation = transform.rotation;
        water = GetComponentInChildren<ParticleSystem>();
    }

    /// <summary>
    /// Waters the plant the watering can is dropped onto if there is one.
    /// </summary>
    public void Drop(GameObject onto) {
        Plant plantToWater = onto.GetComponent<PlantSpot>().currentFlower;
        if (plantToWater != null) {
            plantToWater.StartWatering();
        }
    }

    /// <summary>
    /// Stops watering the plant the watering can is lifted from if there is one.
    /// </summary>
    public void Lift(GameObject from) {
        if (watering) {
            Plant plantToWater = from.GetComponent<PlantSpot>().currentFlower;
            if (plantToWater != null) {
                plantToWater.StopWatering();
            }
        }
    }

    /// <summary>
    /// Tilts the transform from initialRotation to wateringRotation in tiltTime seconds.
    /// </summary>
    /// <returns></returns>
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
            transform.GetChild(1).rotation = Quaternion.Slerp(negativeSourceRotation, negativeTargetRotation, currentTiltTime / tiltTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        PlantSpot collidedPlantSpot = collision.GetComponent<PlantSpot>();
        if (collidedPlantSpot != null && collidedPlantSpot.currentFlower != null) {
            currentPlant = collision.gameObject;
            if (watering) {
                Drop(onto: currentPlant);
            }
        }

        if (startShadow == null) {
            startShadow = collision.gameObject;
        }
        if (collision.gameObject == startShadow) {
            onStartShadow = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject == currentPlant) {
            if (watering) {
                Lift(from: currentPlant);
            }
            currentPlant = null;
        }

        if (collision.gameObject == startShadow) {
            onStartShadow = false;
        }
    }

    private void OnMouseDown() {
        

        if (currentPlant != null) {
            Drop(onto: currentPlant);
        }

        if (!onStartShadow) {
            watering = true;
            water.Play();
            StopCoroutine("RotateGradually");
            StartCoroutine("RotateGradually");
        }
        
    }

    private void OnMouseUp() {
        if (!onStartShadow || transform.rotation != initialRotation) {
            watering = false;
            water.Stop();
            StopCoroutine("RotateGradually");
            StartCoroutine("RotateGradually");
        }

        if (currentPlant != null) {
            Lift(from: currentPlant);
        }
    }
}
