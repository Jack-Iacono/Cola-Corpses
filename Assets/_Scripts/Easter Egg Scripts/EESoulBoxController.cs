using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EESoulBoxController : MonoBehaviour
{
    #region Soul Box Fill Things
    public LayerMask blockLayers;
    public Transform soulPoint;
    public bool moving = false;

    public int spawnedSouls = 0;
    public int requiredSouls { get; set; }

    [Header("Text Variables")]
    [Tooltip("True: This object has a counter for souls remaining\nFalse: This object has no text")]
    public bool hasText;
    [Tooltip("The text that will display the soul count")]
    public TMP_Text soulText;

    //For moving soul box only
    public int movesRemaining { get; set; } = -1;
    #endregion

    #region Function Variables

    private PlayerController player;
    public Vector3 mapLocation { get; set; }
    public int boxIndex { get; set; } = -1;

    private GameController gameCont;
    private MapGenerationController mapGenCont;
    private SoundManager soundCont;
    private ObjectPool objPool;

    private float soulRadius = 10f;
    public bool inRange { get; private set; } = false;

    #endregion

    public void Initialize(Vector3 location, int rSoul, int index)
    {
        player = FindObjectOfType<PlayerController>();
        gameCont = FindObjectOfType<GameController>();
        mapGenCont = FindObjectOfType<MapGenerationController>();
        soundCont = FindObjectOfType<SoundManager>();
        objPool = FindObjectOfType<ObjectPool>();

        mapLocation = location;
        boxIndex = index;

        if (moving)
        {
            movesRemaining = gameCont.checkList[4][boxIndex];

            requiredSouls = rSoul;
        }
        else
        {
            requiredSouls = rSoul;
        }

        //Sets the amount of souls that should be displayed
        if (hasText)
            soulText.text = requiredSouls.ToString();
    }
    public void Initialize(Vector3 location)
    {
        mapLocation = location;

        //Sets the amount of souls that should be displayed
        if (hasText)
            soulText.text = requiredSouls.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            other.GetComponent<Enemy>().SetSoulBox(this, true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            other.GetComponent<Enemy>().SetSoulBox(this, false);
        }
    }

    public void EnemyKill(GameObject obj)
    {
        if (enabled)
        {
            Vector3 orig = Vector3.zero;
            Vector3 dest = Vector3.zero;

            orig = soulPoint.position;
            dest = obj.transform.position;

            //Checks to ensure a surplus of souls is not spawned
            if (spawnedSouls < requiredSouls)
            {
                Ray ray = new Ray(orig, (dest - orig).normalized);
                Debug.DrawRay(orig, (dest - orig).normalized * 100, Color.cyan, 1000f);

                RaycastHit hit;

                float range = soulRadius;

                if (Physics.Raycast(ray, out hit, range, blockLayers))
                {
                    if (hit.collider.gameObject == obj)
                    {
                        //Creates a soul to move toward the box
                        GameObject soul = objPool.GetPooledObject("Soul");
                        soul.SetActive(true);
                        soul.GetComponentInChildren<EESoulController>().Activate(obj.transform.position, soulPoint.position, this);
                        spawnedSouls++;
                    }
                }
            }
        }
    }
    public void SoulReceieve()
    {
        requiredSouls--;
        spawnedSouls--;

        if (requiredSouls <= 0)
        {
            ActivateSoulBox();
        }
        else
        {
            //Sets the amount of souls that should be displayed
            if (hasText)
                soulText.text = requiredSouls.ToString();
        }

        if (moving)
            mapGenCont.map.roomList[(int)mapLocation.x].tileList[(int)mapLocation.y].partArray[(int)mapLocation.z].addData = new int[] { requiredSouls };
        else
            mapGenCont.map.roomList[(int)mapLocation.x].tileList[(int)mapLocation.y].furnArray[(int)mapLocation.z].addData = new int[] { requiredSouls };
    }

    public void ActivateSoulBox()
    {
        switch (moving)
        {
            case true:
                SoundManager.PlaySoundSource(transform.position, SoundManager.eeMove);

                if (movesRemaining > 0)
                {
                    //Moves the box to a new location

                    int oldIndex = mapGenCont.map.roomList[(int)mapLocation.x].tileList[(int)mapLocation.y].partArray[(int)mapLocation.z].partIndex;

                    //Sets the location on the map to be open
                    mapGenCont.map.roomList[(int)mapLocation.x].tileList[(int)mapLocation.y].partArray[(int)mapLocation.z] = new TilePart();

                    movesRemaining--;
                    gameCont.UpdateChecklist("movingSoulBox", boxIndex, movesRemaining);

                    mapGenCont.ReAssignPart(gameObject ,2, oldIndex);
                }
                else
                {
                    //Removes the soul Box

                    //Sends message to the game controller
                    gameCont.UpdateChecklist("movingSoulBox", boxIndex, -1);

                    //Sets the location on the map to be open
                    mapGenCont.map.roomList[(int)mapLocation.x].tileList[(int)mapLocation.y].partArray[(int)mapLocation.z] = new TilePart();

                    if (hasText)
                        soulText.text = "Full";

                    //Destroys the moving soul box
                    Destroy(gameObject);
                }
                break;
            case false:
                SoundManager.PlaySoundSource(transform.position, SoundManager.success);

                //Sends message to the game controller
                gameCont.UpdateChecklist("soulBox", boxIndex, -1);

                if (hasText)
                    soulText.text = "Full";

                //Disables the soul box controller so that it is no longer active (it can hold other parts so it has to stay)
                enabled = false;
                break;
        }
    }
}
