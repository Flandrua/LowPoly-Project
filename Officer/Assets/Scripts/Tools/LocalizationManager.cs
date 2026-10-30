using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationManager : MonoSingleton<LocalizationManager>
{
    private readonly Dictionary<string, string[]> _table = new Dictionary<string, string[]>();

    public int KeyCount => _table.Count;
    public string LastError = "";

    private bool _loaded;

    private void Awake()
    {
        EnsureLoaded();
    }

    private void EnsureLoaded()
    {
        if (_loaded)
        {
            return;
        }

        _loaded = true;
        Load();
    }

    private void Load()
    {
        try
        {
            TextAsset asset = Resources.Load<TextAsset>("Localization");
            if (asset == null || string.IsNullOrEmpty(asset.text))
            {
                LastError = "asset null or empty";
                return;
            }

            string[] lines = asset.text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length <= 1)
            {
                LastError = "lines<=1: " + lines.Length;
                return;
            }

            for (int i = 1; i < lines.Length; i++)
            {
                string[] fields = ParseCsvLine(lines[i]);
                if (fields.Length < 3)
                {
                    continue;
                }

                string key = fields[0].Trim();
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }

                _table[key] = new string[]
                {
                    Unescape(fields[1]),
                    Unescape(fields[2])
                };
            }

            LastError = "loaded, keys=" + _table.Count;
        }
        catch (Exception e)
        {
            LastError = "exception: " + e.Message;
        }
    }

    private string[] ParseCsvLine(string line)
    {
        List<string> fields = new List<string>();
        bool inQuotes = false;
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    sb.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(sb.ToString());
                sb.Length = 0;
            }
            else
            {
                sb.Append(c);
            }
        }

        fields.Add(sb.ToString());
        return fields.ToArray();
    }

    private string Unescape(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        return value.Replace("\\n", "\n").Replace("\\t", "\t");
    }

    public string Get(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return key;
        }

        // Localization can be requested from another component's Awake() before this
        // component's Awake() has run, so make sure the table is loaded on first use.
        EnsureLoaded();

        if (_table.TryGetValue(key, out string[] values))
        {
            bool chinese = GameManager.Instance == null || GameManager.Instance.IsChinese;
            int index = chinese ? 0 : 1;
            if (index < values.Length && !string.IsNullOrEmpty(values[index]))
            {
                return values[index];
            }
        }

        return key;
    }

    public static string GetText(string key)
    {
        if (LocalizationManager.Instance == null)
        {
            return key;
        }

        return LocalizationManager.Instance.Get(key);
    }

    public static string NormalizeKey(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return string.Empty;
        }

        StringBuilder builder = new StringBuilder(raw.Length);
        foreach (char ch in raw.Trim())
        {
            if (char.IsLetterOrDigit(ch))
            {
                builder.Append(char.ToLowerInvariant(ch));
            }
            else if (char.IsWhiteSpace(ch) || ch == '-' || ch == '_')
            {
                if (builder.Length > 0 && builder[builder.Length - 1] != '_')
                {
                    builder.Append('_');
                }
            }
        }

        return builder.ToString().Trim('_');
    }
}
