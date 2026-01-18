using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource steps;
    [SerializeField] private AudioSource recharge;
    [SerializeField] private AudioSource backgroundMenu;
    [SerializeField] private AudioSource shoot;
    [SerializeField] private AudioSource sprint;
    [SerializeField] private AudioSource jetpack;
    [SerializeField] private AudioSource take;


    private void Awake()
    {
        //PlayerPrefs.DeleteAll();
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void PlayStepsSound()
    {
        if (!steps.isPlaying)
            steps.Play();
    }

    public void PlaySprintSound()
    {
        if (!sprint.isPlaying)
            sprint.Play();
    }

    public void PlayJetpackSound()
    {
        if (!jetpack.isPlaying)
            jetpack.Play();
    }

    public void PlayRechargeSound()
    {
        recharge.Play();
    }
    public void PlayShootSound()
    {
        shoot.Play();
    }
    public void PlayBackgroundMenuPlay()
    {
        backgroundMenu.Play();
    }
    public void PlayTakePlay()
    {
        take.Play();
    }
    public void StopBackgroundMenuPlay()
    {
        backgroundMenu.Stop();
    }
    public void StopStepsSound()
    {
        if (steps.isPlaying)
            steps.Stop();
    }

    public void StopSprintSound()
    {
        if (sprint.isPlaying)
            sprint.Stop();
    }

    public void StopJetpackSound()
    {
        if (jetpack.isPlaying)
            jetpack.Stop();
    }

}
