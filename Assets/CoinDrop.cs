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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.rotation = Random.rotation;
        moveDirection = Vector3.forward;
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        if (!grounded)
        {
            moveDirection.y += gravity * Time.deltaTime;
            transform.Translate(moveDirection * 3 * Time.deltaTime, Space.World);
            Ray ray = new Ray(this.transform.position + Vector3.up * 0.1f, Vector3.down);
            float range = 0.2f;
            grounded = Physics.Raycast(ray, range);
            
        }
        if (distanceToPlayer < 5)
        {
            pickup = true;
        }
        if(pickup)
        {
            transform.LookAt(player.transform);
            transform.Translate(Vector3.forward * 5 * Time.deltaTime);
            if (Vector3.Distance(player.transform.position, transform.position) < 0.2f)
            {
                player.GetComponent<Movement>().score++;
                Destroy(gameObject);
            }
        }
    }
}
