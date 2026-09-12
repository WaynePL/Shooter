using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GhostScript : MonoBehaviour
{
    public Animator Anim;
    public CharacterController Ctrl;
    private Vector3 MoveDirection = Vector3.zero;
    // Cache hash values
    private static readonly int IdleState = Animator.StringToHash("Base Layer.idle");
    private static readonly int MoveState = Animator.StringToHash("Base Layer.move");
    private static readonly int SurprisedState = Animator.StringToHash("Base Layer.surprised");
    private static readonly int AttackState = Animator.StringToHash("Base Layer.attack_shift");
    private static readonly int DissolveState = Animator.StringToHash("Base Layer.dissolve");
    private static readonly int AttackTag = Animator.StringToHash("Attack");
    // dissolve
    [SerializeField] private SkinnedMeshRenderer[] MeshR;
    private float Dissolve_value = 1;
    private bool DissolveFlg = false;
    private const int maxHP = 1;
    private int HP = maxHP;
    private Text HP_text;
    public GameObject player;
    public GameObject gate;

    public int status_name;
    public float range;
    // moving speed
    [SerializeField] private float speed = 4;

    public Material dissolveMaterial;
public Mesh[] particleMesh;

    void Start()
    {
        Anim = this.GetComponent<Animator>();
        Ctrl = this.GetComponent<CharacterController>();
        player = GameObject.Find("Player");
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    void Update()
    {
        //rotate towards the player
        transform.LookAt(player.transform.position);
        GRAVITY();
        // Dissolve
        if (HP <= 0 && !DissolveFlg)
        {

            if (gate) gate.GetComponent<Spawner>().instanceCount--;
            
            GameObject dissolve = new GameObject();
            dissolve.transform.position = new Vector3(transform.localPosition.x, transform.localPosition.y + 0.5f, transform.localPosition.z);
            dissolve.transform.localScale *= 5f;
            var facing = gameObject.transform.eulerAngles;
            facing.x += 270;
            dissolve.transform.eulerAngles = facing;

            ParticleSystem dissolveParticle = dissolve.AddComponent<ParticleSystem>();
            dissolveParticle.Stop();
            var dissolveRenderer = dissolveParticle.GetComponent<Renderer>();
            dissolveRenderer.material = dissolveMaterial;
            var dissolveParticleRenderer = dissolveParticle.GetComponent<ParticleSystemRenderer>();
            dissolveParticleRenderer.renderMode = ParticleSystemRenderMode.Mesh;
            dissolveParticleRenderer.SetMeshes(particleMesh);

            var main = dissolveParticle.main;

            main.duration = 1f;
            main.startLifetime = 1f;
            main.startSize = 1f;
            main.loop = false;
            main.startSpeed = 0.1f;
            main.startSpeedMultiplier = 0.5f;
            
            

            var shape = dissolveParticle.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 70;
            shape.radius = 0;

            ParticleSystem.EmissionModule em = dissolveParticle.emission;
            em.enabled = true;
            em.rateOverTime = 0;
            em.SetBursts(
                new ParticleSystem.Burst[]
                {
                    new ParticleSystem.Burst(0, 5),
                    new ParticleSystem.Burst(0.1f, 5),
                    new ParticleSystem.Burst(0.2f, 5)
                }
            );

            dissolveParticle.Play();

            Destroy(dissolve, 1f);
            
            Destroy(gameObject);
        }
        // processing at respawn
        else if (HP == maxHP && DissolveFlg)
        {
            DissolveFlg = false;
        }

        if (Vector3.Distance(player.transform.position, transform.position) > range)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

    }

    //---------------------------------------------------------------------
    // gravity for fall of this character
    //---------------------------------------------------------------------
    private void GRAVITY ()
    {
        if(Ctrl.enabled)
        {
            if(CheckGrounded())
            {
                if(MoveDirection.y < -0.1f)
                {
                    MoveDirection.y = -0.1f;
                }
            }
            MoveDirection.y -= 0.1f;
            Ctrl.Move(MoveDirection * Time.deltaTime);
        }
    }
    //---------------------------------------------------------------------
    // whether it is grounded
    //---------------------------------------------------------------------
    private bool CheckGrounded()
    {
        if (Ctrl.isGrounded && Ctrl.enabled)
        {
            return true;
        }
        Ray ray = new Ray(this.transform.position + Vector3.up * 0.1f, Vector3.down);
        float range = 0.2f;
        return Physics.Raycast(ray, range);
    }

    public void TakeDamage(int damage)
    {
        HP = 0;


    }
}