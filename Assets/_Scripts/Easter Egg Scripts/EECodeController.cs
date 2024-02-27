using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EECodeController : InteractableStationController
{
    //X = room index, y = tile index, z = furnArray index
    public Vector3 mapLocation { get; set; }
    public TMP_Text signText;
    public int codeIndex = -1;

    private bool activated = false;

    public void Initialize(Vector3 location, int index)
    {
        mapLocation = location;

        codeIndex = index;

        //Differentiate betweeen active and done somehow
        switch (GameController.gameCont.checkList[GameController.GetEEIndex("wallCode")][codeIndex])
        {
            case -1:
                activated = true;
                break;
            case 1:
                activated = false;
                break;
        }

        signText.text = (codeIndex + 1).ToString();
    }

    protected override void Interact()
    {
        base.Interact();

        if (!activated)
        {
            if (codeIndex == 0 || GameController.gameCont.checkList[GameController.GetEEIndex("wallCode")][codeIndex - 1] == -1)
            {
                activated = true;

                GameObject soundSrc = ObjectPool.objPool.GetPooledObject("SoundSource");
                if (soundSrc)
                {
                    soundSrc.SetActive(true);
                    soundSrc.GetComponent<SoundSourceController>().PlaySound(transform.position, SoundManager.eeMove);
                }

                //Sends message to the game controller
                GameController.gameCont.UpdateChecklist("wallCode", codeIndex, -1);
            }
            else
            {
                foreach (EECodeController codes in FindObjectsOfType<EECodeController>())
                {
                    codes.activated = false;
                }

                GameController.gameCont.ResetList("wallCode");
            }
        }
    }

    protected override void InRangeAction()
    {
        
    }
    protected override void OutRangeAction()
    {
        
    }
}
