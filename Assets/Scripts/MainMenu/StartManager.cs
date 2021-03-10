using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    // Menu Manager
    private MainMenu MenuManager;

    [SerializeField] private List<Player> players;
    [SerializeField] private CharactersBag charactersBag;
    [SerializeField] private int maxCharacters;

    [HideInInspector] public Player mainPlayer;

    // Start is called before the first frame update
    private void Awake()
    {
        MenuManager = GetComponent<MainMenu>();
    }

    public void RecountCharacters()
    {
        foreach (Player p in players)
        {
            if (p.IsMainPlayer())
            {
                mainPlayer = p;

                // Hide Character Buttons
                foreach (Transform child in charactersBag.transform)
                {
                    child.gameObject.SetActive(false);
                }

                // Show Character Buttons if have Characters
                for (int c = 0; c < maxCharacters; c++)
                {
                    if (p.PlayerCharacters[c].GetId() != 0)
                    {
                        charactersBag.transform.GetChild(c).gameObject.SetActive(true);
                        charactersBag.transform.GetChild(c).GetComponent<PseudoCharacter>().MyCharacter(p.PlayerCharacters[c]); // Set Character
                    }
                }
            }
        }

        if (mainPlayer.CharactersCount < maxCharacters)
        {
            MenuManager.GetSelectedPanel().destinyPanel.transform.Find("PlayerCharacters").Find("NewCharacterBtn").GetComponent<Button>().interactable = true;
        }
        else
            MenuManager.GetSelectedPanel().destinyPanel.transform.Find("PlayerCharacters").Find("NewCharacterBtn").GetComponent<Button>().interactable = false;
    }

    public void CreateNewCharacter(string newName)
    {
        for (int c = 0; c < mainPlayer.PlayerCharacters.Count; c++)
        {
            if (mainPlayer.PlayerCharacters[c].GetId() == 0)
            {
                mainPlayer.PlayerCharacters[c].SetId(false);
                mainPlayer.PlayerCharacters[c].SetName(newName);

                MenuManager.startManager.mainPlayer.charactersCount++;
                RecountCharacters();

                return;
            }
        }

        RecountCharacters();
    }
}
