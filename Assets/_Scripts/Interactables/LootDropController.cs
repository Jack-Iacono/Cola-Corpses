using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LootDropController : InteractableController 
{
    private Rigidbody rigidBody;
    private BoxCollider col;

    #region Materials
    //Materials for particles
    public List<Material> rarityMaterials;
    #endregion

    //Despawn time in seconds
    private float despawnTime = 20;

    #region Part Variables
    //Types of parts that could drop
    private SodaPart part;
    #endregion

    #region Overrides

    public bool isOverride = false;
    public SodaPart.Rarity rarityOverride = SodaPart.Rarity.UNKNOWN;

    #endregion

    private List<float> timers = new List<float>();

    #region Movement Variables

    private float originHeight;
    private float currentPeriod;

    private bool isMoving = false;

    #endregion

    public override void Initialize()
    {
        base.Initialize();

        rigidBody = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();

        //Adds timer to list
        timers.Add(-1f);

        SetTimer(0, despawnTime);

        if (isOverride)
            ActivatePickup(Vector3.zero, UniqueItem.supremeFizz);
    }

    protected override void ExtraUpdate()
    {
        if (!GameController.gamePaused)
        {
            //Checks to see if the object is currently moving
            if (!isMoving)
            {
                if (!ValueStoreController.isTutorial)
                    TimerManager();

                //Makes the object hover in mid air
                transform.position = new Vector3(transform.position.x, originHeight + 0.25f * Mathf.Sin(currentPeriod), transform.position.z);
                transform.Rotate(new Vector3(0, Time.deltaTime * 90, 0));
                currentPeriod += Time.deltaTime * 1.25f;
            }
        }
    }

    protected override void InRangeAction()
    {
        InteractPopUpController.StartPopup(gameObject, ValueStoreController.keyData.keyInteract.ToString(), Vector3.up * 0.5f);
    }
    protected override void OutRangeAction()
    {
        InteractPopUpController.EndPopup(gameObject);
    }

    protected override void Interact()
    {
        if (InventoryController.invCont.CheckInvOpen())
        {
            InventoryController.invCont.AddInvPart(part);
            EndPickup();
        }
    }

    #region Collision Methods

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Ground" && isMoving)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.y, 0));

            rigidBody.isKinematic = true;
            rigidBody.useGravity = false;

            col.isTrigger = true;

            //Sets the position of the object
            originHeight = transform.position.y + 1;
            currentPeriod = 0;

            GetComponent<ParticleController>().PlayParticle();

            SetTimer(0, despawnTime);

            isMoving = false;
        }
    }

    #endregion

    #region Activation Methods

    public void ActivatePickup(Vector3 force, SodaPart sPart)
    {
        //Checks for first time initialization (replaces start method since object is deactivated before start can run)
        if(timers.Count < 1)
        {
            //Adds timer to list
            timers.Add(-1f);
        }

        part = sPart;
        GetParticleMaterial();

        rigidBody.isKinematic = false;
        rigidBody.useGravity = true;

        rigidBody.AddForce(force, ForceMode.Impulse);

        col.isTrigger = false;

        isMoving = true;
    }

    public void EndPickup()
    {
        base.Interact();

        part = null;
        gameObject.SetActive(false);

        InteractPopUpController.EndPopup(gameObject);

        GetComponent<ParticleController>().StopParticle();
    }

    #endregion

    #region Timer Methods

    public void TimerManager()
    {
        if (timers.Count > 0)
        {
            for (int i = 0; i < timers.Count; i++)
            {
                if (timers[i] <= 0)
                {
                    timers[i] = 0;

                    switch (i)
                    {
                        case 0:
                            EndPickup();
                            break;
                    }

                }
                else
                {
                    timers[i] -= 1 * Time.deltaTime;
                }
            }
        }
    }

    /*  The below method uses integers to represent the different timers that may be used within this script, the ints represent the following timers
     * 0: Item Despawn Timer
    */
    public void SetTimer(int i, float f)
    {
        timers[i] = f;
    }

    #endregion

    #region Material Selection Methods

    private void GetParticleMaterial()
    {
        GetComponent<ParticleSystemRenderer>().material = rarityMaterials[(int)part.rarity];
    }

    #endregion
}
