using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EEPieceController : InteractableController
{
    //X = room index, y = tile index, z = partArray index
    public Vector3 mapLocation { get; set; }
    public int pieceIndex = -1;

    public void Initialize(Vector3 location, int index)
    {
        mapLocation = location;

        pieceIndex = index;

        GameController.gameCont.UpdateChecklist("part", pieceIndex, 1);
    }
    public void Initialize(Vector3 location)
    {
        mapLocation = location;
    }

    protected override void Interact()
    {
        base.Interact();

        SoundManager.PlaySoundSource(transform.position, SoundManager.eePickup);

        //Sends message to the game controller
        GameController.gameCont.UpdateChecklist("part", pieceIndex, -1);

        //Sets the location on the map to be open
        MapGenerationController.mapGenCont.map.roomList[(int)mapLocation.x].tileList[(int)mapLocation.y].partArray[(int)mapLocation.z] = new TilePart();

        //Gets rid of the piece
        Destroy(gameObject);
    }
}
