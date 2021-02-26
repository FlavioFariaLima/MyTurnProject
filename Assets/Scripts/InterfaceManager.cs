using Microsoft.Unity.VisualStudio.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
    private Inventory _playerInventory;
    public Inventory CharacterInventory
    {
        get
        {
            return _playerInventory;
        }
        set
        {
            _playerInventory = value;
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
    public GameObject panel_PlayerInventory;
    public GameObject slotsParent_PlayerInventory;
    public GameObject characterCraftPanel;
    public GameObject craftSlotsParents;
    public GameObject craftSlotPrefab;
    public InventorySlot[] _playerSlots;
    [SerializeField] private GameObject characterActions;
    [SerializeField] public RectTransform enemyInfo;

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
    [SerializeField] private GameObject characterInfo;
    [SerializeField] public TextMeshProUGUI distanceInfo;
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

    private Color color1 = new Color(0.5f, 1f, .4f, 1f);
    private Color color2 = new Color(0.0f, .9f, .95f, 1f);

    [Header("Match Manager")]
    [SerializeField] public GameObject TurnPanel;
    [SerializeField] public GameObject CharacterSheetIcons;
    [SerializeField] public GameObject CharacterIconsPrefab;
    [SerializeField] public GameObject EndMatch;
    [SerializeField] private GameObject passTurnBtn;
    [SerializeField] private GameObject portrait;

    private CharacterSheet characterSelectedToUI;

    [Header("Menu and Stuff")]
    [SerializeField] private GameObject matchMenu;
    [SerializeField] public GameObject playerCharactersShotcut;

    [Header("Cursors")]
    [SerializeField] public Texture2D cursorDefault;
    [SerializeField] public Texture2D cursorInteract;
    [SerializeField] public Texture2D cursorMove;
    [SerializeField] public Texture2D cursorMelee;
    [SerializeField] public Texture2D cursorRange;

    [Header("Misc")]
    // Float Stats
    [SerializeField] public GameObject popupTextPrefab;

    private Texture2D lastCursor;
    public Texture2D LastCursor
    {
        get { return lastCursor; }
    }

    // Get/Set Variables
    public void SetCursor(Texture2D c, bool temp)
    {
        // Cursor Settings
        CursorMode cursorMode = CursorMode.ForceSoftware;
        Vector2 hotSpot = new Vector2(12, 12);

        if (!temp)
        {
            // Set Cursor
            Cursor.SetCursor(c, hotSpot, cursorMode);
            lastCursor = c;
        }
        else
        {
            Cursor.SetCursor(c, hotSpot, cursorMode);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetCursor(cursorDefault, false);

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

        _playerInventory = selectedCharacterObject.GetComponent<Inventory>();
        _playerSlots = slotsParent_PlayerInventory.GetComponentsInChildren<InventorySlot>();
        _hotbarSlots = slotsParent_playerHotbar.GetComponentsInChildren<InventorySlot>();

        // Inicia Globals
        Global.Setup();

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
        RaycastHit mainHit;

        // Mouse Buttons
        if (Physics.Raycast(mainRay, out mainHit, 50))
        {
            if (mainHit.transform.GetComponent<DropedItem>() != null)
            {
                mouseOverObject = mainHit.transform.gameObject;
            }
            else
            {
                mouseOverObject = null;
            }
        }

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
            if (mouseOverSlot != null)
            {
                RaycastResult destinySlot = new RaycastResult();
                destinySlot.gameObject = new GameObject();

                if (mouseOverSlot.transform.parent.gameObject == Global.UI.slotsParent_PlayerInventory)
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
            Global.ShowMatchMenu();
        }
    }

    IEnumerator CloseMenu()
    {
        yield return new WaitForSeconds(.2f);
        Global.UI.InterectiveMenu.SetActive(false);
    }

    // UI
    public void UpdateCharacterInventory()
    {
        // Player Inventory
        for (int i = 0; i < _playerSlots.Length; i++)
        {
            if (i < CharacterInventory.Items.Count)
            {
                _playerSlots[i].HoldSlot(CharacterInventory.Items[i]);
            }
            else
            {
                _playerSlots[i].CleanSlot();
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

        foreach (UltraMare.Item it in CharacterInventory.Items)
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

    public void DropItem(Vector3 position, UltraMare.Item item, bool reverseDir)
    {
        GameObject newItem = Instantiate(DropItemPrefab(), new Vector3(position.x, position.y + 1, position.z), Quaternion.identity);
        newItem.GetComponent<DropedItem>().SetItemBlueprint(item.itemBlueprint);

        if (!reverseDir)
            newItem.GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.forward * 4);
        else
            newItem.GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.back * 2);
    }

    // Informations UI
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
                {
                    child.GetComponent<TextMeshProUGUI>().text = mouseOverSlot.Item().itemName;
                }

                if (child.name == "ItemTipo")
                {
                    string typeStr = string.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA(color1), mouseOverSlot.Item().itemType.ToString());

                    if (mouseOverSlot.Item().itemProperties.Count > 0)
                        typeStr += string.Format(", <color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA(color2), mouseOverSlot.Item().itemProperties[0].ToString());

                    child.GetComponent<TextMeshProUGUI>().text = typeStr;
                }

                if (child.name == "ItemIcon")
                {
                    child.GetComponent<UnityEngine.UI.Image>().sprite = mouseOverSlot.Item().itemIcon;
                }

                if (child.name == "Description")
                {
                    child.GetComponent<TextMeshProUGUI>().text = mouseOverSlot.Item().description;
                }

                // Item Stats
                if (child.name == "ItemValue")
                {
                    child.GetComponent<TextMeshProUGUI>().text = Mathf.Round(mouseOverSlot.Item().onUseValue).ToString();
                }

                if (child.name == "ItemCondition")
                {
                    child.GetComponent<TextMeshProUGUI>().text = $"{mouseOverSlot.Item().condition.ToString()} / 100";
                }

                if (child.name == "ItemWeight")
                {
                    child.GetComponent<TextMeshProUGUI>().text = $"{mouseOverSlot.Item().weight} kg";
                }
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
                    {
                        child.GetComponent<TextMeshProUGUI>().text = pointedItem.itemName;
                    }

                    if (child.name == "ItemTipo")
                    {
                        string typeStr = string.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA(color1), pointedItem.itemType.ToString());

                        if (pointedItem.itemProperties.Count > 0)
                            typeStr += string.Format(", <color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA(color2), pointedItem.itemProperties[0].ToString());

                        child.GetComponent<TextMeshProUGUI>().text = typeStr;
                    }

                    if (child.name == "ItemIcon")
                    {
                        child.GetComponent<UnityEngine.UI.Image>().sprite = pointedItem.itemIcon;
                    }

                    if (child.name == "Description")
                    {
                        child.GetComponent<TextMeshProUGUI>().text = pointedItem.description;
                    }

                    // Item Stats
                    if (child.name == "ItemValue")
                    {
                        child.GetComponent<TextMeshProUGUI>().text = pointedItem.onUseValue.ToString();
                    }

                    if (child.name == "ItemCondition")
                    {
                        child.GetComponent<TextMeshProUGUI>().text = $"{pointedItem.condition.ToString()} / 100";
                    }

                    if (child.name == "ItemWeight")
                    {
                        child.GetComponent<TextMeshProUGUI>().text = $"{pointedItem.weight} kg";
                    }
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

    public void ShowCharacterInfo(CharacterSheet character)
    {
        //Debug.Log("Mostrar Character Information...");

        foreach (Transform child in characterInfo.transform)
        {
            if (child.name == "name")
            {
                child.GetComponent<TextMeshProUGUI>().text = $"{character.GetName()}";
            }
            else if (child.name == "health")
            {
                child.GetComponent<TextMeshProUGUI>().text = $"{character.GetHealth()}";
            }
            else if (child.name == "armour")
            {
                child.GetComponent<TextMeshProUGUI>().text = $"{character.GetArmour()}";
            }
            else if (child.name == "meleeAttack")
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

        //characterInfo.SetActive(true);
    }

    // UI - Buttons On Click
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

    // UI - Turn and Time
    public void UpdateTurnClock(float counterValue)
    {
        TurnPanel.transform.Find("Timer").GetComponent<TextMeshProUGUI>().text = counterValue.ToString();
    }

    // UI - Characters Actions
    public void SelectCharacterForUI(int id)
    {
        // Get Character Sheed
        characterSelectedToUI = Global.Match.InGameCharacters().Find(x => x.GetId() == id);

        // Sent to UI
        UpdatePortrait(characterSelectedToUI);

        // Active if is Turn Owner
        if (characterSelectedToUI == Global.Match.InGameCharacters()[Global.Match.TurnOwnerId])
        {
            SetupActionsOwner(Global.Match.InGameCharacters()[Global.Match.TurnOwnerId].controller);
        }
        else // Disable All Butons
        {
            DisablActionsBtns();
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

    public void DisableActionBtn(int index)
    {
        characterActions.transform.GetChild(index).GetComponent<Button>().onClick.RemoveAllListeners();
        characterActions.transform.GetChild(index).GetComponent<Button>().interactable = false;
    }

    public void SetupActionsOwner(PlayerCharacterController character)
    {
        if (!character.IsAi())
        {
            // Clen Buttons
            foreach (Transform child in characterActions.transform)
            {
                child.GetComponent<Button>().onClick.RemoveAllListeners();
                child.GetComponent<Button>().interactable = true;
            }

            passTurnBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            passTurnBtn.GetComponent<Button>().interactable = true;


            // Set Action Buttons
            characterActions.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { character.AllowToMove(true); });
            characterActions.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { character.AttackMelee(); });
            characterActions.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { character.AttackRange(); });

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

    public void EnableActionButton(int index, bool enable)
    {
        characterActions.transform.GetChild(index).GetComponent<Button>().interactable = enable;
    }

    //
    public void UpdatePortrait(CharacterSheet character)
    {
        portrait.transform.Find("CharHp").GetComponent<TextMeshProUGUI>().text = $"{character.GetCurrrentHelth()} / {character.GetHealth()}";
        portrait.transform.Find("PortraitSprite").GetComponent<UnityEngine.UI.Image>().sprite = character.CharPortrait;
    }

    // Menu
    public GameObject GetMatchMenu()
    {
        return matchMenu;
    }

    public void MatchMenu()
    {
        Global.ShowMatchMenu();
    }

    // Move Panels
    public float ScaleFactor()
    {
        return Global.CanvasManager.gameObject.GetComponent<Canvas>().scaleFactor;
    }

    public IEnumerator LerpMoveObject(RectTransform rectTransform, CharacterSheet enemy, MovePanelType type, bool open)
    {
        float timeOfTravel = 0.4f; //time to object reach a target place 
        float currentTime = 0; // actual floting time 
        float normalizedValue;

        rectTransform.ForceUpdateRectTransforms();

        if (type == MovePanelType.leftRight)
        {
            if (open)
            {
                Vector3 startPosition = rectTransform.position;
                Vector3 endPosition = new Vector3(rectTransform.position.x - (rectTransform.sizeDelta.x * ScaleFactor()), rectTransform.position.y, rectTransform.position.z);

                while (currentTime <= timeOfTravel)
                {
                    currentTime += Time.deltaTime;
                    normalizedValue = currentTime / timeOfTravel; // we normalize our time 

                    rectTransform.position = Vector3.Lerp(startPosition, endPosition, normalizedValue);
                    yield return null;
                }
            }
            else
            {
                Vector3 startPosition = rectTransform.position;
                Vector3 endPosition = new Vector3(rectTransform.position.x + (rectTransform.sizeDelta.x * ScaleFactor()), rectTransform.position.y, rectTransform.position.z);

                while (currentTime <= timeOfTravel)
                {
                    currentTime += Time.deltaTime;
                    normalizedValue = currentTime / timeOfTravel; // we normalize our time 

                    rectTransform.position = Vector3.Lerp(startPosition, endPosition, normalizedValue);
                    yield return null;
                }

                rectTransform.transform.GetChild(rectTransform.transform.childCount - 1).gameObject.SetActive(true);
                Debug.Log("Lerp Close Panel");
            }
        }
        else if (type == MovePanelType.rightLett)
        {
            if (!open)
            {
                Vector3 startPosition = rectTransform.position;
                Vector3 endPosition = new Vector3(rectTransform.position.x - (rectTransform.sizeDelta.x * ScaleFactor()), rectTransform.position.y, rectTransform.position.z);

                while (currentTime <= timeOfTravel)
                {
                    currentTime += Time.deltaTime;
                    normalizedValue = currentTime / timeOfTravel; // we normalize our time 

                    rectTransform.position = Vector3.Lerp(startPosition, endPosition, normalizedValue);
                    yield return null;
                }

                rectTransform.transform.GetChild(rectTransform.transform.childCount - 1).gameObject.SetActive(true);
            }
            else
            {
                Vector3 startPosition = rectTransform.position;
                Vector3 endPosition = new Vector3(rectTransform.position.x + (rectTransform.sizeDelta.x * ScaleFactor()), rectTransform.position.y, rectTransform.position.z);

                while (currentTime <= timeOfTravel)
                {
                    currentTime += Time.deltaTime;
                    normalizedValue = currentTime / timeOfTravel; // we normalize our time 

                    rectTransform.position = Vector3.Lerp(startPosition, endPosition, normalizedValue);
                    yield return null;
                }

                Debug.Log("Lerp Close Panel");
            }
        }
        else if (type == MovePanelType.topBotton)
        {
            if (open)
            {
                Vector3 startPosition = rectTransform.position;
                Vector3 endPosition = new Vector3(rectTransform.position.x, rectTransform.position.y - (rectTransform.sizeDelta.y * ScaleFactor()), rectTransform.position.z);

                while (currentTime <= timeOfTravel)
                {
                    currentTime += Time.deltaTime;
                    normalizedValue = currentTime / timeOfTravel; // we normalize our time 

                    rectTransform.position = Vector3.Lerp(startPosition, endPosition, normalizedValue);
                    yield return null;
                }

            }
            else
            {
                Vector3 startPosition = rectTransform.position;
                Vector3 endPosition = new Vector3(rectTransform.position.x, rectTransform.position.y + (rectTransform.sizeDelta.y * ScaleFactor()), rectTransform.position.z);

                while (currentTime <= timeOfTravel)
                {
                    currentTime += Time.deltaTime;
                    normalizedValue = currentTime / timeOfTravel; // we normalize our time 

                    rectTransform.position = Vector3.Lerp(startPosition, endPosition, normalizedValue);
                    yield return null;
                }

                rectTransform.transform.GetChild(rectTransform.transform.childCount - 1).gameObject.SetActive(true);
            }
        }
        else if (type == MovePanelType.bottonTop)
        {
            if (!open)
            {
                Vector3 startPosition = rectTransform.position;
                Vector3 endPosition = new Vector3(rectTransform.position.x, rectTransform.position.y - (rectTransform.sizeDelta.y * ScaleFactor()), rectTransform.position.z);

                while (currentTime <= timeOfTravel)
                {
                    currentTime += Time.deltaTime;
                    normalizedValue = currentTime / timeOfTravel; // we normalize our time 

                    rectTransform.position = Vector3.Lerp(startPosition, endPosition, normalizedValue);
                    yield return null;
                }

                rectTransform.transform.GetChild(rectTransform.transform.childCount - 1).gameObject.SetActive(true);
            }
            else
            {
                Vector3 startPosition = rectTransform.position;
                Vector3 endPosition = new Vector3(rectTransform.position.x, rectTransform.position.y + (rectTransform.sizeDelta.y * ScaleFactor()), rectTransform.position.z);

                while (currentTime <= timeOfTravel)
                {
                    currentTime += Time.deltaTime;
                    normalizedValue = currentTime / timeOfTravel; // we normalize our time 

                    rectTransform.position = Vector3.Lerp(startPosition, endPosition, normalizedValue);
                    yield return null;
                }
            }
        }
    }    
    
    public void LerpCloseObject_LeftRight(RectTransform panel)
    {
        panel.GetComponent<UIPanel>().isActive = false;
        StartCoroutine(LerpMoveObject(panel, new CharacterSheet(), MovePanelType.leftRight, false));
    }

    public void LerpOpenObject_LeftRight(RectTransform panel)
    {
        panel.GetComponent<UIPanel>().isActive = true;
        panel.GetComponent<UIPanel>().AjustSizeAsGrid();

        panel.transform.GetChild(panel.transform.childCount - 1).gameObject.SetActive(false);
        StartCoroutine(LerpMoveObject(panel, new CharacterSheet(), MovePanelType.leftRight, true));
    }

    public void LerpCloseObject_RightLeft(RectTransform panel)
    {
        panel.GetComponent<UIPanel>().isActive = false;
        StartCoroutine(LerpMoveObject(panel, new CharacterSheet(), MovePanelType.rightLett, false));
    }

    public void LerpOpenObject_RightLeft(RectTransform panel)
    {
        panel.GetComponent<UIPanel>().isActive = true;
        panel.GetComponent<UIPanel>().AjustSizeAsGrid();

        panel.transform.GetChild(panel.transform.childCount - 1).gameObject.SetActive(false);
        StartCoroutine(LerpMoveObject(panel, new CharacterSheet(), MovePanelType.rightLett, true));
    }

    public void LerpCloseObject_TopBotton(RectTransform panel)
    {
        panel.GetComponent<UIPanel>().isActive = false;
        StartCoroutine(LerpMoveObject(panel, new CharacterSheet(), MovePanelType.topBotton, false));
    }

    public void LerpOpenObject_TopBotton(RectTransform panel)
    {
        panel.GetComponent<UIPanel>().isActive = true;
        panel.GetComponent<UIPanel>().AjustSizeAsGrid();

        panel.transform.GetChild(panel.transform.childCount - 1).gameObject.SetActive(false);
        StartCoroutine(LerpMoveObject(panel, new CharacterSheet(), MovePanelType.topBotton, true));
    }

    public void LerpCloseObject_BottonTop(RectTransform panel)
    {
        panel.GetComponent<UIPanel>().isActive = false;
        StartCoroutine(LerpMoveObject(panel, new CharacterSheet(), MovePanelType.bottonTop, false));
    }

    public void LerpOpenObject_BottonTop(RectTransform panel)
    {
        panel.GetComponent<UIPanel>().isActive = true;
        panel.GetComponent<UIPanel>().AjustSizeAsGrid();

        panel.transform.GetChild(panel.transform.childCount - 1).gameObject.SetActive(false);
        StartCoroutine(LerpMoveObject(panel, new CharacterSheet(), MovePanelType.bottonTop, true));
    }
}
