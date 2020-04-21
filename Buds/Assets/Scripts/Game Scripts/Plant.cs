using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/** <summary>
 * Keeps track of the data about a plant as it grows,
 * and updates that data when time passes or the plant is watered.
 * </summary>
 *
 * <remarks>
 * This includes species-sepcific data, that doesn't change during gameplay,
 * and the plant's current status moment-to-moment.
 * </remarks>
 */

public class Plant: MonoBehaviour, IDraggable
{
    [Header("General")]
    public bool showIndicators;

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
        Seed, Sprout, FullPlant, Blossom
    }

    private string[] lifeStageStrings = { "Seed", "Sprout", "<size=16>Full</size>\nPlant", "Blossom\n<size=16>(Final)</size>" };

    private SpriteRenderer soil;
    private ParticleSystem sparkles;
    private Text growthStageText;
    private Slider waterLevelBar;
    private Image sunlightIcon;

    void Awake()
    {
        daysToNextStage = daysBetweenStages;

        soil = transform.GetChild(0).GetComponent<SpriteRenderer>();
        sparkles = GetComponentInChildren<ParticleSystem>();
        growthStageText = transform.GetChild(2).GetChild(2).GetComponent<Text>();
        waterLevelBar = transform.GetChild(2).GetChild(3).GetComponent<Slider>();
        sunlightIcon = transform.GetChild(2).GetChild(4).GetComponent<Image>();
    }

    /// <summary>
    /// Fully refills the plant's hydration level.
    /// </summary>
    public void WaterCompletely() {
        daysToNextWatering = daysOfWaterNeeded;
        hasEnoughWater = true;

        UpdateAppearance();
    }

    /// <summary>
    /// Gradually refills the plant's hydration level.
    /// </summary>
    public void StartWatering() {
        StartCoroutine("WaterGradually");
    }


    /// <summary>
    /// Stops gradual watering process.
    /// </summary>
    public void StopWatering() {
        StopCoroutine("WaterGradually");
    }

    /// <summary>
    /// Simulates plant's growth and water use for specified number of days.
    /// </summary>
    public void PassTime(int days = 1) {
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
        hasEnoughSun = onto.GetComponent<PlantSpot>().sunlightLevel >= sunlightNeeded;
        onto.GetComponent<PlantSpot>().currentFlower = this;
         
        UpdateAppearance();
    }

    public void Lift(GameObject from) {
        hasEnoughSun = false;
        from.GetComponent<PlantSpot>().currentFlower = null;

        UpdateAppearance();
    }

    // Private methods

    // Causes plant to grow one day if it is in good condition
    private void Grow() {
        if (hasEnoughSun && hasEnoughWater &&
                growthStage != LifeStage.Blossom) {

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

    // Waters plant gradually in real-time
    private IEnumerator WaterGradually() {
        while (daysToNextWatering < daysOfWaterNeeded) {
            yield return null;
            daysToNextWatering += Time.deltaTime;

            if (daysToNextWatering >= daysOfWaterNeeded) {
                daysToNextWatering = daysOfWaterNeeded;
                hasEnoughWater = true;
            }

            UpdateAppearance();
        }
    }

    // Updates soil color and sparkles based on whether the plant has enough
    // water and sunlight
    private void UpdateAppearance() {
        if (showIndicators) {
            transform.GetChild(2).gameObject.SetActive(true);
        }
        else {
            transform.GetChild(2).gameObject.SetActive(false);
        }
        

        float soilDarkness = daysToNextWatering / daysOfWaterNeeded;
        soil.color = new Color(soil.color.r, soil.color.g, soil.color.b, soilDarkness);
        growthStageText.text = lifeStageStrings[(int)growthStage];
        waterLevelBar.value = soilDarkness;

        if (hasEnoughSun) {
            sunlightIcon.color = new Color(1f, 1f, 0f);
        }
        else {
            sunlightIcon.color = new Color(0.5f, 0.5f, 0f);
        }
        

        if (hasEnoughSun && hasEnoughWater) {
            sparkles.Play();
        }
        else {
            sparkles.Stop();
        }
    }

    private void OnDisable() {
        PersistentData.instance.StoreData(gameObject.name, this);
        PersistentData.instance.StoreData($"{gameObject.name} position", transform.position);
    }

    private void OnEnable() {
        if (PersistentData.instance != null && PersistentData.instance.ContainsKey(gameObject.name)) {
            Plant previousValues = (Plant)PersistentData.instance.ReadData(gameObject.name);
            hasEnoughSun = previousValues.hasEnoughSun;
            hasEnoughWater = previousValues.hasEnoughWater;
            daysToNextWatering = previousValues.daysToNextWatering;
            growthStage = previousValues.growthStage;
            daysToNextStage = previousValues.daysToNextStage;
            transform.position = (Vector3)PersistentData.instance.ReadData($"{gameObject.name} position");
            PassTime();
        }
    }
}
