using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{

    //Mouse Input
    private float mouseX, mouseY;

    //Cam rotations
    private float desiredYaw, desiredPitch, yaw, pitch, headTilt;
    [SerializeField] private float roll = 1f;
    [SerializeField] private Vector2 sens = Vector2.zero;
    [SerializeField] private Vector2 smoothAmount = Vector2.zero;

    //Ref to gameObject that holds the cam
    private Transform pitchTransform;

    [SerializeField] private Transform playerTransform;


    //Movement Input
    Vector2 movementInput;


    //Headbob variables
    [SerializeField, Range(0, 0.1f)] private float amplitude = 0.015f;
    [SerializeField, Range(0, 30)] private float frequency = 10f;
    private Vector3 cameraStartPos;
    private bool moving;


    //Drop Headbob Variables
    [SerializeField] private GameObject playerObject;
    private Movement movementScript;
    private bool grounded;
    private bool bobbing;
    private Vector3 lastPos;
    [SerializeField] private float bobDepth;
    [SerializeField] private float bobLength;
    [SerializeField] private float maxBob;

    public Transform footstep;


    /// <summary>
    /// Input Calls
    /// </summary>
    public void OnLook(InputValue value)
    {

        Vector2 mouseInput = value.Get<Vector2>();

        mouseX = mouseInput.x;
        mouseY = mouseInput.y;
    }

    public void OnMovement(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }


    /// <summary>
    /// Function Calls
    /// </summary>
    private void Awake()
    {
        GetComponents();
        InitValues();
        CursorSettings();
    }

    private void LateUpdate()
    {
        //Look Functions
        CalculateRotation();
        SmoothRotation();
        ApplyRotation();

        //Headbob Functions
        HeadBob();
        ResetCam();

        /** Jump Functions
        CheckGrounded();
        LandBob();
        RecordPos();
        **/
    }


    /// <summary>
    /// Initial Components and Values
    /// </summary>
    void GetComponents()
    {
        pitchTransform = transform.GetChild(0).transform;
        cameraStartPos = transform.localPosition;
        movementScript = playerObject.GetComponent<Movement>();
    }

    void InitValues()
    {
        yaw = playerTransform.eulerAngles.y;
        desiredYaw = mouseX;

        moving = false;
    }

    void CursorSettings()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    /// <summary>
    /// Look functions
    /// </summary>
    void CalculateRotation()
    {
        desiredYaw += mouseX * sens.x * Time.deltaTime;
        desiredPitch -= mouseY * sens.y * Time.deltaTime;

        desiredPitch = Mathf.Clamp(desiredPitch, -90, 90);
    }

    void SmoothRotation()
    {
        yaw = Mathf.Lerp(yaw, desiredYaw, smoothAmount.x * Time.deltaTime);
        pitch = Mathf.Lerp(pitch, desiredPitch, smoothAmount.y * Time.deltaTime);
    }

    void ApplyRotation()
    {

        //Head Sway while moving sideways ----- If statement can be removed if you want head to tilt whenever you press right or left,
        //but rn itll only tilt if youre moving only sideways
        if (movementInput.y == 0)
        {
            headTilt = Mathf.Lerp(headTilt, -movementInput.x * roll, Time.deltaTime * 10);
        }
        else
        {
            headTilt = Mathf.Lerp(headTilt, 0, Time.deltaTime * 10);
        }


        playerTransform.eulerAngles = new Vector3(0f, yaw, 0f);
        pitchTransform.localEulerAngles = new Vector3(pitch, 0f, headTilt);
    }


    /// <summary>
    /// Headbob Functions
    /// </summary>
    void HeadBob()
    {
        if (movementInput != Vector2.zero && !grounded && !bobbing)
        {
            Vector3 currentPos = Vector3.zero;
            currentPos.y += Mathf.Sin(Time.time * frequency) * amplitude;
            currentPos.x += Mathf.Cos(Time.time * frequency / 2) * amplitude * 2;
            transform.localPosition = currentPos + cameraStartPos;
            moving = true;

            if (Mathf.Sin(Time.time * frequency) <= -0.95 && transform != null && footstep != null)
            {
                for (int i = 0; i < footstep.childCount; i++)
                {
                    if (footstep.GetChild(i).GetComponent<AudioSource>().isPlaying) return;
                }
                footstep.GetChild(Random.Range(0, 3)).GetComponent<AudioSource>().Play();
            }
        }
        else
        {
            moving = false;
        }
    }


    //Resets the cam after headbob
    void ResetCam()
    {
        if ((transform.localPosition != cameraStartPos) && !moving && !bobbing)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, cameraStartPos, Time.deltaTime);
        }
    }


    /// <summary>
    /// Landing Functions
    /// </summary>
    void CheckGrounded()
    {
        grounded = movementScript.grounded;
    }

    void LandBob()
    {
        if ((lastPos.y != playerTransform.transform.position.y) && grounded)
        {
            bobbing = true;
            float posDiff = (playerTransform.transform.position.y - lastPos.y) * 0.5f;
            posDiff = Mathf.Clamp(posDiff, 0, -maxBob);
            Vector3 bobPosition = new Vector3(transform.localPosition.x, lastPos.y - posDiff, transform.localPosition.z);

            transform.localPosition = Vector3.Slerp(transform.localPosition, bobPosition, Time.deltaTime);
        }
        else
        {
            bobbing = false;
        }
    }

    void RecordPos()
    {
        if ((playerObject.GetComponent<CharacterController>().velocity.y == 0) && !bobbing)
        {
            lastPos = playerTransform.transform.position;
        }
    }
}
