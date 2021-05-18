﻿using System.Collections;
using UnityEngine;
using Cinemachine;

[AddComponentMenu("Cube Invaders/Camera Shake")]
public class CameraShake : MonoBehaviour
{
    [Header("Important")]
    [SerializeField] float amplitudeGain = 1;
    [SerializeField] float frequemcyGain = 1;
    [SerializeField] float shakeDuration = 1;

    CinemachineFreeLook cmFreeCam;
    Coroutine shakeCoroutine;

    void Awake()
    {
        //get cinemachine
        cmFreeCam = FindObjectOfType<CinemachineFreeLook>();

        //by default set noise to 0
        Noise(0, 0);
    }

    public void DoShake()
    {
        //start coroutine
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        if(gameObject.activeInHierarchy)
            shakeCoroutine = StartCoroutine(ShakeCoroutine(amplitudeGain, frequemcyGain, shakeDuration));
    }

    public void DoShake(float amplitude, float frequency, float duration)
    {
        //start coroutine
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        if (gameObject.activeInHierarchy)
            shakeCoroutine = StartCoroutine(ShakeCoroutine(amplitude, frequency, duration));
    }

    public IEnumerator ShakeCoroutine(float amplitude, float frequency, float duration)
    {
        //noise
        Noise(amplitude, frequency);

        //wait and stop noise
        yield return new WaitForSeconds(duration);
        Noise(0, 0);
    }

    void Noise(float amplitude, float frequency)
    {
        //set amplitude
        cmFreeCam.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = amplitude;
        cmFreeCam.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = amplitude;
        cmFreeCam.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = amplitude;

        //set frequency
        cmFreeCam.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = frequency;
        cmFreeCam.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = frequency;
        cmFreeCam.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = frequency;
    }
}
