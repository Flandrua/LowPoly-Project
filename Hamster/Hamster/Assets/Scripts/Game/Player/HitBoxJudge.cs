using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxJudge : MonoBehaviour
{
    private bool canHit = true;
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
        if (other.gameObject.layer == LayerMask.NameToLayer("NPC") && canHit)
        {
            if(PlayerBehaviour.Instance.health.isDead) { return; }
            canHit = false;
            Animator animator = PlayerBehaviour.Instance.animator[0];
            animator.Play("Eyes_Squint", animator.GetLayerIndex("Shapekey"));
            hams.SetColor("_Color", hitColor);
            TimeManager.Instance.AddTask(1.5f, false, () =>
            {
                canHit = true;
                hams.SetColor("_Color", defalutColor);
            }, this);
            if (PlayerBehaviour.Instance.isSewage)
            {
                Vector3 collisionDirection = other.transform.position - transform.position;
                Vector3 normalizedDirection = collisionDirection.normalized;
                normalizedDirection *= -2;
                normalizedDirection.y = 1;
                PlayerBehaviour.Instance.movePosition.AddMovement(normalizedDirection,0.5f);
            }
            else
            {
                if (PlayerBehaviour.Instance.flip.localScale == new Vector3(1, 1, 1))
                {
                    PlayerBehaviour.Instance.movePosition.AddMovement(new Vector3(-2f, 1, 0), 0.5f);
                }
                else if (PlayerBehaviour.Instance.flip.localScale == new Vector3(-1, 1, 1))
                {
                    PlayerBehaviour.Instance.movePosition.AddMovement(new Vector3(2f, 1, 0), 0.5f);
                }
            }
            PlayerAudio.Instance.Hurt();
            PlayerBehaviour.Instance.health.TakeDamage(1);
        }
    }
}
