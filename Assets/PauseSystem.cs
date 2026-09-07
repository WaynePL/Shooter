using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseSystem : MonoBehaviour
{
    public bool paused = false;
    public float times;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                Time.timeScale = 1;
                paused = false;
            }
            else
            {
                Time.timeScale = 0;
                paused = true;
            }
        }
        
    }
}
