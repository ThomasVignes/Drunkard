using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomEffect : QuestEffect
{
    [SerializeField] private Transform BoomSpot;
    public override void Effect()
    {
        Collider[] cols = Physics.OverlapSphere(BoomSpot.position, 2f);

        foreach (var c in cols)
        {
            if (c.attachedRigidbody != null)
                c.attachedRigidbody.AddExplosionForce(50000f, BoomSpot.position, 2f);
        }
    }
}
