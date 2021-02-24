using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    private int slotID;
    [SerializeField] private MyParameters.SlotType slotType;

    [SerializeField] Item item;
    [SerializeField] Text itemName;
    [SerializeField] Image itemIcon;
    [SerializeField] Text itemQnt;

    // Drag And Drop
    private event Action<InventorySlot> OnBeginDragEvent;
    private event Action<InventorySlot> OnEndDragEvent;
    private event Action<InventorySlot> OnDragEvent;
    private event Action<InventorySlot> OnDropEvent;

    private bool isDragging;
    private Color dragColor = new Color(1, 1, 1, 0.5f);
    private GameObject dragItemImg;
    private List<GameObject> hoveredList;

    // Start is called before the first frame update
    void Awake()
    {
        slotID = transform.GetSiblingIndex();
        OnDragEvent += Dragging;
        OnBeginDragEvent += StartDrag;
        OnEndDragEvent += FinishDrag;
    }

    public Item Item()
    {
        return item;
    }

    public void CleanSlot()
    {
        item = null;
        itemName.text = string.Empty;
        itemIcon.sprite = null;
        itemQnt.text = string.Empty;

        itemIcon.gameObject.SetActive(false);
        itemName.gameObject.SetActive(false);
        itemQnt.gameObject.SetActive(false);
    }

    public void HoldSlot(Item _item)
    {
        item = _item;
        itemName.text = _item.itemName;
        itemIcon.sprite = _item.itemIcon;
        itemQnt.text = _item.quantity.ToString();

        itemIcon.gameObject.SetActive(true);
        itemName.gameObject.SetActive(true);
        itemQnt.gameObject.SetActive(true);
    }

    public void OnDropButton()
    {       
        if (slotType == MyParameters.SlotType.PlayerInventory)
        {
            Global.UI.DropItem(Global.UI.selectedCharacterPosition, Global.UI.CharacterInventory.Items[slotID], false);
            Global.UI.CharacterInventory.RemoveItem(slotID);
        }

        if (slotType == MyParameters.SlotType.OtherInventory)
        {
            Global.UI.DropItem(Global.UI.ActiveStationInventory.transform.position, Global.UI.ActiveStationInventory.Items[slotID], true);
            Global.UI.ActiveStationInventory.RemoveItem(slotID);
        }

        if (slotType == MyParameters.SlotType.Hotbar)
        {
            Global.UI.DropItem(Global.UI.selectedCharacterPosition, Global.UI.CharacterHotbar.Items[slotID], false);
            Global.UI.CharacterHotbar.RemoveItem(slotID);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Global.UI.mouseOverSlot = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Global.UI.mouseOverSlot = null;
    }

    #region Drag And Drop // Item drag
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;

        if (item != null)
        {
            OnBeginDragEvent(this);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        if (item != null)
        {
            hoveredList = eventData.hovered;
            OnEndDragEvent(this);
            Destroy(dragItemImg.gameObject);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            OnDragEvent(this);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (OnDropEvent != null)
            OnDropEvent(this);
    }

    // Methods
    void StartDrag(InventorySlot obj)
    {
        dragItemImg = Global.UseDragEffect(itemIcon.sprite);
        dragItemImg.gameObject.SetActive(true);
    }

    void Dragging(InventorySlot obj)
    {
        dragItemImg.GetComponent<RectTransform>().position = Input.mousePosition;
        dragItemImg.gameObject.SetActive(true);
    }

    void FinishDrag(InventorySlot obj)
    {
        dragItemImg.gameObject.SetActive(false);

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerId = -1,
        };

        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult r in results)
        {
            TransferItem(false, r);
        }
    }
    #endregion

    public void TransferItem(bool isShotcut, RaycastResult r)
    {
        if (isShotcut)
        {
            if (item != null)
            {
                if (r.gameObject.name == "Station")
                {
                    Global.UI.ActiveStationInventory.AddItem(item);
                    Global.UI.CharacterInventory.RemoveItem(slotID);
                }

                if (r.gameObject.name == "Player")
                {
                    Global.UI.CharacterInventory.AddItem(item);
                    Global.UI.ActiveStationInventory.RemoveItem(slotID);
                }

                Global.UI.activeStation.UpdateStationPanel();
            }

            return;
        }

        int destinyIndex = 0;
        bool hasDestiny = false;

        try
        {
            destinyIndex = r.gameObject.transform.parent.GetSiblingIndex();
            hasDestiny = true;
        }
        catch
        {
            try
            {
                destinyIndex = Global.UI.ActiveStationInventory.Items.Count;
                hasDestiny = true;
            }
            catch
            {

            }
        }

        if (hasDestiny)
        {

            if (r.gameObject.transform.parent.parent == transform.parent)
            {
                if (r.gameObject.transform.name == "PlayerInventoryButton")
                {
                    Global.UI.CharacterInventory.ChangePosition(destinyIndex, slotID, item);
                }

                if (r.gameObject.transform.name == "OtherInventoryButton")
                {
                    Global.UI.ActiveStationInventory = r.gameObject.transform.parent.parent.parent.parent.parent.GetComponent<UI_OtherInventoryPanel>().myStation.GetStationInventory();
                    Global.UI.ActiveStationInventory.ChangePosition(destinyIndex, slotID, item);
                    r.gameObject.transform.parent.parent.parent.parent.parent.GetComponent<UI_OtherInventoryPanel>().myStation.UpdateStationPanel();
                }

                if (r.gameObject.transform.name == "HotbarButton")
                {
                    Global.UI.CharacterHotbar.AddItem(item, destinyIndex, slotID, true);
                    //Global.manager.PlayerHotbar.RemoveItem(slotID);
                }
            }
            else
            {
                if (r.gameObject.transform.name == "PlayerInventoryButton")
                {

                    Global.UI.CharacterInventory.AddItem(item);

                    if (slotType == MyParameters.SlotType.OtherInventory)
                    {
                        transform.parent.parent.parent.parent.GetComponent<UI_OtherInventoryPanel>().myStation.GetStationInventory().RemoveItem(slotID);
                        transform.parent.parent.parent.parent.GetComponent<UI_OtherInventoryPanel>().myStation.UpdateStationPanel();
                    }

                    if (slotType == MyParameters.SlotType.Hotbar)
                        Global.UI.CharacterHotbar.RemoveItem(slotID);

                }

                if (r.gameObject.transform.name == "OtherInventoryButton")
                {
                    Global.UI.ActiveStationInventory = r.gameObject.transform.parent.parent.parent.parent.parent.GetComponent<UI_OtherInventoryPanel>().myStation.GetStationInventory();
                    Global.UI.ActiveStationInventory.AddItem(item);
                    r.gameObject.transform.parent.parent.parent.parent.parent.GetComponent<UI_OtherInventoryPanel>().myStation.UpdateStationPanel();

                    if (slotType == MyParameters.SlotType.PlayerInventory)
                        Global.UI.CharacterInventory.RemoveItem(slotID);

                    if (slotType == MyParameters.SlotType.Hotbar)
                        Global.UI.CharacterHotbar.RemoveItem(slotID);

                    if (slotType == MyParameters.SlotType.OtherInventory)
                    {
                        transform.parent.parent.parent.parent.GetComponent<UI_OtherInventoryPanel>().myStation.GetStationInventory().RemoveItem(slotID);
                        transform.parent.parent.parent.parent.GetComponent<UI_OtherInventoryPanel>().myStation.UpdateStationPanel();
                    }
                }


                if (r.gameObject.transform.name == "HotbarButton")
                {
                    Global.UI.CharacterHotbar.AddItem(item, destinyIndex, slotID, false);

                    if (slotType == MyParameters.SlotType.PlayerInventory)
                        Global.UI.CharacterInventory.RemoveItem(slotID);

                    if (slotType == MyParameters.SlotType.OtherInventory)
                        Global.UI.ActiveStationInventory.RemoveItem(slotID);

                }
            }
        }
    }
}
