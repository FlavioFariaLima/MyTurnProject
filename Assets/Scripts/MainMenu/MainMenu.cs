using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    // Managers
    public StartManager startManager;
    public CreateCharacter createCharacter;

    [Header("Menu Options")]
    [SerializeField] private List<MainMenuOptions> options;

    private MainMenuOptions selectedPanel;
    public MainMenuOptions GetSelectedPanel()
    {
        return selectedPanel;
    }


    [Header("Scenes")]
    [SerializeField] private string newMatchScene;

    [Header("Misc")]
    [SerializeField] private Slider loaderBar;    

    // Start is called before the first frame update
    void Awake()
    {
        // Managers
        startManager = GetComponent<StartManager>();
        createCharacter = GetComponent<CreateCharacter>();

        // Hide Stuff
        loaderBar.gameObject.SetActive(false);

        // Set Main Menu Options
        foreach (MainMenuOptions m in options)
        {
            if (m.destinyPanel != null)
                m.destinyPanel.SetActive(false);
        }        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowOptionPanel(string option)
    {
        Debug.Log($"Clicked {option}");

        //options.Find(x => x.optionName == option).destinyPanel.SetActive(true);

        foreach (MainMenuOptions m in options)
        {
            if (m.destinyPanel != null && m.optionName == option)
            {
                selectedPanel = m;
                m.destinyPanel.SetActive(true);
            }
            else if (m.destinyPanel != null && m.optionName != option)
                m.destinyPanel.SetActive(false);
        }

        // Set Individual
        if (option == "NewGame")
            startManager.RecountCharacters();
    }

    public void LoadMatchScene()
    {
        StartCoroutine(LoadSceneAsync(newMatchScene));
    }

    IEnumerator LoadSceneAsync(string levelName)
    {
        loaderBar.gameObject.SetActive(true);
        AsyncOperation op = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);

        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / .9f);
            loaderBar.value = progress;
            loaderBar.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = $" {(int)(progress * 100f)}%";

            yield return null;
        }
    }

    public void QuitGame()
    {

        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                 Application.Quit();
        #endif
    }
}

[Serializable]
public struct MainMenuOptions
{
    public string optionName;
    public GameObject destinyPanel;
}
