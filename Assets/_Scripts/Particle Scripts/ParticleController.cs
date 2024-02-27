using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    private ParticleSystem part;
    private ParticleSystem.EmissionModule emMod;
    private ParticleSystem.ShapeModule shapeMod;
    private ParticleSystemRenderer partRend;

    private int loopCounter = -1;

    void Start()
    {
        //Sets up the particle component to get as well as the emission module
        part = GetComponent<ParticleSystem>();
        emMod = part.emission;
        shapeMod = part.shape;
        partRend = GetComponent<ParticleSystemRenderer>();
    }

    public void PlayParticle()
    {
        //Plays particle when called
        part.Play();
    }
    public void LoopParticle(int i)
    {
        if (loopCounter == -1)
        {
            loopCounter = i;
        }

        if (loopCounter > 0)
        {
            part.Play();
        }
        else
        {
            loopCounter = -1;
        }
    }
    public void SetParticleAmount(int i)
    {
        emMod.enabled = true;
        emMod.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0.0f, i) });
    }
    public void SetRotation(Vector3 dir)
    {
        //This function only handles the Y rotation for now

        shapeMod.enabled = true;

        //Adjusts to compensate for the direction the zombie is facing
        float pRotation = Mathf.Repeat(transform.rotation.eulerAngles.y + 180, 360) - 180;
        shapeMod.rotation = Vector3.up * (Mathf.DeltaAngle(pRotation, dir.y));
    }
    public void StopParticle()
    {
        part.Stop();
    }
    public void SetParticleColor(Material mat)
    {
        partRend.material = mat;
        partRend.trailMaterial = mat;
    }
}
