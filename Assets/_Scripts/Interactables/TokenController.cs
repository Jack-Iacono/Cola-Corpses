using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenController : CollectableController
{
    #region Gameobjects
    private Collider col;
    #endregion

    public int value = 0;
    public LayerMask terrainLayers;
    private Rigidbody rb;

    private float[] timers { get; set; } = new float[1];
    private float despawnTime = 10;

    private float currentPeriod = 0;
    private float originHeight = 0;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        for(int i = 0; i < timers.Length; i++)
        {
            timers[i] = -1;
        }

        rb.velocity = Vector3.zero;
    }

    public void Spawn(int newValue, Vector3 pos)
    {
        rb = GetComponent<Rigidbody>();
        transform.position = pos;

        rb.isKinematic = false;
        rb.useGravity = true;

        value = newValue;

        rb.AddForce(new Vector3(Random.Range(-10,10), 300, Random.Range(-10, 10)));
    }

    protected override void CollectAction()
    {
        InventoryController.invCont.AddTokens(value);

        SoundManager.PlaySoundSource(transform.position, SoundManager.coin);

        gameObject.SetActive(false);
    }
    protected override void ExtraTriggerFunction(Collider other)
    {
        if (MyFunctions.LayermaskContains(terrainLayers, other.gameObject.layer))
        {
            transform.position += Vector3.up * 0.5f;
            transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.y, 0));

            GetComponent<Collider>().isTrigger = true;
            rb.isKinematic = true;
            rb.useGravity = false;

            //Sets the position of the object
            originHeight = transform.position.y + 1;
            currentPeriod = 0;

            if (!ValueStoreController.isTutorial)
                timers[0] = despawnTime;
        }
    }

    protected override void ExtraUpdate()
    {
        TimerManager();

        if (col.isTrigger)
        {
            //Makes the object hover in mid air
            transform.position = new Vector3(transform.position.x, Mathf.MoveTowards(transform.position.y, originHeight + 0.25f * Mathf.Sin(currentPeriod), 0.005f), transform.position.z);
            transform.Rotate(new Vector3(0, Time.deltaTime * 90, 0));
            currentPeriod += Time.deltaTime * 1.25f;
        }
    }

    private void TimerManager()
    {
        for (int i = 0; i < timers.Length; i++)
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
                        gameObject.SetActive(false);
                        break;
                }

                timers[i] = -1;
            }
        }
    }
}
