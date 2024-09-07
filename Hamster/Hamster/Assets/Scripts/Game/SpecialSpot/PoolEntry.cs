using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolEntry : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerBehaviour.Instance.IsSwim = true;
            PlayerBehaviour.Instance.fish.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerBehaviour.Instance.IsSwim = false;
            PlayerBehaviour.Instance.fish.SetActive(false);
        }
    }
}

