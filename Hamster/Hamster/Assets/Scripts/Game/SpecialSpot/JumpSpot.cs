using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpSpot : MonoBehaviour
{
    // Start is called before the first frame update
    public ParticleSystem _particle;
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerBehaviour.Instance.movePosition.AddMovement(new Vector3(0, 4, 0), 1, true, false, false);
            _particle.Play();
            //Animator animator = PlayerBehaviour.Instance.animator;
            //animator.SetTrigger("jump");
    
        }
    }
}
