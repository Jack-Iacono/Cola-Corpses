using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EEHSController : InteractableController
{
    //X = room index, y = tile index, z = partArray index
    public Vector3 mapLocation { get; set; }
    public int hsIndex = -1;

    private int movesRemaining = 0;

    public void Initialize(Vector3 location, int moves, int index)
    {
        mapLocation = location;
        hsIndex = index;

        if (GameController.gameCont.checkList[5][hsIndex] == -1)
        {
            movesRemaining = moves;
        }
        else
        {
            movesRemaining = GameController.gameCont.checkList[5][hsIndex];
        }

        GameController.gameCont.UpdateChecklist("hideAndSeek", hsIndex, movesRemaining);
    }
    public void Initialize(Vector3 location)
    {
        mapLocation = location;
    }

    protected override void Interact()
    {
        base.Interact();

        SoundManager.PlaySoundSource(transform.position, SoundManager.eeMove);

        if (movesRemaining > 0)
        {
            int oldIndex = MapGenerationController.mapGenCont.map.roomList[(int)mapLocation.x].tileList[(int)mapLocation.y].partArray[(int)mapLocation.z].partIndex;

            //Sets the location on the map to be open
            MapGenerationController.mapGenCont.map.roomList[(int)mapLocation.x].tileList[(int)mapLocation.y].partArray[(int)mapLocation.z] = new TilePart();

            //Subtracts one move from the object
            movesRemaining--;
            GameController.gameCont.UpdateChecklist("hideAndSeek", hsIndex, movesRemaining);

            //Changes the position of the object
            MapGenerationController.mapGenCont.ReAssignPart(gameObject, 3, oldIndex);
        }
        else
        {
            //Sends message to the game controller
            GameController.gameCont.UpdateChecklist("hideAndSeek", hsIndex, -1);

            //Sets the location on the map to be open
            MapGenerationController.mapGenCont.map.roomList[(int)mapLocation.x].tileList[(int)mapLocation.y].partArray[(int)mapLocation.z] = new TilePart();

            //Gets rid of the piece
            Destroy(gameObject);
        }
    }
}
