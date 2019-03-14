﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon, IDropable
{
    [Header("Weapon Fields")]
    public GameObject gunPickupVersion = null;
    public int gunDurability = 15;
    public int gunMaxDurability = 15;
    private bool firedShot = false;

    //ADD SOUND VARIABLES

    //OTHER VARIABLES
    [Header("STATS")]
    [SerializeField] GameObject bulletSpawn = null;
    [SerializeField] GameObject bullet = null;
    [SerializeField] float rateOfFire = 1f;
    

    [Header("Audio")]
    [SerializeField] string pickupFileName = null;
    [SerializeField] float pickupVolume = 0.65f;
    [SerializeField] float pickupPitchMinimum = 0.95f;
    [SerializeField] float pickupPitchMaximum = 1.05f;

    [SerializeField] string fireFileName = null;
    [SerializeField] float fireVolume = 0.65f;
    [SerializeField] float firePitchMinimum = 0.90f;
    [SerializeField] float firePitchMaximum = 1.10f;

    AudioPlayer m_audioPlayer;
    float attackTime = 1f;
    PlayerController player;

    private void Awake()
    {
        player = PlayerController.instance;
        m_audioPlayer = GetComponentInChildren<AudioPlayer>();
        m_audioPlayer.addSFX(pickupFileName);
        m_audioPlayer.addSFX(fireFileName);

        weaponRange = 10;
        pickupVersion = gunPickupVersion;
    }

    protected void Start()
    {
        if (this.transform.parent.CompareTag("PlayerTorso"))
        {
            m_audioPlayer.setSpatialBlend(0.0f);
        }

        m_audioPlayer.playSFX(pickupFileName, pickupVolume, pickupPitchMinimum, pickupPitchMaximum);
    }

    private void Update()
    {
        attackTime += Time.deltaTime;
    }

    public override void Attack()
    {
        if (attackTime >= rateOfFire)
        {
            attackTime = 0;
            GameObject newBullet = Instantiate(bullet, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
            firedShot = true;
            m_audioPlayer.playSFX(fireFileName, fireVolume, firePitchMinimum, firePitchMaximum);
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
        if (firedShot)
        {
            durability--;
            firedShot = false;
            if (durability == 0) return true;
            return false;
        }
        return false;
    }
}
