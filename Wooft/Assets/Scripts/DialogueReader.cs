using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

public class DialogueReader : MonoBehaviour
{
    public static DialogueReader Instance;

    protected DialogueController.ConversationArgs currentArgs;

    [SerializeField] private TextAsset textFile;

    // FileName -> (SectionName -> Lines)
    public Dictionary<string, Dictionary<string, List<string>>> dialogueData
        = new();

    const string SECTIION_DEFAULT = "DEFAULT";

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

        ParseTargetFiles();
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
    protected void ParseTargetFiles()
    {
        dialogueData.Clear();

        TextAsset[] files = Resources.LoadAll<TextAsset>("Dialogue");

        // Optional: deterministic order
        System.Array.Sort(files, (a, b) => a.name.CompareTo(b.name));

        foreach (TextAsset file in files)
        {
            Dictionary<string, List<string>> sections = new();

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

                    currentSection = line.Substring(1);

                    sections[currentSection] = new List<string>();
                    continue;
                }

                // Normal line
                if (currentSection != null)
                {
                    sections[currentSection].Add(line);
                }
                else
                {
                    Debug.LogWarning(
                        $"Line found before any section in {file.name}: {line}"
                    );
                }
            }

            dialogueData[file.name] = sections;
        }
    }
}
