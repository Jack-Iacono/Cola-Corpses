using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractableController : MonoBehaviour
{
    public static InteractableController Instance { get; private set; }

    public Collider sightOverrideCollider = null;
    private Collider sightCollider;
    public float interactRange = 10;
    public Vector3 originOffset;
    
    public bool canHit;
    public bool canInteract;

    private void Start()
    {
        Initialize();
    }
    public virtual void Initialize()
    {
        if (sightOverrideCollider != null)
            sightCollider = sightOverrideCollider;
        else
            sightCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (!GameController.gamePaused)
        {
            if (Instance == null || Instance == this)
            {
                if (CameraController.Instance.GetCameraSight(sightCollider, interactRange))
                {
                    if (Instance == null)
                    {
                        InRangeAction();
                        Instance = this;
                    }

                    if (canInteract && Input.GetKeyDown(ValueStoreController.keyData.keyInteract))
                    {
                        Interact();
                    }
                }
                else
                {
                    if (Instance == this)
                    {
                        OutRangeAction();
                    }

                    Instance = null;
                }
            }

            ExtraUpdate();
        }
    }
    protected virtual void ExtraUpdate() { }

    public void CanHit()
    {
        if (canHit)
            ObjectHit();
    }

    protected virtual void ObjectHit() { }

    protected virtual void Interact()
    {
        Instance = null;
    }

    public void SetInsance(InteractableController cont)
    {
        Instance = cont;
    }

    protected virtual void InRangeAction()
    {
        //Do Nothing
    }
    protected virtual void OutRangeAction()
    {
        //Do Nothing
    }

    ~InteractableController()
    {
        if (Instance == this)
            Instance = null;
    }
}
