using UnityEngine;
using System.Collections.Generic;

public static class CSVLoader
{
    public static List<ConceptModel> LoadConceptsFromCSV()
    {
        List<ConceptModel> concepts = new List<ConceptModel>();

        TextAsset file = Resources.Load<TextAsset>("concepts");

        if (file == null)
        {
            Debug.LogError("❌ CSV file not found in Resources!");
            return concepts;
        }

        string[] lines = file.text.Split('\n');

        for (int i = 1; i < lines.Length; i++) // skip header
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = SplitCSVLine(lines[i]);

            // Expecting at least 11 fields
            if (values.Length < 11)
            {
                Debug.LogWarning($"⚠️ Skipping malformed line {i}: {lines[i]}");
                continue;
            }

            ConceptModel concept = new ConceptModel
            {
                id = ParseLong(values[0]),
                title = Clean(values[1]),
                description = Clean(values[2]),
                mediaUrl = Clean(values[3]),
                memoryObject = Clean(values[4]),
                location = Clean(values[5]),
                visualCue = Clean(values[6]),

                createdAt = Clean(values[7]),
                updatedAt = Clean(values[8]),
                strength = ParseInt(values[9]),
                repetitions = ParseInt(values[10]),
                lastReviewed = values.Length > 11 ? Clean(values[11]) : null
            };

            concepts.Add(concept);
        }

        Debug.Log($"✅ Loaded {concepts.Count} concepts from CSV");
        return concepts;
    }

    // =========================
    // CSV SAFE SPLIT (HANDLES QUOTES)
    // =========================
    private static string[] SplitCSVLine(string line)
    {
        List<string> result = new List<string>();
        bool inQuotes = false;
        string current = "";

        foreach (char c in line)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current);
                current = "";
            }
            else
            {
                current += c;
            }
        }

        result.Add(current);
        return result.ToArray();
    }

    // =========================
    // CLEAN STRING (REMOVE QUOTES)
    // =========================
    private static string Clean(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";

        s = s.Trim();

        if (s.StartsWith("\"") && s.EndsWith("\""))
        {
            s = s.Substring(1, s.Length - 2);
        }

        return s.Replace("\"\"", "\""); // handle escaped quotes
    }

    // =========================
    // SAFE PARSERS
    // =========================
    private static int ParseInt(string s)
    {
        int.TryParse(s, out int result);
        return result;
    }

    private static long ParseLong(string s)
    {
        long.TryParse(s, out long result);
        return result;
    }
}