using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CoinDrop : MonoBehaviour
{
    public bool grounded;
    Vector3 moveDirection;
    public GameObject player;
    public float distanceToPlayer;
    public Vector3 playerLocation;
    public float gravity = -9.81f;
    bool pickup = false;
    public AudioSource pickupSound;
    private bool pickedUp;
    public TextMeshProUGUI coinTextField;
    public GameObject coinTextObject;
    public int coinCounter;
    public int coinAmount;
    public AudioSource coinLevelUp;
    public float speed = 5f;
    public Vector3 playerPosition;
    private RaycastHit hit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.rotation = Random.rotation;
        moveDirection = Vector3.forward;
        player = GameObject.Find("Player");
        coinAmount = 1;
        coinTextField = coinTextObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!grounded)
        {
            moveDirection.y += gravity * Time.deltaTime;
            transform.Translate(moveDirection * 3 * Time.deltaTime, Space.World);
            Ray ray = new Ray(this.transform.position + Vector3.up * 0.1f, Vector3.down);
            float range = 0.2f;
            if (Physics.Raycast(ray, out hit,range) && hit.transform.name == "Floor")
            {
                grounded = true;
            }
            
            
        }
        playerPosition = new Vector3(player.transform.position.x, player.transform.position.y - 1, player.transform.position.z);
        distanceToPlayer = Vector3.Distance(playerPosition, transform.position);
        if (distanceToPlayer < 5)
        {
            pickup = true;
        }
        
        if(pickup)
        {
            transform.LookAt(playerPosition);
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            coinCounter++;
            speed += 0.001f;
            if (coinCounter % 500 == 0 && !pickedUp)
            {
                coinAmount++;
                coinTextField.SetText("$" + coinAmount);
                //coinLevelUp.Play();
            }
            
            if (distanceToPlayer < 0.2f && !pickedUp)
            {
                pickupSound.Play();
                player.GetComponent<Movement>().score += coinAmount;
                Destroy(gameObject, 0.7f);
                pickedUp = true;
                gameObject.GetComponent<MeshRenderer>().enabled = false;
                coinTextField.enabled = false;
            }
        }
    }
}
