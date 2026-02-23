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

    // Initializerer audio systemet ved at preload det første klip.
    void Start()
    {
        // Tjekker at vi faktisk har clips og en AudioSource
        if (audioClips.Length > 0 && audioSource != null)
        {
            PreloadClip(audioClips[0]);
            nextStartTime = AudioSettings.dspTime;
            StartCoroutine(PlayLoop());
        }
    }


    // Preloader et klip ved at starte det og stoppe det med det samme. Dette tvinger Unity til at indlæse klippet i memory,
    void PreloadClip(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
        audioSource.Stop();
    }

    // Coroutine der scheduler og spiller audio clips i loop.
    // Invoker et event queued track starter.
    IEnumerator PlayLoop()
    {
        // Kører uendeligt indtil StopAllCoroutines bliver kaldt
        while (true)
        {
            // Hvis vi har bedt om at spille næste track (via TriggerNextAudio)
            // og vi har mere end ét klip,
            // og vi endnu ikke har spillet det alternative track
            if (isWaitingForNext && audioClips.Length > 1 && !hasPlayedDifferentTrack)
            {
                // Skifter til det klip der er queued
                currentClipIndex = nextAudioToPlay;
                hasPlayedDifferentTrack = true;
            }
            else
            {
                // Ellers fallback til default (main sang loopet)
                currentClipIndex = 0;
            }

            // Henter det klip der skal afspilles i denne iteration
            AudioClip clipToPlay = audioClips[currentClipIndex];
            audioSource.clip = clipToPlay;

            // Planlægger præcist hvornår klippet skal starte
            audioSource.PlayScheduled(nextStartTime);
            nextStartTime = AudioSettings.dspTime + clipToPlay.length;

            // Hvis vi er i gang med et queued track, Notify alle der er subscribet til eventet at en ny sang er startet. 
            if (isWaitingForNext)
            {
                OnQueuedTrackStarted?.Invoke();
            }

            //gemmer hvor langt tid det tager før dette lydKlip er færdigt.
            double waitUntil = nextStartTime;

            // Venter frame-for-frame indtil DSP-tiden når slutningen.
            // yield return null betyder: vent én frame og fortsæt.
            while (AudioSettings.dspTime < waitUntil)
            {
                yield return null;
            }

            // Hvis vi spillede et alternativt klip (index > 0), så gå tilbage til main loopet.
            if (currentClipIndex > 0)
            {
                isWaitingForNext = false;
                hasPlayedDifferentTrack = false;
            }
        }
    }

    // Starter det næste playback af det næste audio clip.
    public void TriggerNextAudio()
    {
        isWaitingForNext = true;
    }

    // stopper loop

    public void StopLooping()
    {
        StopAllCoroutines();
        audioSource.Stop();
    }
}

