using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

public class DialogueReader : MonoBehaviour
{
    public static DialogueReader Instance;

    [SerializeField] private TextAsset textFile;

    // File -> (SectionName -> Speaker + Lines)
    public Dictionary<string, Dictionary<string, List<DialogueArgs>>> dialogueData
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
        dialogueData.Clear();

        TextAsset[] files = Resources.LoadAll<TextAsset>("Dialogue");

        // Format: Deterministic order
        System.Array.Sort(files, (a, b) => a.name.CompareTo(b.name));

        foreach (TextAsset file in files)
        {
            Dictionary<string, List<DialogueArgs>> sections = new();
            string currentSection = null;

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
                    // Optional: Allow lowercase header input but convert to upper

                    currentSection = line.Substring(1).ToUpperInvariant();

                    sections[currentSection] = new List<DialogueArgs>();
                    continue;
                }

                if (currentSection == null)
                {
                    Debug.LogWarning($"Line before section in {file.name}: {line}");
                    continue;
                }

                // Speaker and line parsing
                DialogueArgs dialogueLine = ParseDialogueLine(line, file.name);
                sections[currentSection].Add(dialogueLine);
            }

            dialogueData[file.name] = sections;
        }
    }

    DialogueArgs ParseDialogueLine(string line, string fileName)
    {
        // Expected: [SPEAKER]: text
        if (line.StartsWith("["))
        {
            int closeBracket = line.IndexOf(']');
            int colon = line.IndexOf(':', closeBracket + 1);

            if (closeBracket > 0 && colon > closeBracket)
            {
                string speaker = line.Substring(1, closeBracket - 1).Trim();
                string text = line.Substring(colon + 1).Trim();

                return new DialogueArgs(speaker, text);
            }
        }

        // Fallback: narrator or untagged line
        return new DialogueArgs(SPEAKER_DEFAULT, line);
    }
}
