using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public PlayerController player;

    public TMP_Text drinkText;
    public TMP_Text throwText;
    public TMP_Text pauseText;

    // Start is called before the first frame update
    void Start()
    {
        KeybindData keys = ProfileSaveController.LoadKeyData(ValueStoreController.fileOwner);

        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = new Vector3(80,1,10);
        player.GetComponent<CharacterController>().enabled = true;

        drinkText.text = "Press " + KeybindData.KeyCodeToString(keys.keyDrink) + " to drink";
        throwText.text = "Press " + KeybindData.KeyCodeToString(keys.keyFire) + " to throw";
        pauseText.text = "Pause with " + KeybindData.KeyCodeToString(keys.keyPause) + " to see stats";
    }
}
