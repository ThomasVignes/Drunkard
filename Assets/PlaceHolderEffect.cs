using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceHolderEffect : QuestEffect
{
    [SerializeField] private string TextFinalEffect;
    [SerializeField] private QuestReciever Reciever;
    [SerializeField] private Text text;

    private void Start()
    {
        Reciever.Done += () => Effect();
    }

    public override void Effect()
    {
        text.text = TextFinalEffect;
    }
}
