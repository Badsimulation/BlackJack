using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    //audio buttons
    public Button musicOn;
    public Button musicOff;

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource musicSource;

    [Header("Audio Clips")]
    public AudioClip buttonPress;
    public AudioClip betButtonPress;
    public AudioClip cardShuffle;
    public AudioClip cardPlaced;
    public AudioClip gameOverSound;

    public AudioClip regularMusic;
    public AudioClip gameOverMusic;
    
    private void Awake()
    {
        //singleton pattern
        if (instance == null)
        {
            instance = this;

            //button listeners
            musicOn.onClick.AddListener(() => MusicOnClicked());
            musicOff.onClick.AddListener(() => MusicOffClicked());
        }
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        sfxSource.PlayOneShot(clip, volume);
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    private void MusicOnClicked()
    {
        this.musicSource.UnPause();

        //swap buttons
        musicOn.gameObject.SetActive(false);
        musicOff.gameObject.SetActive(true);
    }
    private void MusicOffClicked()
    {
        this.musicSource.Pause();

        //swap buttons
        musicOn.gameObject.SetActive(true);
        musicOff.gameObject.SetActive(false);
    }
}
