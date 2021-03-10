using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GoodButtonsGroup : MonoBehaviour
{
    [SerializeField] public int checkNumber;
    private List<GoodButton> allButtons = new List<GoodButton>();
    [SerializeField] public List<GoodButton> selectedButtons = new List<GoodButton>();

    // Start is called before the first frame update
    void Awake()
    {
        allButtons.AddRange(GetComponentsInChildren<GoodButton>());
    }

    public void SelectButton(Transform option)
    {
        if (option.GetComponent<GoodButton>().isSelected)
        {
            // Remove One
            selectedButtons.Remove(option.GetComponent<GoodButton>());
            option.GetComponent<GoodButton>().SetSelectedState(false);

            GetComponentInParent<CharactersBag>().CheckForCharacter();

            return;
        }

        if (option.GetComponent<GoodButton>().isSelected)
        {
            option.GetComponent<GoodButton>().SetSelectedState(false);
        }

        if (selectedButtons.Count < checkNumber)
        {
            option.GetComponent<GoodButton>().SetSelectedState(true);
            selectedButtons.Add(option.GetComponent<GoodButton>());
        }
        else
        {
            // Remove One
            selectedButtons[0].SetSelectedState(false);
            selectedButtons.RemoveAt(0);

            // Add New
            option.GetComponent<GoodButton>().SetSelectedState(true);
            selectedButtons.Add(option.GetComponent<GoodButton>());
        }

        GetComponentInParent<CharactersBag>().CheckForCharacter();
    }
}
