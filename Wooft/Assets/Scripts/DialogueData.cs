using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DialogueNode 
{
    public float DelayBetweenChars = 0.05f;

    public abstract string ExtractMessage();
    public abstract string ExtractSpeaker();

}

[System.Serializable]
public struct DialogueLine
{
    public string SpeakerName;
    public string Message;

    public DialogueLine(string speaker, string text)
    {
        this.SpeakerName = speaker;
        this.Message = text;
    }
}

[System.Serializable]
public struct DialogueChoice
{
    public string Message;        // What the player sees
    public string nextSection;      // Section to jump to

    public DialogueChoice(string text, string target)
    {
        this.Message = text;
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

    public override string ExtractMessage()
    {
        return line.Message;
    }

    public override string ExtractSpeaker()
    {
        return line.SpeakerName;
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
    public override string ExtractMessage()
    {
        var message = string.Empty;

        foreach (var choice in choices)
        {
            message += choice.Message + "\n";
        }

        return message;
    }

    public override string ExtractSpeaker()
    {
        return DialogueReader.SPEAKER_DEFAULT;
    }
}



