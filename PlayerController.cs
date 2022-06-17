/*

Player Movement Script

Features Added
  -  Player can move left and right
    -  Player has set acceleration and decceleration
    -  Player can move around while in the air

  -  Player can jump
    -  Variable jump height based on how long the button has been pressed for
    -  Player can jump while in the air a set number of times
    -  Player can jump even after pressing the jump button just after leaving a platform
    -  Player can still jump even if the jump button is pressed just before reaching the ground
      -  Waits for the player to reach the ground again before jumping again
  -  The player has a different falling gravity than regular gravity
  -  Player can slide down wall slower than regularly falling
  -  Player can wall jump
  -  Player can dash
    -  Player can dash once while in the air

Features to be added
  -  long dashes
  -  gravity flipping

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Draw Gizmos")]
    public bool drawGizmos;

    [Space(20)]

    [Header("Player Movement Abilities")]
    public bool canDash;
    public bool canWallJump;
    public bool canDoubleJump;

    [Space(20)]

    [Header("Movement")]
    public float moveSpeed;
    private float moveInput;
    public float acceleration;
    public float decceleration;
    public float velPower;
    public float frictionAmount;
    
    [Space(15)]

    [Header("Jump")]
    public float jumpForce;
    private float lastGroundedTime;
    public float coyoteTime;
    private bool jumpBuffer;
    private int extraJumps;
    public int extraJumpsValue;
    public float jumpCutMultiplyer;

    [Space(15)]

    [Header("Wall Jump")]
    public float wallJumpForce;
    public float wallJumpXSpeed;
    public float lastWallJumpTimeValue;
    private float lastWallJumpTime;

    [Space(5)]

    [Header("Wall Slide")]
    public float wallSlideSpeedMultiplier;

    [Space(15)]

    [Header("Dash")]
    public float dashSpeed;
    private float dashTime;
    public float dashTimeValue;
    private float lastDashTime;
    public float lastDashTimeValue;
    private bool isDashing;
    private bool isDashCoolDown;
    private bool dashedInAir;
    public float triggerTheshold;
    private float direction;

    [Space(15)]

    [Header("Clamp Fall Speed")]
    public float maxFallSpeed;
    public float gravityScale;
    public float fallGravityScale;

    [Space(15)]

    [Header("Ground Check")]
    public Transform groundCheckPoint;
    public Vector2 groundCheckSize;
    public Transform closeGroundCheckPoint;
    public Vector2 closeGroundCheckSize;
    public Transform wallJumpGroundCheckPoint;
    public Vector2 wallJumpGroundCheckSize;
    public LayerMask groundLayerCheck;
    private bool isGrounded;
    private bool isCloseGrounded;
    private bool isGroundedWallJump;
    private Rigidbody2D rb;
    public SpriteRenderer sr;
    public MovablePlatform movablePlatform;

    [Space(5)]

    [Header("Wall Check")]
    public Transform leftWallCheckPoint;
    public Transform rightWallCheckPoint;
    public Vector2 wallCheckSize;
    public LayerMask whatIsWallJumpable;
    public bool isOnWallLeft;
    public bool isOnWallRight;

    // Start is called before the first frame update
    void Start() {
        // Get the rigidbody2D component
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        direction = 1;
        dashTime = dashTimeValue;
        lastDashTime = lastDashTimeValue;
    }

    // FixedUpdate is called once per physics frame
    private void FixedUpdate() {
        // Movement Logic
        if (!isDashing && lastWallJumpTime < 0)
        {
            // Set the target velocity
            float targetSpeed = moveInput * moveSpeed;
            // Get the difference between the target velocity and current velocity
            float speedDif = targetSpeed - rb.velocity.x;
            // Multiply by an acceleration or decceleration value, remove the sign
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;
            // Raise the movement value by a set power to smooth the movement, and bring the sign back
            float movement =Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
            // Apply the force to the rigidbody2D
            rb.AddForce(movement * Vector2.right);
        }

        // Artificial Friction
        // Check player is not pressing forwards or backwards and is not dashing
        if (Mathf.Abs(moveInput) < 0.01f && !isDashing)
        {
            // use either a set friction amount or use current velocity to calculate friction
            float amount = Mathf.Min(Mathf.Abs(rb.velocity.x) * Mathf.Abs(frictionAmount));
            // Set to movement direction
            amount *= Mathf.Sign(rb.velocity.x);
            // Apply the force to the rigidbody2D
            rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }

        // Fix player velocity having a seizure by checking if its between a very small value and setting it to 0 if so
        if (rb.velocity.x  < 0.1f && rb.velocity.x > -0.1f && Mathf.Abs(moveInput) < 0.01f)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        // Clamp the player fall speed to be between a maxFallSpeed and infinity
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -maxFallSpeed, Mathf.Infinity));

        // Variable player gravity, if the player is falling use a stronger gravity
        if (rb.velocity.y <= 0)
        {
            rb.gravityScale = fallGravityScale;
        }
        else if (!isDashing)
        {
            rb.gravityScale = gravityScale;
        }

        // Check if the player is touching a wall and moving downwards, multiply the y speed by the wall slide speed multiplier
        if (canWallJump)
        {
            if (isOnWallLeft && rb.velocity.y < 0 || isOnWallRight && rb.velocity.y < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * wallSlideSpeedMultiplier);
            }
        }
    }

    // Update is called once per frame
    void Update() {
        /* ########## Timers and Checks ########## */

        // Check if the player is on the ground
        if (Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayerCheck) || movablePlatform.isPlatform)
        {
            isGrounded = true;
        } else
        {
            isGrounded = false;
        }
        // Check if the player is close to the ground
        isCloseGrounded = Physics2D.OverlapBox(closeGroundCheckPoint.position, closeGroundCheckSize, 0, groundLayerCheck);
        // Check if the player is on a wall to the left
        isOnWallLeft = Physics2D.OverlapBox(leftWallCheckPoint.position, wallCheckSize, 0, whatIsWallJumpable);
        // Check if the player is on a wall to the right
        isOnWallRight = Physics2D.OverlapBox(rightWallCheckPoint.position, wallCheckSize, 0, whatIsWallJumpable);
        // Check if the player is on the ground for wall jumping
        isGroundedWallJump = Physics2D.OverlapBox(wallJumpGroundCheckPoint.position, wallJumpGroundCheckSize, 0, groundLayerCheck);


        // Get the player's facing direction from the movement input
        if (moveInput > 0)
        {
            direction = 1;
        }
        else if (moveInput < 0)
        {
            direction = -1;
        }

        // Flip the player's sprite to face the direction they are moving
        Flip();

        // Timer for when the player last wall jumped
        // For disabling player movement right after wall jumping
        lastWallJumpTime -= Time.deltaTime;
        if (lastWallJumpTime < -30)
        {
            lastWallJumpTime = -1;
        }

        // Reset values when the player touches the ground, and run timers if the player is not on the ground
        if (isGrounded)
        {
            // Reset Values
            // Reset the coyote timer
            lastGroundedTime = coyoteTime;
            // Reset the extra jumps
            extraJumps = extraJumpsValue;
            // Reset the dash in air check
            dashedInAir = false;
        }
        else
        {
            // Run the coyote timer
            lastGroundedTime -= Time.deltaTime;
            // Keep the coyote timer from going to low, still keeping it below any checks
            if (lastGroundedTime < -30)
            {
                lastGroundedTime = -1;
            }
        }

        // Run the dashing timer
        if (isDashing)
        {
            dashTime -= Time.deltaTime;
        }
        // Run the dash cooldown timer
        if (isDashCoolDown)
        {
            lastDashTime -= Time.deltaTime;
        }
        // Reset the dashing timer once it has run out
        if (dashTime < 0)
        {
            isDashing = false;
            dashTime = dashTimeValue;
        }
        // Reset the dash cooldown timer once it has run out
        if (lastDashTime < 0)
        {
            isDashCoolDown = false;
            lastDashTime = lastDashTimeValue;
        }


        // Reset the values and counters when the player is on a wall
        if (isOnWallLeft || isOnWallRight)
        {
            dashedInAir = false;
            extraJumps = extraJumpsValue;
        }



        /* ########## Running Logic and Movement ########## */

        /* Horizontal Movement */
        moveInput = Input.GetAxisRaw("Horizontal");

        /* Dashing */
        // Check if the player has pressed the dash button
        // Check if the dash dash timer is reset
        // Check if the player has dashed in the air already
        if (canDash)
        {
            if (Input.GetAxis("Dash") > triggerTheshold && lastDashTime == lastDashTimeValue && !dashedInAir)
            {
                // If the player is dashing and is in the air, set the dash in air check to true
                if (!isGrounded)
                {
                    dashedInAir = true;
                }
                Dash();
            }
        }


        /* Jumping */
        // Check if the player has pressed the jump button
        // Check if the player is not currently dashing
        if (Input.GetButtonDown("Jump") && !isDashing)
        {
            // Check if the player is close to the ground
            // If so, set the jumpBuffer to true
            // This will be checked for later so the player can jump if they are close to the ground and press the jump button
            if (isCloseGrounded)
            {
                jumpBuffer = true;
            }
            // If the player is on the ground, or just ran off a platform, jump
            if (lastGroundedTime > 0)
            {
                Jump();
            }
            // If the player can jump while in the air, jump
            else if (extraJumps > 0 && !isOnWallLeft && !isOnWallRight)
            {
                if (canDoubleJump)
                {
                    // Set the player's y velocity to 0, in anticipation for the jump
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                    // Jump
                    Jump();
                    // Decrease the extra jumps left
                    extraJumps--;
                    // Reset the dashed in air check
                    dashedInAir = false;
                }
            }

            // Check if the player is on a wall and is still above the ground
            if (canWallJump)
            {
                if (isOnWallLeft && !isGroundedWallJump || isOnWallRight && !isGroundedWallJump)
                {
                    WallJump();
                }
            }
        }
        
        // Check if the jump buffer is true and the player has reached the ground
        if (jumpBuffer && isGrounded && !isDashing) {
            Jump();
        }

        // If the player releases the jump button, cut the jump by adding force downwards onto the player
        if (Input.GetButtonUp("Jump"))
        {
            if (rb.velocity.y > 0 && !isGrounded)
            {
                rb.AddForce(Vector2.down * rb.velocity.y * (1 - jumpCutMultiplyer), ForceMode2D.Impulse);
            }
        }
    }


    /* ########## Player Movement Functions ########## */
    // Dash
    private void Dash() {
        // Start dashing timers
        isDashing = true;
        isDashCoolDown = true;
        // Set the player's gravity to be 0
        rb.gravityScale = 0;
        // Flip the player's direction if they are on a wall
        if (isOnWallLeft || isOnWallRight)
        {
            direction *= -1;
        }
        // Set the player's velocity to be the dash speed
        rb.velocity = new Vector2(dashSpeed * direction, 0);
    }

    // Wall Jump
    private void WallJump() {
        // Set the wall jump movement timer
        lastWallJumpTime = lastWallJumpTimeValue;
        // Set the player's y velocity to 0 and start moving the player in the direction away from the wall
        rb.velocity = new Vector2(wallJumpXSpeed * -direction, 0);
        // Make the player jump up
        rb.AddForce((Vector2.up * wallJumpForce), ForceMode2D.Impulse);
    }

    // Jump
    private void Jump() {
        // Reset the timers
        lastGroundedTime = -1;
        // Set jump buffer to false
        jumpBuffer = false;
        // Set isGrounded to false
        isGrounded = false;
        // Set the player's currect y velocity to 0
        rb.velocity = new Vector2(rb.velocity.x, 0);
        // Add force to the rigidbody2D
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    // Flip the player's sprite based on the direction
    private void Flip() {
        if (direction == 1 && sr.flipX)
        {
            sr.flipX = false;
        }
        else if (direction == -1 && !sr.flipX)
        {
            sr.flipX = true;
        }
    }

    // Draw box around check points in the editor
    void OnDrawGizmosSelected() {
        if (drawGizmos)
        {
            // Draw a semitransparent blue cube at the transforms position
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawCube(leftWallCheckPoint.position, new Vector3(0.05f, wallCheckSize.y, 1));
            Gizmos.DrawCube(rightWallCheckPoint.position, new Vector3(0.05f, wallCheckSize.y, 1));
            Gizmos.DrawCube(groundCheckPoint.position, new Vector3(groundCheckSize.x, 0.05f, 1));
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Gizmos.DrawCube(closeGroundCheckPoint.position, new Vector3(closeGroundCheckSize.x, closeGroundCheckSize.y, 1));
            Gizmos.color = new Color(0, 0, 1, 0.5f);
            Gizmos.DrawCube(wallJumpGroundCheckPoint.position, new Vector3(wallJumpGroundCheckSize.x, wallJumpGroundCheckSize.y, 1));
        }
    }
}
