using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private int slotID;
    [SerializeField] public ItemRecipe recipe;
    [SerializeField] public Text itemName;
    [SerializeField] public Image itemIcon;

    public bool hasIngredients;

    private bool isDragging;
    private Color dragColor = new Color(1, 1, 1, 0.5f);

    // Start is called before the first frame update
    void Awake()
    {
        slotID = transform.GetSiblingIndex();
        hasIngredients = true;
    }

    void Update()
    {
        if (hasIngredients == true)
        {
            transform.GetChild(0).GetComponent<Image>().color = new Color(.5f, .5f, 0, .5f);
        }
        else
        {
            transform.GetChild(0).GetComponent<Image>().color = new Color(0, 0, 0, .5f);
        }
    }

    public ItemRecipe Item()
    {
        return recipe;
    }

    public void CleanSlot()
    {
        recipe = null;
        itemName.text = string.Empty;
        itemIcon.sprite = null;

        itemIcon.gameObject.SetActive(false);
        itemName.gameObject.SetActive(false);
    }

    public void HoldSlot(ItemRecipe _recipe)
    {
        recipe = _recipe;
        itemName.text = _recipe.ResultItem.itemName;
        itemIcon.sprite = _recipe.ResultItem.itemIcon;

        itemIcon.gameObject.SetActive(true);
        itemName.gameObject.SetActive(true);
    }

    public void Craft()
    {
        if (hasIngredients)
            Debug.Log("Craft Item");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Global.UI.mouseOverCraftSlot = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Global.UI.mouseOverCraftSlot = null;
        //Debug.Log("Mouse exit GameObject.");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Craft();
    }
}
