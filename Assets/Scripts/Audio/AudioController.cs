using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AudioController : MonoBehaviour
{
    //this would've been much easier with odin inspector
    private static Dictionary<string, AudioClip> m_AudioClips;

    [SerializeField] private string[] m_ClipIds;
    [SerializeField] private AudioClip[] m_Clips;

    [SerializeField] private GameObject m_SoundsGO;
    [SerializeField] private GameObject m_MusicGO;

    private static AudioSource m_SoundAudioSource;
    private static AudioSource m_MusicAudioSource;

    private void Awake()
    {
        m_SoundAudioSource = m_SoundsGO.GetComponent<AudioSource>();
        m_MusicAudioSource = m_MusicGO.GetComponent<AudioSource>();
        m_AudioClips = new Dictionary<string, AudioClip>();

        for(int i = 0; i < m_ClipIds.Length; i++)
        {
            m_AudioClips.Add(m_ClipIds[i], m_Clips[i]);
        }
    }

    public static void PlaySound(string clipId)
    {
        AudioClip clip;
        if (m_AudioClips.TryGetValue(clipId, out clip))
            m_SoundAudioSource.PlayOneShot(clip);
        else
            Debug.LogError("Missing audio clip! Given id: " + clipId);
    }

    public static void PlayMusic(string clipId)
    {
        AudioClip clip;
        if (m_AudioClips.TryGetValue(clipId, out clip))
        {
            m_MusicAudioSource.clip = clip;
            m_MusicAudioSource.Play();
        }
        else
            Debug.LogError("Missing audio clip! Given id: " + clipId);
    }
}

