using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseShit : MonoBehaviour
{
    private PlayerController Player;

    private void Start()
    {
        Player = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), Mathf.Infinity, Player.GrabbableLayer);
            foreach (RaycastHit hit in hits)
            {
                Clickable Go = hit.collider.gameObject.GetComponent<Clickable>();
                if (Go != null)
                {
                    Go.Selected = true;
                }
            }
        }
    }
}
