using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderedSceneNavigator : MonoBehaviour
{
    public int[] buildIndexes;
    public GameObject[] scheduleItems;

    public Animator phoneAnim;

    private int[] sceneOrder;
    bool disabled;

    float top = 1.874481f;
    float middle = -0.64019f;
    float bottom = -3.1174f;

    private void Start() {
        sceneOrder = new int[] {0, 1, 2};
    }

    void FixedUpdate()
    {
        
        for (int i = 0; i < sceneOrder.Length; i++) {
            buildIndexes[i] = -1;
            if (Mathf.Abs(scheduleItems[i].transform.position.y - top) < 0.01f) {
                sceneOrder[0] = buildIndexes[i];
            }
            else if (Mathf.Abs(scheduleItems[i].transform.position.y - middle) < 0.01f) {
                sceneOrder[1] = buildIndexes[i];
            }
            else if (Mathf.Abs(scheduleItems[i].transform.position.y - bottom) < 0.01f) {
                sceneOrder[2] = buildIndexes[i];
            }
        }

        disabled = buildIndexes[0] == -1 || buildIndexes[1] == -1 || buildIndexes[2] == -1;
    }

    public void ShowMenu() {
        phoneAnim.SetBool("finished", false);
        phoneAnim.gameObject.SetActive(true);
    }

    public void HideMenu() {
        phoneAnim.SetBool("finished", true);
    }
}
