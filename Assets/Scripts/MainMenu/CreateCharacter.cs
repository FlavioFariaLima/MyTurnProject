using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CreateCharacter : MonoBehaviour
{
    // Menu Manager
    private MainMenu MenuManager;

    [Header("Buttons & Panels")]
    [SerializeField] private Button createNewBtn;
    [SerializeField] private GameObject createCharacterPanel;

    [Header("Tab Panels")]
    [SerializeField] private GameObject[] tabsPanels;

    [Header("Bio InputFields")]
    [SerializeField] private TMP_InputField inputName;
    [SerializeField] private TMP_Dropdown selectGender;
    [SerializeField] private TMP_Dropdown selectRegion;
    [SerializeField] private TMP_Dropdown selectOrigin;

    [Header("Races")]
    [SerializeField] private List<Race> races;
    [SerializeField] private GameObject racesParent;
    [SerializeField] private TextMeshProUGUI racesDescription;

    private Player mainPlayer;
    private StartCharacter creatingCharacter;

    // Start is called before the first frame update
    void Awake()
    {
        MenuManager = GetComponent<MainMenu>();
        createCharacterPanel.SetActive(false);

        // Races
        SetRacesButtons();
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

    // Deal With UI and Stuff
    private void CleanAll()
    {
        inputName.text = string.Empty;
    }

    public void StartNewCharacter()
    {
        if (creatingCharacter == null)
            creatingCharacter = new StartCharacter();
    }

    public void SetRacesButtons()
    {
        for (int r = 0; r < races.Count; r++ )
        {
            racesParent.transform.GetChild(r).GetComponentInChildren<TextMeshProUGUI>().text = races[r].raceName;
            racesParent.transform.GetChild(r).gameObject.SetActive(true);
        }

        racesParent.transform.GetChild(0).GetComponent<GoodButton>().SetSelectedState(true);
    }

    public void SelectRace(GoodButton btn)
    {
        creatingCharacter.race = races[btn.tabIndex];
        racesDescription.text = creatingCharacter.race.description;
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


[Serializable]
public class StartCharacter
{
    public string characterName;
    public Sprite portrait;

    // Stats
    [Header("Character Experience")]
    public int level;
    public int xp;

    public Race race;
    public Classe classe;

    [Header("strength - constitution - dexterity - intelligence - wisdow - charisma")]
    public int[] abilitiesValues;
    private Abilities abilities;

    // Combat
    public int health;
    public int armorClass;

    // BaseAttack
    private int baseAttack;
    public int BaseAttack
    {
        get { return baseAttack; }
        set { baseAttack = level / value; }
    }

    public int meleeAttack;
    public int meleeDamage;
    public float meleeDistance;

    public int rangeAttack;
    public int rangeDamage;
    public float rangeDistance;

    // Resistences
    public Resistences resistences;

    // Movement
    public int movement;
}
