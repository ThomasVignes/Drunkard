using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimEffect : QuestEffect
{
    [SerializeField] private Animator animator;
    [SerializeField] private string animTrigger;
    private float delay, timer;

    private void Start()
    {
        delay = 0.05f;
    }

    private void Update()
    {
        if (timer > 0)
            timer -= Time.deltaTime;
        else
        {
            if (timer != 0)
            {
                animator.ResetTrigger(animTrigger);
                timer = 0;
            }
        }
    }
    public override void Effect()
    {
        animator.SetTrigger(animTrigger);
        timer = delay;
    }
}
