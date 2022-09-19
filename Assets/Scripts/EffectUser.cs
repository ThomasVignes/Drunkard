using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectUser : MonoBehaviour
{
    [SerializeField] private QuestReciever Reciever;
    [SerializeField] private List<QuestEffect> Effects = new List<QuestEffect>();
    [SerializeField] private List<QuestEffect> OutEffects = new List<QuestEffect>();

    private void Start()
    {
        Reciever.Done += () => AllEffects();

        if (Reciever.HasExitEffect)
            Reciever.Out += () => AllOutEffects();
    }

    private void AllEffects()
    {
        foreach (var e in Effects)
        {
            e.Effect();
        }
    }

    private void AllOutEffects()
    {
        foreach (var e in OutEffects)
        {
            e.Effect();
        }
    }
}
