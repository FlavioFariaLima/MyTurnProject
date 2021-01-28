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
    public static MatchManager Manager;

    private static GameObject dragItemImg = new GameObject();

    // Start and set prioritary parameters.
    public static void Setup()
    {
        UI = GameObject.Find("GameManager").GetComponent<InterfaceManager>();
        Commands = GameObject.Find("GameManager").GetComponent<PlayerCommands>();
        Manager = GameObject.Find("GameManager").GetComponent<MatchManager>();
        Terrain = GameObject.Find("Terrain").GetComponent<TerrainManager>();
    }

    // Open/Close Panels
    public static void ShowCharacterInventory()
    {
        UI.UpdateCharacterInventory();

        UI.panel_PlayerInventory.SetActive(!UI.panel_PlayerInventory.activeSelf);

        if (UI.panel_PlayerInventory.activeSelf)
        {
            if (UI.slotsParent_PlayerInventory.transform.childCount < UI.CharacterInventory.InventorySize())
            {
                for (int i = 0; i <= UI.CharacterInventory.InventorySize() - UI.slotsParent_PlayerInventory.transform.childCount; i++)
                {
                    UI.slotsParent_PlayerInventory.transform.GetChild(UI.slotsParent_PlayerInventory.transform.childCount - i).gameObject.gameObject.SetActive(true);
                }
            }
            else
            {
                for (int i = 1; i <= UI.slotsParent_PlayerInventory.transform.childCount - UI.CharacterInventory.InventorySize(); i++)
                {
                    UI.slotsParent_PlayerInventory.transform.GetChild(UI.slotsParent_PlayerInventory.transform.childCount - i).gameObject.gameObject.SetActive(false);
                }
            }

            UI._playerSlots = UI.slotsParent_PlayerInventory.GetComponentsInChildren<InventorySlot>();

            for (int i = 0; i < UI.slotsParent_PlayerInventory.transform.childCount; i++)
            {
                if (i < 10)
                    UI.slotsParent_PlayerInventory.transform.GetChild(i).gameObject.SetActive(true);
                else
                {
                    if (i < UI.CharacterInventory.Items.Count + 1)
                        UI.slotsParent_PlayerInventory.transform.GetChild(i).gameObject.SetActive(true);
                    else
                        UI.slotsParent_PlayerInventory.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }

    public static void ShowCharacterCraft()
    {
        UI.characterCraftPanel.SetActive(!UI.characterCraftPanel.activeSelf);

        if (UI.characterCraftPanel.activeSelf)
        {
            UI.UpdateCharacterCraft();

            GameObject.Find("Scrollbar Vertical Station").GetComponent<Scrollbar>().value = 1;
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
        Hotbar = 2
    }

    public enum StationType
    {
        Chest = 0,
        Campfire = 1
    }

    public enum ItemType
    {
        vegetal = 0,
        mineral = 1,
        food = 2,
        tool = 3
    }

    public enum ItemProperties
    {
        fuel = 0,
        sharp = 1,
        raw = 2,
        cooked = 3
    }
}
