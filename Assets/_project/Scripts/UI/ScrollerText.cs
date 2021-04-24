////////////////////////////////////////////////////////////
// File: ScrollerText.cs
// Author: Charles Carter
// Date Created: 16/02/21
// Brief: The text that shows events from the game
//////////////////////////////////////////////////////////// 

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScrollerText : MonoBehaviour
{
    private enum RoundTexts
    {
        START       = 0, 
        TAGGED      = 1,
        ELIMINATED  = 2,
        PLAYERS_WON  = 3,
        BLUE_TEAM_GOAL = 4,
        RED_TEAM_GOAL = 5
    }

    [SerializeField]
    private RoundManager manager;

    //The text queue
    [SerializeField]
    private Queue<GameObject> textmeshlist = new Queue<GameObject>();

    [SerializeField]
    private int maxText = 4;

    [SerializeField]
    private GameObject textParent;
    [SerializeField]
    private GameObject[] textMeshPrefabs;

    [SerializeField]
    private List<RectTransform> rectTransforms;

    private void OnEnable()
    {
        RoundManager.CountdownStarted += AddGameStartText;
        RoundManager.CountdownEnded += AddEliminationText;
        RoundManager.RoundEnded += AddWinText;
    }

    private void OnDisable()
    {
        RoundManager.CountdownStarted -= AddGameStartText;
        RoundManager.CountdownEnded -= AddEliminationText;
        RoundManager.RoundEnded -= AddWinText;
    }

    //Checking the current queue
    private void CheckQueue()
    {
        //Going through the queue
        for (int i = 0; i < rectTransforms.Count; ++i)
        {
            if (rectTransforms[i])
            {
                //Getting the new pos for the text object
                Vector3 newPos = new Vector3(rectTransforms[i].localPosition.x, rectTransforms[i].localPosition.y + 20, rectTransforms[i].localPosition.z);

                //Moving it upwards
                rectTransforms[i].localPosition = newPos;
            }
        }
    }

    private void CheckTop()
    {
        if (isOverMaxLength())
        {            
            GameObject texttoremove = textmeshlist.Dequeue();
            rectTransforms.Remove(texttoremove.GetComponent<RectTransform>());
            Destroy(texttoremove);
        }
    }

    private bool isOverMaxLength()
    {
        //If the count is above the max text
        if (textmeshlist.Count > maxText)
        {
            return true;
        }

        return false;
    }

    //Functions to add the different texts
    private void AddGameStartText()
    {
        AddText(RoundTexts.START);
    }

    public void AddTaggedText()
    {
        AddText(RoundTexts.TAGGED);
    }

    public void AddEliminationText()
    {
        AddText(RoundTexts.ELIMINATED);
    }

    public void AddWinText()
    {
        AddText(RoundTexts.PLAYERS_WON);
    }

    public void AddGoalText(bool blueTeamScored)
    {
        if (blueTeamScored)
        {
            AddText(RoundTexts.BLUE_TEAM_GOAL);
        }
        else
        {
            AddText(RoundTexts.RED_TEAM_GOAL);
        }
    }

    //Instantiating the text object and adding it to the queue, then checking to see if a text needs removing
    private void AddText(RoundTexts REvent)
    {
        //Debug.Log(REvent.ToString());

        //Moving the other texts up
        CheckQueue();

        //Adding the new text
        GameObject gObject = Instantiate(textMeshPrefabs[(int)REvent], textParent.transform);
        textmeshlist.Enqueue(gObject);
        rectTransforms.Add(gObject.GetComponent<RectTransform>());

        //Seeing if the top text needs to be removed
        CheckTop();
    }
}
