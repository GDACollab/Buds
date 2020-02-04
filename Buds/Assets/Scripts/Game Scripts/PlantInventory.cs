﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** <summary>A holder for plants that are not being grown in the cemetery.</summary>
 */

public class PlantInventory : MonoBehaviour
{
    [Tooltip("The plants that are currently in the inventory")]
    public GameObject[] plants;

    [Tooltip("The plant prefab that should be placed in the inventory")]
    public GameObject plantPrefab;

    [Tooltip("The distance between each plant in the inventory")]
    public float displayDistance;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < plants.Length; i++) {
            plants[i] = Instantiate(plantPrefab, transform);
            plants[i].transform.localPosition =
                new Vector3(i * displayDistance,
                    plants[i].transform.localPosition.y,
                    plants[i].transform.localPosition.z);
        }

        transform.position =
            new Vector3(transform.position.x - (displayDistance * ((plants.Length - 1) / 2.0f)),
                transform.position.y,
                transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
