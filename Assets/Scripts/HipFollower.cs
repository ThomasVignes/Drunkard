using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HipFollower : MonoBehaviour
{
    [SerializeField] private PlayerController Player;
    [SerializeField] private GameObject Hip;

    void FixedUpdate()
    {
        transform.position = Hip.transform.position;

        if (Player != null)
        {
            if (!Player.Stumbling && Player.Dir.magnitude >= 0.1f)
                transform.forward = Player.Dir;
        }
    }
}
