using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageRemote : MonoBehaviour
{
    [SerializeField] private Material GlowMat;
    [SerializeField] private MeshRenderer Renderer;
    private Material OriginalMat;

    private void Start()
    {
        OriginalMat = Renderer.material;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayAnimEffect>())
            Renderer.material = GlowMat;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayAnimEffect>())
            Renderer.material = OriginalMat;
    }
}
