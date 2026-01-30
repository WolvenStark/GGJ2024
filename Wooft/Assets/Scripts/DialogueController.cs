using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;



[System.Serializable]
public struct DialogueArgs
{
    public string SpeakerName;
    public string Message;
    public float DelayBetweenChars;

    public DialogueArgs(string speaker, string text, float delay = 0.005f)
    {
        this.SpeakerName = speaker;
        this.Message = text;
        this.DelayBetweenChars = delay;
    }
}

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance;

    public TMP_Text speakerText;
    public TMP_Text dialogueText;

    protected DialogueArgs currentArgs;
    protected Coroutine currentMessageRoutine;


    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void SnapConversation(DialogueArgs args)
    {
        speakerText.text = args.SpeakerName;
        dialogueText.text = args.Message;
    }

    public void NextDialogue(DialogueArgs args)
    {
        SnapConversation(currentArgs);

        if (currentMessageRoutine != null)
        {
            StopCoroutine(currentMessageRoutine);
            currentMessageRoutine = null;
        }

        currentArgs = args;
        currentMessageRoutine = StartCoroutine(ScrollConversation(args));
    }

    public IEnumerator ScrollConversation(DialogueArgs args)
    {
        yield return new WaitForSeconds(args.DelayBetweenChars);

        string currentMessage = string.Empty;

        speakerText.text = args.SpeakerName;

        foreach (char c in args.Message)
        {
            currentMessage += c;
            dialogueText.text = currentMessage;

            yield return new WaitForSeconds(args.DelayBetweenChars);

        }

        currentMessageRoutine = null;

    }
}
