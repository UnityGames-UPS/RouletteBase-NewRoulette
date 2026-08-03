using System.Collections.Generic;
using UnityEngine;
internal class AudioController : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgSource;
    [SerializeField] private AudioSource gameSource;
    [SerializeField] private AudioSource uiSource;

    [Header("Background")]
    [SerializeField] private AudioClip bgMusic;

    [Header("Game Sounds")]
    [SerializeField] private AudioClip ballRolling;
    [SerializeField] private AudioClip ballStopping;
    [SerializeField] private AudioClip winPopup;

    [Header("UI Sounds")]
    [SerializeField] private AudioClip chipSound;
    [SerializeField] private AudioClip spinButton;
    [SerializeField] private AudioClip uiButton;
    [SerializeField] private AudioClip navigation;

    private void Start()
    {
        PlayBackground();
    }

    internal void PlayBackground()
    {
        if (!bgMusic) return;

        bgSource.clip = bgMusic;
        bgSource.loop = true;
        if (!bgSource.isPlaying)
            bgSource.Play();
    }

    internal void StopBackground()
    {
        bgSource.Stop();
    }

    internal void PlayBallRolling()
    {
        PlayGame(ballRolling, false);
    }

    internal void PlayBallStop()
    {
        PlayGame(ballStopping, false);
    }

    internal void PlayWinPopup()
    {
        PlayGame(winPopup, false);
    }

    private void PlayGame(AudioClip clip, bool loop)
    {
        if (!clip) return;

        gameSource.Stop();
        gameSource.clip = clip;
        gameSource.loop = loop;
        gameSource.Play();
    }

    internal void StopGameAudio()
    {
        gameSource.Stop();
        gameSource.loop = false;
    }

    internal void PlayChip()
    {
        uiSource.PlayOneShot(chipSound);
    }

    internal void PlaySpinButton()
    {
        uiSource.PlayOneShot(spinButton);
    }

    internal void PlayUIButton()
    {
        uiSource.PlayOneShot(uiButton);
    }

    internal void PlayNavigation()
    {
        uiSource.PlayOneShot(navigation);
    }

    private bool isForceMuted = false;
    private readonly Dictionary<AudioSource, bool> preFocusMuteState = new Dictionary<AudioSource, bool>();

    internal void SetMuteAll(bool forceMute)
    {
        if (forceMute == isForceMuted) return;
        isForceMuted = forceMute;

        var sources = new[] { bgSource, gameSource, uiSource };
        foreach (var source in sources)
        {
            if (source == null) continue;
            if (forceMute)
            {
                preFocusMuteState[source] = source.mute;
                source.mute = true;
            }
            else
            {
                source.mute = preFocusMuteState.TryGetValue(source, out bool prevMuted) ? prevMuted : source.mute;
            }
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        SetMuteAll(!focus);
    }

    internal void SetBGVolume(float value)
    {
        if (!bgSource) return;

        bgSource.volume = value;
        bgSource.mute = value <= 0f;

        if (value > 0f && !bgSource.isPlaying && bgMusic != null)
        {
            bgSource.clip = bgMusic;
            bgSource.loop = true;
            bgSource.Play();
        }
    }

    internal void SetSoundVolume(float value)
    {
        bool mute = value <= 0f;

        if (gameSource)
        {
            gameSource.volume = value;
            gameSource.mute = mute;
        }

        if (uiSource)
        {
            uiSource.volume = value;
            uiSource.mute = mute;
        }
    }


    internal void MuteBackground(bool mute) => bgSource.mute = mute;
    internal void MuteGame(bool mute) => gameSource.mute = mute;
    internal void MuteUI(bool mute) => uiSource.mute = mute;
}
