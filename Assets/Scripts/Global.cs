using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public static class Global
{
    public static InterfaceManager UI;
    public static PlayerCommands Commands;
    public static TerrainManager Terrain;
    public static MatchManager Match;
    public static CanvasManager Canvas;

    private static GameObject dragItemImg = new GameObject();

    // Start and set prioritary parameters.
    public static void Setup()
    {
        UI = GameObject.Find("GameManager").GetComponent<InterfaceManager>();
        Canvas = GameObject.Find("Canvas").GetComponent<CanvasManager>();
        Match = GameObject.Find("GameManager").GetComponent<MatchManager>();
        Commands = GameObject.Find("GameManager").GetComponent<PlayerCommands>();
        Terrain = GameObject.Find("Terrain").GetComponent<TerrainManager>();
    }

    // Open/Close Panels
    public static void ShowCharacterInventory()
    {
        UI.UpdateCharacterInventory();

        UI.panel_PlayerInventory.SetActive(!UI.panel_PlayerInventory.activeSelf);

        if (UI.panel_PlayerInventory.activeSelf)
        {
            if (UI.slotsParent_characterInventory.transform.childCount < UI.CharacterInventory.InventorySize())
            {
                for (int i = 0; i <= UI.CharacterInventory.InventorySize() - UI.slotsParent_characterInventory.transform.childCount; i++)
                {
                    UI.slotsParent_characterInventory.transform.GetChild(UI.slotsParent_characterInventory.transform.childCount - i).gameObject.gameObject.SetActive(true);
                }
            }
            else
            {
                for (int i = 1; i <= UI.slotsParent_characterInventory.transform.childCount - UI.CharacterInventory.InventorySize(); i++)
                {
                    UI.slotsParent_characterInventory.transform.GetChild(UI.slotsParent_characterInventory.transform.childCount - i).gameObject.gameObject.SetActive(false);
                }
            }

            UI.inventorySlots = UI.slotsParent_characterInventory.GetComponentsInChildren<InventorySlot>();

            for (int i = 0; i < UI.slotsParent_characterInventory.transform.childCount; i++)
            {
                if (i < UI.CharacterInventory.InventorySize())
                    UI.slotsParent_characterInventory.transform.GetChild(i).gameObject.SetActive(true);
                else
                {
                    if (i < UI.CharacterInventory.Items.Count + 1)
                        UI.slotsParent_characterInventory.transform.GetChild(i).gameObject.SetActive(true);
                    else
                        UI.slotsParent_characterInventory.transform.GetChild(i).gameObject.SetActive(false);
                }
            }

            UI.slotsParent_characterInventory.transform.parent.parent.Find("Scrollbar Vertical").GetComponent<Scrollbar>().value = 1;
        }
    }

    public static void ShowCharacterCraft()
    {
        UI.characterCraftPanel.SetActive(!UI.characterCraftPanel.activeSelf);

        if (UI.characterCraftPanel.activeSelf)
        {
            UI.UpdateCharacterCraft();
        }
    }

    public static void ShowExaminePanel()
    {
        UI.ExaminePanel.SetActive(!UI.ExaminePanel.activeSelf);
    }

    // Drag and Drop Effect
    public static GameObject UseDragEffect(Sprite _icon)
    {
        dragItemImg = new GameObject();

        if (!dragItemImg.GetComponent<RectTransform>())
            dragItemImg.AddComponent<RectTransform>();

        if (!dragItemImg.GetComponent<Image>())
            dragItemImg.AddComponent<Image>();

        dragItemImg.transform.SetParent(GameObject.Find("Canvas").transform);

        dragItemImg.GetComponent<Image>().sprite = _icon;

        dragItemImg.SetActive(true);

        dragItemImg.SetActive(false);

        return dragItemImg;
    }

    public static void PauseGame()
    {
        Time.timeScale = 0;
    }

    public static void ResumeGame()
    {
        Time.timeScale = 1;
    }

    public static  void FaceTextMeshToCamera(Transform obj)
    {
        Vector3 origRot = obj.eulerAngles;

        obj.LookAt(obj.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);

        origRot.y = obj.eulerAngles.y;
        obj.eulerAngles = origRot;
    }

}

public enum MovePanelType
{
    leftRight = 0,
    rightLett = 1,
    topBotton = 2,
    bottonTop = 3
}

// Items
public enum ItemType
{
    vegetal = 0,
    mineral = 1,
    food = 2,
    tool = 3,
    weapon = 4,
    armor = 5
}

public enum ItemRarity
{
    trash = 0,
    commom = 1,
    exceptional = 2,
    uncommon = 3,
    rare = 4,
    unique = 5
}

public enum WeaponType
{
    Bludgeoning = 0,
    Piercing = 1,
    Slashing = 2
}

public static class MyParameters
{
    public enum TurnState
    {
        Waiting = 0,
        MyTurn = 1
    }

    public enum ObjectCategory
    {
        Item = 0,
        Station = 1,
        Creature = 2
    }

    public enum SlotType
    {
        PlayerInventory = 0,
        OtherInventory = 1,
        Hotbar = 2,
        Equipment = 3
    }

    public enum StationType
    {
        Chest = 0,
        Campfire = 1
    }

    public enum ItemProperties
    {
        fuel = 0,
        sharp = 1,
        raw = 2,
        cooked = 3
    }
}
