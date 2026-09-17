using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public bool loadScene = false;
    public Scene scene1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        if(GUI.Button(new Rect(Screen.width / 2, Screen.height / 2, 100, 30), "Try Again"))
        {
        SceneManager.LoadScene("scene1", LoadSceneMode.Single);
        }
    }
}
