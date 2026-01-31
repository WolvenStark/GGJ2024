using System.Collections;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance;

    public TMP_Text speakerText;
    public TMP_Text dialogueText;

    protected DialogueNode currentNodeArgs;
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

    public void Start()
    {
        // Load the first messages
        var node = DialogueReader.Instance.DialogueData.First().Value.First().Value[0];

        NextDialogue(DialogueReader.Instance.DialogueData.First().Value.First().Value[0]);
    }

    // Explicit convesion
    public void SnapConversation(DialogueNode node)
    {
        speakerText.text = node.ExtractSpeaker();
        dialogueText.text = node.ExtractMessage();
    }

    public void NextDialogue(DialogueNode node)
    {
        if (currentNodeArgs != null)
        {
            SnapConversation(currentNodeArgs);
        }

        if (currentMessageRoutine != null)
        {
            StopCoroutine(currentMessageRoutine);
            currentMessageRoutine = null;
        }

        currentNodeArgs = node;
        currentMessageRoutine = StartCoroutine(ScrollConversation(node));

        //if (node is DialogueTextNode text)
        //{
        //    ShowLine(text.line);
        //}
        //else if (node is DialogueChoiceNode choice)
        //{
        //    ShowChoices(choice);
        //    //break; // wait for player input
        //}
    }

    public IEnumerator ScrollConversation(DialogueNode args)
    {
        yield return new WaitForSeconds(args.DelayBetweenChars);

        string currentMessage = string.Empty;

        speakerText.text = args.ExtractSpeaker();

        foreach (char c in args.ExtractMessage())
        {
            currentMessage += c;
            dialogueText.text = currentMessage;

            yield return new WaitForSeconds(args.DelayBetweenChars);

        }

        currentMessageRoutine = null;
    }

    //void PlaySection(string file, string section)
    //{
    //    var nodes =  DialogueReader.Instance.DialogueData[file][section];

    //    foreach (DialogueNode node in nodes)
    //    {
    //        if (node is DialogueTextNode text)
    //        {
    //            ShowLine(text.line);
    //        }
    //        else if (node is DialogueChoiceNode choice)
    //        {
    //            ShowChoices(choice);
    //            break; // wait for player input
    //        }
    //    }
    //}

    //void OnChoiceSelected(string file, DialogueChoice choice)
    //{
    //    PlaySection(file, choice.nextSection);
    //}

    //void ShowLine(DialogueLine line)
    //{
    //    speakerText.text = line.speaker;
    //    dialogueText.text = line.text;

    //    // Hide choices if any were left
    //    ClearChoices();
    //}

    //void ShowChoices(DialogueChoiceNode choiceNode)
    //{
    //    ClearChoices();

    //    // Show prompt as narrator text
    //    speakerText.text = "";
    //    dialogueText.text = choiceNode.prompt;

    //    foreach (DialogueChoice choice in choiceNode.choices)
    //    {
    //        Button button = Instantiate(choiceButtonPrefab, choicesContainer);
    //        button.GetComponentInChildren<TextMeshProUGUI>().text = choice.Message;

    //        //button.onClick.AddListener(() =>
    //        //{
    //        //    OnChoiceSelected(choice);
    //        //});
    //    }
    //}
}
