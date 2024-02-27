using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EETargetController : InteractableController
{
    //X = room index, y = tile index, z = partArray index
    public Vector3 mapLocation { get; set; }
    public int targetIndex = -1;

    public void Initialize(Vector3 location, int index)
    {
        mapLocation = location;

        targetIndex = index;

        GameController.gameCont.UpdateChecklist("target", targetIndex, 1);
    }
    public void Initialize(Vector3 location)
    {
        mapLocation = location;
    }

    protected override void ObjectHit()
    {
        //Sends message to the game controller
        GameController.gameCont.UpdateChecklist("target", targetIndex, -1);

        SoundManager.PlaySoundSource(transform.position, SoundManager.eeHit);

        //Sets the location on the map to be open
        MapGenerationController.mapGenCont.map.roomList[(int)mapLocation.x].tileList[(int)mapLocation.y].partArray[(int)mapLocation.z] = new TilePart();

        //Gets rid of the piece
        Destroy(gameObject);
    }
}
