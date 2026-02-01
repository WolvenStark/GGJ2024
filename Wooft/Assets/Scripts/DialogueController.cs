using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance;

    public TMP_Text speakerText;
    public TMP_Text dialogueText;

    [SerializeField] private Button choiceButtonPrefab;
    [SerializeField] private Transform choicesContainer;
    private readonly List<Button> buttonPool = new List<Button>();
    private readonly List<DialogueChoice> activeChoices = new List<DialogueChoice>();

    protected DialogueNode currentNodeArgs;
    protected Coroutine currentMessageRoutine;

    protected string FileDataName = string.Empty;
    protected string SectionDataName = string.Empty;
    protected int LineDataId = 0;

    public bool isPaused = true;


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
        var startingFile = DialogueReader.Instance.DialogueData.First();
        FileDataName = startingFile.Key;

        var sectionName = startingFile.Value.First();
        SectionDataName = sectionName.Key;

        var node = sectionName.Value[LineDataId];
        NextDialogue(node);
    }

    // Explicit convesion
    public void SnapConversation(DialogueNode node)
    {
        if (currentMessageRoutine != null)
        {
            StopCoroutine(currentMessageRoutine);
            currentMessageRoutine = null;
        }

        if (currentNodeArgs != null)
        {
            speakerText.text = node.ExtractSpeaker();
            dialogueText.text = node.ExtractMessage();
        }
    }

    public void NextDialogue(DialogueNode node)
    {
        SnapConversation(currentNodeArgs);

        currentNodeArgs = node;
        currentMessageRoutine = StartCoroutine(ScrollConversation(node));
    }

    public IEnumerator ScrollConversation(DialogueNode args)
    {
        yield return new WaitForSeconds(args.DelayBetweenChars);

        string currentMessage = string.Empty;

        speakerText.text = args.ExtractSpeaker();

        foreach (char c in args.ExtractMessage())
        {
            // Wait inbetween being paused
            while (!DialogueController.Instance.isPaused)
            {
                yield return null;
            }

            currentMessage += c;
            dialogueText.text = currentMessage;

            yield return new WaitForSeconds(args.DelayBetweenChars);

        }

        currentMessageRoutine = null;
    }

    void JumpToSectionLine(string file, string section, int lineId)
    {
        // Reset LineData back to zero
        LineDataId = lineId;
        SectionDataName = section;
        SectionDataName = section;
        FileDataName = file;

        var nodes = DialogueReader.Instance.DialogueData[FileDataName][SectionDataName];
        var currentNode = nodes[lineId];

        if (currentNode is DialogueTextNode text)
        {
            NextDialogue(currentNode);
        }
        else if (currentNode is DialogueChoiceNode choice)
        {
            ShowChoices(currentNode as DialogueChoiceNode);
        }
    }

    void ShowLine(DialogueLine line)
    {
        // Hide choices if any were left
        ClearChoices();

        speakerText.text = line.SpeakerName;
        dialogueText.text = line.Message;
    }


    void ClearChoices()
    {
        foreach (var button in buttonPool)
        {
            button.onClick.RemoveAllListeners();
            button.gameObject.SetActive(false);
        }
    }

    public Button GetButton()
    {
        foreach (var button in buttonPool)
        {
            if (!button.gameObject.activeSelf)
            {
                button.gameObject.SetActive(true);
                return button;
            }
        }

        // Create new one if none available
        Button newButton = Instantiate(choiceButtonPrefab, choicesContainer);
        buttonPool.Add(newButton);
        return newButton;
    }

    public void ShowChoices(DialogueChoiceNode node)
    {
        ClearChoices();
        currentNodeArgs = node;

        foreach (var option in node.choices)
        {
            Button button = GetButton();

            // Set label text
            TMP_Text label = button.GetComponentInChildren<TMP_Text>();
            label.text = option.Message;

            // Assign listener
            button.onClick.AddListener(() =>
            {
                OnChoiceSelected(option);
            });
        }
    }

    public void SelectChoiceByKeyControl(KeyCode code)
    {
        switch (code)
        {
            case KeyCode.Space:
                HandleRequestNextLine();
                break;
            case KeyCode.Q:
                SelectChoiceByIndex(0);
                break;
            case KeyCode.E:
                SelectChoiceByIndex(1);
                break;

        }
    }

    public void HandleRequestNextLine()
    {
        if (currentNodeArgs is DialogueTextNode)
        {
            if (currentMessageRoutine != null)
            {
                UnityEngine.Debug.LogWarning("Snap to end of dialogue");

                // We are still trying to display it. Snap to end
                SnapConversation(currentNodeArgs);
                return;
            }
            else if (LineDataId <= DialogueReader.Instance.DialogueData[FileDataName][SectionDataName].Count - 1)
            {
                UnityEngine.Debug.LogWarning("Look at next line");

                //// Look at next line
                LineDataId++;

                // Read the (next) section with new lineId
                JumpToSectionLine(FileDataName, SectionDataName, LineDataId);
            }
            else
            {
                UnityEngine.Debug.LogWarning("End of dialogue line. Where should be jump now?");
            }
        }
        else
        {
            UnityEngine.Debug.LogWarning("Cannot skip option. Use Q or E");
        }
    }

    public void SelectChoiceByIndex(int index)
    {
        if (index < 0 || index >= activeChoices.Count)
        {
            UnityEngine.Debug.LogError("Choice Index is invalid");
            return;
        }

        DialogueChoice choice = activeChoices[index];
        OnChoiceSelected(choice);
    }

    public void OnChoiceSelected(DialogueChoice choice)
    {
        ClearChoices();
        JumpToSectionLine(FileDataName, choice.NextSection, 0);
    }


}
