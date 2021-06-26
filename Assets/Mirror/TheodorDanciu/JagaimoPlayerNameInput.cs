/////////////////////////////////////////////////////////////
//  File: JagaimoPlayerNameInput.cs
//  Creator: Theodor Danciu
//  Brief: Logic that handles the player's name when its been setup for the first time or when it loads based on the playerprefs file
/////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JagaimoPlayerNameInput : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField nameInputField = null;
    [SerializeField] private Button continueButton = null;

    public static string DisplayName { get; private set; }

    private const string playerPrefsNameKey = "PlayerName";

    private void Start() => SetUpInputField();

    /// <summary>
    /// Checks weather the player's name is saved, if it then display it on the screen
    /// </summary>
    private void SetUpInputField()
    {
        if (!PlayerPrefs.HasKey(playerPrefsNameKey))
        {
            return;
        }
        
        string defaultName = PlayerPrefs.GetString(playerPrefsNameKey);
        nameInputField.text = defaultName;

        SetPlayerName(defaultName);
    }

    /// <summary>
    /// Checks if input filed has text in it and based on it the button will become intractable 
    /// </summary>
    /// <param name="name"> Player's name </param>
    public void SetPlayerName(string name)
    {
        continueButton.interactable = !string.IsNullOrEmpty(name);
    }

    /// <summary>
    /// If the player's name is changed, then save it in the unity's save file
    /// </summary>
    public void SavePlayerName()
    {
        DisplayName = nameInputField.text;
        PlayerPrefs.SetString(playerPrefsNameKey, DisplayName);
    }
}
