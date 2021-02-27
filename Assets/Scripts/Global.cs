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
    public static CanvasManager CanvasManager;

    private static GameObject dragItemImg = new GameObject();


    // Start and set prioritary parameters.
    public static void Setup()
    {
        UI = GameObject.Find("GameManager").GetComponent<InterfaceManager>();
        Commands = GameObject.Find("GameManager").GetComponent<PlayerCommands>();
        Match = GameObject.Find("GameManager").GetComponent<MatchManager>();
        Terrain = GameObject.Find("Terrain").GetComponent<TerrainManager>();
        CanvasManager = GameObject.Find("Canvas").GetComponent<CanvasManager>();
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
                if (i < UI.CharacterInventory.InventorySize())
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

    // Menu and Stuff
    public static void ShowMatchMenu()
    {
        UI.GetMatchMenu().transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
        UI.GetMatchMenu().SetActive(!UI.GetMatchMenu().activeSelf);

        if (UI.GetMatchMenu().activeSelf)
            PauseGame();
        else
            ResumeGame();
    }

    // Misc
    public static void GetGridColumnAndRow(GridLayoutGroup glg, out int column, out int row)
    {
        column = 0;
        row = 0;

        if (glg.transform.childCount == 0)
            return;

        //Column and row are now 1
        column = 1;
        row = 1;

        //Get the first child GameObject of the GridLayoutGroup
        RectTransform firstChildObj = glg.transform.
            GetChild(0).GetComponent<RectTransform>();

        Vector2 firstChildPos = firstChildObj.anchoredPosition;
        bool stopCountingRow = false;

        //Loop through the rest of the child object
        for (int i = 1; i < glg.transform.childCount; i++)
        {
            //Get the next child
            RectTransform currentChildObj = glg.transform.
           GetChild(i).GetComponent<RectTransform>();

            Vector2 currentChildPos = currentChildObj.anchoredPosition;

            //if first child.x == otherchild.x, it is a column, ele it's a row
            if (firstChildPos.x == currentChildPos.x)
            {
                column++;
                //Stop couting row once we find column
                stopCountingRow = true;
            }
            else
            {
                if (!stopCountingRow)
                    row++;
            }
        }
    }
}

public enum MovePanelType
{
    leftRight = 0,
    rightLett = 1,
    topBotton = 2,
    bottonTop = 3
}

public enum ItemType
{
    vegetal = 0,
    mineral = 1,
    food = 2,
    tool = 3,
    weapon = 4
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
        Hotbar = 2
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
