using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndividualParticleController : MonoBehaviour
{
    private ParticleSystem part;

    private void OnParticleSystemStopped()
    {
        if (!part)
        {
            part = GetComponent<ParticleSystem>();
        }

        gameObject.SetActive(false);
    }
}
