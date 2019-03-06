﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arm : Weapon
{
    [Header("Combat Data")]
    [SerializeField] float knockback = 50;
    [SerializeField] float damage = 5;
    [SerializeField] float stun = 0.125f;



    [Header("Sound")]
    [SerializeField] float collideSoundVolume = 0.65f;
    [SerializeField] float collidePitchMinimum = 0.95f;
    [SerializeField] float collidePitchMaximum = 1.05f;
    [SerializeField] string punchSoundEffect;

    AudioPlayer m_audioPlayer;

    void Awake()
    {
        m_audioPlayer = GetComponentInChildren<AudioPlayer>();
        weaponRange = .75f;
        m_audioPlayer.addSFX(punchSoundEffect);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IDamageable damageableComponent = other.GetComponent<IDamageable>();
        if (damageableComponent != null)
        {
            damageableComponent.Damage(damage, stun, knockback * (Vector2)(other.transform.position - transform.position));
        }

        if (other.CompareTag("Player") || other.CompareTag("Enemy")) //So it registers a hit and plays sounds only when hitting enemies or players
        {
            m_audioPlayer.playSFX(punchSoundEffect, collideSoundVolume, collidePitchMinimum, collidePitchMaximum);
        }
    }
}
