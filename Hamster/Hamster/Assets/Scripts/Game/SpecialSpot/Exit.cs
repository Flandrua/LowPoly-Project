using com;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Exit : MonoBehaviour
{
    private Animator _animator;
    private PlayerBehaviour _playerBehaviour;
    private bool isPlayerExist = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(DataCenter.Instance.GameData.Abilities.Contains(Ability.Phone))
            {
                //½á¾Ö

            }
        }
    }
}
