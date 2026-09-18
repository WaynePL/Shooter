using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.InputSystem.Interactions;
using JetBrains.Annotations;
using Unity.VisualScripting;
using System.Linq;
using System;
using System.Security.Cryptography;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.UI;

public class Movement : MonoBehaviour
{
    public float moveSpeed;
    public float rotateSpeed;
    private float jumpSpeed;
    public float jumpConstant;
    public float gravityConstant;
    private float gravity;
    private Vector2 m_Rotation;
    private Vector2 lookDirection;
    public Vector2 moveDirection;
    public bool jumpState;
    public GameObject floor;
    public float distanceToFloor;

    public bool menuState;
    public bool loading = true;
    public Vector3 mousePos;
    public bool mouseOutOfBounds;

    public CharacterController characterController;

    GUIStyle onScreenStyle;

    public AudioSource audioSource;

    public Material particle;

    public bool dash;

    public int dashCooldown = 0;
    public int score = 0;
    public int health = 0;
    private int tookDamage;
    public Texture damageTexture;
    public Texture dashTexture;
    public Scene gameoverScene;
    public Gun currentGun;
    public List<Gun> guns;
    public int currentGunNumber = 0;
    public bool autofire = false;
    public int autofireCooldown = 0;

    // Start is called before the first frame update
    void Start()
    {
        onScreenStyle = new GUIStyle() { fontSize = 30};
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        gravity = gravityConstant;
        loading = false;
        currentGun = guns[currentGunNumber];
    }

    // Update is called once per frame
    public void Update()
    {
        if (health <= 0)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Confined;
            SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
        }

        mousePos = Input.mousePosition;
        mouseOutOfBounds = mousePos.x < 0 || mousePos.x > Screen.width || mousePos.y < 0 || mousePos.y > Screen.height;
        if(!mouseOutOfBounds) 
        {
            // Update orientation first, then move. Otherwise move orientation will lag
            // behind by one frame.
            if(!menuState)
            {
                Look(lookDirection);
                Move(moveDirection);
            }
        }
        if (dashCooldown > 0)
        {
            dashCooldown--;
        }
        if (autofireCooldown == 0 && autofire)
        {
            Shoot();
            autofireCooldown = 20;
        }
        else if (autofire)
        {
            autofireCooldown--;
        }        
    }    

    public void FixedUpdate()
    {
        if (!mouseOutOfBounds)
        {
            if(!menuState)
            {

            }
        }
    }
    public void OnGUI()
    {
        if(menuState)
        {
            GUI.backgroundColor = Color.white;
            GUI.Box(new Rect(100, 100, 200, 100), "Paused", onScreenStyle);
        }
        if (tookDamage > 0)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), damageTexture, ScaleMode.StretchToFill);
            tookDamage--;
        }
        GUI.Box(new Rect(100, 50, 200, 100), "Score: " + score, onScreenStyle);
        GUI.Box(new Rect(100, 25, 200, 100), "Health: " + health, onScreenStyle);
    }
    public void OnScroll(InputAction.CallbackContext context)
    {
        Transform currentGunTransform = currentGun.gunObject.transform;
        currentGunTransform.localPosition = new Vector3(currentGunTransform.localPosition.x, currentGunTransform.localPosition.y, currentGun.zPosition - 1);
        currentGun.gunObject.SetActive(false);
        if (context.ReadValue<float>() > 0)
        {
            if (currentGunNumber == guns.Count() - 1)
            {
                currentGunNumber = 0;
            }
            else
            {
                currentGunNumber++;
            }
        }
        else
        {
            if (currentGunNumber == 0)
            {
                currentGunNumber = guns.Count() - 1;
            }
            else
            {
                currentGunNumber--;
            }
        }
        currentGun = guns[currentGunNumber];
        currentGun.gunObject.SetActive(true);
        currentGun.gunObject.transform.localPosition = new Vector3(currentGunTransform.localPosition.x, currentGunTransform.localPosition.y, currentGun.zPosition);
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookDirection = context.ReadValue<Vector2>();
    }
    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started && !menuState && !loading)
        {
            if (currentGun.firetype == Firetype.single)
            {
                Shoot();
            }
            else if (currentGun.firetype == Firetype.automatic)
            {
                autofire = true;
            }
        }
        if (context.canceled  && !menuState && !loading && currentGun.firetype == Firetype.automatic)
        {
            autofire = false;
            autofireCooldown = 0;
        }
    }

    private void Shoot()
    {
        audioSource.Play();
        Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.47F, 0)), out RaycastHit hit);



        if (hit.transform)
        {//Impact point sparks
            GameObject hitObject = hit.transform.gameObject;
            hitObject.SendMessage("TakeDamage", currentGun.damage, SendMessageOptions.DontRequireReceiver);
            Debug.Log("Hit " + hitObject.name);
            GameObject impact = new GameObject();
            impact.transform.position = hit.point;
            var dir = (hit.point - gameObject.transform.position).normalized;
            var facing = gameObject.transform.eulerAngles;
            facing.y += 180;
            facing.x = hit.normal.y * -90;
            impact.transform.eulerAngles = facing;

            ParticleSystem impactParticle = impact.AddComponent<ParticleSystem>();
            impactParticle.Stop();
            var impactRenderer = impactParticle.GetComponent<Renderer>();
            impactRenderer.material = particle;

            var main = impactParticle.main;

            main.duration = 0.5f;
            main.startLifetime = 0.1f;
            main.startSize = 0.05f;
            main.loop = false;

            var shape = impactParticle.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 30;
            shape.radius = 0;

            ParticleSystem.EmissionModule em = impactParticle.emission;
            em.enabled = true;
            em.rateOverTime = 0;
            em.SetBursts(
                new ParticleSystem.Burst[]
                {
                    new ParticleSystem.Burst(0, 5)
                }
            );

            impactParticle.Play();

            Destroy(impact, 1f);
        }
    }

    public void OnMenu(InputAction.CallbackContext context)
    {
        if (context.started) 
        {
            if(!menuState)
            {
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                menuState = true;
            }
            else
            {
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                menuState = false;
            }
        }
        
        
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && !menuState && !loading && dashCooldown == 0)
        {
            dash = true;
        }
       
    }

    private void Move(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.01)
            return;
        var scaledMoveSpeed = moveSpeed * Time.deltaTime;
        if (dash)
        {
            scaledMoveSpeed *= 100;
            dash = false;
            dashCooldown = 100;
        }
        // For simplicity's sake, we just keep movement in a single plane here. Rotate
        // direction according to world Y rotation of player.
        var move = Quaternion.Euler(0, transform.eulerAngles.y, 0) * new Vector3(direction.x, 0, direction.y);
        characterController.Move(move * scaledMoveSpeed);
    }


    private void Look(Vector2 rotate)
    {
        if (rotate.sqrMagnitude < 0.01)
            return;
        var scaledRotateSpeed = rotateSpeed * Time.deltaTime;
        m_Rotation.y += rotate.x * scaledRotateSpeed;
        m_Rotation.x = Mathf.Clamp(m_Rotation.x - rotate.y * scaledRotateSpeed, -89, 89);
        transform.localEulerAngles = m_Rotation;
    }

    private void TakeDamage(int damage)
    {
        health -= damage;
        tookDamage = 150;
    }
}

public enum Firetype
{
    single,
    automatic
}

[System.Serializable]
public class Gun
{
    public string name;
    public float zPosition;
    public GameObject gunObject;
    public Firetype firetype;
    public int damage;
    public bool acquired;
    public Gun(string name, float zPosition, GameObject gunObject, Firetype firetype, int damage, bool acquired)
    {
        this.name = name;
        this.zPosition = zPosition;
        this.gunObject = gunObject;
        this.firetype = firetype;
        this.damage = damage;
        this.acquired = acquired;

    }
}