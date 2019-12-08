﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] float mKickForce = 10f;
    [SerializeField] public float mHealth = 100f;
    [SerializeField] float mHoldingDamage = 1.5f;
    [SerializeField] float mMovingDamage = 1.5f;
    public bool isHeldByCat = false;
    private Rigidbody2D rb;
    private bool isMoving = false;
    private bool isKicked = false;
    private Transform initialTransform;
    private Animator ac;
    public static bool throwToUpPlayer = false;

    
    void Awake()
    {
        isKicked = false;
        rb = GetComponent<Rigidbody2D>();
        initialTransform = rb.transform;
        ac = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Damage();
        int state = (int)(mHealth / 10) + 1;
        if (state > 10)
        {
            state = 10;
        }
        if (mHealth <= 0)
        {
            state = 0;
        }
        ac.SetInteger("state", state);
        if (isKicked)
        {
            if (rb.velocity.y <= 0)
            {
                GetComponent<Collider2D>().enabled = true;
                isKicked = false;
                // Returns all the blocked colliders
                Physics2D.IgnoreLayerCollision(8, 9, false);
                Physics2D.IgnoreLayerCollision(8, 11, false);
                Physics2D.IgnoreLayerCollision(8, 12, false);
            }
        }
        // TODO: remove
        //rb.AddForce(Vector2.right * 5 * Input.GetAxis("Horizontal"));
    }

    private void Damage()
    {
        if (mHealth > 0)
        {
            if (isHeldByCat)
            {
                mHealth -= mHoldingDamage * Time.deltaTime;
            }

            if (isMoving)
            {
                mHealth -= mMovingDamage * Time.deltaTime;
            }

            if (!isMoving && !isHeldByCat)
            {
                mHealth -= Time.deltaTime;
            }
        }
    }

    public void Kick(float angle, Transform throwerP)
    {
        isHeldByCat = false;
        GetComponent<Collider2D>().enabled = true;
        if (isKicked) return;
        // Blocks the collider of the throwing player.
        Physics2D.IgnoreLayerCollision(8, 9);
        if (throwerP.CompareTag("Player1"))
        {
            Physics2D.IgnoreLayerCollision(8, 11);
        }
        else if (throwerP.CompareTag("Player2"))
        {
            Physics2D.IgnoreLayerCollision(8, 12);
        }
        isKicked = true;
        float ax = Mathf.Cos(angle);
        float ay = Mathf.Sin(angle);
        rb.AddForce(new Vector2(ax,  ay) * mKickForce, ForceMode2D.Impulse);
    }

    public void Hold()
    {
        isHeldByCat = true;
    }

    public void Release()
    {
        isHeldByCat = false;
    }

    public void OnEnable()
    {
        isKicked = false;
        rb.transform.position = initialTransform.position;
    }
}

