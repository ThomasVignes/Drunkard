using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    [Header("State Machine")]
    [SerializeField] private bool Moving;
    [SerializeField] private bool Grounded;
    public bool CanMove;
    public bool Stumbling;
    public bool Drinking;
    [SerializeField] private GameObject GrabbedObject;
    [SerializeField] private GrabObject GrabRefObject;

    [Header("Settings")]
    [SerializeField] private float MoveForce;
    [SerializeField] private float StumbleForce;
    private float Speed;
    [SerializeField] private float MinimumJumpSwipe;
    [SerializeField] private float JumpWindow;
    private float JumpTimer;
    [SerializeField] private float JumpForce, AirForce;
    [SerializeField] private float TurnDelay;
    private float TurnTimer;
    private float CurrentAngle;
    [SerializeField] private float UpForce;
    [SerializeField] private float DownForce;
    [SerializeField] LayerMask WhatIsGround;
    [SerializeField] LayerMask Grabbable;

    [Header("References")]
    [SerializeField] private Transform GrabSpot;
    [SerializeField] private GameObject Pivot;
    public GameObject Pelvis;
    [SerializeField] private RigBuilder Rig;
    [SerializeField] private GameObject Rsts;
    [SerializeField] private GameObject Compensators;
    [SerializeField] private TwoBoneIKConstraint RarmIK;
    [SerializeField] private MultiAimConstraint HeadLookAt;
    [SerializeField] private ConfigurableJoint pelvisJoint;
    [SerializeField] private List<Rigidbody> MovingParts = new List<Rigidbody>();
    [SerializeField] private List<Rigidbody> JumpingParts = new List<Rigidbody>();
    [SerializeField] private List<Rigidbody> UpParts = new List<Rigidbody>();
    [SerializeField] private List<Rigidbody> DownParts = new List<Rigidbody>();
    [SerializeField] private LegStepper leftLegStepper;
    [SerializeField] private LegStepper rightLegStepper;
    [SerializeField] private Transform checkUp;
    [SerializeField] private Transform checkDown;

    public Vector3 Dir;
    private Vector3 forward, right;

    public LayerMask GrabbableLayer
    {
        get { return Grabbable; }
        set { Grabbable = value; }
    }

    private Vector2 startTouchPos, endTouchPos;

    private float ZInput, XInput;

    void Awake()
    {
        Instance = this;
        StartCoroutine(LegUpdateCoroutine());
    }

    private void Start()
    {
        Speed = MoveForce;
        Rig.enabled = false;
    }

    private void Update()
    {
        //Grounded = Physics.CheckSphere(Pivot.transform.position, 0.2f, WhatIsGround);
        Grounded = Physics.CheckCapsule(checkDown.position, checkUp.position, 0.7f, WhatIsGround);

        if (Rsts.activeSelf == Grounded)
            Rsts.SetActive(!Grounded);

        if (Compensators.activeSelf != Grounded)
            Compensators.SetActive(Grounded);

        InitializeMoveDir();

        Inputs();

        Move();

        if (Input.GetButtonDown("Drink"))
        {
            Drinking = !Drinking;
            Rig.enabled = Drinking;
        }

        if (Drinking)
        {
            HeadLookAt.weight = Mathf.Lerp(HeadLookAt.weight, 0.8f, 0.03f);
            RarmIK.weight = Mathf.Lerp(RarmIK.weight, 1f, 0.03f);
        }
        else
        {
            HeadLookAt.weight = Mathf.Lerp(HeadLookAt.weight, 0, 0.07f);
            RarmIK.weight = Mathf.Lerp(RarmIK.weight, 0, 0.07f);
        }
    }

    void FixedUpdate()
    {
        if (Moving && Grounded)
        {
            foreach (Rigidbody r in MovingParts)
            {
                r.AddForce(Dir * Speed);
            }
        }
        else
        {
            foreach (Rigidbody r in MovingParts)
            {
                r.AddForce(Dir * AirForce);
            }
        }

        if (Grounded)
        {
            foreach (Rigidbody r in UpParts)
            {
                r.AddForce(Vector3.up * UpForce);
            }

            foreach (Rigidbody r in DownParts)
            {
                r.AddForce(Vector3.down * DownForce);
            }
        }
    }
    private void Inputs()
    {
        Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 inputDir = Vector2.zero ;

        if (Grounded)
        {
            if (JumpTimer < JumpWindow)
            {
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                    startTouchPos = Input.GetTouch(0).position;
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    endTouchPos = Input.GetTouch(0).position;
                    if (Vector2.Distance(startTouchPos, endTouchPos) > MinimumJumpSwipe && CanMove)
                    {
                        Vector2 jumpInput = (endTouchPos - startTouchPos).normalized;

                        Vector3 jumpDir = Vector3.Normalize(forward * jumpInput.y + right * jumpInput.x);

                        foreach (Rigidbody r in JumpingParts)
                        {
                            r.AddForce(Vector3.up * JumpForce);
                            r.AddForce(jumpDir * JumpForce / 4);
                        }
                    }
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                foreach (Rigidbody r in JumpingParts)
                {
                    r.AddForce(Vector3.up * JumpForce);
                    r.AddForce(Dir * JumpForce / 4);
                }
            }

            if (Input.GetMouseButton(0))
            {
                CheckForGrabs();

                if (JumpTimer < JumpWindow)
                {
                    JumpTimer += Time.deltaTime;
                }
                inputDir = new Vector2(Input.mousePosition.x - center.x, Input.mousePosition.y - center.y);
            }
            else
            {
                if (GrabbedObject != null)
                {
                    GrabbedObject.transform.parent = null;
                    foreach (var rb in GrabRefObject.Rbs)
                    {
                        rb.isKinematic = false;
                    }
                    foreach (var c in GrabRefObject.Cols)
                    {
                        c.enabled = true;
                    }
                    GrabbedObject = null;
                    GrabRefObject = null;
                }

                JumpTimer = 0;
            }
        }
        else
        {
            JumpTimer = 0;
        }


        Vector2 input = inputDir.normalized;

        ZInput = input.y;
        XInput = input.x;

        Dir = Vector3.Normalize(forward * ZInput + right * XInput);

        if (Dir.magnitude >= 0.1f && CanMove)
        {
            Moving = true;

            if (Stumbling)
                Speed = Mathf.Lerp(Speed, StumbleForce, 0.07f);
            else
                Speed = Mathf.Lerp(Speed, MoveForce, 0.07f);
        }
        else
        {
            Moving = false;
        }
    }

    public void Move()
    {
        if (Moving)
        {
            float targetAngle = Mathf.Atan2(-Dir.z, -Dir.x) * Mathf.Rad2Deg;

            var targetQuaternion = Quaternion.Euler(0f, targetAngle, 0f);
            var angleDiff = Quaternion.Angle(pelvisJoint.targetRotation, targetQuaternion);

            if (angleDiff > 15)
            {
                if (Grounded)
                {
                    if (Mathf.Abs(angleDiff - CurrentAngle) > 10)
                    {
                        TurnTimer = TurnDelay;
                        CurrentAngle = angleDiff;
                    }

                    if (TurnTimer > 0)
                    {
                        TurnTimer -= Time.deltaTime;
                    }
                    else
                    {
                        TurnTimer = 0;
                        pelvisJoint.targetRotation = targetQuaternion;
                    }

                    Stumbling = TurnTimer > 0;
                }
                else
                {
                    pelvisJoint.targetRotation = targetQuaternion;
                    Stumbling = false;
                }
            }
        }
    }

    public void CheckForGrabs()
    {
        if (GrabbedObject == null)
        {
            Collider[] GrabObjects = Physics.OverlapCapsule(checkDown.position, checkUp.position, 1.7f, Grabbable);
            if (GrabObjects.Length > 0)
            {
                Vector3 closestVector = Quaternion.Euler(0, -180, 0) * transform.forward;

                foreach (var Go in GrabObjects)
                {
                    Vector3 itemVector = Go.gameObject.transform.position - transform.position;
                    if (Vector3.Angle(transform.forward, itemVector) < Vector3.Angle(transform.forward, closestVector))
                    {
                        closestVector = itemVector;

                        Clickable go = Go.gameObject.GetComponent<Clickable>();
                        if (go != null)
                        {
                            if (go.Selected)
                            {
                                if (go.gameObject.GetComponent<GrabObject>())
                                {
                                    GrabRefObject = go.gameObject.GetComponent<GrabObject>();
                                    GrabbedObject = GrabRefObject.Object;
                                    foreach (var rb in GrabRefObject.Rbs)
                                    {
                                        rb.isKinematic = true;
                                        rb.velocity = Vector3.zero;
                                    }
                                    foreach (var c in GrabRefObject.Cols)
                                    {
                                        c.enabled = false;
                                    }

                                    GrabbedObject.transform.position = GrabSpot.position;
                                    GrabbedObject.transform.rotation = GrabSpot.rotation;
                                    GrabbedObject.transform.parent = GrabSpot;
                                }

                                if (go.gameObject.GetComponent<CarClickCollider>())
                                {
                                    CarClickCollider Car = go.gameObject.GetComponent<CarClickCollider>();
                                    CanMove = false;
                                    Car.Sitting = true;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    IEnumerator LegUpdateCoroutine()
    {
        // Run continuously
        while (true)
        {
            do
            {
                leftLegStepper.CanStep = Grounded;
                if (Moving && Grounded)
                    leftLegStepper.TryMove();

                yield return null;

            } while (leftLegStepper.Moving);


            do
            {
                rightLegStepper.CanStep = Grounded;
                if (Moving && Grounded)
                    rightLegStepper.TryMove();

                yield return null;

            } while (rightLegStepper.Moving);
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
