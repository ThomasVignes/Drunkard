using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Interior : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool IsIn;
    [SerializeField] private float FadeAmount;
    [SerializeField] private LayerMask Player;
    [Header("References")]
    [SerializeField] private List<MeshRenderer> Exterior = new List<MeshRenderer>();
    [SerializeField] private List<SpriteRenderer> Sprites = new List<SpriteRenderer>();
    [SerializeField] private Material FadeMat;

    private List<Material> InitialMats = new List<Material>();

    private void Start()
    {
        for (int i = 0; i < Exterior.Count; i++)
        {
            InitialMats.Add(Exterior[i].material);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsIn && other.gameObject.layer == ToLayer(Player))
        {
            IsIn = true;

            for (int i = 0; i < Exterior.Count; i++)
            {
                Exterior[i].material = FadeMat;
                Exterior[i].material.DOFade(FadeAmount, 0.2f);
            }

            foreach(SpriteRenderer s in Sprites)
                s.DOFade(FadeAmount, 0.2f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsIn && other.gameObject.layer == ToLayer(Player))
        {
            IsIn = false;
            for (int i = 0; i < Exterior.Count; i++)
            {
                Exterior[i].material.DOFade(0.8f, 0.2f);
                Exterior[i].material = InitialMats[i];
            }

            foreach (SpriteRenderer s in Sprites)
                s.DOFade(1f, 0.2f);
        }
    }

    public static int ToLayer(int bitmask)
    {
        int result = bitmask > 0 ? 0 : 31;
        while (bitmask > 1)
        {
            bitmask = bitmask >> 1;
            result++;
        }
        return result;
    }
}
