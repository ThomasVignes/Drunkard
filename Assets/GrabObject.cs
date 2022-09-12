using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObject : Clickable
{
    public GameObject Object;
    public List<Collider> Cols = new List<Collider>();
    public List<Rigidbody> Rbs = new List<Rigidbody>();

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
            Selected = false;
    }
}
