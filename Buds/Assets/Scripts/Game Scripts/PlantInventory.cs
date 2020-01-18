using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** <summary>Holds plants that are not growing right now in the cemetery</summary>
 */

public class PlantInventory : MonoBehaviour
{
    [Tooltip("The plants that are not growing right now in the cemetery")]
    public GameObject[] plants;
    public GameObject plantPrefab;

    public float displayDistance;

    private float totalDistanceSoFar;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < plants.Length; i++) {
            var newPlant = Instantiate(plantPrefab, transform);
            newPlant.transform.localPosition = new Vector3(newPlant.transform.localPosition.x + totalDistanceSoFar, newPlant.transform.localPosition.y, newPlant.transform.localPosition.z);
            totalDistanceSoFar += displayDistance;
        }
        transform.position = new Vector3(transform.position.x + (-1 * (totalDistanceSoFar - 1) / 2), transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
