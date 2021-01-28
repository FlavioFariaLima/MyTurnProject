using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropedItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private ItemBlueprint itemBlueprint;

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

    public void OnPointerExit(PointerEventData eventData)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }
}
