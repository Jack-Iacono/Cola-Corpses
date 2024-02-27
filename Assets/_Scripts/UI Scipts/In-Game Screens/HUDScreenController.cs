using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDScreenController : InterfaceScreenController
{
    [Header("UI Elements")]
    public TMP_Text messageText;
    public Image healthImage;
    public TMP_Text tokenText;
    public TMP_Text waveText;

    public Sprite[] buffArray;
    public GameObject buffArea;
    private List<GameObject> activeBuffs = new List<GameObject>();

    public GameObject reticle;

    public Animator healthAnim;

    public override void InitializeScreen()
    {
        base.InitializeScreen();

        for (int j = 0; j < buffArea.transform.childCount; j++)
        {
            activeBuffs.Add(buffArea.transform.GetChild(j).gameObject);
            activeBuffs[j].SetActive(false);
        }

        NewWave((GameController.waveNum + 1).ToString());
        NewReticle(0);
    }

    public override void ShowScreen()
    {
        NewToken(InventoryController.invCont.tokens);
        base.ShowScreen();
    }
    public override void HideScreen()
    {
        base.HideScreen();
    }

    public void NewHealth(float percent)
    {
        // If the object isn't active, just change the health without animation
        if(gameObject.activeInHierarchy)
            StartCoroutine(MoveHealthBar(percent));
        else
            healthImage.fillAmount = percent;
    }
    IEnumerator MoveHealthBar(float percent)
    {
        int steps = 30;

        for (int i = 0; i < steps; i++)
        {
            healthImage.fillAmount = Mathf.MoveTowards(healthImage.fillAmount, percent, 0.5f / steps);
            yield return new WaitForSeconds(0.5f / steps);
        }
    }

    public void NewMessage(string msg)
    {
        messageText.text = msg;
    }
    public void NewToken(int tokens)
    {
        tokenText.text = tokens.ToString();
    }
    public void NewWave(string s)
    {
        waveText.text = s;
    }

    public void UpdateBuffs(BuffData buffData)
    {
        int count = 0;

        for (int i = 0; i < buffData.buffTimers.Length; i++)
        {
            if (buffData.buffTimers[i] >= 0)
            {
                activeBuffs[count].SetActive(true);
                activeBuffs[count].GetComponent<Image>().sprite = buffArray[i];
                activeBuffs[count].GetComponent<Image>().fillAmount = buffData.buffTimers[i] / buffData.buffMaxTime[i];

                activeBuffs[count].GetComponentInChildren<TMP_Text>().text = (buffData.activeBuff[i] + 1).ToString();

                count++;
            }
        }

        //Removes the unused buffs
        for (int i = count; i < activeBuffs.Count; i++)
        {
            activeBuffs[i].SetActive(false);
        }
    }

    public void NewReticle(float fillPercent)
    {
        reticle.GetComponent<Image>().fillAmount = fillPercent;

        if (fillPercent <= 0)
            reticle.SetActive(false);
        else
            reticle.SetActive(true);
    }
    public void NewReticle(float fillPercent, string text)
    {
        reticle.GetComponent<Image>().fillAmount = (float)fillPercent;
        reticle.GetComponentInChildren<TMP_Text>().text = (string)text;

        if (fillPercent <= 0)
            reticle.SetActive(false);
        else
            reticle.SetActive(true);
    }

}
