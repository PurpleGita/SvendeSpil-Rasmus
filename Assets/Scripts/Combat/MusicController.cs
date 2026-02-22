using UnityEngine;
using System;
using System.Collections;

public class MusicController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] audioClips;
    private int currentClipIndex = 0;
    private bool isWaitingForNext = false;
    private bool hasPlayedDifferentTrack = false;
    private double nextStartTime = 0;
    public int nextAudioToPlay = 1;

    // event til at sige når den næste queued starter med at spille
    public event Action OnQueuedTrackStarted;

    /// Initializerer audio systemet ved at preload det første klip.
    void Start()
    {
        if (audioClips.Length > 0 && audioSource != null)
        {
            PreloadClip(audioClips[0]);
            nextStartTime = AudioSettings.dspTime;
            StartCoroutine(PlayLoop());
        }
    }


    /// Preloader et klip ved at starte det og stoppe det med det samme.
    void PreloadClip(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
        audioSource.Stop();
    }

    /// Coroutine der scheduler og spiller audio clips i loop.
    /// Invoker et event queued track starter.
    IEnumerator PlayLoop()
    {
        while (true)
        {
            if (isWaitingForNext && audioClips.Length > 1 && !hasPlayedDifferentTrack)
            {
                currentClipIndex = nextAudioToPlay;
                hasPlayedDifferentTrack = true;
            }
            else
            {
                currentClipIndex = 0;
            }

            AudioClip clipToPlay = audioClips[currentClipIndex];
            audioSource.clip = clipToPlay;
            audioSource.PlayScheduled(nextStartTime);
            nextStartTime = AudioSettings.dspTime + clipToPlay.length;

            // Notify alle der er subscribet til eventet når en ny sang begynder at en ny sang er startet.
            if (isWaitingForNext)
            {
                OnQueuedTrackStarted?.Invoke();
            }

            //gemmer hvor langt tid det tager før dette lydKlip er færdigt.
            double waitUntil = nextStartTime;

            while (AudioSettings.dspTime < waitUntil)
            {
                yield return null;
            }

            if (currentClipIndex > 0)
            {
                isWaitingForNext = false;
                hasPlayedDifferentTrack = false;
            }
        }
    }

    /// Triggers the playback of the next audio clip in the array on the next loop iteration.
    public void TriggerNextAudio()
    {
        isWaitingForNext = true;
    }

    /// Stops all running coroutines and stops audio playback.

    public void StopLooping()
    {
        StopAllCoroutines();
        audioSource.Stop();
    }
}

