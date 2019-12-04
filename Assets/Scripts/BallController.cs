using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] float mKickForce = 10f;
    [SerializeField] public float mHealth = 100f;
    [SerializeField] float mHoldingDamage = 1.5f;
    [SerializeField] float mMovingDamage = 1.5f;
    private Rigidbody2D rb;
    private bool isHeldByCat = false;
    private bool isMoving = false;
    private bool isKicked = false;
    private Transform initialTransform;
    private Animator ac;

    
    void Awake()
    {
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
                Physics2D.IgnoreLayerCollision(8, 9, false);
                Physics2D.IgnoreLayerCollision(8, 10, false);
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

    public void Kick(float angle)
    {
        isHeldByCat = false;
        GetComponent<Collider2D>().enabled = true;
        if (isKicked) return;
        Physics2D.IgnoreLayerCollision(8, 9);
        Physics2D.IgnoreLayerCollision(8, 10);
        isKicked = true;
        float ax = Mathf.Cos(angle);
        float ay = Mathf.Sin(angle);
        rb.AddForce(new Vector2(ax, ay) * mKickForce, ForceMode2D.Impulse);
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
        Debug.Log("hiii");
        rb.transform.position = initialTransform.position;
    }
}

