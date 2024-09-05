using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDebug : MonoBehaviour
{
    public GameObject[] poolEntry;
    // Start is called before the first frame update
    void Start()
    {
        DataCenter.Instance.GameData.Abilities.Add(Ability.Spider);
        DataCenter.Instance.GameData.Abilities.Add(Ability.Fish);
        DataCenter.Instance.GameData.LockerTypes.Add(LockerType.K);
        DataCenter.Instance.GameData.LockerTypes.Add(LockerType.N);
        foreach (GameObject go in poolEntry)
        {
            go.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
