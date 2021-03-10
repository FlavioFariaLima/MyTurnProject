using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharactersBag : MonoBehaviour
{
    // Menu Manager
    private MainMenu MenuManager;

    [SerializeField] public int partySize;
    [SerializeField] private Player player;
    [SerializeField] private List<CharacterSheet> selectedCharacters = new List<CharacterSheet>();

    private void Awake()
    {
        MenuManager = GameObject.Find("Global").GetComponent<MainMenu>();
    }

    public void CheckForCharacter()
    {
        selectedCharacters.Clear();

        foreach (GoodButton btn in GetComponent<GoodButtonsGroup>().selectedButtons)
        {
            if (selectedCharacters.Count < partySize && !selectedCharacters.Contains(btn.GetComponent<PseudoCharacter>().MyCharacter()))
                selectedCharacters.Add(btn.GetComponent<PseudoCharacter>().MyCharacter());
        }

        if (selectedCharacters.Count == partySize)
        {
            MenuManager.GetSelectedPanel().destinyPanel.transform.Find("StartBtn").GetComponent<Button>().interactable = true;
        }
        else
        {
            MenuManager.GetSelectedPanel().destinyPanel.transform.Find("StartBtn").GetComponent<Button>().interactable = false;
        }
    }

    public void DeleteCharacter(Transform option)
    {
        if (option.GetComponent<GoodButton>().isSelected)
        {
            // Remove One
            GetComponent<GoodButtonsGroup>().selectedButtons.Remove(option.GetComponent<GoodButton>());
            option.GetComponent<GoodButton>().SetSelectedState(false);
        }

        option.GetComponent<PseudoCharacter>().MyCharacter().SetId(true);
        option.GetComponent<PseudoCharacter>().MyCharacter().SetName(string.Empty);
        selectedCharacters.Remove(option.GetComponent<PseudoCharacter>().MyCharacter());

        MenuManager.startManager.mainPlayer.charactersCount--;
        option.gameObject.SetActive(false);
        MenuManager.startManager.RecountCharacters();
    }
}
