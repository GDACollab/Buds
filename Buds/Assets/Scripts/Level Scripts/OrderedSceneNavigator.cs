using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OrderedSceneNavigator : MonoBehaviour
{
    public int[] buildIndexes;
    public int endOfDayIndex;

    public GameObject[] scheduleItems;

    public Animator phoneAnim;

    public Button confirmButton;

    public Image fadeOutUIImage;
    public enum FadeDirection { In, Out }


    private SortedList<float, int> sceneOrder;
    private int numCompleted;
    private bool menuEnabled;
    private System.DateTime date;
    private readonly float fadeSpeed = 0.8f;

    
    private float fadeStartValue;
    private float fadeEndValue;
    private bool fadeCompleted;
    private bool loading;
    private bool fadeStarted;
    private SpriteRenderer spriteRenderer;

    private void Awake() {
        Comparer<float> descendingComparer = Comparer<float>.Create((x, y) => y.CompareTo(x));

        sceneOrder = new SortedList<float, int>(3, descendingComparer);

        FixedUpdate();

        RetrieveSchedule();

        fadeOutUIImage.enabled = true;
        StartCoroutine(Fade(FadeDirection.Out));
    }

    void FixedUpdate()
    {
        sceneOrder.Clear();
        for (int i = 0; i < 3; i++) {
            if (!sceneOrder.ContainsKey(scheduleItems[i].transform.position.y)) {
                sceneOrder.Add(scheduleItems[i].transform.position.y, buildIndexes[i]);
            }
        }
        sceneOrder.Add(float.NegativeInfinity, endOfDayIndex);
    }

    public void ShowOrHideMenu() {
        phoneAnim.gameObject.SetActive(true);
        if (!menuEnabled) {
            ShowMenu();
        }
        else {
            HideMenu();
        }
    }

    public void ShowMenu() {
        phoneAnim.SetBool("finished", false);

        menuEnabled = true;
    }

    public void HideMenu() {
        phoneAnim.SetBool("finished", true);

        menuEnabled = false;
    }

    public void LoadScene() {
        FadeLoadScene();
    }

    public void Reset() {
        SceneManager.LoadScene(0);
    }

    public void SetSchedule() {
        PersistentData.instance.StoreData("todaysSchedule", sceneOrder.Values);
        PersistentData.instance.StoreData("numCompleted", (numCompleted + 1) % 3);

        if (numCompleted == 0) {
            PersistentData.instance.StoreData("date", date.AddDays(1));
        }
    }

    public void RetrieveSchedule() {
        if (!PersistentData.instance.ContainsKey("numCompleted")) {
            PersistentData.instance.StoreData("numCompleted", 0);

            //Vector3 tempPos = scheduleItems[0].transform.position;
            //scheduleItems[0].transform.position = scheduleItems[1].transform.position;
            //scheduleItems[1].transform.position = tempPos;
            FixedUpdate();

            PersistentData.instance.StoreData("todaysSchedule", sceneOrder.Values);
            date = System.DateTime.Today - System.TimeSpan.FromDays(1);
            PersistentData.instance.StoreData("date", date);
        }
        numCompleted = (int)PersistentData.instance.ReadData("numCompleted");
        date = (System.DateTime)PersistentData.instance.ReadData("date");

        if (PersistentData.instance.ContainsKey("todaysSchedule")) {

            IList<int> newScheduleOrder = (IList<int>)PersistentData.instance.ReadData("todaysSchedule");

            for (int i = 0; i < 3; i++) {
                int newOrder = System.Array.IndexOf(buildIndexes, newScheduleOrder[i]);

                float oldX = scheduleItems[newOrder].transform.position.x;
                float newY = sceneOrder.Keys[i];

                scheduleItems[newOrder].transform.position = new Vector3(oldX, newY, 0f);

                DragAndDrop dnd = scheduleItems[newOrder].GetComponent<DragAndDrop>();

                confirmButton.transform.parent.GetChild(1).GetComponent<Text>().text = date.ToString("M月d日(ddd)", new System.Globalization.CultureInfo("ja-JP"));

                switch (numCompleted)
                {
                    case 0:
                        confirmButton.transform.GetChild(0).GetComponent<Text>().text = "Start a New Day";
                        
                        confirmButton.transform.parent.GetChild(2).GetComponent<Text>().text = "18:00";

                        break;
                    case 1:
                        if (i == 0)
                        {
                            dnd.enabled = false;
                            scheduleItems[newOrder].transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
                        }
                        else
                        {
                            dnd.maxY = (dnd.maxY + dnd.minY) / 2;
                        }

                        confirmButton.transform.parent.GetChild(2).GetComponent<Text>().text = "10:00";

                        break;
                    case 2:
                        dnd.enabled = false;
                        if (i == 0 || i == 1)
                        {
                            scheduleItems[newOrder].transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
                        }

                        confirmButton.transform.GetChild(0).GetComponent<Text>().text = "OK";
                        confirmButton.transform.parent.GetChild(2).GetComponent<Text>().text = "14:00";

                        break;
                        // case 3 is for if there is a "Start of Day" scene like at MC's bedroom or something
                    //case 3:
                    //    dnd.enabled = false;
                    //    scheduleItems[newOrder].transform.GetChild(1).GetChild(1).gameObject.SetActive(true);

                    //    confirmButton.transform.GetChild(0).GetComponent<Text>().text = "Enter the Void";
                    //    confirmButton.transform.parent.GetChild(2).GetComponent<Text>().text = "18:00";

                    //    break;
                }
            }
        }
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
        spriteRenderer = sr;
        StartCoroutine(Fade(FadeDirection.Out, sr));
    }

    // Fade in a SpriteRenderer as it appears
    public void FadeAppear(SpriteRenderer sr) {
        spriteRenderer = sr;
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
            while (fadeStartValue <= fadeEndValue) {
                if (sr == null)
                    SetTransparencyImage(FadeDirection.In);
                else
                    SetTransparencySR(FadeDirection.In);
                yield return null;
            }
        }

        // Load next scene once fade is complete if in a load transition
        if (sr == null && loading && !fadeCompleted) {
            SetSchedule();
            int sceneIndex = sceneOrder.Values[numCompleted];
            Debug.Log("Loading scene at index " + sceneIndex);
            StartCoroutine(FinishLoadScene(sceneIndex));
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
        spriteRenderer.color = new Color(
            spriteRenderer.color.r,
            spriteRenderer.color.g,
            spriteRenderer.color.b,
            fadeStartValue
            );
        if (fadeDirection == FadeDirection.Out)
            fadeStartValue -= Time.deltaTime / fadeSpeed;
        else
            fadeStartValue += Time.deltaTime / fadeSpeed;
    }
}

