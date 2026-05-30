using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardController : MonoSingleton<KeyboardController>
{
    public int requireHit = 6;
    private int actualHit = 0;
    private GameObject _workEffect;
    private Scrollbar _bar; // Interaction progress bar.
    private ParticleSystem _star;
    public List<AudioClip> sounds = new List<AudioClip>();
    private AudioSource _as;
    public bool IsWorkInputCompleted => actualHit >= requireHit;
    // Start is called before the first frame update

    void Start()
    {
        _workEffect = transform.parent.Find("Work").gameObject;
        _star = transform.parent.Find("Work").Find("Star").GetComponent<ParticleSystem>();
        _bar = transform.parent.Find("Work").Find("Canvas").Find("Scrollbar").GetComponent<Scrollbar>();
        _as = GetComponent<AudioSource>();
        EventManager.AddListener(EventCommon.NEXT_STAGE, ResetToDefault);
    }
    private void OnDestroy()
    {
        EventManager.RemoveListener(EventCommon.NEXT_STAGE, ResetToDefault);
    }
    public void ResetToDefault()
    {
        actualHit = 0;
        _bar.size= 0;
    }
    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TimeManager.Instance.RemoveTask(BarHide,this); // Clear any pending hide task.
            _workEffect.SetActive(true);
            TimeManager.Instance.AddTask(5, false, BarHide, this); // Hide the bar after 5 seconds.
            InstantaneousSpeedCalculator calculator = other.GetComponent<InstantaneousSpeedCalculator>();
            if (calculator != null)
            {
                // Read the current hit speed.
                Vector3 velocity = calculator.InstantaneousSpeed;
                float mag = velocity.magnitude;
                if (mag > 1.5) // Treat this as a hit.
                    HitHandle();
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        //if (other.CompareTag("Player"))
        //{
        //    _workEffect.SetActive(false);
        //}
    }
    private void BarHide()
    {
        _workEffect?.SetActive(false);
    }

    private void HitHandle()
    {
        // Play a random key hit sound.
        int randomIndex = Random.Range(0, sounds.Count);
        _as.clip = sounds[randomIndex];
        _as.Play();

        if (actualHit < requireHit)
        {
            actualHit++;
            _bar.size = ((float)actualHit / (float)requireHit);
            Debug.Log(actualHit);
        }
        else if(actualHit == requireHit)
        {
            actualHit++; // Prevent this branch from firing again on extra hits.
            _star.Play();
            // Notify that work for this stage is complete.
            EventManager.DispatchEvent<string>(EventCommon.PREPARE_CHANGE_TIME,"work");
        }
    }
}
