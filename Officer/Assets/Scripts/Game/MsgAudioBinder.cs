using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Automatically reassigns AudioSource clips under Boss/Colleagues/Others
/// to the correct MSG_CN or MSG_EN file based on the current language.
/// Each entry maps an AudioSource to a MSG relative path (without language prefix).
/// </summary>
[DefaultExecutionOrder(-1000)]
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
        if (IsLowStressVersion())
        {
            DisableMsgAudioSources();
        }
        // Delay initialization until Start so GameManager (and its language setting)
        // has finished Awake. Awake ordering is not guaranteed for components on
        // the same GameObject, and LocalizationPaths depend on GameManager.
    }

    void Start()
    {
        if (IsLowStressVersion())
        {
            DisableMsgAudioSources();
            _hasInitialized = true;
            return;
        }

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
            if (IsLowStressVersion())
            {
                DisableMsgAudioSources();
            }
            else
            {
                RefreshAll();
            }
        }
    }

    /// <summary>
    /// Reassign all MSG AudioSources to the correct language version.
    /// </summary>
    public void RefreshAll()
    {
        if (IsLowStressVersion())
        {
            DisableMsgAudioSources();
            return;
        }

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
        if (IsLowStressVersion())
        {
            DisableMsgAudioSources();
            return;
        }

        if (autoDiscover)
        {
            entries.Clear();
            DiscoverEntries();
        }
        RefreshAll();
    }
    private void DiscoverEntries()
    {
        if (IsLowStressVersion())
        {
            return;
        }
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

    private bool IsLowStressVersion()
    {
        if (GameManager.Instance != null)
        {
            return GameManager.Instance.IsLowStressVersion;
        }

        GameManager gm = FindObjectOfType<GameManager>();
        return gm != null && gm.LowStressVersion;
    }

    private void DisableMsgAudioSources()
    {
        string[] rootNames = { "Boss", "Colleagues", "Others" };
        for (int r = 0; r < rootNames.Length; r++)
        {
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            for (int i = 0; i < allObjects.Length; i++)
            {
                GameObject candidate = allObjects[i];
                if (candidate == null || candidate.name != rootNames[r] || !candidate.scene.IsValid())
                {
                    continue;
                }

                AudioSource[] sources = candidate.GetComponentsInChildren<AudioSource>(true);
                for (int s = 0; s < sources.Length; s++)
                {
                    AudioSource source = sources[s];
                    if (source == null) continue;

                    source.Stop();
                    source.mute = true;
                    source.volume = 0f;
                    source.playOnAwake = false;
                    source.clip = null;
                    source.enabled = false;
                }
            }
        }
    }
}
