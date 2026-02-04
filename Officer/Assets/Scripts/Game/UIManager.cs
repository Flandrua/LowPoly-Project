using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    // Start is called before the first frame update
    public Text txtHeight;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHeight(float height)
    {
        string displayValue = height.ToString("F1");
        txtHeight.text = displayValue.ToString();
    }
}
