using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxJudge : MonoBehaviour
{
    private bool canHit=true;
    public Color hitColor;
    public Color defalutColor;
    public Material hams;
    
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("NPC")&&canHit)
        {
            canHit = false;
            Animator animator = PlayerBehaviour.instance.animator[0];
            animator.Play("Eyes_Squint", animator.GetLayerIndex("Shapekey"));
            hams.SetColor("_Color", hitColor);
            TimeManager.Instance.AddTask(1.5f, false, () => {
                canHit = true;
                hams.SetColor("_Color", defalutColor);
            }, this);
            if (PlayerBehaviour.instance.flip.localScale == new Vector3(1, 1, 1))
            {
                PlayerBehaviour.instance.movePosition.AddMovement(new Vector3(-2f,1, 0),0.5f);
            }
            else if (PlayerBehaviour.instance.flip.localScale == new Vector3(-1, 1, 1))
            {
                PlayerBehaviour.instance.movePosition.AddMovement(new Vector3(2f, 1, 0),0.5f);
            }

            PlayerBehaviour.instance.health.TakeDamage(1);

        }
    }
}
