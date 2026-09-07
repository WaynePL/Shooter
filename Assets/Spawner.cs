using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public int instanceCount = 0;
    public int spawnMax = 0;
    public GameObject spawnObject, enemy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(instanceCount < spawnMax)
        {
            enemy = Instantiate(spawnObject, transform.position, transform.rotation);
            enemy.GetComponent<GhostScript>().gate = this.gameObject;
            instanceCount++;
        }

    }

}
