using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Player movement options")]
    public float runSpeed;
    public float jumpPower;
    public float jumpForceDuration;
    public float gravityPower;

    [Header("Ground detection options")]
    public float rayOriginHeight;
    public float groundRayDist;
    public float safetyRayDist;

    [Header("Player sound effects")]
    public AudioClip jumpSFX;
    public AudioClip takeDamageSFX;

    [Header("Touch info, probably don't touch")]
    public TouchInfo touch1;
    public TouchInfo touch2;

    [Header("Gameplay test camera")]
    public GameObject altCamera;

    CharacterController player;
    AudioSource playerSFX;

    Vector3 startPos;
    Quaternion startRot;

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

        startPos = transform.position;
        startRot = transform.rotation;
        mask = LayerMask.GetMask("Tower");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        UpdateRays();
        PlayerForces();
        AutoForces();
    }

    private void FixedUpdate()
    {
        Move();

        movement = Vector3.zero;
    }

    void PlayerForces()
    {
        if (allowInput)
        {
            // update touch info
            if (Input.touchCount > 0)
                touch1.touch = Input.GetTouch(0);

            if (Input.touchCount > 1)
                touch2.touch = Input.GetTouch(1);

            //
            //  MOVEMENT
            //

            // move via axis input
            if (Input.GetAxis("Horizontal") > 0)
            {
                movement += transform.forward * runSpeed * 0.01f;
            }

            if (Input.GetAxis("Horizontal") < 0)
            {
                movement += -transform.forward * runSpeed * 0.01f;
            }

            // move via touch + drag motion
            if (touch1.IsTouching)
            {
                movement += transform.forward * touch1.direction * runSpeed * 0.01f;
            }
            if (touch2.IsTouching)
            {
                movement += transform.forward * touch2.direction * runSpeed * 0.01f;
            }

            //
            //  JUMPING
            //

            // jump via jump button
            if (Input.GetButtonDown("Jump") && IsGrounded())
            {
                ActivateJump();
            }

            // jump via tapping motion
            if (touch1.activateTap)
            {
                touch1.activateTap = false;

                if (IsGrounded())
                    ActivateJump();
            }
            if (touch2.activateTap)
            {
                touch2.activateTap = false;

                if (IsGrounded())
                    ActivateJump();
            }

            // jump info
            if (jumpTimer > 0)
            {
                movement += transform.up * jumpPower * Mathf.Min(jumpTimer, Time.deltaTime);
                gravityForce = Vector3.zero;
            }

            jumpTimer -= Time.deltaTime;
        }
        else ReactivateControl();
    }

    // checks for no more input before allowing input again
    // keeps player from moving immediately after respawn
    void ReactivateControl()
    {
        if (Input.GetAxis("Horizontal") == 0 || Input.touchCount == 0)
            allowInput = true;
    }

    void ActivateJump()
    {
        jumpTimer = jumpForceDuration;
        gravityForce = Vector3.zero;

        if (jumpSFX && !GameOptions.Instance.MuteSFX)
            playerSFX.PlayOneShot(jumpSFX);
    }

    void AutoForces()
    {
        if (IsGrounded())
            gravityForce = Vector3.zero;
        else
            gravityForce += -transform.up * gravityPower * 0.01f;
    }

    void Move()
    {
        player.Move(movement + gravityForce);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Corner"))
        {
            transform.position = new Vector3(
                other.transform.position.x,
                transform.position.y,
                other.transform.position.z);

            if (Input.GetAxis("Horizontal") > 0 || touch1.direction > 0 || touch2.direction > 0)
                transform.Rotate(new Vector3(0, 1, 0), -90);

            if (Input.GetAxis("Horizontal") < 0 || touch1.direction < 0 || touch2.direction < 0)
                transform.Rotate(new Vector3(0, 1, 0), 90);
        }
        
        if (other.CompareTag("Death"))
            Respawn();
        
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
            jumpTimer = jumpForceDuration;
        }
    }

    private void OnDrawGizmos()
    {
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
            transform.position + -transform.up * rayOriginHeight  + -transform.forward * safetyRayDist,
            transform.position + -transform.up * rayOriginHeight  + -transform.forward * safetyRayDist + -transform.up * groundRayDist,
            IsGrounded() == true ? Color.green : Color.red);

        //player movement (x10 for easier viewing)
        Debug.DrawLine(transform.position, transform.position + movement, Color.red);
    }
}