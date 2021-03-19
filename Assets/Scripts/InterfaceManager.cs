﻿using Microsoft.Unity.VisualStudio.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] public CameraControl mainCamera;
    [SerializeField] public Camera uiCamera;

    [Header("Characters")]
    public GameObject selectedCharacterObject;
    public CharacterSheet characterSheet;
    private Inventory characterInventory;
    public Inventory CharacterInventory
    {
        get
        {
            return characterInventory;
        }
        set
        {
            characterInventory = value;
        }
    }
    [SerializeField] private CharacterHotbar characterHotbar;
    public CharacterHotbar CharacterHotbar
    {
        get
        {
            return characterHotbar;
        }
        set
        {
            characterHotbar = value;
        }
    }
    public Vector3 selectedCharacterPosition;

    [Header("Characters Panel")]
    [SerializeField] private GameObject characterInfo;
    public GameObject panel_PlayerInventory;
    public GameObject slotsParent_characterInventory;
    public GameObject equipamentInfo;
    public GameObject slotsParent_characterEquipment;
    public GameObject characterCraftPanel;
    public GameObject craftSlotsParents;
    public GameObject craftSlotPrefab;
    public InventorySlot[] inventorySlots;
    public InventorySlot[] equipmentSlots;
    [SerializeField] private GameObject characterActions;
    [SerializeField] public RectTransform enemyInfo;
    [SerializeField] private GameObject portrait;
    [SerializeField] private GameObject barHealth;
    [SerializeField] private GameObject barMovement;

    [Header("Characters Hotbar")]
    public GameObject panel_playerHotbar;
    public GameObject slotsParent_playerHotbar;
    private InventorySlot[] _hotbarSlots;

    [Header("Stations & Other Inventories")]
    public GameObject panel_OtherInventory;
    [SerializeField] public CraftStation activeStation;
    public Inventory ActiveStationInventory
    {
        get
        {
            return _activeStationInventory;
        }
        set
        {
            _activeStationInventory = value;
        }
    }
    private Inventory _activeStationInventory;

    [SerializeField] public GameObject StationPanelPrefab;
    [SerializeField] public GameObject StationPanelPrefab_Parent;

    [Header("Informations Panels")]
    [SerializeField] private GameObject logPanel;
    [SerializeField] private GameObject LogInputPrefab;
    [SerializeField] private RectTransform itemInfoPanel;
    [SerializeField] private RectTransform recipeInfoPanel;
    [SerializeField] private GameObject dropedItemPrefab;
    [SerializeField] public TextMeshProUGUI distanceInfo;
    [SerializeField] public GameObject floatInfoPanel;
    public GameObject DropItemPrefab()
    {
        return dropedItemPrefab;
    }

    [Header("Interactive Menu")]
    [SerializeField] public GameObject InterectiveMenu;
    [SerializeField] public GameObject ExaminePanel;

    [HideInInspector] public InventorySlot mouseOverSlot = null;
    [HideInInspector] public GameObject mouseOverObject = null;
    [HideInInspector] public CraftSlot mouseOverCraftSlot = null;

    [Header("Match Manager")]
    [SerializeField] public GameObject TurnPanel;
    [SerializeField] public GameObject CharacterSheetIcons;
    [SerializeField] public GameObject CharacterIconsPrefab;
    [SerializeField] public GameObject EndMatch;
    [SerializeField] private GameObject passTurnBtn;
    [HideInInspector] public CharacterSheet characterSelectedToUI;

    [Header("Menu and Stuff")]
    [SerializeField] private GameObject matchMenu;
    [SerializeField] public GameObject playerCharactersShotcut;

    [Header("Colors")]
    [SerializeField] public Color defaultColor = new Color(1, 1, 1, 1);
    [SerializeField] public Color actColor = new Color(0, 0.8f, 0.2f, 1);
    [SerializeField] public Color dmgColor = new Color(0.8f, 0.2f, 0.2f, 1);
    [SerializeField] private Color color1 = new Color(0.5f, 1f, .4f, 1f);
    [SerializeField] private Color color2 = new Color(0.0f, .9f, .95f, 1f);

    [Header("Misc")]
    // Float Stats
    [SerializeField] public GameObject popupTextPrefab;
    [SerializeField] public LineRenderer pathLine;
    public float alpha = 1.0f;
    public Gradient gradient = new Gradient();
    [SerializeField] private GameObject destinationEffect;
    [SerializeField] private LineRenderer destinationPath;

    // Start is called before the first frame update
    void Start()
    {
        Global.Canvas.SetCursor(Global.Canvas.cursorDefault, false);

        // Disable and Hide Stuff
        matchMenu.SetActive(false);

        // Character Features
        characterSheet = selectedCharacterObject.GetComponent<CharacterSheet>();

        // Update Player Position
        selectedCharacterPosition = selectedCharacterObject.transform.position;

        // Hotbar
        characterHotbar = selectedCharacterObject.GetComponent<CharacterHotbar>();

        // Hide UI Stuff
        panel_PlayerInventory.SetActive(false);
        panel_OtherInventory.SetActive(false);
        characterCraftPanel.SetActive(false);
        InterectiveMenu.SetActive(false);
        ExaminePanel.SetActive(false);
        characterInfo.SetActive(false);

        characterInventory = selectedCharacterObject.GetComponent<Inventory>();
        equipmentSlots = slotsParent_characterEquipment.GetComponentsInChildren<InventorySlot>();
        inventorySlots = slotsParent_characterInventory.GetComponentsInChildren<InventorySlot>();
        _hotbarSlots = slotsParent_playerHotbar.GetComponentsInChildren<InventorySlot>();

        // Events
        if (CharacterInventory != null)
        {
            CharacterInventory.OnAdd += this.UpdateCharacterCraft;
            CharacterInventory.OnRemove += this.UpdateCharacterCraft;
        }

        //
        StartCoroutine(AddPlayerLogInput($"Iniciando a Partida!", string.Empty, string.Empty));
    }

    // Update is called once per frame
    void Update()
    {
        // Update Player Position
        selectedCharacterPosition = selectedCharacterObject.transform.position;

        // Manage Key Pressing
        KeysManager();

        // Ui
        ShowItemInfo();
        ShowRecipeInfo();
    }

    void KeysManager()
    {
        // Check what in under mouse cursor
        Ray mainRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Mouse Buttons
        if (true) // Only work if UI Panels are closed?
        {
            // Mouse Left Button
            if (Input.GetMouseButtonDown(0))
            {
                int layer_mask = LayerMask.GetMask("Character", "AmbientObject");
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, layer_mask))
                {
                    if (hit.transform.tag == "DropedItem")
                    {

                    }
                    else if (hit.transform.tag == "CraftStation")
                    {

                    }
                }

                // Reset and Close Stuff
                StartCoroutine(CloseMenu());
            }

            // Mouse Right Button
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.tag == "CraftStation")
                    {
                        InteractiveMenu.SetupMenu(MyParameters.ObjectCategory.Station, hit.transform.gameObject);

                        Global.UI.InterectiveMenu.transform.position = Input.mousePosition;
                        Global.UI.InterectiveMenu.SetActive(true);
                    }
                    else if (hit.transform.tag == "DropedItem")
                    {
                        InteractiveMenu.SetupMenu(MyParameters.ObjectCategory.Item, hit.transform.gameObject);

                        Global.UI.InterectiveMenu.transform.position = Input.mousePosition;
                        Global.UI.InterectiveMenu.SetActive(true);
                    }
                    else
                        Global.UI.InterectiveMenu.SetActive(false);
                }
            }
        }

        // Keys
        // Open Inventory and Player Stats
        if (Input.GetButtonDown("CharacterPanel"))
        {
            Global.ShowCharacterInventory();

            //
            characterInfo.SetActive(!characterInfo.activeSelf);
        }

        if (Input.GetButtonDown("CraftPanel"))
        {
            Global.ShowCharacterCraft();
        }

        // Inventory Keys
        if (Input.GetButtonDown("TransferItem"))
        {
            if (mouseOverSlot != null && activeStation != null)
            {
                RaycastResult destinySlot = new RaycastResult();
                destinySlot.gameObject = new GameObject();

                if (mouseOverSlot.transform.parent.gameObject == Global.UI.slotsParent_characterInventory)
                    destinySlot.gameObject.name = "Station";

                else if (mouseOverSlot.transform.parent.parent.name == "OtherInventory")
                    destinySlot.gameObject.name = "Player";

                mouseOverSlot.TransferItem(true, destinySlot);
            }
        }

        if (Input.GetButtonDown("DropItem"))
        {
            if (mouseOverSlot != null)
            {
                mouseOverSlot.OnDropButton();
            }
        }

        // Menu and Stuff
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Global.Canvas.ShowMatchMenu();
        }
    }

    IEnumerator CloseMenu()
    {
        yield return new WaitForSeconds(.2f);
        Global.UI.InterectiveMenu.SetActive(false);
        Debug.Log("CloseMenu()");
    }

    // UI
    public void UpdateCharacterInventory()
    {
        // Player Inventory
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (i < CharacterInventory.Items.Count)
            {
                inventorySlots[i].HoldSlot(CharacterInventory.Items[i]);
            }
            else
            {
                inventorySlots[i].CleanSlot();
            }
        }

        // Player Hotbar
        if (characterHotbar != null)
        {
            // Hotbar
            for (int s = 0; s < _hotbarSlots.Length; s++)
            {
                if (CharacterHotbar.Items.ContainsKey(s))
                {
                    var myItem = CharacterHotbar.Items[s];
                    _hotbarSlots[s].HoldSlot(myItem);
                }
                else
                {
                    _hotbarSlots[s].CleanSlot();
                }
            }
        }       
    }

    public void UpdateCharacterCraft()
    {
        if (craftSlotsParents.transform.childCount < characterSheet.GetKnowRecipes().Count)
        {
            int slotsMissing = characterSheet.GetKnowRecipes().Count - craftSlotsParents.transform.childCount;

            for (int s = 0; s < slotsMissing; s++)
            {
                GameObject.Instantiate(craftSlotPrefab, craftSlotsParents.transform);
            }
        }
        else if (craftSlotsParents.transform.childCount > characterSheet.GetKnowRecipes().Count)
        {
            int slotsMenos = craftSlotsParents.transform.childCount - characterSheet.GetKnowRecipes().Count;

            for (int s = 1; s < slotsMenos; s++)
            {
                GameObject.Destroy(craftSlotsParents.transform.GetChild(craftSlotsParents.transform.childCount - s));
            }
        }

        List<ItemBlueprint> characterIngredients = new List<ItemBlueprint>();

        foreach (Item it in CharacterInventory.Items)
        {
            characterIngredients.Add(it.itemBlueprint);
        }

        // Update List of Recipes
        for (int i = 0; i < craftSlotsParents.transform.childCount; i++)
        {
            if (i < characterSheet.GetKnowRecipes().Count)
            {
                craftSlotsParents.transform.GetChild(i).GetComponent<CraftSlot>().HoldSlot(characterSheet.GetKnowRecipes()[i]);

                craftSlotsParents.transform.GetChild(i).GetComponent<CraftSlot>().hasIngredients = true;

                foreach (RecipeIngredients ingredient in characterSheet.GetKnowRecipes()[i].RecipeIngredients)
                {
                    if (!characterIngredients.Contains(ingredient.item))
                        craftSlotsParents.transform.GetChild(i).GetComponent<CraftSlot>().hasIngredients = false;
                    else
                    {
                        List<ItemBlueprint> results = characterIngredients.FindAll(s => s.Equals(ingredient.item));

                        if (results.Count < ingredient.amount)
                            craftSlotsParents.transform.GetChild(i).GetComponent<CraftSlot>().hasIngredients = false;
                    }
                }
            }
            else
            {
                craftSlotsParents.transform.GetChild(i).GetComponent<CraftSlot>().CleanSlot();
            }
        }
    }

    public void UpdateCharacterEquipment()
    {
        for (int s = 0; s < equipmentSlots.Length; s++)
        {
            if (characterSelectedToUI.controller.Equipment().equipments.ContainsKey(s))
            {
                var myItem = characterSelectedToUI.controller.Equipment().equipments[s];
                equipmentSlots[s].HoldSlot(myItem);
            }
            else
            {
                equipmentSlots[s].CleanSlot();
            }
        }
    }

    public void DropItem(Vector3 position, Item item, bool reverseDir)
    {
        GameObject newItem = Instantiate(DropItemPrefab(), new Vector3(position.x, position.y + 1, position.z), Quaternion.identity);
        newItem.GetComponent<DropedItem>().SetItemBlueprint(item.itemBlueprint);

        if (!reverseDir)
            newItem.GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.forward * 4);
        else
            newItem.GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.back * 2);
    }

    /// <summary>
    /// Informations UI
    /// </summary>
    /// 

    public void SelectCharacterForUI(int id)
    {
        // Get Character Sheed
        characterSelectedToUI = Global.Match.InGameCharacters().Find(x => x.GetId() == id);

        // Sent to UI
        UpdateCharacterInUI(characterSelectedToUI);

        // Active if is Turn Owner
        if (characterSelectedToUI == Global.Match.InGameCharacters()[Global.Match.TurnOwnerId])
        {
            SetupActionsOwner(Global.Match.InGameCharacters()[Global.Match.TurnOwnerId].controller);
        }
        else // Disable All Butons
        {
            DisablActionsBtns();
        }

        foreach (Transform child in Global.UI.playerCharactersShotcut.transform)
        {
            if (child.name == Global.Match.InGameCharacters()[Global.Match.TurnOwnerId].GetId().ToString())
            {
                child.Find("Border").gameObject.SetActive(true);

            }
            else
                child.Find("Border").gameObject.SetActive(false);
        }

        foreach (Transform child in Global.UI.CharacterSheetIcons.transform)
        {
            if (child.name == Global.Match.InGameCharacters()[Global.Match.TurnOwnerId].GetId().ToString())
            {
                child.Find("Border").gameObject.SetActive(true);

                // Send Character Information to Panels
                Global.UI.characterSheet = characterSelectedToUI;
                Global.UI.CharacterInventory = characterSelectedToUI.controller.Inventory();
                Global.UI.CharacterHotbar = characterSelectedToUI.controller.Hotbar();

            }
            else
                child.Find("Border").gameObject.SetActive(false);
        }
    }

    // Inventory Item Portrait
    public void ShowItemInfo()
    {
        if (mouseOverSlot != null && mouseOverSlot.Item() != null) // Inventory Objects
        {
            itemInfoPanel.transform.SetAsLastSibling();
            itemInfoPanel.transform.position = Input.mousePosition;
            itemInfoPanel.gameObject.SetActive(true);

            foreach (Transform child in itemInfoPanel.transform)
            {
                if (child.name == "ItemName")
                    child.GetComponent<TextMeshProUGUI>().text = mouseOverSlot.Item().itemName;

                if (child.name == "ItemTipo")
                {
                    string typeStr = string.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA(color1), mouseOverSlot.Item().itemType.ToString());

                    if (mouseOverSlot.Item().itemProperties.Count > 0)
                        typeStr += string.Format(", <color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA(color2), mouseOverSlot.Item().itemProperties[0].ToString());

                    child.GetComponent<TextMeshProUGUI>().text = typeStr;
                }

                if (child.name == "ItemIcon")
                    child.GetComponent<UnityEngine.UI.Image>().sprite = mouseOverSlot.Item().itemIcon;                

                if (child.name == "Description")
                    child.GetComponent<TextMeshProUGUI>().text = mouseOverSlot.Item().description;                

                // Item Stats
                if (child.name == "ItemMainValue")
                {
                    // If Weapon
                    if (mouseOverSlot.Item().itemType == ItemType.weapon)
                    {
                        child.gameObject.SetActive(true);

                        child.Find("Value").GetComponent<TextMeshProUGUI>().text = $"1d{mouseOverSlot.Item().weaponStats.dmgM}";
                        child.Find("TypeInfo").GetComponent<TextMeshProUGUI>().text = $"{mouseOverSlot.Item().weaponStats.weaponType[0]}";
                        child.Find("Value2").GetComponent<TextMeshProUGUI>().text = mouseOverSlot.Item().weaponStats.critical.ToString();
                        child.Find("Value3").GetComponent<TextMeshProUGUI>().text = $"x{mouseOverSlot.Item().weaponStats.criticalMultiply}";
                    }
                    else
                        child.gameObject.SetActive(false);
                }

                if (child.name == "ItemCondition")
                    child.GetComponent<TextMeshProUGUI>().text = $"{mouseOverSlot.Item().condition.ToString()} / 100";                

                if (child.name == "ItemWeight")
                    child.GetComponent<TextMeshProUGUI>().text = $"{mouseOverSlot.Item().weight} kg";                
            }
        }
        else if (mouseOverObject != null) // Scene Objects
        {
            itemInfoPanel.transform.SetAsLastSibling();
            itemInfoPanel.transform.position = Input.mousePosition;
            itemInfoPanel.gameObject.SetActive(true);

            if (mouseOverObject.tag == "DropedItem")
            {

                foreach (Transform child in itemInfoPanel.transform)
                {
                    ItemBlueprint pointedItem = mouseOverObject.GetComponent<DropedItem>().GetItemBlueprint();

                    if (child.name == "ItemName")
                        child.GetComponent<TextMeshProUGUI>().text = pointedItem.itemName;

                    if (child.name == "ItemTipo")
                    {
                        string typeStr = string.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA(color1), pointedItem.itemType.ToString());

                        if (pointedItem.itemProperties.Count > 0)
                            typeStr += string.Format(", <color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA(color2), pointedItem.itemProperties[0].ToString());

                        child.GetComponent<TextMeshProUGUI>().text = typeStr;
                    }

                    if (child.name == "ItemIcon")
                        child.GetComponent<UnityEngine.UI.Image>().sprite = pointedItem.itemIcon;

                    if (child.name == "Description")
                        child.GetComponent<TextMeshProUGUI>().text = pointedItem.description;

                    // Item Stats
                    if (child.name == "ItemMainValue")
                    {
                        // If Weapon
                        if (pointedItem.itemType == ItemType.weapon)
                        {
                            child.Find("Value").GetComponent<TextMeshProUGUI>().text = $"1d{pointedItem.dmgM}";
                            child.Find("TypeInfo").GetComponent<TextMeshProUGUI>().text = $"{pointedItem.weaponType[0]}";
                            child.Find("Value2").GetComponent<TextMeshProUGUI>().text = pointedItem.critical.ToString();
                            child.Find("Value3").GetComponent<TextMeshProUGUI>().text = $"x{pointedItem.criticalMultiply}";

                            child.gameObject.SetActive(true);
                        }
                        else
                            child.gameObject.SetActive(false);
                    }

                    if (child.name == "ItemCondition")
                        child.GetComponent<TextMeshProUGUI>().text = $"{pointedItem.condition} / 100";

                    if (child.name == "ItemWeight")
                        child.GetComponent<TextMeshProUGUI>().text = $"{pointedItem.weight} kg";                    
                }
            }
        }
        else
            itemInfoPanel.gameObject.SetActive(false);
    }

    public void ShowRecipeInfo()
    {
        if (mouseOverCraftSlot)
        {
            recipeInfoPanel.transform.SetAsLastSibling();
            recipeInfoPanel.transform.position = Input.mousePosition;
            recipeInfoPanel.gameObject.SetActive(true);

            foreach (Transform child in recipeInfoPanel.transform)
            {
                if (child.name == "ItemName")
                {
                    child.GetComponent<TextMeshProUGUI>().text = mouseOverCraftSlot.recipe.ResultItem.itemName;
                }

                if (child.name == "ItemTipo")
                {
                    string typeStr = string.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA(color1), mouseOverCraftSlot.recipe.ResultItem.itemType.ToString());

                    if (mouseOverCraftSlot.recipe.ResultItem.itemProperties.Count > 0)
                        typeStr += string.Format(", <color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA(color2), mouseOverCraftSlot.recipe.ResultItem.itemProperties[0].ToString());

                    child.GetComponent<TextMeshProUGUI>().text = typeStr;
                }

                if (child.name == "ItemIcon")
                {
                    child.GetComponent<UnityEngine.UI.Image>().sprite = mouseOverCraftSlot.recipe.ResultItem.itemIcon;
                }

                if (child.name == "Description")
                {
                    child.GetComponent<TextMeshProUGUI>().text = mouseOverCraftSlot.recipe.ResultItem.description;
                }

                // Item Stats
                if (child.name == "ItemValue")
                {
                    child.GetComponent<TextMeshProUGUI>().text = Mathf.Round(mouseOverCraftSlot.recipe.ResultItem.onUseValue).ToString();
                }

                if (child.name == "ItemCondition")
                {
                    child.GetComponent<TextMeshProUGUI>().text = $"{mouseOverCraftSlot.recipe.ResultItem.condition.ToString()} / 100";
                }

                if (child.name == "ItemWeight")
                {
                    child.GetComponent<TextMeshProUGUI>().text = $"{mouseOverCraftSlot.recipe.ResultItem.weight} kg";
                }

                if (child.name == "Ingredients")
                {
                    for (int i = 0; i < mouseOverCraftSlot.recipe.RecipeIngredients.Count; i++)
                    {
                        child.transform.GetChild(i).GetComponent<UnityEngine.UI.Image>().sprite = mouseOverCraftSlot.recipe.RecipeIngredients[i].item.itemIcon;
                        child.transform.GetChild(i).gameObject.SetActive(true);
                    }

                    int slotsNeeded = child.transform.childCount - mouseOverCraftSlot.recipe.RecipeIngredients.Count;

                    for (int s = child.transform.childCount - 1; s >= mouseOverCraftSlot.recipe.RecipeIngredients.Count; s--)
                    {
                        child.transform.GetChild(s).gameObject.SetActive(false);
                    }
                }
            }
        }
        else
            recipeInfoPanel.gameObject.SetActive(false);
    }

    public IEnumerator AddPlayerLogInput(string text, string text2, string text3)
    {
        //        
        DateTime thisDate = DateTime.Now;
        string curTime = string.Format("{0:D2}:{1:D2}:{2:D2}", thisDate.Hour, thisDate.Minute, thisDate.Second);

        GameObject newLogEntry = Instantiate(LogInputPrefab, logPanel.transform);
        newLogEntry.GetComponentInChildren<TextMeshProUGUI>().text = $"[{curTime}] Turn #{Global.Match.CurrentTurn}:\n{text}\n{text2}\n{text3}";

        StartCoroutine(ForceScrollDown());

        yield return null;

        IEnumerator ForceScrollDown()
        {
            // Wait for end of frame AND force update all canvases before setting to bottom.
            yield return new WaitForEndOfFrame();
            Canvas.ForceUpdateCanvases();
            logPanel.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 0f;
            Canvas.ForceUpdateCanvases();
        }
    }

    public void UpdateCharacterInUI(CharacterSheet character)
    {
        portrait.transform.Find("PortraitSprite").GetComponent<UnityEngine.UI.Image>().sprite = character.Portrait();
        UpdateCharacterInfoPanel(character);

        // Add Character Inventory to UI Panel
        CharacterInventory = character.controller.Inventory();
        CharacterHotbar = character.controller.Hotbar();
        UpdateCharacterInventory();
        UpdateCharacterEquipment();

        UpdateHealthBar(character);
        UpdateMovementBar(character);
        Global.Match.UpdateCharactersIcons();
    }

    public void UpdateCurrentCharacterInfo()
    {
        if (characterSelectedToUI)
        {
            portrait.transform.Find("PortraitSprite").GetComponent<UnityEngine.UI.Image>().sprite = characterSelectedToUI.Portrait();
            UpdateCharacterInfoPanel(characterSelectedToUI);

            // Add Character Inventory to UI Panel
            CharacterInventory = characterSelectedToUI.controller.Inventory();
            CharacterHotbar = characterSelectedToUI.controller.Hotbar();
            UpdateCharacterInventory();

            UpdateHealthBar(characterSelectedToUI);
            UpdateMovementBar(characterSelectedToUI);
            Global.Match.UpdateCharactersIcons();
        }
    }

    public void UpdateCharacterInfoPanel(CharacterSheet character)
    {
        foreach (Transform child in characterInfo.transform)
        {
            if (child.name == "name")
            {
                child.GetComponent<TextMeshProUGUI>().text = $"{character.GetName()}";
            }
            else if (child.name == "classe")
            {
                child.GetComponent<TextMeshProUGUI>().text = $"{character.GetClass()}";
            }
            else if (child.name == "race")
            {
                child.GetComponent<TextMeshProUGUI>().text = $"{character.GetRace()}";
            }
            else if (child.name == "health")
            {
                child.GetComponent<TextMeshProUGUI>().text = $"{character.GetHealth()}";
            }          
            else if (child.name == "movement")
            {
                child.GetComponent<TextMeshProUGUI>().text = $"{character.GetMovement()}m";
            }
            else if (child.name == "Abilities")
            {
                foreach (Transform c in child.transform)
                {
                    if (c.name == "Str")
                    {
                        c.GetComponent<TextMeshProUGUI>().text = $"{character.GetAbilities().strength[0]}";
                        c.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{character.GetAbilities().strength[1]}";
                    }
                    else if (c.name == "Con")
                    {
                        c.GetComponent<TextMeshProUGUI>().text = $"{character.GetAbilities().constitution[0]}";
                        c.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{character.GetAbilities().constitution[1]}";
                    }
                    else if (c.name == "Dex")
                    {
                        c.GetComponent<TextMeshProUGUI>().text = $"{character.GetAbilities().dexterity[0]}";
                        c.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{character.GetAbilities().dexterity[1]}";
                    }
                    else if (c.name == "Int")
                    {
                        c.GetComponent<TextMeshProUGUI>().text = $"{character.GetAbilities().intelligence[0]}";
                        c.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{character.GetAbilities().intelligence[1]}";
                    }
                    else if (c.name == "Wis")
                    {
                        c.GetComponent<TextMeshProUGUI>().text = $"{character.GetAbilities().wisdow[0]}";
                        c.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{character.GetAbilities().wisdow[1]}";
                    }
                    else if (c.name == "Cha")
                    {
                        c.GetComponent<TextMeshProUGUI>().text = $"{character.GetAbilities().charisma[0]}";
                        c.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{character.GetAbilities().charisma[1]}";
                    }
                }
            }
        }

        foreach (Transform child in equipamentInfo.transform)
        {
            if (child.name == "armour")
            {
                child.GetComponent<TextMeshProUGUI>().text = $"{character.GetArmour()}";
            }
            else if(child.name == "meleeAttack")
            {
                child.GetComponent<TextMeshProUGUI>().text = $"+{character.GetMeleeAttack()}";
            }
            else if (child.name == "rangeAttack")
            {
                child.GetComponent<TextMeshProUGUI>().text = $"+{character.GetRangeAttack()}";
            }
            else if (child.name == "meleeDamage")
            {
                child.GetComponent<TextMeshProUGUI>().text = $"1d{character.GetMeleeDamage()} + {character.GetAbilities().strength[1]}";
            }
            else if (child.name == "rangeDamage")
            {
                child.GetComponent<TextMeshProUGUI>().text = $"1d{character.GetRangeDamage()}";
            }
        }

        //characterInfo.SetActive(true);
    }

    // Stats Bars
    public void UpdateHealthBar(CharacterSheet character)
    {
        barHealth.transform.GetComponentInChildren<TextMeshProUGUI>().text = $"{(int)character.GetCurrrentHelth()} / {(int)character.GetHealth()}";
        barHealth.transform.GetComponentInChildren<Slider>().maxValue = character.GetHealth();
        barHealth.transform.GetComponentInChildren<Slider>().value = (int)character.GetCurrrentHelth();

        Global.Match.UpdateIconHealthBar(character);
    }

    public void UpdateMovementBar(CharacterSheet character)
    {
        barMovement.transform.GetComponentInChildren<TextMeshProUGUI>().text = $"{(int)character.GetMovement() - (int)character.controller.HasMoved} / {(int)character.GetMovement()}";
        barMovement.transform.GetComponentInChildren<Slider>().maxValue = character.GetMovement();
        barMovement.transform.GetComponentInChildren<Slider>().value = (int)character.GetMovement() - (int)character.controller.HasMoved;
    }

    /// <summary>
    ///  UI - Character Panels
    /// </summary>
    /// 
    public void ShowHideCharacterInfo()
    {
        characterInfo.SetActive(!characterInfo.activeSelf);
    }

    public void ShowHideInventory()
    {
        Global.ShowCharacterInventory();
    }

    public void ShowHideCraft()
    {
        Global.ShowCharacterCraft();
    }

    public void ShowHideExaminePanel()
    {
        Global.ShowExaminePanel();
    }

    /// <summary>
    ///  UI - Turn and Time
    /// </summary>
    /// <param name="counterValue"></param>
    public void UpdateTurnClock(float counterValue)
    {
        TurnPanel.transform.Find("Timer").GetComponent<TextMeshProUGUI>().text = counterValue.ToString();
    }

    /// <summary>
    ///  UI - Characters Actions Buttons
    /// </summary>
    /// <param name="id"></param>
    public void EnableActionButton(string name, bool enable, PlayerCharacterController character)
    {
        characterActions.transform.Find(name).GetComponent<Button>().interactable = false;
        characterActions.transform.Find(name).GetComponent<Button>().onClick.RemoveAllListeners();

        if (enable)
        {
            characterActions.transform.Find(name).GetComponent<Button>().interactable = enable;

            if (name == "Range")
            {
                characterActions.transform.Find(name).GetComponent<Button>().onClick.AddListener(delegate { character.AttackRange(); });
                characterActions.transform.Find(name).GetComponent<Button>().interactable = true;
            }
        }
    }

    public void DisablActionsBtns()
    {
        foreach (Transform child in characterActions.transform)
        {
            child.GetComponent<Button>().onClick.RemoveAllListeners();
            child.GetComponent<Button>().interactable = false;
        }

        // Pass Btn
        passTurnBtn.GetComponent<Button>().onClick.RemoveAllListeners();
        passTurnBtn.GetComponent<Button>().interactable = false;
    }

    public void DisableActionBtn(string name)
    {
        characterActions.transform.Find(name).GetComponent<Button>().onClick.RemoveAllListeners();
        characterActions.transform.Find(name).GetComponent<Button>().interactable = false;
    }

    public void SetupActionsOwner(PlayerCharacterController character)
    {
        if (!character.IsAi())
        {
            // Clen Buttons
            foreach (Transform child in characterActions.transform)
            {
                child.GetComponent<Button>().onClick.RemoveAllListeners();
                child.GetComponent<Button>().interactable = false;
            }

            passTurnBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            passTurnBtn.GetComponent<Button>().interactable = true;

            if (character.CheckAttacks())
            {
                characterActions.transform.Find("Melee").GetComponent<Button>().onClick.AddListener(delegate { character.AttackMelee(); });
                characterActions.transform.Find("Melee").GetComponent<Button>().interactable = true;

                bool hasRange = character.Equipment().CheckIfHasWeapon()[1];

                if (hasRange)
                {
                    characterActions.transform.Find("Range").GetComponent<Button>().onClick.AddListener(delegate { character.AttackRange(); });
                    characterActions.transform.Find("Range").GetComponent<Button>().interactable = true;
                }
            }

            // Set Pass Turn Button
            passTurnBtn.GetComponent<Button>().onClick.AddListener(delegate { Global.Match.PassTurn(); });
        }
        else
        {
            foreach (Transform child in characterActions.transform)
            {
                child.GetComponent<Button>().onClick.RemoveAllListeners();
                child.GetComponent<Button>().interactable = false;

                // Pass Btn
                passTurnBtn.GetComponent<Button>().onClick.RemoveAllListeners();
                passTurnBtn.GetComponent<Button>().interactable = false;
            }
        }
    }

    /// <summary>
    /// Menus & Move Panels
    /// </summary>
    /// <returns></returns>
    // Menu
    public IEnumerator ShowDestinationPoint()
    {
        destinationPath.startWidth = 0.02f;
        destinationPath.endWidth = 0.02f;
        destinationPath.colorGradient = Global.UI.gradient;

        while (Global.Commands.GetMainSelectedCharacterTransform().GetComponent<PlayerCharacterController>().CharacterAgent.hasPath)
        {
            destinationEffect.SetActive(true);
            destinationEffect.transform.position = Global.Commands.GetMainSelectedCharacterTransform().GetComponent<PlayerCharacterController>().CharacterAgent.destination;

            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(Global.Commands.GetMainSelectedCharacterTransform().position, // Saves the path in the path variable.
                Global.Commands.GetMainSelectedCharacterTransform().GetComponent<PlayerCharacterController>().CharacterAgent.destination, NavMesh.AllAreas, path); 
            Vector3[] corners = path.corners;
            destinationPath.SetPositions(corners);

            destinationPath.gameObject.SetActive(true);
            yield return null;
        }

        destinationEffect.SetActive(false);
        destinationPath.gameObject.SetActive(false);
    }

    public GameObject GetMatchMenu()
    {
        return matchMenu;
    }
}
