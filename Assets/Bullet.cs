using System.Collections.Generic;
using Mono.Cecil.Cil;
using NUnit.Framework;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    SphereCollider bulletCollider;
    List<string> colliderTags = new List<string> {"Floor", "Wall", "Player"};
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        bulletCollider = gameObject.GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * 0.1f);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (colliderTags.Contains(collider.gameObject.tag))
        {
            collider.gameObject.SendMessage("TakeDamage", 10);
            Destroy(gameObject);
        }
    }
}
