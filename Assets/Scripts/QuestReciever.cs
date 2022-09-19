using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestReciever : MonoBehaviour
{
    public Action Done, Out;
    [SerializeField] private string TargetID;
    public bool HasExitEffect;
    private bool AlreadyDone;


    private void OnTriggerEnter(Collider other)
    {
        QuestObject Qo = other.gameObject.GetComponent<QuestObject>();
        if (HasExitEffect)
        {
            if (Qo != null)
            {
                if (Qo.ID == TargetID)
                {
                    Done?.Invoke();
                }
            }
        }
        else
        {
            if (!AlreadyDone && Qo != null)
            {
                if (Qo.ID == TargetID)
                {
                    Done?.Invoke();
                    AlreadyDone = true;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        QuestObject Qo = other.gameObject.GetComponent<QuestObject>();
        if (HasExitEffect)
        {
            if (Qo != null)
            {
                if (Qo.ID == TargetID)
                {
                    Out?.Invoke();
                }
            }
        }
    }
}
