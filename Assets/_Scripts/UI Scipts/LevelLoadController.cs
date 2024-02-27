using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoadController : MonoBehaviour
{
    public Animator transition;

    public List<Animator> transitionList;

    public int selectedTransition = 0;

    public float transitionTime = 1f;

    private void Start()
    {
        //DeActivates the screens that are not being used
        foreach (Animator anim in transitionList)
        {
            if (anim.name != transitionList[selectedTransition].name)
                anim.gameObject.SetActive(false);
            else
                anim.gameObject.SetActive(true);
        }
    }

    public void LoadLevel(string levelName)
    {
        StartCoroutine(RunTransition(levelName));
    }
    public void LoadLevel(int levelIndex)
    {
        StartCoroutine(RunTransition(levelIndex));
    }

    IEnumerator RunTransition(string levelName)
    {
        //Tells the animator to begin transitioning
        transition.SetTrigger("start");

        //Waits for the set seconds before moving to the next scene
        yield return new WaitForSeconds(transitionTime);

        //Switches to the next scene
        SceneManager.LoadScene(levelName, LoadSceneMode.Single);
    }
    IEnumerator RunTransition(int levelIndex)
    {
        //Tells the animator to begin transitioning
        transition.SetTrigger("start");

        //Waits for the set seconds before moving to the next scene
        yield return new WaitForSeconds(transitionTime);

        //Switches to the next scene
        SceneManager.LoadScene(levelIndex, LoadSceneMode.Single);
    }

}
