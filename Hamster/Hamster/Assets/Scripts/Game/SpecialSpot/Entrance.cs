using com;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public enum SceneType
{
    Ventilation,
    Sewer
}
public class Entrance : MonoBehaviour
{
    public GameObject tpPos;
    public Entrance targerEntrance;
    public SceneType curScene;
    private Animator _animator;
    private PlayerBehaviour _playerBehaviour;
    private bool isPlayerExist = false;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _playerBehaviour = PlayerBehaviour.Instance;
        EventManager.AddListener(EventCommon.ENTRANCE, EnterScene);
        EventManager.AddListener(EventCommon.CHANGE_SCENE_EXIT, ChangeSceneExit);//�������¼�
        EventManager.AddListener(EventCommon.CHANGE_SCENE_ENTER, ChangeSceneEnter);//�������¼�
    }
    private void OnDestroy()
    {
        EventManager.RemoveListener(EventCommon.ENTRANCE, EnterScene);
        EventManager.RemoveListener(EventCommon.CHANGE_SCENE_EXIT, ChangeSceneExit);
        EventManager.RemoveListener(EventCommon.CHANGE_SCENE_ENTER, ChangeSceneEnter);//�������¼�
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (Input.GetKey(KeyCode.E) && _playerBehaviour.move.canInput)
            {
                isPlayerExist = true;
                ResetToTarget(_playerBehaviour.gameObject, tpPos);
                MmoCameraBehaviour.Instance.target = tpPos.transform.Find("fix");
                if (curScene == SceneType.Sewer)
                    _playerBehaviour.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                _playerBehaviour.move.FlipRight();
                _playerBehaviour.animator[0].SetTrigger("entrance");
                _playerBehaviour.move.canInput = false;//����������Ʒ���animator��state FSM������
                _playerBehaviour.move.ResetSpeed();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
    }
    private void EnterScene()
    {
        if (isPlayerExist)
        {
            MmoCameraBehaviour.Instance.target = targerEntrance.tpPos.transform.Find("fix");
            ResetToTarget(_playerBehaviour.gameObject, targerEntrance.tpPos);
            MmoCameraBehaviour.Instance.testToggle = true;
            if (targerEntrance.curScene == SceneType.Sewer)
                _playerBehaviour.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }
    }
    private void ChangeSceneExit()
    {
        if (isPlayerExist)
        {
            isPlayerExist = false;
            MmoCameraBehaviour.Instance.target = _playerBehaviour.rig.transform;
            MmoCameraBehaviour.Instance.target = _playerBehaviour.rig.transform;
            return;

        }
    }
    private void ChangeSceneEnter()
    {
        if (isPlayerExist)
        {
            switch (targerEntrance.curScene)
            {
                case SceneType.Sewer:
                    {
                        _playerBehaviour.isSewage = true;
                        return;
                    }
                case SceneType.Ventilation:
                    {
                        _playerBehaviour.isSewage = false;
                        return;
                    }
            }
        }
    }

    void ResetToTarget(GameObject go, GameObject target)
    {
        // ��ʱ����ǰ����ĸ���������Ϊ targetEntrance
        Transform originalParent = transform.parent;
        go.transform.SetParent(target.transform);

        // ���ֲ���ת����Ϊ��
        go.transform.localRotation = Quaternion.Euler(Vector3.zero);

        go.transform.position = target.transform.position;

        // �ָ�������
        go.transform.SetParent(target.transform.parent);
    }
}
