using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingManager : MonoSingleton<EndingManager>
{
    private string hamsterEnding;
    private string workEnding;

    public GameObject Container;
    public GameObject hamsterLove;
    public GameObject hamsterNormal;
    public GameObject hamsterDead;
    public Text endingTxt;

    // Start is called before the first frame update
    void Start()
    {


    }

    /// <summary>
    /// ?Resources?????HamsterNormal????
    /// </summary>
    private void LoadHamsterEndingText(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>($"{path}");
        if (textAsset != null)
        {
            hamsterEnding = textAsset.text;
        }
        else
        {
            Debug.LogError("LoadHamsterEndingText cant find");
        }
    }
    private void LoadWorkEndingText(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>($"{path}");
        if (textAsset != null)
        {
            workEnding = textAsset.text;
        }
        else
        {
            Debug.LogError("LoadWorkEndingText cant find");
        }
    }


    // Update is called once per frame
    void Update()
    {

    }

    public void Ending()
    {
        HandleEndingTextAndGameObjects();
        endingTxt.text = workEnding + "\r" + hamsterEnding;
        Container.SetActive(true);
    }

    private void HandleEndingTextAndGameObjects()
    {
        if (DataCenter.Instance.GameData.PlayerData.workProgress >= GameManager.Instance.goalWorkPrgoress && DataCenter.Instance.GameData.HamsterData.favorability >= 10 && HamsterController.Instance.isDead == false)
        {
            //Good Ending 
            LoadHamsterEndingText("TTS/Ending/Hamster/HamsterLover");
            LoadWorkEndingText("TTS/Ending/Work/Workaholic");
            hamsterLove.gameObject.SetActive(true);
            TTSManager.Instance.PlayTTSChain("TTS/Ending/Work/Workaholic", "TTS/Ending/Hamster/HamsterLover");
            return;
        }
        if (DataCenter.Instance.GameData.PlayerData.workProgress >= GameManager.Instance.goalWorkPrgoress)
        {
            //Normal work ending
            LoadWorkEndingText("TTS/Ending/Work/WorkStandard");
            TTSManager.Instance.PlayTTS("TTS/Ending/Work/WorkStandard");
        }
        else
        {
            //Fail to work
            LoadWorkEndingText("TTS/Ending/Work/WorkFailed");
            TTSManager.Instance.PlayTTS("TTS/Ending/Work/WorkFailed");
        }

        if (DataCenter.Instance.GameData.HamsterData.favorability >= 10 && HamsterController.Instance.isDead == false)
        {
            //Hmaster love
            LoadHamsterEndingText("TTS/Ending/Hamster/HamsterLover");
            hamsterLove.gameObject.SetActive(true);
            TTSManager.Instance.EnqueueTTS("TTS/Ending/Hamster/HamsterLover");
        }
        else if (HamsterController.Instance.isDead)
        {
            //Hamster dead
            LoadHamsterEndingText("TTS/Ending/Hamster/HamsterBad");
            hamsterDead.gameObject.SetActive(true);
            TTSManager.Instance.EnqueueTTS("TTS/Ending/Hamster/HamsterBad");
        }
        else
        {
            //Hamster normal
            LoadHamsterEndingText("TTS/Ending/Hamster/HamsterNormal");
            hamsterNormal.gameObject.SetActive(true);
            TTSManager.Instance.EnqueueTTS("TTS/Ending/Hamster/HamsterNormal");
        }
    }
}
