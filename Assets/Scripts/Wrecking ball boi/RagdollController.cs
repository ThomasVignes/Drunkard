using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [SerializeField] private bool startAsRagdoll;
    [SerializeField] private CapsuleCollider MainCollider;
    [SerializeField] private Rigidbody MainRb;
    [SerializeField] private Animator animator;

    [SerializeField] private List<Collider> RagdollParts = new List<Collider>();

    [SerializeField] private GameObject ball;
    // Start is called before the first frame update
    void Awake()
    {
        SetRagdollParts();
        MainCollider.enabled = true;
        if (startAsRagdoll)
        {
            TurnOnRagdoll();
        }
    }

    void SetRagdollParts()
    {
        Collider[] colliders = this.gameObject.GetComponentsInChildren<Collider>();

        foreach (Collider c in colliders)
        {
            if (c.gameObject != this.gameObject)
            {
                c.isTrigger = true;
                c.attachedRigidbody.useGravity = false;
                RagdollParts.Add(c);
            }
        }
    }

    public void TurnOnRagdoll()
    {
        MainRb.useGravity = false;
        MainRb.velocity = Vector3.zero;
        MainCollider.enabled = false;
        animator.enabled = false;
        animator.avatar = null;

        foreach (Collider c in RagdollParts)
        {
            if (c.gameObject != this.gameObject)
            {
                c.isTrigger = false;
                c.attachedRigidbody.velocity = Vector3.zero;
                c.attachedRigidbody.useGravity = true;
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject == ball)
        {
            TurnOnRagdoll();
        }
    }
}
