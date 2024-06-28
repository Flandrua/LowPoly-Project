using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlurManager : Singleton<BlurManager>
{
    /// <summary>
    /// 是否设置模糊
    /// </summary>
    /// <param name="value"></param>
    public void SetBlur(bool value, Camera camera, float startTime=1f, float endTime = 1f)
    {
            RapidBlurEffect rapidBlurEffect = camera.gameObject.GetComponent<RapidBlurEffect>();
            if (rapidBlurEffect == null)
            {
                camera.gameObject.AddComponent<RapidBlurEffect>();
                rapidBlurEffect = camera.gameObject.GetComponent<RapidBlurEffect>();
            }
            if (value)
            {
                rapidBlurEffect.DownSampleNum = 0;
                rapidBlurEffect.BlurSpreadSize = 0;
                rapidBlurEffect.BlurIterations = 0;
                DOTween.To(() => rapidBlurEffect.DownSampleNum, v => rapidBlurEffect.DownSampleNum = v, 0, startTime).SetUpdate(true);
                DOTween.To(() => rapidBlurEffect.BlurSpreadSize, v => rapidBlurEffect.BlurSpreadSize = v, 3, startTime).SetUpdate(true); 
                DOTween.To(() => rapidBlurEffect.BlurIterations, v => rapidBlurEffect.BlurIterations = v, 6, startTime).SetUpdate(true);
            }
            else
            {
                DOTween.To(() => rapidBlurEffect.DownSampleNum, v => rapidBlurEffect.DownSampleNum = v, 0, endTime).SetUpdate(true);
                DOTween.To(() => rapidBlurEffect.BlurSpreadSize, v => rapidBlurEffect.BlurSpreadSize = v, 0, endTime).SetUpdate(true);
                DOTween.To(() => rapidBlurEffect.BlurIterations, v => rapidBlurEffect.BlurIterations = v, 0, endTime).SetUpdate(true);
            }
        }
}
