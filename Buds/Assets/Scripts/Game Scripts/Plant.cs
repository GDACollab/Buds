using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** <summary>Keeps track of the data about a plant as it grows,
 * and updates that data when time passes or the plant is watered.</summary>
 *
 * This includes species-sepcific data, that doesn't change during gameplay,
 * and the plant's current status moment-to-moment.</summary>
 */

public class Plant: MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        daysToNextStage = daysBetweenStages;
    }

    // Fully replentishes the plant's hydration level
    public void Water() {
        daysToNextWatering = daysOfWaterNeeded;
        hasEnoughWater = true;
    }

    // Simulates plant's growth and water use for specified number of days
    public void PassTime(int days) {
        for (int i = 0; i < days; i++) {
            Grow();
            UseWater();
        }
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

    // Uses up one day of the plant's hydration level and sets it to parched if
    // it runs out of water
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
}
