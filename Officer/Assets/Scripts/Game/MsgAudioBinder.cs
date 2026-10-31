using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Automatically reassigns AudioSource clips under Boss/Colleagues/Others
/// to the correct MSG_CN or MSG_EN file based on the current language.
/// Each entry maps an AudioSource to a MSG relative path (without language prefix).
/// </summary>
[DefaultExecutionOrder(100)]
public class MsgAudioBinder : MonoBehaviour
{
    [Serializable]
    public class MsgAudioEntry
    {
        public AudioSource audioSource;
        [Tooltip("MSG relative path without language prefix, e.g. \"Boss/Look around\"")]
        public string msgRelativePath;
    }

    [Header("MSG Audio Entries")]
    [Tooltip("List of AudioSource -> MSG path mappings. Auto-populated on Awake if empty.")]
    public List<MsgAudioEntry> entries = new List<MsgAudioEntry>();

    [Header("Auto-Discovery")]
    [Tooltip("If true, automatically discover AudioSources under Boss/Colleagues/Others on Awake.")]
    public bool autoDiscover = true;

    private bool _hasInitialized;

    void Awake()
    {
        // Delay initialization until Start so GameManager (and its language setting)
        // has finished Awake. Awake ordering is not guaranteed for components on
        // the same GameObject, and LocalizationPaths depend on GameManager.
    }

    void Start()
    {
        if (autoDiscover && (entries == null || entries.Count == 0))
        {
            DiscoverEntries();
        }
        RefreshAll();
        _hasInitialized = true;
    }

    void OnEnable()
    {
        if (_hasInitialized)
        {
            RefreshAll();
        }
    }

    /// <summary>
    /// Reassign all MSG AudioSources to the correct language version.
    /// </summary>
    public void RefreshAll()
    {
        if (entries == null) return;

        foreach (var entry in entries)
        {
            if (entry == null || entry.audioSource == null || string.IsNullOrEmpty(entry.msgRelativePath))
            {
                continue;
            }

            string fullPath = LocalizationPaths.Msg(entry.msgRelativePath);
            AudioClip clip = Resources.Load<AudioClip>(fullPath);
            bool wasPlaying = entry.audioSource.isPlaying;
            if (wasPlaying)
            {
                entry.audioSource.Stop();
            }
            entry.audioSource.clip = clip;
            if (wasPlaying && clip != null)
            {
                entry.audioSource.Play();
            }
            if (clip == null)
            {
                Debug.LogWarning($"MsgAudioBinder: Could not load MSG clip at '{fullPath}'.", this);
            }
        }
    }

    /// <summary>
    /// Auto-discover AudioSources and refresh clips. Can be called from editor/MCP.
    /// </summary>
    public void DiscoverAndRefresh()
    {
        if (autoDiscover)
        {
            entries.Clear();
            DiscoverEntries();
        }
        RefreshAll();
    }
    private void DiscoverEntries()
    {
        // Hardcoded fallback mapping for AudioSources whose clip names don't match MSG file names
        var fallbackMap = new Dictionary<string, string>
        {
            { " proficient ", "Colleagues/Work for me" },
            { "cannot find", "Colleagues/Can't find" },
            { "scolding", "Colleagues/get scolding" },
            { "Message from bank", "Others/Credit card" },
            { "message from family", "Others/Phone call" },
            { "Message from friends", "Others/Overtime weekend" },
            { "Message from GYM", "Others/gym expired" },
        };

        string[] rootNames = { "Boss", "Colleagues", "Others" };
        foreach (string rootName in rootNames)
        {
            GameObject root = GameObject.Find(rootName);
            if (root == null) continue;

            AudioSource[] sources = root.GetComponentsInChildren<AudioSource>(true);
            foreach (AudioSource src in sources)
            {
                if (src == null || src.clip == null) continue;

                string clipName = src.clip.name;
                string category = rootName;
                string msgPath = $"{category}/{clipName}";

                // Try clip name match first
                AudioClip testClip = Resources.Load<AudioClip>(LocalizationPaths.Msg(msgPath));
                if (testClip == null)
                {
                    // Try fallback mapping by GameObject name
                    if (fallbackMap.TryGetValue(src.gameObject.name, out string fallbackPath))
                    {
                        msgPath = fallbackPath;
                        testClip = Resources.Load<AudioClip>(LocalizationPaths.Msg(msgPath));
                    }
                }

                if (testClip == null)
                {
                    // Try without category prefix
                    msgPath = clipName;
                    testClip = Resources.Load<AudioClip>(LocalizationPaths.Msg(msgPath));
                }

                if (testClip != null)
                {
                    entries.Add(new MsgAudioEntry { audioSource = src, msgRelativePath = msgPath });
                }
                else
                {
                    Debug.LogWarning($"MsgAudioBinder: No MSG file found for AudioSource '{src.gameObject.name}' (clip: '{clipName}'). Manual mapping required.", this);
                }
            }
        }
    }
}
