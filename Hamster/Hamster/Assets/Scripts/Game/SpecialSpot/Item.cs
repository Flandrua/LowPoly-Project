using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Ability
{
    Spider,
    Fish
}
public class Item : MonoBehaviour
{
    public Ability type;
    public GameObject[] poolEntry;
    private Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _animator.SetBool("get", true);
            DataCenter.Instance.GameData.Abilities.Add(type);
            switch (type)
            {
                case Ability.Spider:
                    break;
                    case Ability.Fish:                    
                    foreach(GameObject go in poolEntry)
                    {
                        go.SetActive(false);
                    }
                    break;
            }
        }
    }
}
