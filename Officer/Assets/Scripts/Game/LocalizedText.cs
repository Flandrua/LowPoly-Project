using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    [Tooltip("Localization.csv 中的 key。")]
    public string key;

    private TMP_Text _tmpText;
    private Text _legacyText;

    private void Awake()
    {
        _tmpText = GetComponent<TMP_Text>();
        _legacyText = GetComponent<Text>();
        Refresh();
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (string.IsNullOrEmpty(key))
        {
            return;
        }

        string value = LocalizationManager.GetText(key);
        if (_tmpText != null)
        {
            _tmpText.text = value;
        }

        if (_legacyText != null)
        {
            _legacyText.text = value;
        }
    }
}
