////////////////////////////////////////////////////////////
// File: CreditsScreen.cs
// Author: Charles Carter
// Date Created: 25/05/21
// Brief: The script for the credits screen in the main menu
////////////////////////////////////////////////////////////

//This script uses these namespaces
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CreditsScreen : MonoBehaviour {
    #region Variables Needed

    [SerializeField]
    private ScrollRect scrollingText;
    [SerializeField]
    private float fCreditsTime = 3f;

    #endregion

    private void Awake() {
        if (!scrollingText && Debug.isDebugBuild) {
            Debug.Log("No scroll rect selected for credits", this);
        }
    }

    private void OnEnable() {
        Debug.Log("Scrolling credits");

        if (!scrollingText) {
            scrollingText = GetComponentInChildren<ScrollRect>();
        }

        StartCoroutine(Co_ScrollingText(fCreditsTime));
    }

    private void OnDisable() {
        StopCoroutine(Co_ScrollingText(fCreditsTime));

        if (scrollingText) {
            scrollingText.verticalNormalizedPosition = 1;
        }
    }

    private IEnumerator Co_ScrollingText(float textTimer) {
        for (float t = 1; t > 0; t -= Time.deltaTime / textTimer) {
            scrollingText.verticalNormalizedPosition = t;
            yield return null;
        }
    }
}
