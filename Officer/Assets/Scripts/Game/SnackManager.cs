using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    private string _snackName;
    private string _desc;
    private Outline _outline;
    private TextMeshProUGUI _content = null;
    private TextMeshProUGUI _name = null;
    private bool isEating = false;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    public bool isPlayer = false;
    public bool isHamster = false;

    void Start()
    {
        EventManager.AddListener<bool>(EventCommon.HAMSTER_EATING, HamsterEating);
        EventManager.AddListener<bool>(EventCommon.PLAYER_EATING, PlayerEating);
        EventManager.AddListener(EventCommon.NEXT_STAGE, ResetToDefault);
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        _animation = GetComponent<Animation>();
        _animation.enabled = false;
        _animationName = "VanishEffect";
        _col = GetComponent<Collider>();
        _snacks = GetChildren(transform.Find("Container"));
        _content = UIMonitorController.Instance.content;
        _name = UIMonitorController.Instance.name;
        RandomSnack();

    }
    public void ShowUIDec(bool flag)
    {
        _content.text = _desc;
        _name.text = _snackName;
        _outline.enabled = flag;
        UIMonitorController.Instance.Show(flag);
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
        EventManager.RemoveListener(EventCommon.NEXT_STAGE, ResetToDefault);
    }
    public void RandomSnack()
    {
        if (_curSnacks != null)
        {
            if (_curSnacks.activeInHierarchy)//如果是没有吃的零食，就隐藏掉
            {
                _curSnacks.SetActive(false);
            }
        }
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
        SnackData snackData = _curSnacks.GetComponent<SnackData>();
        _snackName = snackData.name;
        _desc = snackData.desc;
        _outline=_curSnacks.GetComponent<Outline>();
    }
    private void ResetToDefault()
    {
        if (!_curSnacks.activeInHierarchy)
        {
            _animation.enabled = false;
            //_curSnacks.SetActive(false);
        }
        transform.position = initialPosition;
        transform.rotation = Quaternion.identity;
        //RandomSnack();

    }
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

    public void FinishEating()//动画事件
    {
        _col.enabled = false;
        _curSnacks.SetActive(false);
        //RandomSnack();//注意，目前测试用，后续此处的random要删除
        if (isHamster)
        {
            HamsterController.Instance.isEating = false;
            EventManager.DispatchEvent(EventCommon.HAMSTER_FINISH_EATING);
        }
        if (isPlayer)
        {
            EventManager.DispatchEvent(EventCommon.PLAYER_FINISH_EATING);
        }
    }


}
