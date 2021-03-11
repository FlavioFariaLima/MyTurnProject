using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateCharacter : MonoBehaviour
{
    // Menu Manager
    private MainMenu MenuManager;

    [Header("Buttons & Panels")]
    [SerializeField] private Button createNewBtn;
    [SerializeField] private GameObject createCharacterPanel;

    [Header("Tab Panels")]
    [SerializeField] private GameObject[] tabsPanels;

    [Header("InputFields")]
    [SerializeField] private TMP_InputField inputName;
    [SerializeField] private TMP_Dropdown selectGender;
    [SerializeField] private TMP_Dropdown selectRegion;
    [SerializeField] private TMP_Dropdown selectOrigin;


    private Player mainPlayer;

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

    private bool CheckRequestFields()
    {
        if (inputName.text != string.Empty)
        {
            return true;
        }

        return false;
    }

    public void CreateNewCharacter(Player player)
    {
        mainPlayer = player;

        if (!CheckRequestFields())
            return;

        for (int c = 0; c < mainPlayer.PlayerCharacters.Count; c++)
        {
            if (mainPlayer.PlayerCharacters[c].GetId() == 0)
            {
                mainPlayer.PlayerCharacters[c].SetId(false);
                mainPlayer.PlayerCharacters[c].SetName(inputName.text);

                MenuManager.startManager.mainPlayer.charactersCount++;
                RecountCharacters();
                CloseCreateCharPanel();

                return;
            }
        }

        CloseCreateCharPanel();
    }

    public void RecountCharacters()
    {
        foreach (Player p in MenuManager.startManager.players)
        {
            if (p.IsMainPlayer())
            {
                mainPlayer = p;

                // Hide Character Buttons
                foreach (Transform child in MenuManager.startManager.charactersBag.transform)
                {
                    child.gameObject.SetActive(false);
                }

                // Show Character Buttons if have Characters
                for (int c = 0; c < MenuManager.startManager.maxCharacters; c++)
                {
                    if (p.PlayerCharacters[c].GetId() != 0)
                    {
                        MenuManager.startManager.charactersBag.transform.GetChild(c).gameObject.SetActive(true);
                        MenuManager.startManager.charactersBag.transform.GetChild(c).GetComponent<PseudoCharacter>().MyCharacter(p.PlayerCharacters[c]); // Set Character
                    }
                }
            }
        }

        if (mainPlayer.CharactersCount < MenuManager.startManager.maxCharacters)
        {
            MenuManager.GetSelectedPanel().destinyPanel.transform.Find("PlayerCharacters").Find("NewCharacterBtn").GetComponent<Button>().interactable = true;
        }
        else
            MenuManager.GetSelectedPanel().destinyPanel.transform.Find("PlayerCharacters").Find("NewCharacterBtn").GetComponent<Button>().interactable = false;
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

    public void SelectTabPanel(int index)
    {
        for (int t = 0; t < tabsPanels.Length; t++)
        {
            if (t == index)
                tabsPanels[t].SetActive(true);
            else
                tabsPanels[t].SetActive(false);

        }
    }
}
