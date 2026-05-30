using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIMonitorController : MonoSingleton<UIMonitorController>
{
    // Start is called before the first frame update
    private Animator _animator;
    public TextMeshProUGUI content = null;
    public TextMeshProUGUI nameTxt = null;
    [Tooltip("Displays the player's work efficiency and favorability from DataCenter. Auto-found by child name 'PlayerInfo' if left empty.")]
    public TextMeshProUGUI playerInfo = null;

    void Start()
    {
        _animator = GetComponent<Animator>();
        if (content == null)
            content = transform.Find("Content").GetComponent<TextMeshProUGUI>();
        if (nameTxt == null)
            nameTxt = transform.Find("Name").GetComponent<TextMeshProUGUI>();
        if (playerInfo == null)
            playerInfo = FindChildText("PlayerInfo");

        EventManager.AddListener(EventCommon.UPDATE_MONITOR, UpdatePlayerInfo);
        UpdatePlayerInfo();
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener(EventCommon.UPDATE_MONITOR, UpdatePlayerInfo);
    }

    private TextMeshProUGUI FindChildText(string childName)
    {
        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i] != null && texts[i].gameObject.name == childName)
            {
                return texts[i];
            }
        }
        return null;
    }

    private void UpdatePlayerInfo()
    {
        if (playerInfo == null ||
            DataCenter.Instance == null ||
            DataCenter.Instance.GameData == null ||
            DataCenter.Instance.GameData.PlayerData == null ||
            DataCenter.Instance.GameData.HamsterData == null)
        {
            return;
        }

        int efficiency = DataCenter.Instance.GetTotalWorkEfficiency();
        int favorabilityAbility = DataCenter.Instance.GetTotalFavorabilityAbility();
        playerInfo.text = $"Work Efficiency: {efficiency}\nFavorability: {favorabilityAbility}";
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Show(bool flag)
    {
        //_animator.SetBool("Show", flag);
    }
}
