using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using static SodaPart;

public class CanController : MonoBehaviour
{
    private WeaponController weaponCont;
    private Rigidbody rb;

    public bool contactExplode { get; set; } = true;
    public LayerMask contactLayers;
    private int despawnTimer = 5;

    private int damage = 0;
    private float explodeRadius = 0;

    //Soda traits store the level of each trait while traitChecks is used to see if it should proc
    public int[] sodaTraits = new int[Enum.GetNames(typeof(SodaPart.Trait)).Length];
    bool[] traitChecks = new bool[Enum.GetNames(typeof(SodaPart.Trait)).Length];

    //Leaving this as an array in case of future expansion
    private float[] timers { get; set; } = new float[1];

    private Vector3 storedVelocity = Vector3.zero;

    public void Initialize()
    {
        //Makes the cans not collide with eachother or the player
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("CanProj"), LayerMask.NameToLayer("Player"));
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("CanProj"), LayerMask.NameToLayer("CanProj"));

        rb = GetComponent<Rigidbody>();

        GameController.RegisterObserver(this);
    }
    private void OnDestroy()
    {
        GameController.UnRegisterObserver(this);
    }

    public void Activate(int canDamage, float xRad, int[] sTraits, WeaponController wCont)
    {
        rb.isKinematic = false;

        weaponCont = wCont;

        damage = canDamage;
        explodeRadius = xRad;
        sodaTraits = sTraits;

        for (int i = 0; i < timers.Length; i++)
        {   
            timers[i] = -1;
        }

        for(int i = 0; i < traitChecks.Length; i++)
        {
            traitChecks[i] = false;
        }

        if (contactExplode)
        {
            timers[0] = despawnTimer * 10;
        }
        else
        {
            timers[0] = despawnTimer;
        }
    }
    
    void Update()
    {
        TimerManager();
    }
    public void GamePause(bool b)
    {
        if (b)
        {
            storedVelocity = rb.velocity;
            rb.isKinematic = true;
        }
        else if (!b)
        {
            rb.isKinematic = false;
            rb.velocity = storedVelocity;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //This statement takes the contactLayers LayerMask which is a 32 0s adn 1s (0 = no layer selected, 1 = layer selected)
        //  this is then compared to the same bitMask if the new comparing layer is added, f it is different, then the layer was not in the mask
        if (contactLayers == (contactLayers | 1 << collision.gameObject.layer))
        {
            if (contactExplode)
            {
                //Change later to unload object and create a particle effect
                CanImpact(collision);
            }
        }
    }

    public void CanImpact(Collision col)
    {
        //Initial Collision
        col.gameObject.SendMessage("CanHit", damage, SendMessageOptions.DontRequireReceiver);

        //Checks for the random chance for ammo types so that it is consistent across all hit targets
        for(int i = 0; i < sodaTraits.Length; i++)
        {
            if (sodaTraits[i] > 0)
            {
                if (weaponCont.buffData.activeBuff[(int)SodaPart.Flavor.Cola] > -1)
                {
                    traitChecks[i] = true;
                    //Ends the current timer for the buff
                    weaponCont.buffData.buffTimers[3] = 0;
                }
                else
                    traitChecks[i] = SodaPart.SodaTraitCheck(i, sodaTraits[i]);
            }
            else
            {
                traitChecks[i] = false;
            }
        }

        Explode();
        ResetCan();
    }
    public void Explode()
    {
        foreach (MeshRenderer m in GetComponentsInChildren<MeshRenderer>())
        {
            m.enabled = false;
        }
        GetComponent<Collider>().enabled = false;

        GetComponent<ParticleSystem>().Play();

        //Causes secondary explosion damage
        Collider[] coll = Physics.OverlapSphere(transform.position, explodeRadius, contactLayers);
        foreach (Collider c in coll)
        {
            //This statement takes the contactLayers LayerMask which is a 32 0s adn 1s (0 = no layer selected, 1 = layer selected)
            //  this is then compared to the same bitMask if the new comparing layer is added, f it is different, then the layer was not in the mask
            if (contactLayers == (contactLayers | 1 << c.gameObject.layer))
            {
                if (contactExplode)
                {
                    c.gameObject.SendMessage("ExplodeHit", Mathf.CeilToInt((float)damage / 2), SendMessageOptions.DontRequireReceiver);
                    SodaTraitEffects(c.gameObject);
                }
            }
        }
    }

    #region Soda Trait Methods
    private void SodaTraitEffects(GameObject target)
    {
        for(int i = 0; i < traitChecks.Length; i++)
        {
            if (traitChecks[i])
            {
                target.SendMessage("SodaTraitStart", SodaPotencyCheck((Trait)i, i), SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    #endregion

    #region Deactivation Methods
    public void OnParticleSystemStopped()
    {
        foreach(MeshRenderer m in GetComponentsInChildren<MeshRenderer>())
        {
            m.enabled = true;
        }
        GetComponent<Collider>().enabled = true;

        rb.isKinematic = false;
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;

        transform.rotation = Quaternion.Euler(Vector3.zero);
        gameObject.SetActive(false);
    }
    public void ResetCan()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = false;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        storedVelocity = Vector3.zero;
    }

    #endregion
    private void TimerManager()
    {
        for(int i = 0; i < timers.Length; i++)
        {
            if (timers[i] > 0)
            {
                //Decrements the time on an active timer
                timers[i] -= Time.deltaTime;

                if (timers[i] < 0)
                    timers[i] = 0;
            }
            //-1 is the number indicating inactive timer
            else if (timers[i] != -1)
            {
                switch (i)
                {
                    case 0:
                        Explode();
                        break;
                }

                timers[i] = -1;
            }
        }
    }
}
