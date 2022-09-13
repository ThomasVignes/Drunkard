using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public bool CanMove, Moving, Grounded, Turning;
    [SerializeField] private float MoveForce;
    [SerializeField] private float TurnRate;
    [SerializeField] private float TurnLimit;
    [SerializeField] private LayerMask WhatIsGround;
    [SerializeField] private float RollSpeed;
    [SerializeField] private List<Transform> Wheels = new List<Transform>();
    [SerializeField] private Transform groundCheckForward, groundCheckBack;
    [SerializeField] private GameObject Lights;
    
    Vector3 Dir, forward, right;
    float ZInput, XInput, Speed;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        InitializeMoveDir();

        if (Lights.activeSelf != CanMove)
            Lights.SetActive(CanMove);

        Grounded = Physics.CheckCapsule(groundCheckForward.position, groundCheckBack.position, 0.1f, WhatIsGround);

        Inputs();


        if (Moving && Grounded)
        {
            transform.forward = Vector3.Lerp(transform.forward, Dir, TurnRate);

            Turning = Vector3.Angle(transform.forward, Dir) > TurnLimit;

            foreach (var v in Wheels)
            {
                v.Rotate(RollSpeed * Time.deltaTime, 0, 0);
            }
        }
    }

    private void FixedUpdate()
    {
        if (Grounded)
        {
            if (!Turning)
                rb.AddForce(transform.forward * Speed);
            else
                rb.AddForce(-transform.forward * Speed);
        }
    }

    private void Inputs()
    {
        Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 inputDir = Vector2.zero;

        if (Grounded)
        {
            if (Input.GetMouseButton(0))
            {
                inputDir = new Vector2(Input.mousePosition.x - center.x, Input.mousePosition.y - center.y);
            }
        }

        Vector2 input = inputDir.normalized;

        ZInput = input.y;
        XInput = input.x;

        Dir = Vector3.Normalize(forward * ZInput + right * XInput);

        if (Dir.magnitude >= 0.1f && CanMove)
        {
            Moving = true;

            Speed = Mathf.Lerp(Speed, MoveForce, 0.07f);
        }
        else
        {
            Speed = Mathf.Lerp(Speed, 0f, 0.07f);
            Moving = false;
        }
    }

    private void InitializeMoveDir()
    {
        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
    }
}
