using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FguiFramework;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityDecryptDllWaterDrop;
using Snappy;

public class LoadManager : MonoSingleton<LoadManager>
{
	public delegate void LoadingCallback();
    public delegate void LoadAssetBundleCallBack(AssetBundle assetBundle);
    private AsyncOperation async;
	private LoadingCallback loadingCallback = null;
	private int nowProcess = 0;

	public const string assetBundleKey = "abcd1234";
	public void LoadGameScene(string name,LoadingCallback cb=null)
	{
		if (async==null)
		{
			loadingCallback = cb;
			Resources.UnloadUnusedAssets();
			TimeManager.Instance.AddTask(0.01f, false, () =>
			{
				StartCoroutine(StartLoad(name));

			}, this);
		}
    }

    IEnumerator StartLoad(string name)
    {

		UIManager2D.Instance.OpenPanel(LoadView.Name);
		async = SceneManager.LoadSceneAsync(name);
		async.allowSceneActivation = false;
		yield return async;
    }

	public AssetBundle LoadAssetBundle(string bundleName)
	{

        byte[] binaryData = null;
        string filePath = Path.Combine(Application.streamingAssetsPath, $"{bundleName}.bin");

        if (File.Exists(filePath))
        {
            binaryData = File.ReadAllBytes(filePath);
            binaryData = SnappyCodec.Uncompress(binaryData);
            binaryData = MachineCodeHandler.DecryptBytes(binaryData, assetBundleKey);
            AssetBundle assetBundle = AssetBundle.LoadFromMemory(binaryData);
			return assetBundle;
        }
        else
        {
			LogManager.Instance.Log($"未找到Bundle:{bundleName}.bin");
        }
		return null;
    }

    public void LoadAssetBundleAsync(string bundleName,LoadAssetBundleCallBack callBack)
    {

        byte[] binaryData = null;
        string filePath = Path.Combine(Application.streamingAssetsPath, $"{bundleName}.bin"); 
        if (File.Exists(filePath))
        {
            binaryData = File.ReadAllBytes(filePath);
            binaryData = SnappyCodec.Uncompress(binaryData);
            binaryData = MachineCodeHandler.DecryptBytes(binaryData, assetBundleKey);
            StartCoroutine(AsyncLoadBundle(binaryData, callBack));
        }
        else
        {
            LogManager.Instance.Log($"未找到Bundle:{bundleName}.bin");
        }
    }
	private IEnumerator AsyncLoadBundle(byte[] binaryData,LoadAssetBundleCallBack callBack)
	{
        AssetBundleCreateRequest resourcesRequest = AssetBundle.LoadFromMemoryAsync(binaryData);
        while (!resourcesRequest.isDone)
        {
            yield return null;
        }
		callBack?.Invoke(resourcesRequest.assetBundle);
    }

    void Update()
    {
        if (async == null)
        {
            return;
        }
		int toProcess;


		if (async.progress < 0.9f)
		{

			toProcess = (int)(async.progress * 100);
		}

		else
		{

			toProcess = 100;
		}


		if (nowProcess < toProcess)
		{

			nowProcess++;
		}


		var progressValue = nowProcess / 100f;
		
		EventManager.DispatchEvent(EventCommon.REFRESH_LOADING_PROGRESS,nowProcess);
		if (nowProcess == 100)
		{

			Time.timeScale = 1.0f;
			loadingCallback?.Invoke();
            UIManager2D.Instance.ClosePanel(LoadView.Name);
			async.allowSceneActivation = true;
			async = null;
		}
	}

}
