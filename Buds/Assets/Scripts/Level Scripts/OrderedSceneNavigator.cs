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

    private SortedList<float, int> sceneOrder;

    private int numCompleted;

    bool menuEnabled;

    private void Awake() {
        Comparer<float> descendingComparer = Comparer<float>.Create((x, y) => y.CompareTo(x));

        sceneOrder = new SortedList<float, int>(3, descendingComparer);

        FixedUpdate();

        RetrieveSchedule();
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
        SetSchedule();
        int sceneIndex = sceneOrder.Values[numCompleted];
        SceneManager.LoadSceneAsync(sceneIndex);
    }

    public void Reset() {
        SceneManager.LoadScene(0);
    }

    public void SetSchedule() {
        PersistentData.instance.StoreData("todaysSchedule", sceneOrder.Values);
        PersistentData.instance.StoreData("numCompleted", (numCompleted + 1) % 4);
    }

    public void RetrieveSchedule() {
        if (!PersistentData.instance.ContainsKey("numCompleted")) {
            PersistentData.instance.StoreData("numCompleted", 0);
        }
        numCompleted = (int)PersistentData.instance.ReadData("numCompleted");

        if (PersistentData.instance.ContainsKey("todaysSchedule")) {

            IList<int> newScheduleOrder = (IList<int>)PersistentData.instance.ReadData("todaysSchedule");

            for (int i = 0; i < 3; i++) {
                int newOrder = System.Array.IndexOf(buildIndexes, newScheduleOrder[i]);

                float oldX = scheduleItems[newOrder].transform.position.x;
                float newY = sceneOrder.Keys[i];

                scheduleItems[newOrder].transform.position = new Vector3(oldX, newY, 0f);

                DragAndDrop dnd = scheduleItems[newOrder].GetComponent<DragAndDrop>();
                
                switch (numCompleted)
                {
                    case 0:
                        confirmButton.transform.GetChild(0).GetComponent<Text>().text = "Confirm Schedule";
                        confirmButton.transform.parent.GetChild(2).GetComponent<Text>().text = "8:00";

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
                    case 3:
                        dnd.enabled = false;
                        scheduleItems[newOrder].transform.GetChild(1).GetChild(1).gameObject.SetActive(true);

                        confirmButton.transform.parent.GetChild(2).GetComponent<Text>().text = "18:00";

                        break;
                }
            }
        }
    }

}
