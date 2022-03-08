using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootEffectSound : MonoBehaviour
{
    AudioSource source;
    public AudioClip m_clip;
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.PlayOneShot(m_clip);
        Destroy(gameObject, m_clip.length);
    }

}
