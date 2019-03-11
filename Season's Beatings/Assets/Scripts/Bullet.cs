﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Combat Data")]
    [SerializeField] float knockback = 100;
    [SerializeField] float damage = 50;
    [SerializeField] float stun = 0.25f;

    //SOUND VARIABLES

    //OTHER VARIABLES
    float lifetime = 0.0f;

    private void Awake()
    {
        //transform.rotation.z = 0;
    }

    private void Update()
    {
        if (lifetime >= 15.0f)
            Destroy(this.gameObject);
        lifetime += Time.deltaTime;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy") || collision.collider.CompareTag("Player"))
        {
            IDamageable damageableComponent = collision.collider.GetComponent<IDamageable>();
            if (damageableComponent != null)
            {
                damageableComponent.Damage(damage, stun, knockback * (Vector2)(collision.transform.position - transform.position));
            }
            Destroy(this.gameObject);
        }
        
    }
}