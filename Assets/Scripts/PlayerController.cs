using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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
    private bool mCanJump = false;
    public bool IsHoldingBall = false;
    
    
    readonly float gravity = -9.8f;
    private float ground_Y = -2.74f; //should be 0 in the real game
    private bool isGrounded = true;
    private float velocity_Y;
//    private float deltaFromLastColl = 0f;
    private Quaternion NormRotation;

    // Start is called before the first frame update
    void Start()
    {
        NormRotation = mPlayer.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(IsHoldingBall)
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
        //        float xValue = 0;
        if (Input.GetKey(mRightKey))
        {
            mPlayer.transform.Translate(Vector2.right * (mPlayerSpeed * Time.deltaTime));
        }
        else if (Input.GetKey(mLeftKey))
        {
            mPlayer.transform.Translate(Vector2.left * (mPlayerSpeed * Time.deltaTime));
        }

        if (Input.GetKeyDown(mUpKey) && isGrounded)
        {
            velocity_Y = jumpForce;
            isGrounded = false;
        }

        // If the player still didnt touched a ground continue using the gravity force on it.
        if (!isGrounded)
        {
            float newPosition_Y = mPlayer.position.y + velocity_Y * Time.deltaTime;
            if (newPosition_Y <= ground_Y)
            {
                mPlayer.position = new Vector2(mPlayer.position.x, ground_Y);
                isGrounded = true;
                velocity_Y = 0;
            }
            else
            {
               mPlayer.position = new Vector2(mPlayer.position.x, newPosition_Y);
                velocity_Y += gravity * Time.deltaTime;
            }
        }
    }

    // +++++++++++++ need to be executed
    public void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Obstacle"))
        {
            velocity_Y = 0;
            Debug.Log("Ouch!");
            isGrounded = false;
        } 
        else if (other.CompareTag("Ground"))
        {
            isGrounded = true;
            velocity_Y = 0; // do i need this?
            mPlayer.rotation = other.transform.rotation;
            Debug.Log("Touched an obstacle!! " + mPlayer.rotation.z);
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
            isGrounded = false;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            Debug.Log("Fell from an obstacle!!");
            isGrounded = false;
            mPlayer.rotation = NormRotation;
        }
    }
}
