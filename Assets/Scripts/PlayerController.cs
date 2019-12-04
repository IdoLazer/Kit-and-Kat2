using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //constants
    private int IDLE = 0;
    private int WALKING = 1;
    private int JUMP = 2;
    private int GRAB_BALL = 3;
    private int THROW_BALL = 4;


    public float mPlayerSpeed;
//    public Rigidbody2D mPlayer;
    public Transform mPlayer;
    public Rigidbody2D ball;
    public KeyCode mRightKey;
    public KeyCode mLeftKey;
    public KeyCode mUpKey;
    public float jumpForce;

    private KeyCode mThrowKey = KeyCode.Space;
//    private float mPlayerAngle = 0f;
    private bool mCanJump = true;
    private bool canWalkLeft = true;
    private bool canWalkRight = true;
    private bool IsHoldingBall = false;
    private SpriteRenderer spriteRenderer;
    private List<Collider2D> platformList = new List<Collider2D>();
    private Animator ac;

    
    
    readonly float gravity = -9.8f;
    private bool isGrounded = false;
    private float velocity_Y;
    private GameObject curPlatform = null;
//    private float deltaFromLastColl = 0f;
    private Quaternion NormRotation;

    // Start is called before the first frame update
    void Start()
    {
        NormRotation = mPlayer.rotation;
        spriteRenderer = GetComponent<SpriteRenderer>();
        ac = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // If the player still didnt touched a ground continue using the gravity force on it.
        if (!isGrounded)
        {
            float newPosition_Y = mPlayer.position.y + velocity_Y * Time.deltaTime;
            mPlayer.position = new Vector2(mPlayer.position.x, newPosition_Y);
            velocity_Y += gravity * Time.deltaTime;

        }
        if (IsHoldingBall)
        {
            if (Input.GetKeyDown(mThrowKey))
            {
                IsHoldingBall = false;
                ball.isKinematic = false;
                ball.constraints = RigidbodyConstraints2D.None;
                ball.transform.parent = null;
                ball.GetComponent<BallController>().Release();
                ball.GetComponent<BallController>().Kick(Mathf.Deg2Rad * 45);
            }
            return;
        }
        if (velocity_Y == 0 || (velocity_Y < 0 && ac.GetInteger("state") != JUMP))
        {
            ac.SetInteger("state", IDLE);
        }
        if (Input.GetKey(mRightKey) && canWalkRight)
        {
            if (isGrounded)
            {
                ac.SetInteger("state", WALKING);
            }
            mPlayer.transform.Translate(Vector2.right * (mPlayerSpeed * Time.deltaTime));
            spriteRenderer.flipX = false;
        }
        else if (Input.GetKey(mLeftKey) && canWalkLeft)
        {
            Debug.Log("here");
            if (isGrounded)
            {
                Debug.Log("here?");
                ac.SetInteger("state", WALKING);
            }
            mPlayer.transform.Translate(Vector2.left * (mPlayerSpeed * Time.deltaTime));
            spriteRenderer.flipX = true;
        }

        if (Input.GetKeyDown(mUpKey) && isGrounded && mCanJump)
        {
            ac.SetInteger("state", JUMP);
            velocity_Y = jumpForce;
            isGrounded = false;
        }
        
    }

    public void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Left Wall"))
        {
            canWalkLeft = false;
        }
        if (other.CompareTag("Right Wall"))
        {
            canWalkRight = false;
        }
        if (other.CompareTag("Obstacle"))
        {
            velocity_Y = 0;
        } 
        else if (other.CompareTag("Ground"))
        {
            platformList.Add(other);
            curPlatform = other.gameObject;
            isGrounded = true;
            velocity_Y = Mathf.Max(velocity_Y, 0);
            mPlayer.rotation = other.transform.rotation;
        }
        else if (other.CompareTag("Ball"))
        {

            //Check if the trigger is a ball trigger or cat  
            //Update the game controller that you have the ball? 
            IsHoldingBall = true;
            other.transform.parent = this.transform;
            other.transform.position = transform.position;
            other.enabled = false;
            ball = other.GetComponent<Rigidbody2D>();
            ball.constraints = RigidbodyConstraints2D.FreezeAll;
            ball.isKinematic = true;
            ball.GetComponent<BallController>().Hold();
        }
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            if (other.transform.parent.gameObject != curPlatform)
            {
                mCanJump = false;
                if (!isGrounded)
                {
                    canWalkRight = false;
                    canWalkLeft = false;
                }
                else
                {
                    canWalkRight = true;
                    canWalkLeft = true;
                }
            }
            else
            {
                mCanJump = true;
                canWalkRight = true;
                canWalkLeft = true;
            }
        }
        if (other.CompareTag("Ground") && velocity_Y < 0)
        {
            velocity_Y = 0;
            isGrounded = true;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            platformList.Remove(other);
            if (platformList.Count == 0)
            {
                isGrounded = false;
            }
            curPlatform = null;
            mPlayer.rotation = NormRotation;
        }
        if (other.CompareTag("Obstacle"))
        {
            mCanJump = true;
            canWalkRight = true;
            canWalkLeft = true;
        }
        if (other.CompareTag("Left Wall"))
        {
            canWalkLeft = true;
        }
        if (other.CompareTag("Right Wall"))
        {
            canWalkRight = true;
        }
    }
}
