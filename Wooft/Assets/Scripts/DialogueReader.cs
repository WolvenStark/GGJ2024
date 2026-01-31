using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

public class DialogueReader : MonoBehaviour
{
    public static DialogueReader Instance;

    // File -> (SectionName -> Speaker + Lines)
    public SortedDictionary<string, Dictionary<string, List<DialogueNode>>> DialogueData
        = new();

    const string SECTIION_DEFAULT = "DEFAULT";
    const string SPEAKER_DEFAULT = "NARRATOR";

    protected void Awake()
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

        ParseFiles();
    }


    /// <summary>
    /// Parses the text file based on inputs. Example text format below
    /// 
    ///#INTRO
    ///Hello there
    ///Welcome, traveler
    ///
    /// Suports double slash for comments
    ///#BATTLE
    ///You dare challenge me?
    ///Prepare yourself
    /// </summary>
    protected void ParseFiles()
    {
        DialogueData.Clear();

        TextAsset[] files = Resources.LoadAll<TextAsset>("Dialogue");

        // Format: Deterministic order
        System.Array.Sort(files, (a, b) => a.name.CompareTo(b.name));

        foreach (TextAsset file in files)
        {
            Dictionary<string, List<DialogueNode>> sections = new();
            string currentSection = null;
            DialogueChoiceNode currentChoiceNode = null;

            // Split by line endings (Windows + Unix safe)
            string[] lines = file.text.Split(
                new[] { "\r\n", "\n" },
                System.StringSplitOptions.None
            );

            foreach (string rawLine in lines)
            {
                // Format: Skip empty lines
                if (string.IsNullOrWhiteSpace(rawLine))
                    continue;

                string line = rawLine.Trim();

                // Format: Ignore comments
                if (line.StartsWith("//"))
                    continue;

                // Format: Seperate by section headers
                if (line.StartsWith("#"))
                {
                    // Format: Allow lowercase header input but convert to upper
                    currentSection = line.Substring(1).Trim().ToUpperInvariant();
                    sections[currentSection] = new List<DialogueNode>();
                    currentChoiceNode = null;
                    continue;
                }

                if (currentSection == null)
                {
                    Debug.LogWarning($"Line before section in {file.name}: {line}");
                    continue;
                }

                // Format: Choice prompt
                if (line.StartsWith("?"))
                {
                    currentChoiceNode = new DialogueChoiceNode(
                        line.Substring(1).Trim().ToUpperInvariant()
                    );
                    sections[currentSection].Add(currentChoiceNode);
                    continue;
                }

                // Format: Choice option
                if (line.StartsWith("->") && currentChoiceNode != null)
                {
                    // -> Text | TARGET_SECTION
                    string content = line.Substring(2).Trim();
                    string[] parts = content.Split('|');

                    if (parts.Length == 2)
                    {
                        currentChoiceNode.choices.Add(
                            new DialogueChoice(
                                parts[0].Trim(),
                                parts[1].Trim()
                            )
                        );
                    }
                    continue;
                }

                // Normal Speaker and line parsing
                currentChoiceNode = null;
                sections[currentSection].Add(
                    new DialogueTextNode(ParseDialogueLine(line))
                );

            }

            DialogueData[file.name] = sections;
        }
    }

    protected DialogueLine ParseDialogueLine(string line)
    {
        // Expected: [SPEAKER]: text
        if (line.StartsWith("["))
        {
            int closeBracket = line.IndexOf(']');
            int seperator = line.IndexOf(':', closeBracket + 1);

            if (closeBracket > 0 && seperator > closeBracket)
            {
                string speaker = line.Substring(1, closeBracket - 1).Trim();
                string text = line.Substring(seperator + 1).Trim();

                return new DialogueLine(speaker, text);
            }
        }

        // Fallback: narrator or untagged line
        return new DialogueLine(SPEAKER_DEFAULT, line);
    }

}
