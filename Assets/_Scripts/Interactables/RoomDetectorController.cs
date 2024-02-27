using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDetectorController : MonoBehaviour
{
    public int roomNumber { get; set; } = -1;
    private MapGenerationController mapGenCont;

    private void Start()
    {
        mapGenCont = FindObjectOfType<MapGenerationController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            //Trigger the map unload
            mapGenCont.SetCurrentRoom(roomNumber);
            mapGenCont.DeRenderRooms(roomNumber);
        }
    }

}
