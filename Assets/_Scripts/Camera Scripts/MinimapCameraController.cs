using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MinimapCameraController : MonoBehaviour
{
    public GameObject followObject;
    public Vector3 offset;
    public Vector3 rotation;

    public LayerMask renderLayers;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = followObject.transform.position + offset;
        transform.rotation = Quaternion.Euler(rotation);
    }

    private void Update()
    {
        transform.position = new Vector3
            (
            followObject.transform.position.x + offset.x,
            transform.position.y,
            followObject.transform.position.z + offset.z
            );
        //Allows the camera to rotate with the player
        transform.rotation = Quaternion.Euler(new Vector3(rotation.x, followObject.transform.rotation.eulerAngles.y, 0));
    }
}
