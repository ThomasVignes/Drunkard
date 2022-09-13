using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceHolderEffect : QuestEffect
{
    [SerializeField] private string TextFinalEffect;
    [SerializeField] private Text text;


    public override void Effect()
    {
        text.text = TextFinalEffect;
    }
}
