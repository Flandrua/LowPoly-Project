using UnityEngine;
public enum Ability
{
    Spider,
    Fish,
    Phone
}
public class Item : MonoBehaviour
{
    public Ability type;
    public GameObject[] poolEntry;
    private Animator _animator;
    private AudioSource _as;
    // Start is called before the first frame update
    void Start()
    {
        _as = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        EventManager.AddListener(EventCommon.START_GAME, InitItem);
    }
    private void InitItem()
    {
        _animator.Play("BubbleSpin");
        this.GetComponent<Collider>().enabled = true;
        if (type == Ability.Fish)
        {
            foreach (GameObject go in poolEntry)
            {
                go.SetActive(true);
            }
        }
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
            _as.Play();
            this.GetComponent<Collider>().enabled = false;
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
