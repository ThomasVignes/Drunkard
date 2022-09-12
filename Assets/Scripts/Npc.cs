using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : MonoBehaviour
{
    [SerializeField] private LayerMask Player;
    [SerializeField] private List<string> dialogueLines = new List<string>();
    private int currentDialogue = 0;

    [SerializeField] private float TalkCD;
    private float talkTimer;
    private bool Stop;
    

    bool DialogueOpen;

    private void Start()
    {
        DialogueOpen = true;
    }

    private void Update()
    {
        if (talkTimer > 0)
            talkTimer -= Time.deltaTime;
        else
            talkTimer = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == ToLayer(Player))
        {
            if (Mathf.Abs(collision.relativeVelocity.magnitude) > 3 && talkTimer == 0 && !Stop)
            {
                Talk();
                talkTimer = TalkCD;
            }
        }
    }

    private void Talk()
    {
        DialogueManager.Instance.ShowDialogue(dialogueLines[currentDialogue]);

        if (currentDialogue == dialogueLines.Count - 1)
            Stop = true;
        else
            currentDialogue++;
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
