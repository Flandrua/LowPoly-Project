using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HamsterBehavior
{
    Attack,
    Bounce,
    Clicked,
    Death,
    Eat,
    Fear,
    Fly,
    Hit,
    Idle_A,
    Idle_B,
    Idle_C,
    Jump,
    Roll,
    Run,
    Sit,
    Spin,
    Swim,
    Walk,
};
public enum HamsterEyes
{
    Eyes_Normal,
    Eyes_Annoyed,
    Eyes_Blink,
    Eyes_Cry,
    Eyes_Dead,
    Eyes_Excited,
    Eyes_Happy,
    Eyes_LookDown,
    Eyes_LookIn,
    Eyes_LookOut,
    Eyes_LookUp,
    Eyes_Rabid,
    Eyes_Sad,
    Eyes_Shrink,
    Eyes_Sleep,
    Eyes_Spin,
    Eyes_Squint,
    Eyes_Trauma,
    Sweat_L,
    Sweat_R,
    Teardrop_L,
    Teardrop_R
};
public class HamsterController : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator _animator;




    void Start()
    {
        _animator = GetComponent<Animator>();


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeBehaviorAnimationByStr(string animationName)
    {
        _animator.Play(animationName);
    }
    public void ChangeEyesAnimationByStr(string animationName)
    {
        _animator.Play(animationName, _animator.GetLayerIndex("Shapekey"));
    }

    public void ChangeBehaviorAnimation(HamsterBehavior animationName)
    {
        string animation = animationName.ToString();
        _animator.Play(animation);
    }       
    public void ChangeEyesAnimation(HamsterEyes animationName)
    {
        string animation = animationName.ToString();
        _animator.Play(animation, _animator.GetLayerIndex("Shapekey"));
    }
}
