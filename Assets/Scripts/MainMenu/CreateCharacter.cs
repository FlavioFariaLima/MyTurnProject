using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateCharacter : MonoBehaviour
{
    // Menu Manager
    private MainMenu MenuManager;

    [SerializeField] private Button createNewBtn;
    [SerializeField] private GameObject createCharacterPanel;
    [SerializeField] private TMP_InputField inputName;

    // Start is called before the first frame update
    void Awake()
    {
        MenuManager = GetComponent<MainMenu>();
        createCharacterPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (inputName.text != string.Empty)
            createNewBtn.interactable = true;
        else
            createNewBtn.interactable = false;
    }

    public void CreateNewCharacter()
    {
        if ( inputName.text != string.Empty)
        {
            MenuManager.startManager.CreateNewCharacter(inputName.text);
        }

        CloseCreateCharPanel();
    }

    private void CleanAll()
    {
        inputName.text = string.Empty;
    }

    public void OpenCreateCharPanel()
    {
        createCharacterPanel.SetActive(true);
    }

    public void CloseCreateCharPanel()
    {
        CleanAll();
        createCharacterPanel.SetActive(false);
    }
}
