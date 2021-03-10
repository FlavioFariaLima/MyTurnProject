using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropedItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private ItemBlueprint itemBlueprint;
    private bool mouseIsOver = false;

    public void SetItemBlueprint(ItemBlueprint blueprint)
    {
        itemBlueprint = blueprint;
    }

    public ItemBlueprint GetItemBlueprint()
    {
        return itemBlueprint;
    }

    public void TakeItem(Inventory inventory)
    {
        Item newItem = Item.CreateNewItem(itemBlueprint);
        newItem.itemBlueprint = itemBlueprint;

        bool addItem = inventory.AddItem(newItem);

        if (addItem)
        {
            Global.UI.UpdateCharacterInventory();
            Destroy(gameObject);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (!Global.UI.InterectiveMenu.activeSelf)
            {
                if (Global.Commands.GetSelectedCharacters().Count > 0)
                {
                    Vector3 distance = Global.Commands.GetMainSelectedCharacterTransform().position - transform.position;
                    Debug.Log(distance.sqrMagnitude);

                    if (distance.sqrMagnitude < 1)
                    {
                        if (!Global.UI.InterectiveMenu.activeSelf)
                            TakeItem(Global.UI.CharacterInventory);
                    }
                    else
                    {
                        StartCoroutine(Global.Commands.GetMainSelectedCharacterTransform().GetComponent<PlayerCharacterController>().MoveToObject(transform, MyParameters.ObjectCategory.Item));
                    }
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Global.Canvas.SetCursor(Global.Canvas.cursorInteract, true);
        Global.UI.mouseOverObject = gameObject;
        mouseIsOver = true;
        StartCoroutine(InfoUI());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Global.Canvas.SetCursor(Global.Canvas.LastCursor, false);
        Global.UI.mouseOverObject = null;
        mouseIsOver = false;
    }

    private IEnumerator InfoUI()
    {
        while (mouseIsOver)
        {
            Global.UI.floatInfoPanel.SetActive(true);
            Global.UI.floatInfoPanel.transform.Find("infoText").GetComponent<TextMeshProUGUI>().text = itemBlueprint.itemName;

            yield return null;
        }

        Global.UI.floatInfoPanel.SetActive(false);
    }
}
