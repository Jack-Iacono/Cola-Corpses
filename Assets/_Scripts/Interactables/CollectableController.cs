using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableController : MonoBehaviour
{
    public LayerMask collectLayers;

    private void Start()
    {
        Initialize();
    }
    public virtual void Initialize() { }

    private void Update()
    {
        if (!GameController.gamePaused)
        {
            ExtraUpdate();
        }
    }
    protected virtual void ExtraUpdate() { }

    protected virtual void CollectAction() { Destroy(gameObject); }

    private void OnTriggerEnter(Collider other)
    {
        if (MyFunctions.LayermaskContains(collectLayers, other.gameObject.layer))
        {
            CollectAction();
        }

        ExtraTriggerFunction(other);
    }
    protected virtual void ExtraTriggerFunction(Collider other) { }
}
