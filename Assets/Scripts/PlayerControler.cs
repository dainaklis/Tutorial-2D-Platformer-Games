using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{   
    //JUDEJIMAS is BARDENT mokymu

    private int amountOfJumpsLeft;
    private int facingDirection = 1;

    [SerializeField]
    private int amountOfJumps;



    private float movementInputDirection;
    private bool isFacingRight = true;
    private bool isWalking;
    private bool isGrounded;
    private bool canJump;
    private bool isTouchingWall;
    private bool isWallSlinding;
    private bool isTouchingLedge;
    private bool canClimLedge = false;
    private bool ledgeDetected;


    private Vector2 ledgePosBot;
    private Vector2 ledgePos1;
    private Vector2 ledgePos2;


    private Rigidbody2D rb;
    private Animator anim;



    




    [SerializeField]
    private float speed;

    [SerializeField]
    private float jumpForce;

    [SerializeField]
    private float groundCheckRadius;
    public float wallCheckDistance;
    public float wallSlideSpeed;
    public float moveForceInAir;
    public float airDragMultiplaier = 0.95f;
    public float variableJumpHeightMultiplaier = 0.5f;
    public float wallHopForce;
    public float wallJumpForse;




    public float ledgeClimbXOffset1 = 0f;
    public float ledgeClimbYOffset1 = 0f;
    public float ledgeClimbXOffset2 = 0f;
    public float ledgeClimbYOffset2 = 0f;


    
    public Vector2 wallhopDirection;
    public Vector2 wallJumpDirection;
    




    [SerializeField]
    private Transform groundCheck;
    public Transform wallCheck;
    public Transform ledgeCheck;


    [SerializeField] 
     private LayerMask whatIsGround;


    void Start()
    {
       rb = GetComponent<Rigidbody2D>(); 
       anim = GetComponent<Animator>();
       amountOfJumpsLeft = amountOfJumps;
       wallhopDirection .Normalize();
       wallJumpDirection.Normalize();


    }

    // Update is called once per frame
    void Update()
    {
        ChekInput();
        ChekMovementDirection();
        UpdateAnimations();
        CheckCanJump();
        CheckIfWallSliding();
        CheckLedgeClimb();


        if (Input.GetKeyDown(KeyCode.Escape))
        {

            Application.Quit();  
        }

    }

    private void FixedUpdate() 
    {
        ApplyMovement();
        CheckSurroundings();
    }

    //----------------------------- FUNKCIJOS ----------------------------------------------

    private void UpdateAnimations()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isWallSliding", isWallSlinding);
    }


    private void CheckIfWallSliding()
    {
        if (isTouchingWall && !isGrounded && rb.velocity.y < 0)
        {
           isWallSlinding = true; 
        }
        else
        {
            isWallSlinding = false;
        }
    }


    private void CheckCanJump()
    {
        if ((isGrounded && rb.velocity.y <= 0) || isWallSlinding)
        {
            amountOfJumpsLeft = amountOfJumps;
        }
        if (amountOfJumpsLeft <= 0)
        {
            canJump = false;
        }
        else
        {
            canJump = true;
        }

    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
        
        isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, transform.right, wallCheckDistance, whatIsGround); 

        if (isTouchingWall && !isTouchingLedge && !ledgeDetected)
        {
           ledgeDetected = true;
           ledgePosBot = wallCheck.position;

        }
    }

    private void CheckLedgeClimb()
    {
        if (ledgeDetected && !canClimLedge)
        {
           canClimLedge = true; 

           if (isFacingRight)
           {
               ledgePos1 = new Vector2(Mathf.Floor(ledgePosBot.x * wallCheckDistance) - ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);
               ledgePos2 = new Vector2(Mathf.Floor(ledgePosBot.x * wallCheckDistance) + ledgeClimbXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2);
           }
           else
           {
               ledgePos1 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) + ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);
               ledgePos2 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) - ledgeClimbXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2);
           }

        //    canMove = false;
        //    canFlip = false;
        }

        if (canClimLedge)
        {
            transform.position = ledgePos1;
        }
    }

    private void ChekMovementDirection()
    {
        if (isFacingRight && movementInputDirection < 0)
        {
            Flip();
        }
        else if(!isFacingRight && movementInputDirection > 0)
        {
            Flip();
        }


        // Jeigu JUDAM ideta ANIMACIJA
        if (rb.velocity.x > 0.1 || rb.velocity.x < - 0.1)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }

    }


    private void ChekInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (Input.GetButtonUp("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplaier);
        }
    }

    private void ApplyMovement()
    {   

        if (isGrounded)
        {
            //Jei Su deltaTime reikia GRAVITYSCALE = 1
            //rb.velocity = new Vector2(speed * movementInputDirection * Time.deltaTime, rb.velocity.y);

            rb.velocity = new Vector2(speed * movementInputDirection, rb.velocity.y); 
        }
        else if (!isGrounded && !isWallSlinding && movementInputDirection != 0)
        {
           Vector2 forceToAdd = new Vector2(moveForceInAir * movementInputDirection, 0);
           rb.AddForce(forceToAdd);

           if (Mathf.Abs(rb.velocity.x) > speed)
           {
               rb.velocity = new Vector2(speed * movementInputDirection, rb.velocity.y);
           }
        }
        else if (!isGrounded && !isWallSlinding && movementInputDirection == 0)
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplaier, rb.velocity.y);
        }
        
        if (isWallSlinding)
        {
            if (rb.velocity.y < -wallSlideSpeed)
            {
               rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }
    }

    private void Flip()
    {
        if(!isWallSlinding)
        {
            facingDirection *= -1;
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }



    }

    private void Jump()
    {
        if (canJump && !isWallSlinding)
        {
            //rb.velocity = new Vector2(rb.velocity.x, jumpForce * Time.deltaTime); 
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            amountOfJumpsLeft--;
        }
        else if (isWallSlinding && movementInputDirection == 0 && canJump)
        {
            isWallSlinding = false;
            amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallHopForce * wallhopDirection.x * -facingDirection, wallHopForce * wallhopDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
        else if ((isWallSlinding || isTouchingWall) && movementInputDirection !=0 && canJump)
        {
            isWallSlinding = false;
            amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallJumpForse * wallJumpDirection.x * movementInputDirection, wallJumpForse * wallJumpDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse); 
        }

       

    }



    private void OnDrawGizmos() 
    {
       Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius); 

       Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
       

    }




}
