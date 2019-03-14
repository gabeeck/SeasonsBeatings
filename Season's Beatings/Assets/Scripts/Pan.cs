﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pan : Weapon, IDropable
{
    [Header("Weapon Fields")]
    public new GameObject pickupVersion = null;
    public int panDurability = 15;
    public int panMaxDurability = 15;
    private bool broken = false;

    [Header("Battle Stats")]
    [SerializeField] float attackDuration = .9f;
    [SerializeField] public bool attacking = false;
    [SerializeField] public bool attackReset = false;
    [SerializeField] float knockback = 75;
    [SerializeField] float damage = 34;
    [SerializeField] float stun = 0.6f;

    float attackTime = 0;

    [Header("Sound")]
    [SerializeField] string pickupFileName = null;
    [SerializeField] float pickupVolume = 0.65f;
    [SerializeField] float pickupPitchMinimum = 0.95f;
    [SerializeField] float pickupPitchMaximum = 1.05f;

    [SerializeField] string whooshFileName = null;
    [SerializeField] string whooshFileName_2 = null;
    [SerializeField] float whooshVolume = 0.65f;
    [SerializeField] float whooshPitchMinimum = 0.90f;
    [SerializeField] float whooshPitchMaximum = 1.10f;

    [SerializeField] string collideFileName = null;
    [SerializeField] float collideSoundVolume = 0.65f;
    [SerializeField] float collidePitchMinimum = 0.95f;
    [SerializeField] float collidePitchMaximum = 1.05f;


    AudioPlayer m_audioPlayer;
    public Animator m_animator = null;

    void Awake()
    {
        m_audioPlayer = GetComponentInChildren<AudioPlayer>();
        m_audioPlayer.addSFX(pickupFileName);
        m_audioPlayer.addSFX(whooshFileName);
        m_audioPlayer.addSFX(whooshFileName_2);
        m_audioPlayer.addSFX(collideFileName);

        durability = panDurability;
        maxDurability = panMaxDurability;

        m_animator = this.GetComponent<Animator>();
    }

    private void Start()
    {
        weaponRange = 1f * transform.lossyScale.x;

        if (this.transform.parent.CompareTag("PlayerTorso"))
        {
            m_audioPlayer.setSpatialBlend(0.0f);
        }

        m_audioPlayer.playSFX(pickupFileName, pickupVolume, pickupPitchMinimum, pickupPitchMaximum);
    }

    void Update()
    {
        if (attacking)
        {
            attackTime += Time.deltaTime;
            if (attackTime >= attackDuration)
            {
                attacking = false;
                attackReset = true;
                attackTime = 0;
            }
        }
    }

    override public void Attack()
    {

        if (!attacking)
        {
            attacking = true;
            attackReset = false;
            attackTime = 0;

            m_animator.Play("PanSwing", 0, 0.0f);

            int random = Random.Range(0, 2);
            if (random == 0)
            {
                m_audioPlayer.playSFX(whooshFileName, whooshVolume, whooshPitchMinimum, whooshPitchMaximum);
            }
            else
            {
                m_audioPlayer.playSFX(whooshFileName_2, whooshVolume, whooshPitchMinimum, whooshPitchMaximum);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!attackReset)
        {
            IDamageable damageableComponent = other.GetComponent<IDamageable>();
            if (damageableComponent != null)
            {
                damageableComponent.Damage(damage, stun, knockback * (Vector2)(other.transform.position - transform.position));
                durability--;
                if (durability == 0) broken = true;
                //attacking = false;
                attackReset = true;
            }

            if (other.CompareTag("Player") || other.CompareTag("Enemy")) //So it registers a hit and plays sounds only when hitting enemies or players
            {
                m_audioPlayer.playSFX(collideFileName, collideSoundVolume, collidePitchMinimum, collidePitchMaximum);
            }
        }
    }

    public void Drop()
    {
        Destroy(this.gameObject);
        Instantiate(pickupVersion, PlayerController.instance.transform.position, Quaternion.identity);
    }

    public override GameObject GetPickupVersion()
    {
        return this.pickupVersion;
    }

    public override bool checkDestroy()
    {
        return broken;
    }
}
