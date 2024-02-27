using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EESoulController : MonoBehaviour
{
    public Vector3 startPosition { get; set; }
    public Vector3 endPosition { get; set; }
    private Vector3 moveUnits = Vector3.zero;
    private float moveSpeed = 7.5f;

    public EESoulBoxController targetBox { get; set; }

    public void Activate(Vector3 pos1, Vector3 pos2, EESoulBoxController boxCont)
    {
        startPosition = pos1;

        //Changes the end position if the piece is rotated
        endPosition = pos2;

        targetBox = boxCont;

        transform.position = startPosition;

        moveUnits.x = (endPosition.x - startPosition.x) * (moveSpeed / 10);
        moveUnits.y = (endPosition.y - startPosition.y) * (moveSpeed / 10);
        moveUnits.z = (endPosition.z - startPosition.z) * (moveSpeed / 10);
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.SqrMagnitude(endPosition - transform.position);

        if (distance > 0.01f)
        {
            transform.position += moveUnits * Time.deltaTime;
        }
        else
        {
            //Ensures that extra souls don't crash the game
            targetBox.SendMessage("SoulReceieve", SendMessageOptions.DontRequireReceiver);
            gameObject.SetActive(false);
        }
    }
}
