using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarClickCollider : Clickable
{
    public CarController Vehicle;
    public Transform SitSpot;
    public Transform Goodbye;
    public bool Sitting;
    [SerializeField] private GameObject OriginalCam;
    [SerializeField] private GameObject CarCam;

    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && Selected)
        {
            Selected = false;
            Sitting = false;
            Vehicle.CanMove = false;

            if (CarCam.activeSelf)
                CarCam.SetActive(false);
            if (!OriginalCam.activeSelf)
                OriginalCam.SetActive(true);

            PlayerController.Instance.Pelvis.transform.position = Goodbye.position;
            PlayerController.Instance.CanMove = true;
        }

        if (Sitting)
        {
            PlayerController.Instance.Pelvis.transform.position = SitSpot.position;
            Vehicle.CanMove = true;

            if (!CarCam.activeSelf)
                CarCam.SetActive(true);
            if (OriginalCam.activeSelf)
                OriginalCam.SetActive(false);
        }
    }
}
