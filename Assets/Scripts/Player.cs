using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float runSpeed;
    public float jumpPower;
    public float jumpForceDuration;
    public float gravityPower;
    public float groundDist;
    public float safetyRayDist;

    CharacterController player;
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

        if (IsGrounded())
        {
            Debug.DrawLine(transform.position, transform.position + -transform.up * groundDist, Color.green);
            Debug.DrawLine(
                transform.position + new Vector3(0, 0, safetyRayDist),
                transform.position + new Vector3(0, 0, safetyRayDist) + -transform.up * groundDist,
                Color.green);
            Debug.DrawLine(
                transform.position + new Vector3(0, 0, -safetyRayDist),
                transform.position + new Vector3(0, 0, -safetyRayDist) + -transform.up * groundDist,
                Color.green);
        }
        else
        {
            Debug.DrawLine(transform.position, transform.position + -transform.up * groundDist, Color.red);
            Debug.DrawLine(
                transform.position + new Vector3(0, 0, safetyRayDist),
                transform.position + new Vector3(0, 0, safetyRayDist) + -transform.up * groundDist,
                Color.red);
            Debug.DrawLine(
                transform.position + new Vector3(0, 0, -safetyRayDist),
                transform.position + new Vector3(0, 0, -safetyRayDist) + -transform.up * groundDist,
                Color.red);
        }
    }

    private void FixedUpdate()
    {
        movement = Vector3.zero;

        PlayerForces();
        AutoForces();
        Move();
    }

    Touch touch;
    Vector3 startTouchPos;
    Vector3 currentTouchPos;
    bool hasTapped = false;
    float direction = 0;
    float touchTimer = 0;
    public float tapTimeLimit = 0.01f;

    void PlayerForces()
    {
        if (allowInput)
        {
            //
            //  MOVEMENT
            //

            // keyboard controls
            if (Input.GetAxis("Horizontal") > 0)
            {
                movement += transform.forward * runSpeed * Time.fixedDeltaTime;
            }

            if (Input.GetAxis("Horizontal") < 0)
            {
                movement += -transform.forward * runSpeed * Time.fixedDeltaTime;
            }

            // touch controls
            if (Input.touchCount > 0) //at least one finger currently touching screen
            {
                touch = Input.GetTouch(0); //update touch info
                touchTimer += Time.fixedDeltaTime; //time how long touch occurs for

                if (!hasTapped) //first frame of current touch
                {
                    hasTapped = true;
                    startTouchPos = touch.position;
                }
            }
            else //no fingers touching screen
            {
                if (touchTimer < tapTimeLimit && hasTapped == true)
                    ActivateJump();

                startTouchPos = Vector2.zero; //reset touch positions
                currentTouchPos = Vector2.zero;
                hasTapped = false; //no longer touching screen
                touchTimer = 0; //reset touch timer
            }

            if (hasTapped) //ongoing touch frames
            {
                //movement
                currentTouchPos = touch.position;
                Vector2 dirVec = currentTouchPos - startTouchPos;
                direction = Vector2.Dot(Vector2.right, new Vector2(dirVec.normalized.x, 0));
                movement += transform.forward * direction * runSpeed * Time.fixedDeltaTime;
            }

            //
            //  JUMPING
            //

            if (Input.GetButtonDown("Jump") && IsGrounded())
            {
                ActivateJump();
            }

            if (jumpTimer > 0)
            {
                jumpTimer -= Time.fixedDeltaTime;
                movement += transform.up * jumpPower;
                gravityForce = Vector3.zero;
            }
        }
        else ReactivateControl();
    }

    //checks for no more input before allowing input again
    //keeps player from moving immediately after respawn
    void ReactivateControl()
    {
        if (Input.GetAxis("Horizontal") == 0 || Input.touchCount == 0)
            allowInput = true;
    }

    void ActivateJump()
    {
        jumpTimer = jumpForceDuration;
        gravityForce = Vector3.zero;
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

    void Respawn()
    {
        movement = Vector3.zero;
        gravityForce = Vector3.zero;
        jumpTimer = 0;
        allowInput = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    bool IsGrounded()
    {
        if (Physics.Raycast(transform.position, -transform.up, groundDist, mask) ||
            Physics.Raycast(transform.position + new Vector3(0, 0, safetyRayDist), -transform.up, groundDist, mask) ||
            Physics.Raycast(transform.position + new Vector3(0, 0, -safetyRayDist), -transform.up, groundDist, mask))
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

            if (Input.GetAxis("Horizontal") > 0 || direction > 0)
                transform.Rotate(new Vector3(0, 1, 0), -90);

            if (Input.GetAxis("Horizontal") < 0 || direction < 0)
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("creb");
            Respawn();
        }
    }
}