using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class DropStatController : MonoBehaviour
{
    public TMP_Text statText;

    private GameObject followObject;

    private void Update()
    {
        transform.localRotation = Quaternion.Euler(0, 180 / Mathf.PI * Mathf.Atan2(transform.position.x - followObject.transform.position.x, transform.position.z - followObject.transform.position.z), 0);
    }

    #region Activation and Deactivation

    public void StartStatDisplay(string text, Vector3 pos, GameObject follow)
    {
        statText.text = text;
        transform.position = pos;

        followObject = follow;
    }

    public void EndStatDisplay()
    {
        statText.text = "";
        transform.position = Vector3.zero;

        gameObject.SetActive(false);
    }

    #endregion
}
