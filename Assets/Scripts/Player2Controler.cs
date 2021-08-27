using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2Controler : MonoBehaviour
{

    private float movementInputDirection;


    private int amountOfJumpsLeft;
    private int facingDirection = 1;


    private bool isFacingRight = true;
    private bool isWalking;
    private bool isGrounded;
    private bool canJump;
    private bool isTouchingWall;
    private bool isWallSlinding;




    private Rigidbody2D Player2;
    private Animator anim;



    public int amountOfJumps = 1;





    public float moveSpeed = 10f;
    public float jumpForce = 16f;
    public float groundCheckRadius;
    public float wallCheckDistance;
    public float wallSlidingSpeed;
    public float movementForceInAir;
    public float airDragMultiplaier = 0.95f;
    public float variableJumpHeightMultiplaier = 0.5f;
    public float wallHopForce;
    public float wallJumpForce;



    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;


    public Transform groundCheck;
    public Transform wallCheck;


    public LayerMask whatIsGround;



    void Start() 
    {
        Player2 =GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        amountOfJumpsLeft = amountOfJumps;
        wallHopDirection.Normalize();
        wallHopDirection.Normalize();
        
    }


    void Update() 
    {
        CheckInput();
        CkeckMovementDirection();
        UpdateAnimation();
        CheckIfCanJump();
        CheckIfWallSlinding();
    }

    private void FixedUpdate() 
    {
        ApplyMovement();
        CheckSurroundings();
    }

    //--------------------------------------Funkcijos----------------------------------------------------------


    private void CheckIfWallSlinding()
    {
        if (isTouchingWall && !isGrounded && Player2.velocity.y < 0)
        {
            isWallSlinding = true;
        }
        else 
        {
            isWallSlinding = false;
        }
    }
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
    }
    private void CkeckMovementDirection()
    {

        //-----Patikrinam ikuria puse eina!!!!!!!-------------
        if (isFacingRight && movementInputDirection < 0)
        {
            Flip();
        }
        else if (!isFacingRight && movementInputDirection > 0)
        {
            Flip();
        }


        //------Jeigu JUDAM pakieciam isWalking i TRUE animator-----------------------
        if (Player2.velocity.x > 0.1 || Player2.velocity.x < -0.1)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }


    }

    private void CheckIfCanJump()
    {
        if ((isGrounded && Player2.velocity.y <= 0) || isWallSlinding)
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

    private void UpdateAnimation()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", Player2.velocity.y);
        anim.SetBool("isWallSliding", isWallSlinding);


    }



    private void Flip()
    {
        if (!isWallSlinding)
        {
            facingDirection *= -1;
            isFacingRight =!isFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f); 

            //transform.localScale = new Vector3(Player2.velocity.x < 0 ? -1 : 1, 1, 1);
        }

        
        
        
    }



    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (Input.GetButtonUp("Jump"))
        {
            Player2.velocity = new Vector2(Player2.velocity.x, Player2.velocity.y * variableJumpHeightMultiplaier);
        }

    }

    private void Jump()
    {
        if (canJump && !isWallSlinding)
        {
            Player2.velocity = new Vector2(Player2.velocity.x, jumpForce); 
            amountOfJumpsLeft--;
        }
        else if (isWallSlinding && movementInputDirection == 0 && canJump)
        {
            isWallSlinding = false;
            amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallHopForce * wallHopDirection.x * -facingDirection, wallHopForce * wallHopDirection.y);
            Player2.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
        else if ((isWallSlinding || isTouchingWall) && movementInputDirection !=0 && canJump)
        {
            isWallSlinding = false;
            amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * movementInputDirection, wallJumpForce * wallJumpDirection.y);
            Player2.AddForce(forceToAdd, ForceMode2D.Impulse); 
        }
        
    }

    private void ApplyMovement()
    {   
        if (isGrounded)
        {
            //----JUDAM----------------
            Player2.velocity = new Vector2(moveSpeed * movementInputDirection, Player2.velocity.y);

            //--Dar gali buti, bet Tada  GRAVITY SCALE =1 ---
            //Player2.velocity = new Vector2(movementInputDirection * moveSpeed * Time.deltaTime, Player2.velocity.y);
        }
        else if (!isGrounded && !isWallSlinding && movementInputDirection != 0)
        {
           Vector2 forceToAdd = new Vector2(movementForceInAir * movementInputDirection, 0);
           Player2.AddForce(forceToAdd);

           if (Mathf.Abs(Player2.velocity.x) > moveSpeed)
           {
               Player2.velocity = new Vector2(moveSpeed * movementInputDirection, Player2.velocity.y);
           }
        }
        else if (!isGrounded && !isWallSlinding && movementInputDirection == 0)
        {
            Player2.velocity = new Vector2(Player2.velocity.x * airDragMultiplaier, Player2.velocity.y);
        }
        



        //-----CIUOZIAM SIENA-------------------------
        if (isWallSlinding)
        {
            if (Player2.velocity.y < -wallSlidingSpeed)
            {
                Player2.velocity = new Vector2(Player2.velocity.x, -wallSlidingSpeed);
            }
        }
    }


    private void OnDrawGizmos() 
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z)); 
    }

}
