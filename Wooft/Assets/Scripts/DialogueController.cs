using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance;

    public TMP_Text speakerText;
    public TMP_Text dialogueText;

    protected ConversationArgs currentArgs;
    protected Coroutine currentMessageRoutine;

    public class ConversationArgs
    {
        public string SpeakerName;
        public string DialogueMessage;
        public float DelayBetweenChars;
    }

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

    public void SnapConversation(ConversationArgs args)
    {
        speakerText.text = args.SpeakerName;
        dialogueText.text = args.DialogueMessage;
    }

    public void NextDialogue(ConversationArgs args)
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

    public IEnumerator ScrollConversation(ConversationArgs args)
    {
        yield return new WaitForSeconds(args.DelayBetweenChars);

        string currentMessage = string.Empty;

        speakerText.text = args.SpeakerName;

        foreach (char c in args.DialogueMessage)
        {
            currentMessage += c;
            dialogueText.text = currentMessage;

            yield return new WaitForSeconds(args.DelayBetweenChars);

        }

        currentMessageRoutine = null;

    }
}
