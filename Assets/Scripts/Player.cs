using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public GameObject playerModel;
    public Animator playerAnimator;

    [Header("Player movement options")]
    public float accSpeed;
    public float deccelSpeed;
    public float currentSpeed;
    public float maxRunSpeed;
    public float jumpPower;
    public float jumpForceDuration;
    public float gravityPower;

    [Header("Ground detection options")]
    public float rayOriginHeight;
    public float groundRayDist;
    public float safetyRayDist;
    public float allowJumpDist;

    [Header("Player sound effects")]
    public AudioClip jumpSFX;
    public AudioClip takeDamageSFX;

    public GameObject frontNose;
    public GameObject backNose;

    [Header("Gameplay test camera")]
    public GameObject altCamera;

    [Header("Misc")]
    public float slideFallSpeed = 0.01f;

    CharacterController player;
    AudioSource playerSFX;

    Vector3 movement;
    Vector3 gravityForce;
    float jumpTimer = 0;

    bool allowInput = true;

    LayerMask mask;

    Ray midRay, leftRay, rightRay;
    RaycastHit midHit, leftHit, rightHit;

    private void Start()
    {
        player = GetComponent<CharacterController>();
        playerSFX = GetComponent<AudioSource>();

        if (!Application.isEditor || GameOptions.Instance.TestARInEditor)
            altCamera.SetActive(false);
        else
            altCamera.SetActive(true);

        mask = LayerMask.GetMask("Tower");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        UpdateRays();
        UpdateFacingDirection();

        KeyControls();

        PlayerForces();
        AutoForces();

        Move();

        movement = Vector3.zero;
        moveOffset = Vector3.zero;
    }

    //***TOUCH CONTROLS******************

    public bool IsMovingLeft { get; private set; } = false;
    public bool IsMovingRight { get; private set; } = false;

    public void MoveLeft()
    {
        if (allowInput)
        {
            IsMovingLeft = true;
        }
    }

    public void EndLeft()
    {
        IsMovingLeft = false;
    }

    public void MoveRight()
    {
        if (allowInput)
        {
            IsMovingRight = true;
        }
    }

    public void EndRight()
    {
        IsMovingRight = false;
    }

    public void ActivateJump()
    {
        if (CanJump() && allowInput)
        {
            jumpTimer = jumpForceDuration;
            gravityForce = Vector3.zero;

            if (jumpSFX && !GameOptions.Instance.MuteSFX)
                playerSFX.PlayOneShot(jumpSFX);

            playerAnimator.SetTrigger("jump");
        }
    }
    //-----------------------------------

    //***KEY CONTROLS********************
    public void KeyControls()
    {
        if (Input.GetAxis("Horizontal") > 0)
            movement += transform.forward * maxRunSpeed * Time.deltaTime;

        if (Input.GetAxis("Horizontal") < 0)
            movement += -transform.forward * maxRunSpeed * Time.deltaTime;

        if (Input.GetButtonDown("Jump"))
            ActivateJump();
    }
    //-----------------------------------

    void UpdateFacingDirection()
    {
        if (IsMovingRight || Input.GetAxis("Horizontal") > 0)
        {
            frontNose.SetActive(true);
            backNose.SetActive(false);
            playerModel.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            playerAnimator.SetBool("isRunning", true);
        }
        else if (IsMovingLeft || Input.GetAxis("Horizontal") < 0)
        {
            frontNose.SetActive(false);
            backNose.SetActive(true);
            playerModel.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
            playerAnimator.SetBool("isRunning", true);
        }
        else
            playerAnimator.SetBool("isRunning", false);
    }

    void PlayerForces()
    {
        if (IsMovingLeft)
        {
            if (currentSpeed < accSpeed)
                currentSpeed += accSpeed;
            else
                currentSpeed = maxRunSpeed;

            movement += -transform.forward * currentSpeed * Time.deltaTime;
        }
        else if (IsMovingRight)
        {
            if (currentSpeed < accSpeed)
                currentSpeed += accSpeed;
            else
                currentSpeed = maxRunSpeed;

            movement += transform.forward * currentSpeed * Time.deltaTime;
        }
        else
        {
            if (currentSpeed > 0)
                currentSpeed -= deccelSpeed;
            else
                currentSpeed = 0;
        }

        if (jumpTimer > 0)
        {
            movement += transform.up * jumpPower * Mathf.Min(jumpTimer, Time.deltaTime);
            gravityForce = Vector3.zero;
        }

        jumpTimer -= Time.deltaTime;
    }

    void AutoForces()
    {
        if (IsGrounded())
            gravityForce = Vector3.zero;
        else
            gravityForce += -transform.up * gravityPower * Time.deltaTime;
    }

    void Move()
    {
        if (!onSlide)
            player.Move(movement + moveOffset + gravityForce);
        else
            player.Move(moveOffset);
    }

    public void Respawn()
    {
        movement = Vector3.zero;
        gravityForce = Vector3.zero;
        jumpTimer = 0;
        allowInput = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void UpdateRays()
    {
        midRay = new Ray(transform.position + -transform.up * rayOriginHeight, -transform.up);
        leftRay = new Ray(transform.position + -transform.up * rayOriginHeight + transform.forward * safetyRayDist, -transform.up);
        rightRay = new Ray(transform.position + -transform.up * rayOriginHeight + -transform.forward * safetyRayDist, -transform.up);
    }

    public bool IsGrounded()
    {
        // 3 raycasts to check for ground
        if (Physics.Raycast(midRay, out midHit, groundRayDist, mask) ||
            Physics.Raycast(leftRay, out leftHit, groundRayDist, mask) ||
            Physics.Raycast(rightRay, out rightHit, groundRayDist, mask))
        {
            // make sure not to stand on triggers
            if ((midHit.collider != null && !midHit.collider.isTrigger) ||
                (leftHit.collider != null && !leftHit.collider.isTrigger) ||
                (rightHit.collider != null && !rightHit.collider.isTrigger))
            {
                return true;
            }
            else
                return false;
        }
        else
            return false;
    }

    public bool CanJump()
    {
        // 3 raycasts to check for ground
        if (Physics.Raycast(midRay, out midHit, allowJumpDist, mask) ||
            Physics.Raycast(leftRay, out leftHit, allowJumpDist, mask) ||
            Physics.Raycast(rightRay, out rightHit, allowJumpDist, mask))
        {
            // make sure not to stand on triggers
            if ((midHit.collider != null && !midHit.collider.isTrigger) ||
                (leftHit.collider != null && !leftHit.collider.isTrigger) ||
                (rightHit.collider != null && !rightHit.collider.isTrigger))
            {
                return true;
            }
            else
                return false;
        }
        else
            return false;
    }

    Vector3 currPlatformPos;
    Vector3 prevPlatformPos;
    Vector3 moveOffset;
    bool onSlide = false;
    WaypointController currentPushPlatform = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Death"))
            Respawn();

        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
            jumpTimer = jumpForceDuration;
        }

        if (other.CompareTag("Platform"))
        {
            prevPlatformPos = other.transform.position;
            currPlatformPos = other.transform.position;
        }

        if (other.CompareTag("Slide"))
        {
            onSlide = true;
        }

        if (other.CompareTag("Push"))
        {
            currentPushPlatform = other.GetComponentInParent<WaypointController>();
            prevPlatformPos = other.transform.position;
            currPlatformPos = other.transform.position;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Platform"))
        {
            prevPlatformPos = currPlatformPos;
            currPlatformPos = other.transform.position;

            moveOffset += currPlatformPos - prevPlatformPos;
        }

        if (other.CompareTag("Slide"))
        {
            moveOffset += -other.transform.up * slideFallSpeed;
        }

        if (other.CompareTag("Push"))
        {
            if (currentPushPlatform.CustomBool == true)
            {
                prevPlatformPos = currPlatformPos;
                currPlatformPos = other.transform.position;

                moveOffset += currPlatformPos - prevPlatformPos;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Platform"))
        {
            moveOffset = Vector3.zero;
        }

        if (other.CompareTag("Slide"))
        {
            onSlide = false;
        }

        if (other.CompareTag("Push"))
        {
            currentPushPlatform = null;
            moveOffset = Vector3.zero;
        }
    }

    private void OnDrawGizmos()
    {
        //allow jump rays
        Debug.DrawLine(transform.position + -transform.up * rayOriginHeight,
            transform.position + -transform.up * rayOriginHeight + -transform.up * allowJumpDist,
            CanJump() == true ? Color.cyan : Color.yellow);
        Debug.DrawLine(
            transform.position + -transform.up * rayOriginHeight + transform.forward * safetyRayDist,
            transform.position + -transform.up * rayOriginHeight + transform.forward * safetyRayDist + -transform.up * allowJumpDist,
            CanJump() == true ? Color.cyan : Color.yellow);
        Debug.DrawLine(
            transform.position + -transform.up * rayOriginHeight + -transform.forward * safetyRayDist,
            transform.position + -transform.up * rayOriginHeight + -transform.forward * safetyRayDist + -transform.up * allowJumpDist,
            CanJump() == true ? Color.cyan : Color.yellow);

        //main ground test ray
        Debug.DrawLine(transform.position + -transform.up * rayOriginHeight,
            transform.position + -transform.up * rayOriginHeight + -transform.up * groundRayDist,
            IsGrounded() == true ? Color.green : Color.red);

        //safety rays
        Debug.DrawLine(
            transform.position + -transform.up * rayOriginHeight + transform.forward * safetyRayDist,
            transform.position + -transform.up * rayOriginHeight + transform.forward * safetyRayDist + -transform.up * groundRayDist,
            IsGrounded() == true ? Color.green : Color.red);
        Debug.DrawLine(
            transform.position + -transform.up * rayOriginHeight + -transform.forward * safetyRayDist,
            transform.position + -transform.up * rayOriginHeight + -transform.forward * safetyRayDist + -transform.up * groundRayDist,
            IsGrounded() == true ? Color.green : Color.red);

        //player movement (x10 for easier viewing)
        Debug.DrawLine(transform.position, transform.position + movement * 10, Color.red);
    }
}