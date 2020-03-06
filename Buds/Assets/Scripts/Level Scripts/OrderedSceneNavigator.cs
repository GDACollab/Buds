using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OrderedSceneNavigator : MonoBehaviour
{
    public int[] buildIndexes;
    public GameObject[] scheduleItems;

    public Animator phoneAnim;

    public Button confirmButton;

    private SortedList<float, int> sceneOrder;

    bool menuEnabled;

    private void Start() {
        Comparer<float> descendingComparer = Comparer<float>.Create((x, y) => y.CompareTo(x));

        sceneOrder = new SortedList<float, int>(3, descendingComparer);
    }

    void FixedUpdate()
    {
        sceneOrder.Clear();
        for (int i = 0; i < 3; i++) {
            sceneOrder.Add(scheduleItems[i].transform.position.y, buildIndexes[i]);
        }
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
        int sceneIndex = sceneOrder.Values[0];
        sceneOrder.RemoveAt(sceneOrder.IndexOfValue(sceneIndex));
        SceneManager.LoadSceneAsync(sceneIndex);
    }

}
