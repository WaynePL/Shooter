using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public int instanceCount = 0;
    public int maxOnScreen = 0;
    public int totalSpawns = 0;
    public int spawnMax = 0;
    public GameObject spawnObject;
    public GameObject enemy;
    private int counter;

    // Start is called before the first frame update
    void Start()
    {
        counter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(instanceCount < maxOnScreen && counter > 60)
        {
            enemy = Instantiate(spawnObject, transform.position, transform.rotation);
            enemy.name = gameObject.name + " Ghost " + totalSpawns;
            enemy.GetComponent<GhostScript>().gate = this.gameObject;
            instanceCount++;
            totalSpawns++;
            counter = 0;
        }

        if(totalSpawns == spawnMax)
        {
            Destroy(gameObject);
        }

        counter++;
    }

}
