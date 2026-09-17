using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public Mesh bulletMesh;
    public int attackCooldown = 0;
    // moving speed
    [SerializeField] private float speed = 4;

    public Material dissolveMaterial;
    public Mesh[] particleMesh;
    public Material bulletMaterial;

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
            Ctrl.Move(transform.forward * speed * Time.deltaTime);
        }
        else
        {
            //attack
            if (attackCooldown > 240)
            {            
                rangedAttack();
                attackCooldown = 0;
            }
            else
            {
                attackCooldown++;
            }
        }

    }

    private void rangedAttack()
    {
        GameObject bullet = new GameObject();
        bullet.transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
        bullet.transform.rotation = transform.rotation;
        bullet.transform.localScale *= 0.25f;

        MeshFilter bulletMeshFilter = bullet.AddComponent<MeshFilter>();
        bulletMeshFilter.mesh = bulletMesh;
        
        MeshRenderer bulletMeshRenderer = bullet.AddComponent<MeshRenderer>();
        bulletMeshRenderer.material = bulletMaterial;

        SphereCollider bulletCollider = bullet.AddComponent<SphereCollider>();
        bulletCollider.isTrigger = true;

        bullet.AddComponent<Bullet>();

        Destroy(bullet, 5f);

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
        player.GetComponent<Movement>().score++;

    }
}