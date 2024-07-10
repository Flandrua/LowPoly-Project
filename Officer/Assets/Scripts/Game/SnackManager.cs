using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnackManager : MonoSingleton<SnackManager>
{
    //随机零食功能；不做对象池，直接固定死
    //零食被吃动画处理
    private Animation _animation;
    private string _animationName;
    private Collider _col;
    [SerializeField] private List<GameObject> _snacks = new List<GameObject>();
    [SerializeField] private GameObject _curSnacks;
    private bool isEating = false;
    public bool isPlayer = false;
    public bool isHamster = false;
    void Start()
    {
        EventManager.AddListener<bool>(EventCommon.HAMSTER_EATING, HamsterEating);
        EventManager.AddListener<bool>(EventCommon.PLAYER_EATING, PlayerEating);
        _animation = GetComponent<Animation>();
        _animation.enabled = false;
        _animationName = "VanishEffect";
        _col = GetComponent<Collider>();
        _snacks = GetChildren(transform.Find("Container"));
        RandomSnack();//注意，此处测试用，后续此处删除
    }
    private List<GameObject> GetChildren(Transform parent)
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in parent)
        {
            children.Add(child.gameObject);
        }
        return children;
    }
    private void OnDestroy()
    {
        EventManager.RemoveListener<bool>(EventCommon.HAMSTER_EATING, HamsterEating);
        EventManager.RemoveListener<bool>(EventCommon.PLAYER_EATING, PlayerEating);
    }
    public void RandomSnack()
    {
        if (_snacks.Count == 0)
        {
            Debug.LogWarning("snacks已用完");
            return;
        }
        int randomIndex = Random.Range(0, _snacks.Count);
        _curSnacks = _snacks[randomIndex];
        _curSnacks.SetActive(true);
        _snacks.RemoveAt(randomIndex);
        _col.enabled = true;
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void HamsterEating(bool flag)
    {
        if (isPlayer) { return; }
        isEating = flag;
        if (flag)
        {
            isHamster = true;
            _animation[_animationName].speed = 1;
            _animation.enabled = true;
            _animation.Play();
            //移除3秒后重置动画计时器
            TimeManager.Instance.RemoveTask(StopAnimation, this);

        }
        else
        {
            //先暂停动画
            _animation[_animationName].speed = 0;
            //3秒后重置动画计时器
            TimeManager.Instance.AddTask(3, false, StopAnimation, this);

        }
    }

    public void PlayerEating(bool flag)
    {
        if (isHamster) { return; }
        isEating = flag;
        if (flag)
        {
            isPlayer = true;
            _animation[_animationName].speed = 1;
            _animation.enabled = true;
            _animation.Play();
            //移除3秒后重置动画计时器
            TimeManager.Instance.RemoveTask(StopAnimation, this);

        }
        else
        {
            //先暂停动画
            _animation[_animationName].speed = 0;
            //3秒后重置动画计时器
            TimeManager.Instance.AddTask(3, false, StopAnimation, this);

        }
    }
    public void StopAnimation()
    {
        isHamster = false;
        isPlayer = false;
        ResetAnimation(_animation, _animationName);
    }

    private void ResetAnimation(Animation ani, string name)
    {
        AnimationState state = ani[name];
        ani.Play(name);
        state.time = 0;
        ani.Sample();  //立即生效
        state.enabled = false;
    }

    public void FinishEating()
    {
        _col.enabled = false;
        _curSnacks.SetActive(false);
        RandomSnack();//注意，目前测试用，后续此处的random要删除
        if (isHamster)
        {
            EventManager.DispatchEvent(EventCommon.HAMSTER_FINISH_EATING);
        }
        if (isPlayer)
        {
            EventManager.DispatchEvent(EventCommon.PLAYER_FINISH_EATING);
        }
    }


}
