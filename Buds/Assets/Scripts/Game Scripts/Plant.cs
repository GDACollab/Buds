using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** <summary>
 * Keeps track of the data about a plant as it grows,
 * and updates that data when time passes or the plant is watered.
 * </summary>
 *
 * This includes species-sepcific data, that doesn't change during gameplay,
 * and the plant's current status moment-to-moment.
 */

public class Plant: MonoBehaviour, IDraggable
{
    [Header("Species-Specific Data")]

    [Tooltip("The amount of sunlight the plant needs")]
    public float sunlightNeeded;

    [Tooltip("The maximum number of days until the plant needs to be watered")]
    public float daysOfWaterNeeded;

    [Tooltip("The number of days between each growth stage of the plant")]
    public float daysBetweenStages;


    [Header("Current Plant Status")]

    [Tooltip("Whether the plant has enough sunlight")]
    public bool hasEnoughSun;

    [Tooltip("Whether the plant needs to be watered")]
    public bool hasEnoughWater;

    [Tooltip("The number of days until the plant needs water")]
    public float daysToNextWatering;

    [Tooltip("How much the plant has grown")]
    public LifeStage growthStage = LifeStage.Seed;

    [Tooltip("The number of days the plant needs to grow to the next stage")]
    public float daysToNextStage;

    public enum LifeStage {
        Seed, Seedling, Young, Mature, Flowering
    }

    private IEnumerator beingWatered;

    private SpriteRenderer soil;
    private ParticleSystem sparkles;

    private Color[] soilColors = {
        new Color(0.78f, 0.62f, 0.44f),
        new Color(0.50f, 0.35f, 0.14f),
    };

    void Awake()
    {
        daysToNextStage = daysBetweenStages;

        soil = transform.GetChild(0).GetComponent<SpriteRenderer>();
        sparkles = GetComponentInChildren<ParticleSystem>();

        beingWatered = WateringCoroutine();
    }

    /// <summary>
    /// Fully refills the plant's hydration level.
    /// </summary>
    public void Water() {
        daysToNextWatering = daysOfWaterNeeded;
        hasEnoughWater = true;

        UpdateAppearance();
    }

    public void StartWatering() {
        StartCoroutine(beingWatered);
    }

    public IEnumerator WateringCoroutine() {
        while (daysToNextWatering <= daysOfWaterNeeded) {
            yield return null;
            daysToNextWatering += 0.005f;

            if (daysToNextWatering > daysOfWaterNeeded) {
                daysToNextWatering = daysOfWaterNeeded;
                hasEnoughWater = true;
            }

            UpdateAppearance();
        }
    }

    public void StopWatering() {
        StopCoroutine(beingWatered);
    }

    /// <summary>
    /// Simulates plant's growth and water use for specified number of days.
    /// </summary>
    public void PassTime(int days) {
        for (int i = 0; i < days; i++) {
            Grow();
            UseWater();
        }

        UpdateAppearance();
    }

    /// <summary>
    /// Updates the sun level when the plant is relocated.
    /// </summary>
    public void Drop(GameObject onto) {
        hasEnoughSun = onto.GetComponent<FlowerPot>().sunlightLevel >= sunlightNeeded;

        UpdateAppearance();
    }

    public void Lift(GameObject from) {
        // Not applicable
    }

    // Private methods

    // Causes plant to grow one day if it is in good condition
    private void Grow() {
        if (hasEnoughSun && hasEnoughWater &&
                growthStage != LifeStage.Flowering) {

            daysToNextStage--;

            if (daysToNextStage <= 0) {
                growthStage++;
                daysToNextStage = daysBetweenStages;
            }
        }
    }

    // Uses up one day of the plant's hydration level and sets hasEnoughWater to
    // false if it runs out of water
    private void UseWater() {
        daysToNextWatering--;

        if (daysToNextWatering <= 0) {
            hasEnoughWater = false;
            daysToNextWatering = 0;
        }
        else {
            hasEnoughWater = true;
        }
    }

    // Updates soil color and sparkles based on whether the plant has enough
    // water and sunlight
    private void UpdateAppearance() {
        soil.color = Color.Lerp(soilColors[0], soilColors[1], daysToNextWatering / daysOfWaterNeeded);

        if (hasEnoughSun && hasEnoughWater) {
            sparkles.Play();
        }
        else {
            sparkles.Stop();
        }
    }
}
