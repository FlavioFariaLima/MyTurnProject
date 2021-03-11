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

        SelectButton(allButtons[0].transform);

        for (int b = 1; b < allButtons.Count; b++)
        {
            allButtons[b].SetSelectedState(false);
        }
    }

    public void SelectButton(Transform option)
    {
        if (option.GetComponent<GoodButton>().isSelected)
        {
            // Remove One
            selectedButtons.Remove(option.GetComponent<GoodButton>());
            option.GetComponent<GoodButton>().SetSelectedState(false);

            if (!option.GetComponent<GoodButton>().isTab)
                GetComponentInParent<CharactersBag>().CheckForCharacter();

            return;
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
            selectedButtons.RemoveAt(selectedButtons.Count - 1);

            // Add New
            option.GetComponent<GoodButton>().SetSelectedState(true);
            selectedButtons.Add(option.GetComponent<GoodButton>());
        }

        if (!option.GetComponent<GoodButton>().isTab)
            GetComponentInParent<CharactersBag>().CheckForCharacter();
    }
}
