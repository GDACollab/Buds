using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OrderedSceneNavigator : MonoBehaviour
{
    // The physical UI elements in the phone menu for each of the three locations;
    // the y-positions of the elements are used to determine the player's desired order
    public GameObject[] scheduleItems = null; 

    public Animator phoneAnim;

    public Button confirmButton;

    public Image fadeOutUIImage;
    public Text newDayText;

    public enum FadeDirection { In, Out }

    // Build index variables. Current and next scene
    private int manualNextSceneIndex = 0;
    private int activeSceneIndex;
    private bool useManualNextScene;

    // Build index constants. Used to select next scene
    private const int mainMenuIndex = 0;
    private const int introductionIndex = 1;
    private readonly int[] buildIndexes = { 3, 2, 4 }; // indexes for the main three scenes: RF, flower, GB
    private const int endOfDayIndex = 5; // unused for now
    private const int conclusionIndex = 6;
    private const int creditsIndex = 7;
    private const int dreamSequenceIndex = 8;

    private SortedList<float, int> sceneOrder;
    private int numCompleted; // essentially time of day. 0 = evening, 1 = morning, 2 = afternoon
    private bool menuEnabled;
    private System.DateTime date;
    private readonly float fadeSpeed = 0.8f;

    
    private float fadeStartValue;
    private float fadeEndValue;
    private bool fadeCompleted;
    private bool loading;
    private bool fadeStarted;
    private SpriteRenderer fadeOutSpriteRenderer;

    private void Awake() {
        activeSceneIndex = SceneManager.GetActiveScene().buildIndex;

        Comparer<float> descendingComparer =
            Comparer<float>.Create((x, y) => y.CompareTo(x));

        sceneOrder = new SortedList<float, int>(3, descendingComparer);

        FixedUpdate();

        if (!useManualNextScene && scheduleItems.Length > 0) {
            RetrieveSchedule();
        }

        // Sets screen fade image to fully opaque
        fadeOutUIImage.enabled = true;
        fadeOutUIImage.color = new Color(
            fadeOutUIImage.color.r,
            fadeOutUIImage.color.g,
            fadeOutUIImage.color.b,
            1
            );

        // Shows large date at beginning of the day
        if (numCompleted == 1 && activeSceneIndex != dreamSequenceIndex) {
            
            newDayText.gameObject.SetActive(true);
            newDayText.text =
                date.ToString("M月d日(ddd)",
                              new System.Globalization.CultureInfo("ja-JP")) +
                              "\n<size=40>" + date.ToString("dddd, MMMM d") + "</size>";

            StartCoroutine(DisplayNewDayText());
        }
        // Fades in immediately
        else {
            StartCoroutine(Fade(FadeDirection.Out));
        }
    }

    void FixedUpdate()
    {
        // Checks whether phone draggable elements have changed order
        sceneOrder.Clear();
        if (scheduleItems.Length > 0) {
            
            for (int i = 0; i < 3; i++) {
                if (!sceneOrder.ContainsKey(scheduleItems[i].transform.position.y)) {
                    sceneOrder.Add(scheduleItems[i].transform.position.y, buildIndexes[i]);
                }
            }
        }
        else {
            sceneOrder.Add(0, 5);
        }
    }

    // Toggle to open/close phone
    public void ShowOrHideMenu() {
        phoneAnim.gameObject.SetActive(true);
        if (!menuEnabled) {
            ShowMenu();
        }
        else {
            HideMenu();
        }
    }

    // Opens phone
    public bool ShowMenu() {
        // Check for special condition: reached end of game
        if (PersistentData.instance.ContainsKey("$visited_RF") && PersistentData.instance.ContainsKey("$visited_GB")) {
            Yarn.Value visitedRF = (Yarn.Value)PersistentData.instance.ReadData("$visited_RF");
            Yarn.Value visitedGB = (Yarn.Value)PersistentData.instance.ReadData("$visited_GB");

            // Set next destination to conclusion scene if completed all dialog and plants fully upgraded
            if (visitedRF.AsNumber > 4 && visitedGB.AsNumber > 4) {
                confirmButton.transform.GetChild(0).GetComponent<Text>().text = "Reflect";
                manualNextSceneIndex = conclusionIndex;
                useManualNextScene = true;

                // Clear all persistent data except volume
                float volume = (float)PersistentData.instance.ReadData("Volume");
                PersistentData.instance.Clear();
                PersistentData.instance.StoreData("Volume", volume);

            }
        }

        if (!phoneAnim.gameObject.activeInHierarchy) return false;

        phoneAnim.SetBool("finished", false);

        menuEnabled = true;

        return true;
    }

    // Closes phone
    public void HideMenu() {
        phoneAnim.SetBool("finished", true);

        menuEnabled = false;
    }

    // Loads the next scene as determined by the internal schedule
    public void LoadScene() {
        FadeLoadScene();
    }

    public void ToCredits()
    {
        manualNextSceneIndex = creditsIndex;
        useManualNextScene = true;
        Destroy(PersistentData.instance);

        FadeLoadScene();
    }

    public void ToMainMenu() {
        manualNextSceneIndex = mainMenuIndex;
        useManualNextScene = true;
        Destroy(PersistentData.instance);

        FadeLoadScene();
    }

    // Loads a specific scene. May cause issues with progress tracking, use with caution
    public void ToIndexedSceen(int index)
    {
        manualNextSceneIndex = index;
        useManualNextScene = true;

        // Initializes numCompleted when loading into first plant care scene
        if (activeSceneIndex == introductionIndex) {
            PersistentData.instance.StoreData("numCompleted", 1);
        }

        FadeLoadScene();
    }

    // Updates persistent data schedule to carry over scene ordering choice
    // Also advances day count every third scene
    private void SetSchedule() {
        // introduction and dream sequence do not advance numCompleted
        if (activeSceneIndex != introductionIndex && activeSceneIndex != dreamSequenceIndex) {
            PersistentData.instance.StoreData("todaysSchedule", sceneOrder.Values);
            PersistentData.instance.StoreData("numCompleted", (numCompleted + 1) % 3);

            if (numCompleted == 0) {
                PersistentData.instance.StoreData("date", date.AddDays(7));
            }

            
        }
        if (activeSceneIndex == dreamSequenceIndex) {
            numCompleted = 0;
        }
    }

    // Syncs up phone schedule with the one from persistent data
    public void RetrieveSchedule() {
        // Phone text areas that may need to be updated
        Text confirmButtonText = confirmButton.GetComponentInChildren<Text>();
        Text dateText = confirmButton.transform.parent.GetChild(1).GetComponent<Text>();
        Text timeText = confirmButton.transform.parent.GetChild(2).GetComponent<Text>();

        // Don't need to change the starting phone schedule if on intro
        if (activeSceneIndex == introductionIndex) return;

        // Initializes persistent data for numCompleted on first load
        if (!PersistentData.instance.ContainsKey("numCompleted")) {
            PersistentData.instance.StoreData("numCompleted", 0);

            FixedUpdate();

            // Starts the date cylce on March 14, 2020. Why? No particular reason;
            // it's in the spring, at least
            PersistentData.instance.StoreData("todaysSchedule", sceneOrder.Values);
            date = new System.DateTime(2020, 3, 14);
            PersistentData.instance.StoreData("date", date);
        }

        numCompleted = (int)PersistentData.instance.ReadData("numCompleted");
        date = (System.DateTime)PersistentData.instance.ReadData("date");

        // Don't need to update schedule if on first plant care scene
        if (!PersistentData.instance.ContainsKey("todaysSchedule")) return;

        IList<int> newScheduleOrder =
            (IList<int>)PersistentData.instance.ReadData("todaysSchedule");

        // Updates phone state and appearance to match persistent data
        for (int i = 0; i < 3; i++) {
            int newOrder =
                System.Array.IndexOf(buildIndexes, newScheduleOrder[i]);
            float oldX = scheduleItems[newOrder].transform.position.x;
            float newY = sceneOrder.Keys[i];
            GameObject checkmarkIcon =
                scheduleItems[newOrder].transform.GetChild(1).GetChild(1).gameObject;
            DragAndDrop dragAndDrop =
                scheduleItems[newOrder].GetComponent<DragAndDrop>();

            scheduleItems[newOrder].transform.position = new Vector3(oldX, newY, 0f);

            dateText.text =
                date.ToString("M月d日(ddd)", new System.Globalization.CultureInfo("ja-JP"));

            switch (numCompleted)
            {
                // Evening. Can configure all schedule items
                case 0:
                    if (confirmButtonText.text != "Start") {
                        confirmButtonText.text = "Dream";
                    }

                    timeText.text = "18:00";
                    break;
                // Morning, cannot set morning item but can switch the other two
                case 1:
                    if (i == 0)
                    {
                        // Prevents drag-and-drop on first item
                        dragAndDrop.enabled = false;
                        // Turns on checkmark icon for first item
                        checkmarkIcon.SetActive(true);
                    }
                    else
                    {
                        // Limits drag-and-drop range for lower two items
                        dragAndDrop.maxY = (dragAndDrop.maxY + dragAndDrop.minY) / 2;
                    }

                    timeText.text = "10:00";

                    break;
                // Afternoon, cannot adjust schedule at all
                case 2:
                    // Prevents drag-and-drop on all three items
                    dragAndDrop.enabled = false;
                    if (i == 0 || i == 1)
                    {
                        // Turns on checkmark icon for first and second items
                        checkmarkIcon.SetActive(true);
                    }

                    confirmButtonText.text = "OK";
                    timeText.text = "14:00";

                    break;
            }
        }

        // Set next destination to dream scene if at end of day
        if (numCompleted == 0) {
            
            manualNextSceneIndex = dreamSequenceIndex;
            useManualNextScene = true;
        }
    }

    // Shows large date at beginning of the day for 2 seconds
    private IEnumerator DisplayNewDayText() {
        yield return new WaitForSeconds(3);

        newDayText.gameObject.SetActive(false);
        StartCoroutine(Fade(FadeDirection.Out));
    }

    // Fade in a screen transition image, then load next scene
    public void FadeLoadScene() {

        // Start coroutine Fade, which in turn starts FinishLoadScene
        loading = true;
        fadeCompleted = false;
        if (!fadeStarted)
            StartCoroutine(Fade(FadeDirection.In));
    }

    // Fade out a SpriteRenderer as it disappears
    public void FadeAway(SpriteRenderer sr) {
        fadeOutSpriteRenderer = sr;
        StartCoroutine(Fade(FadeDirection.Out, sr));
    }

    // Fade in a SpriteRenderer as it appears
    public void FadeAppear(SpriteRenderer sr) {
        fadeOutSpriteRenderer = sr;
        sr.enabled = true;
        print(sr);
        StartCoroutine(Fade(FadeDirection.In, sr));
    }

    // Coroutine to fade an Image or SpriteRenderer
    private IEnumerator Fade(FadeDirection direction, SpriteRenderer sr = null) {
        // Set start and end values if just beginning to fade
        if (!fadeStarted) {
            if (direction == FadeDirection.Out)
                fadeStartValue = 1;
            else
                fadeStartValue = 0;
            fadeEndValue = 1 - fadeStartValue;
        }
        fadeStarted = true;

        // Continue to fade in or out until done
        if (direction == FadeDirection.Out) {
            while (fadeStartValue >= fadeEndValue) {
                if (sr == null)
                    SetTransparencyImage(FadeDirection.Out);
                else
                    SetTransparencySR(FadeDirection.Out);
                yield return null;
            }

            // Disable Image/SR once it has disappeared 
            if (sr == null)
                fadeOutUIImage.enabled = false;
            else
                sr.enabled = false;
        }
        else {
            // Enable Image/SR before it appears
            if (sr == null)
                fadeOutUIImage.enabled = true;
            else
                sr.enabled = true;
            while (fadeStartValue <= fadeEndValue + 0.05f) {
                if (sr == null)
                    SetTransparencyImage(FadeDirection.In);
                else
                    SetTransparencySR(FadeDirection.In);
                yield return null;
            }
        }

        // Load next scene once fade is complete if in a load transition
        if (sr == null && loading && !fadeCompleted) {
            // If we have a schedule, update it to be ready for next scene
            if ((!useManualNextScene && scheduleItems.Length > 0) || manualNextSceneIndex == dreamSequenceIndex) {
                SetSchedule();
            }

            // If we have a schedule, load next scene from it. Otherwise, load the manual override scene
            bool haveSchedule =
                (!useManualNextScene && scheduleItems.Length > 0) &&
                activeSceneIndex != mainMenuIndex;
            int nextSceneIndex = haveSchedule ? sceneOrder.Values[numCompleted] : manualNextSceneIndex;

            StartCoroutine(FinishLoadScene(nextSceneIndex));
        }
            
        fadeCompleted = true;
        fadeStarted = false;
    }

    // Coroutine to load a scene
    IEnumerator FinishLoadScene(int sceneIndex) {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);

        while (!asyncLoad.isDone)
            yield return null;
    }

    // Helper function for setting transparency on an Image (UI element)
    private void SetTransparencyImage(FadeDirection fadeDirection) {
        fadeOutUIImage.color = new Color(
            fadeOutUIImage.color.r,
            fadeOutUIImage.color.g,
            fadeOutUIImage.color.b,
            fadeStartValue
            );
        if (fadeDirection == FadeDirection.Out)
            fadeStartValue -= Time.deltaTime / fadeSpeed;
        else
            fadeStartValue += Time.deltaTime / fadeSpeed;
    }

    // Helper function for setting transparency on a SpriteRenderer
    private void SetTransparencySR(FadeDirection fadeDirection) {
        fadeOutSpriteRenderer.color = new Color(
            fadeOutSpriteRenderer.color.r,
            fadeOutSpriteRenderer.color.g,
            fadeOutSpriteRenderer.color.b,
            fadeStartValue
            );
        if (fadeDirection == FadeDirection.Out)
            fadeStartValue -= Time.deltaTime / fadeSpeed;
        else
            fadeStartValue += Time.deltaTime / fadeSpeed;
    }
}

