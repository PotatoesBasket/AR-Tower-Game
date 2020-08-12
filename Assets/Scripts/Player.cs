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

        PlayerForces();
    }

    private void FixedUpdate()
    {
        AutoForces();
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
                movement += transform.forward * runSpeed * Time.deltaTime;
            }

            if (Input.GetAxis("Horizontal") < 0)
            {
                movement += -transform.forward * runSpeed * Time.deltaTime;
            }

            // move via touch + drag motion
            if (touch1.IsTouching)
            {
                movement += transform.forward * touch1.direction * runSpeed * Time.deltaTime;
            }
            if (touch2.IsTouching)
            {
                movement += transform.forward * touch2.direction * runSpeed * Time.deltaTime;
            }

            //
            //  JUMPING
            //

            // jump info
            if (jumpTimer > 0)
            {
                jumpTimer -= Time.deltaTime;
                movement += transform.up * jumpPower;
                gravityForce = Vector3.zero;
            }

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
            gravityForce += -transform.up * gravityPower * Time.fixedDeltaTime;
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

    public bool IsGrounded()
    {
        if (Physics.Raycast(transform.position, -transform.up, groundRayDist, mask) ||
            Physics.Raycast(transform.position + transform.forward * safetyRayDist, -transform.up, groundRayDist, mask) ||
            Physics.Raycast(transform.position + -transform.forward * safetyRayDist, -transform.up, groundRayDist, mask))
            return true;
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
        else if (other.CompareTag("Death"))
            Respawn();
        else if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
            jumpTimer = jumpForceDuration;
        }
    }

    //// uncomment to allow enemies to kill again
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Enemy"))
    //    {
    //        Debug.Log("creb");
    //        Respawn();
    //    }
    //}

    private void OnDrawGizmos()
    {
        //main ground test ray
        Debug.DrawLine(transform.position, transform.position + -transform.up * groundRayDist,
            IsGrounded() == true ? Color.green : Color.red);

        //safety rays
        Debug.DrawLine(
            transform.position + transform.forward * safetyRayDist,
            transform.position + transform.forward * safetyRayDist + -transform.up * groundRayDist,
            IsGrounded() == true ? Color.green : Color.red);
        Debug.DrawLine(
            transform.position + -transform.forward * safetyRayDist,
            transform.position + -transform.forward * safetyRayDist + -transform.up * groundRayDist,
            IsGrounded() == true ? Color.green : Color.red);

        //player movement (x10 for easier viewing)
        Debug.DrawLine(transform.position, transform.position + movement, Color.red);
    }
}