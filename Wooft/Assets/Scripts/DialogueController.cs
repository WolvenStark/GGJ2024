using System.Collections;
using System.Linq;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance;

    public TMP_Text speakerText;
    public TMP_Text dialogueText;

    protected DialogueNode currentArgs;
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

    public void SnapConversation(DialogueNode args)
    {
        speakerText.text = args.SpeakerName;
        dialogueText.text = args.Message;
    }

    public void NextDialogue(DialogueNode node)
    {
        SnapConversation(currentArgs);

        if (node is DialogueTextNode text)
        {
            ShowLine(text.line);
        }
        else if (node is DialogueChoiceNode choice)
        {
            ShowChoices(choice);
            break; // wait for player input
        }

        if (currentMessageRoutine != null)
        {
            StopCoroutine(currentMessageRoutine);
            currentMessageRoutine = null;
        }

        currentArgs = node;
        currentMessageRoutine = StartCoroutine(ScrollConversation(args));
    }

    public IEnumerator ScrollConversation(DialogueNode args)
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

    void PlaySection(string file, string section)
    {
        var nodes =  DialogueReader.Instance.DialogueData[file][section];

        foreach (DialogueNode node in nodes)
        {
            if (node is DialogueTextNode text)
            {
                ShowLine(text.line);
            }
            else if (node is DialogueChoiceNode choice)
            {
                ShowChoices(choice);
                break; // wait for player input
            }
        }
    }

    void OnChoiceSelected(string file, DialogueChoice choice)
    {
        PlaySection(file, choice.nextSection);
    }

    void ShowLine(DialogueLine line)
    {
        speakerText.text = line.speaker;
        dialogueText.text = line.text;

        // Hide choices if any were left
        ClearChoices();
    }

    void ShowChoices(DialogueChoiceNode choiceNode)
    {
        ClearChoices();

        // Show prompt as narrator text
        speakerText.text = "";
        dialogueText.text = choiceNode.prompt;

        foreach (DialogueChoice choice in choiceNode.choices)
        {
            Button button = Instantiate(choiceButtonPrefab, choicesContainer);
            button.GetComponentInChildren<TextMeshProUGUI>().text = choice.ChoiceMessage;

            //button.onClick.AddListener(() =>
            //{
            //    OnChoiceSelected(choice);
            //});
        }
    }
}
