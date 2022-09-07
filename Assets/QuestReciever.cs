using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestReciever : MonoBehaviour
{
    public Action Done;
    [SerializeField] private string TargetID;
    private bool AlreadyDone;

    private void OnTriggerEnter(Collider other)
    {
        QuestObject Qo = other.gameObject.GetComponent<QuestObject>();
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
