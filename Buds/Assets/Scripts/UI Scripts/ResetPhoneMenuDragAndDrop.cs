using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPhoneMenuDragAndDrop : MonoBehaviour
{
    DragAndDrop[] dragAndDrops;

    // Start is called before the first frame update
    void Start()
    {
        dragAndDrops = GetComponentsInChildren<DragAndDrop>(true);
    }

    public void ResetPositions() {
        for (int i = 0; i < dragAndDrops.Length; i ++) {
            dragAndDrops[i].SetTarget();
        }
    }
}
