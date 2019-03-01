﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyController : MonoBehaviour, IDamageable
{
    [Header("Components")]
    [SerializeField] GameObject EnemyTorso;
    [SerializeField] Weapon weapon;
    [SerializeField] GameObject EnemyLegs;
    Rigidbody2D rigidbody2D;
    Collider2D collider2D;

    [Header("Attributes")]
    [SerializeField] float range = 10f;
    [SerializeField] float stun = 0;
    [SerializeField] float viewAngle = 45f;
    [SerializeField] float speed = 5f;
    [SerializeField] bool faceTarget = true;
    [SerializeField] float health = 100f;

    [Header("HealthUI")]
    public Slider slider;
    public Image fillImage;
    public Color FullHealthColor;
    public Color ZeroHealthColor;

    [Header("Sound")]
    [SerializeField] AudioSource m_audioPlayer;
    [SerializeField] AudioClip damagedSound;
    [SerializeField] AudioClip stepSound;
    [SerializeField] float damagedSoundVolume = 0.65f;
    [SerializeField] float damagedPitchMinimum = 0.85f;
    [SerializeField] float damagedPitchMaximum = 1.15f;

    void Awake()
    {
        rigidbody2D = this.GetComponent<Rigidbody2D>();
        collider2D = this.GetComponent<Collider2D>();
        damagedSound = Resources.Load("Audio/SFX/Enemy/EnemyHitGrunt") as AudioClip;
        stepSound = Resources.Load("Audio/SFX/Player/FootstepTest") as AudioClip;
        m_audioPlayer = this.GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Physics2D.IgnoreCollision(collider2D, weapon.GetComponent<Collider2D>());
        SetHealthUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 1)
        {
            if (stun > 0) stun -= Time.deltaTime;

            else
            {
                // Sight Cone
                if (Vector3.Distance(PlayerController.instance.transform.position, transform.position) < range &&
                    Vector3.Angle(PlayerController.instance.transform.position - transform.position, EnemyTorso.transform.up) < viewAngle)
                {
                    EnemyTorso.transform.up = (Vector2)(PlayerController.instance.transform.position - transform.position);
                    rigidbody2D.velocity = EnemyTorso.transform.up.normalized * speed;
                    if (rigidbody2D.velocity != Vector2.zero)
                    {

                        EnemyLegs.transform.up = rigidbody2D.velocity;
                        if (faceTarget && Quaternion.Angle(EnemyTorso.transform.rotation, EnemyLegs.transform.rotation) > 90) EnemyLegs.transform.up = -1 * EnemyLegs.transform.up; // Keeps body facing mouse
                    }
                }

                else
                {
                    rigidbody2D.velocity = Vector2.zero;
                }
            }
        }
        //DEBUG
        //Debug.Log("Distance is " + Vector3.Distance(PlayerController.instance.transform.position, transform.position));
        //Debug.Log("Angle is " + Vector3.Angle(PlayerController.instance.transform.position - transform.position, EnemyTorso.transform.up));
        Debug.DrawRay(transform.position, (Quaternion.Euler(0, 0, viewAngle) * EnemyTorso.transform.up).normalized * range, Color.yellow, .01f);
        Debug.DrawRay(transform.position, (Quaternion.Euler(0, 0, -viewAngle) * EnemyTorso.transform.up).normalized * range, Color.yellow, .01f);
        Debug.DrawRay(transform.position, EnemyTorso.transform.up, Color.red, .01f);
        Debug.DrawRay(transform.position, EnemyLegs.transform.up, Color.green, .01f);
    }

    public void Damage(float damage, float stun, Vector2 knockback)
    {
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.AddForce(knockback);
        health -= damage;
        SetHealthUI();
        this.stun = stun;

        if (health <= 0)
        {
            Destroy(gameObject);
        }

        StartCoroutine(playSoundCoroutine(damagedSound, damagedSoundVolume, damagedPitchMinimum, damagedPitchMaximum));
    }

    private void SetHealthUI()
    {
        slider.value = health;

        fillImage.color = Color.Lerp(ZeroHealthColor, FullHealthColor, health / 100); // 100 is the hardcoded starting health, might need to change later
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            weapon.Attack();
        }
    }

    private IEnumerator playSoundCoroutine(AudioClip sound, float soundVolume, float minimumPitch, float maximumPitch)
    {
        float timePassed = 0.0f;
        m_audioPlayer.pitch = Random.Range(minimumPitch, maximumPitch);
        m_audioPlayer.PlayOneShot(sound, soundVolume);

        while (timePassed < sound.length)
        {
            timePassed += Time.deltaTime;
            yield return null;
        }

        m_audioPlayer.pitch = 1.0f;
    }

}
