using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static PlayerController playerCont;

    #region Objects
    public CharacterController charCont;
    private InterfaceController intCont;
    private GameController gameCont;
    private SoundManager soundCont;
    #endregion

    #region Controller Variables
    public int maxHealth;

    public float moveSpeed = 10;
    public float runSpeed = 15;
    public float jumpHeight = 6;
    public float gravity = 9.8f;

    public float accel = 5;
    public float decel = 5;

    [Serialize]
    public bool autoJump = true;
    #endregion

    #region Private Variables
    private float moveX;
    private float moveY;
    private float moveZ;

    public int currentHealth;

    private float jumpMod = 1;

    public float moveMod { get; set; } = 1;
    private bool allowBHop = false;

    public bool invincible { get; set; }

    public float currentSpeed { get; set; } = 0;
    #endregion

    #region Initialization
    //Setting the static reference
    public void SetStaticInstance()
    {
        if(playerCont != null && playerCont != this)
        {
            Destroy(this);
        }
        else
        {
            playerCont = this;
        }
    }
    public void Initialize()
    {
        intCont = FindObjectOfType<InterfaceController>();
        soundCont = FindObjectOfType<SoundManager>();
        charCont = GetComponent<CharacterController>();
        gameCont = GameController.gameCont;

        if (ValueStoreController.loadData)
        {
            PlayerData playerCont = ValueStoreController.loadedGameData.playerData;

            maxHealth = playerCont.maxHealth;
            moveSpeed = playerCont.moveSpeed;
            runSpeed = playerCont.runSpeed;
            jumpHeight = playerCont.jumpHeight;
            gravity = playerCont.gravity;
            accel = playerCont.accel;
            decel = playerCont.decel;
            autoJump = playerCont.autoJump;
            currentHealth = playerCont.currentHealth;
            allowBHop = playerCont.allowBHop;

            charCont.enabled = false;
            transform.position = playerCont.position;
            transform.rotation = Quaternion.Euler(playerCont.rotation);
            charCont.enabled = true;
        }
        else
        {
            currentHealth = maxHealth;
        }

        intCont.screenPair[InterfaceController.Screen.HUD].GetComponent<HUDScreenController>().NewHealth((float)currentHealth / maxHealth);
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        //Pauses the game when the pause menu is up
        if (!GameController.gamePaused)
        {
            if (currentHealth <= 0)
            {
                //Kills the player, but really just exits the game
                gameCont.EndGame(false);
            }

            MovePlayer();
        }
    }

    #region Movement Methods
    private void MovePlayer()
    {
        moveX = AccelerationCalc(moveX, ValueStoreController.keyData.keyForward, ValueStoreController.keyData.keyBackward);
        moveZ = AccelerationCalc(moveZ, ValueStoreController.keyData.keyRight, ValueStoreController.keyData.keyLeft);

        PlayerJump();

        //Gravity
        if (!charCont.isGrounded)
        {
            if(moveY > 0)
            {
                moveY -= gravity * Time.deltaTime * 1.5f;
            }
            else
            {
                moveY -= gravity * Time.deltaTime * 0.75f;
            }
        }

        //Takes player made movements and accounts for them within a new Vector3
        Vector3 moveFinal = (moveX * transform.forward * moveSpeed * jumpMod * moveMod) + (moveY * transform.up) + (moveZ * transform.right * moveSpeed * jumpMod * moveMod);
        
        if(moveFinal.y < 0 && !charCont.isGrounded)
            currentSpeed = 1;
        else
            currentSpeed = Mathf.Sqrt((moveFinal.x * moveFinal.x) + (moveFinal.z * moveFinal.z));

        charCont.Move(moveFinal * Time.deltaTime);
    }
    private void PlayerJump()
    {
        if (((Input.GetKey(ValueStoreController.keyData.keyJump) && autoJump)|| (Input.GetKeyDown(ValueStoreController.keyData.keyJump) && !autoJump)) && charCont.isGrounded)
        {
            SoundManager.PlaySound(SoundManager.jump);
            moveY = jumpHeight;

            if (allowBHop)
            {
                if (jumpMod < 2)
                {
                    jumpMod += 0.5f;
                }

                if (jumpMod > 2)
                {
                    jumpMod = 1.5f;
                }
            }
        }
        else if(charCont.isGrounded)
        {
            
            if (allowBHop)
            {
                if (jumpMod > 1)
                {
                    jumpMod -= decel * Time.deltaTime;
                }
                else if (jumpMod < 1)
                {
                    jumpMod = 1;
                }
            }
        }
    }

    //Sets the acceleration value of the player given the button that is being pressed
    private float AccelerationCalc(float move, KeyCode pos, KeyCode neg)
    {
        if (Input.GetKey(pos) && move < 1)
        {
            move += accel / 100;
        }
        else if (Input.GetKey(neg) && move > -1)
        {
            move -= accel / 100;
        }
        else
        {
            move = Mathf.MoveTowards(move, 0, decel / 100);
        }

        //Ensures that the values will never exceed 1 and -1 so that the speed of the player remains consistent
        move = Mathf.Clamp(move, -1f, 1f);

        return move;
    }
    public void WarpPlayer(Vector3 pos)
    {
        charCont.enabled = false;
        transform.position = pos;
        charCont.enabled = true;
    }
    #endregion

    #region Health Methods

    public void ChangeHealth(int change)
    {
        if (!invincible)
        {
            if (change < 0)
            {
                SoundManager.PlaySound(SoundManager.hurt);
            }

            currentHealth = Mathf.Clamp(currentHealth + change, 0, maxHealth);

            intCont.screenPair[InterfaceController.Screen.HUD].GetComponent<HUDScreenController>().NewHealth((float)currentHealth / maxHealth);
        }
    }

    public int GetHealthInt()
    {
        return (int)currentHealth;
    }
    public void SetHealth(int i)
    {
        //For debug purposes
        currentHealth = i;
        if(currentHealth > maxHealth)
            maxHealth = currentHealth;

        intCont.screenPair[InterfaceController.Screen.HUD].GetComponent<HUDScreenController>().NewHealth((float)currentHealth / maxHealth);
    }

    #endregion

    #region Enemy Methods

    public void EnemyHit(int damage)
    {
        ChangeHealth(-damage);
    }

    #endregion

    #region Saving Methods

    public PlayerData GetPlayerData()
    {
        PlayerData playerData = new PlayerData();
        playerData.maxHealth = maxHealth;
        playerData.moveSpeed = moveSpeed;
        playerData.runSpeed = runSpeed;
        playerData.jumpHeight = jumpHeight;
        playerData.gravity = gravity;
        playerData.accel = accel;
        playerData.decel = decel;
        playerData.autoJump = autoJump;
        playerData.currentHealth = currentHealth;
        playerData.allowBHop = allowBHop;

        playerData.position = transform.position;
        playerData.rotation = transform.rotation.eulerAngles;

        return playerData;
    }

    #endregion

    #region Buff Methods

    

    #endregion

}