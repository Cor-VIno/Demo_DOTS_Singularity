using UnityEngine;
using UnityEngine.UI;

public class BGMController : MonoBehaviour
{
    [Header("音乐文件")]
    public AudioClip introClip;
    public AudioClip loopClip;

    [Header("UI 控制")]
    public Slider volumeSlider;

    private AudioSource _introSource;
    private AudioSource _loopSource;

    void Start()
    {
        _introSource = gameObject.AddComponent<AudioSource>();
        _loopSource = gameObject.AddComponent<AudioSource>();

        _introSource.clip = introClip;
        _loopSource.clip = loopClip;

        _loopSource.loop = true;
        float initialVolume = volumeSlider != null ? volumeSlider.value : 0.5f;
        SetVolume(initialVolume);

        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        if (introClip != null && loopClip != null)
        {
            double startDspTime = AudioSettings.dspTime + 0.1;

            double introDuration = (double)introClip.samples / introClip.frequency;

            _introSource.PlayScheduled(startDspTime);
            _loopSource.PlayScheduled(startDspTime + introDuration);
        }
    }
    public void SetVolume(float vol)
    {
        if (_introSource != null) _introSource.volume = vol;
        if (_loopSource != null) _loopSource.volume = vol;
    }
}