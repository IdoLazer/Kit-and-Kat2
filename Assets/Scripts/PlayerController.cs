using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //constants
    private int IDLE = 0;
    private int WALKING = 1;
    private int JUMP = 2;
    private int THROW_BALL = 4;
    private int ANGLE_OFFSET = 10;



    public float mPlayerSpeed;
//    public Rigidbody2D mPlayer;
    public Transform mPlayer;
    public Transform otherPlayer;
    public Rigidbody2D ball;
    public KeyCode mRightKey;
    public KeyCode mLeftKey;
    public KeyCode mUpKey;
    public float jumpForce;
    public AudioClip mWalkingSoundEffect;
    public AudioClip mJumpingSoundEffect;
    public AudioClip mHoldingSoundEffect;
    public AudioClip mThrowingSoundEffect;

    private KeyCode mThrowKey = KeyCode.Space;
//    private float mPlayerAngle = 0f;
    private bool mCanJump = true;
    private bool canWalkLeft = true;
    private bool canWalkRight = true;
    private bool IsHoldingBall = false;
    private SpriteRenderer spriteRenderer;
    private List<Collider2D> platformList = new List<Collider2D>();
    private Animator ac;
    private AudioSource audioSource;

    
    
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
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = mWalkingSoundEffect;
    }

    // Update is called once per frame
    void Update()
    {
        // If the player still didnt touch the ground continue applying gravity on him.
        if (!isGrounded)
        {
            float newPosition_Y = mPlayer.position.y + velocity_Y * Time.deltaTime;
            mPlayer.position = new Vector2(mPlayer.position.x, newPosition_Y);
            velocity_Y += gravity * Time.deltaTime;

        }
        // If the player is holding the ball he can't move until he presses the throw key
        if (IsHoldingBall)
        {
            if (Input.GetKeyDown(mThrowKey)) // This means the ball is thrown
            {
                ac.SetInteger("state", THROW_BALL); // Play animation
                AudioSource.PlayClipAtPoint(mThrowingSoundEffect, transform.position); // Play throw SFX
                IsHoldingBall = false;
                ball.isKinematic = false;
                ball.constraints = RigidbodyConstraints2D.None;
                ball.transform.parent = null;
                ball.GetComponent<BallController>().Release();
                ball.GetComponent<BallController>().Kick(CalcAngle(), mPlayer); // Throw ball in some angle;
                EventManager.TriggerEvent("EVENT_AIR_BALL");
            }
            return;
        }
        if (velocity_Y == 0 || (velocity_Y < 0 && ac.GetInteger("state") != JUMP)) // If the cat is standing still or falling (without jumping)
        {
            audioSource.Pause();
            ac.SetInteger("state", IDLE);
        }
        if (Input.GetKey(mRightKey) && canWalkRight) // Move the player right
        {
            if (isGrounded) // If the player is on the ground, a walking animation should start
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.Play(); // play walking SFX
                }
                ac.SetInteger("state", WALKING);
            }
            mPlayer.transform.Translate(Vector2.right * (mPlayerSpeed * Time.deltaTime));
            spriteRenderer.flipX = false;
        }
        else if (Input.GetKey(mLeftKey) && canWalkLeft) // Move the player left
        {
            if (isGrounded) // If the player is on the ground, a walking animation should start
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.Play(); // play walking SFX
                }
                ac.SetInteger("state", WALKING);
            }
            mPlayer.transform.Translate(Vector2.left * (mPlayerSpeed * Time.deltaTime));
            spriteRenderer.flipX = true;
        }

        if (Input.GetKeyDown(mUpKey) && isGrounded && mCanJump) // Make the player jump
        {
            AudioSource.PlayClipAtPoint(mJumpingSoundEffect, transform.position); // Play jumping SFX
            ac.SetInteger("state", JUMP);
            velocity_Y = jumpForce;
            isGrounded = false;
        }
        
    }

    public void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Left Wall")) // Bump into left wall
        {
            canWalkLeft = false;
        }
        if (other.CompareTag("Right Wall")) // Bump into right wall
        {
            canWalkRight = false;
        }
        if (other.CompareTag("Obstacle")) // Bump into platform from the wrong direction (not from above)
        {
            velocity_Y = 0;
        } 
        else if (other.CompareTag("Ground")) // Touch the ground or a platform from above
        {
            platformList.Add(other);
            curPlatform = other.gameObject;
            isGrounded = true;
            velocity_Y = Mathf.Max(velocity_Y, 0);
            mPlayer.rotation = other.transform.rotation;
        }
        else if (other.CompareTag("Ball")) // Touch the ball
        {
            AudioSource.PlayClipAtPoint(mHoldingSoundEffect, transform.position); // Play Grabbing ball SFX
            ac.SetInteger("state" ,-1); // Prevent any other animation while grabbing ball 
            ac.SetTrigger("grab"); // Set animation to grabbing ball
            IsHoldingBall = true;
            other.transform.parent = this.transform;
            other.transform.position = transform.position; // Move the ball to the player's position
            other.enabled = false; // Disable ball collider
            ball = other.GetComponent<Rigidbody2D>();
            ball.constraints = RigidbodyConstraints2D.FreezeAll;
            ball.isKinematic = true;
            ball.GetComponent<BallController>().Hold();
            
            EventManager.TriggerEvent("EVENT_HOLD_BALL");
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

   
   // Calculates the angle the ball will be kicked with.
   private float CalcAngle()
    {
        float deltaY = Mathf.Abs(otherPlayer.position.y - mPlayer.position.y);
        float deltaX = Mathf.Abs(otherPlayer.position.x - mPlayer.position.x);
        float bestAngle = Mathf.Atan(deltaY / deltaX); // The angle that the other player is compere to this player
        
        // When the other player is lower, throw in a flat angle.
        if (otherPlayer.position.y < mPlayer.position.y)
        {
            bestAngle = 0;
        }
        
        // Change the direction of the ball to left
        if (otherPlayer.position.x < mPlayer.position.x) 
        {
            bestAngle = (Mathf.Deg2Rad * 180) - bestAngle;
            bestAngle -= Mathf.Deg2Rad * ANGLE_OFFSET;
        }
        
        if (bestAngle < (Mathf.Deg2Rad * 90) && bestAngle >= 0)
        {
            bestAngle += Mathf.Deg2Rad * ANGLE_OFFSET;
        }
        return bestAngle;
    }
}
