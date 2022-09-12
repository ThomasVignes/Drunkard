using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    [SerializeField] private GameObject canvas;
    [SerializeField] private Text text;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        canvas.SetActive(false);
    }

    public void ShowDialogue(string dialogue)
    {
        StopAllCoroutines();
        StartCoroutine(ShowText(dialogue));
    }

    IEnumerator ShowText(string newText)
    {
        PlayerController.Instance.CanMove = false;
        text.text = newText;

        yield return new WaitForSeconds(0.5f);
        canvas.SetActive(true);
        yield return new WaitForSeconds(2f);
        canvas.SetActive(false);
        PlayerController.Instance.CanMove = true;
    }
}
