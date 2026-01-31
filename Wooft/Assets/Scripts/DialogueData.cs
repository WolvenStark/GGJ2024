using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DialogueNode { }

[System.Serializable]
public struct DialogueLine
{
    public string SpeakerName;
    public string Message;
    public float DelayBetweenChars;

    public DialogueLine(string speaker, string text, float delay = 0.005f)
    {
        this.SpeakerName = speaker;
        this.Message = text;
        this.DelayBetweenChars = delay;
    }
}

[System.Serializable]
public struct DialogueChoice
{
    public string ChoiceMessage;        // What the player sees
    public string nextSection;      // Section to jump to

    public DialogueChoice(string text, string target)
    {
        this.ChoiceMessage = text;
        this.nextSection = target;
    } 
}

public class DialogueTextNode : DialogueNode
{
    public DialogueLine line;

    public DialogueTextNode(DialogueLine line)
    {
        this.line = line;
    }
}
public class DialogueChoiceNode : DialogueNode
{
    public string prompt;
    public List<DialogueChoice> choices = new();

    public DialogueChoiceNode(string prompt)
    {
        this.prompt = prompt;
    }
}



