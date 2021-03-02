using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CraftStation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    [SerializeField] private string stationName;
    [SerializeField] private MyParameters.StationType stationType;
    [SerializeField] private GameObject activeEffect;

    private GameObject mainInventoryPanel;
    private GameObject stationInfoPanel;
    private GameObject stationSlotsParent;
    private Inventory stationInventory;
    private InventorySlot[] stationSlots;

    private bool isUsing = false;
    private bool isActive = false;
    private bool isProcessing = false;
    private bool hasFuel;
    private float fuelAmount = 0;

    private TextMeshProUGUI fuelCountOutput;
    private Button activeButton;

    public string GetStationName()
    {
        return stationName;
    }

    public MyParameters.StationType GetStationType()
    {
        return stationType;
    }

    public float GetFuelAmout()
    {
        return fuelAmount;
    }

    // Start is called before the first frame update
    void Awake()
    {
        stationInventory = GetComponent<Inventory>();
        isProcessing = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isUsing)
        {

            if (stationType == MyParameters.StationType.Campfire)
            {
                if (isActive)
                {
                    activeButton.GetComponent<Image>().color = Color.red;
                    activeEffect.SetActive(true);
                    activeButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Turn Off";
                    DisplayTime(fuelAmount);

                    if (fuelAmount > 0)
                    {
                        fuelAmount -= Time.deltaTime;

                        if (FuelSourceManager() != null)
                        {
                            FuelSourceManager().onUseValue -= Time.deltaTime;

                            if (FuelSourceManager().onUseValue <= 0)
                            {
                                stationInventory.RemoveItem(stationInventory.Items.IndexOf(FuelSourceManager()));
                                UpdateStationPanel();
                            }
                        }

                        if (!isProcessing && ProcessInventoryItems(MyParameters.ItemProperties.raw) != null)
                            StartCoroutine(ProcessItem(ProcessInventoryItems(MyParameters.ItemProperties.raw)));
                    }
                    else
                    {
                        Debug.Log("Fuel has run out!");
                        fuelAmount = 0;
                        isActive = false;
                        //StopCoroutine(ProcessItem(ProcessInventoryItems(Global.ItemProperties.raw)));
                        fuelCountOutput.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (activeEffect.activeSelf)
                    {
                        activeEffect.SetActive(false);
                        activeButton.GetComponent<Image>().color = Color.black;
                        activeButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Turn On";
                    }
                }
            }
        }
    }

    public Inventory GetStationInventory()
    {
        return stationInventory;
    }

    public void UsingStation(bool _isUsing)
    {
        if (_isUsing)
        {
            // Start Panel
            if (!mainInventoryPanel)
            {
                CreateStationPanel();
                Global.UI.activeStation = this;
            }
            else
            {
                if (!mainInventoryPanel.activeSelf)
                {
                    mainInventoryPanel.SetActive(!mainInventoryPanel.activeSelf);
                    Global.UI.activeStation = this;
                }
            }

            isUsing = _isUsing;

            Global.UI.ActiveStationInventory = this.stationInventory;

            foreach (Transform child in stationInfoPanel.transform)
            {
                if (child.name == "Name")
                {
                    child.gameObject.SetActive(true);
                    child.GetComponent<TextMeshProUGUI>().text = stationName;
                }

                if (child.name == "ActiveBtn")
                {
                    child.gameObject.SetActive(false);
                }

                if (stationType == MyParameters.StationType.Campfire)
                {
                    if (child.name == "ActiveBtn")
                    {
                        activeButton = child.GetComponent<Button>();
                        activeButton.gameObject.SetActive(true);
                        activeButton.onClick.AddListener(ActiveStation);
                    }

                    if (child.name == "FuelTime")
                    {
                        child.gameObject.SetActive(true);
                        fuelCountOutput = child.GetComponent<TextMeshProUGUI>();
                    }
                }
                else if (stationType == MyParameters.StationType.Chest)
                {
                    if (child.name == "ActiveBtn")
                    {
                        activeButton = child.GetComponent<Button>();
                        activeButton.gameObject.SetActive(false);
                        activeButton.onClick.AddListener(null);
                    }

                    if (child.name == "FuelTime")
                    {
                        child.gameObject.SetActive(false);
                        fuelCountOutput = null;
                    }
                }
            }

            UpdateStationPanel();

            //Global.Interface.UpdatePlayerInventory();
            //if (!Global.Interface.panel_PlayerInventory.activeSelf)
            //    Global.ShowCharacterInventory();
        }
    }

    private void CreateStationPanel()
    {
        mainInventoryPanel = Instantiate(Global.UI.StationPanelPrefab, Global.UI.StationPanelPrefab_Parent.transform);

        stationInfoPanel = mainInventoryPanel.transform.Find("StationPanel").gameObject;
        stationSlotsParent = mainInventoryPanel.transform.Find("Scroll View").Find("Viewport").Find("").Find("InventorySlotsParent").gameObject;
        stationSlots = stationSlotsParent.GetComponentsInChildren<InventorySlot>();

        mainInventoryPanel.GetComponent<UI_OtherInventoryPanel>().myStation = this;
        mainInventoryPanel.transform.SetSiblingIndex(mainInventoryPanel.transform.childCount);
    }

    public void UpdateStationPanel()
    {
        // Other Inventory
        if (mainInventoryPanel != null)
        {
            // Inventory Window
            for (int i = 0; i < stationSlots.Length; i++)
            {
                if (i < stationInventory.Items.Count)
                {
                    stationSlots[i].HoldSlot(stationInventory.Items[i]);
                }
                else
                {
                    stationSlots[i].CleanSlot();
                }
            }
        }
    }

    private void ActiveStation()
    {
        isActive = !isActive;

        if (isActive)
        {
            if (fuelAmount <= 0)
            {
                OutOfFuel();
                isActive = false;
                return;
            }
            else
            {
                fuelCountOutput.gameObject.SetActive(true);
                activeButton.GetComponent<Image>().color = Color.red;
                activeButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Turn Off";
            }
        }
    }

    /// <summary>
    /// Process Station Items
    /// - Cook Items
    /// </summary>
    /// <param name="propriety"></param>
    /// <returns></returns>
    #region Process Station Items

    private Item ProcessInventoryItems(MyParameters.ItemProperties propriety)
    {
        for (int i = stationInventory.Items.Count - 1; i >= 0; i--)
        {
            if (stationInventory.Items[i].itemProperties.Contains(propriety))
            {
                return stationInventory.Items[i];
            }
        }

        return null;
    }

    IEnumerator ProcessItem(Item item)
    {
        // Processing Item
        isProcessing = true;

        if (stationType == MyParameters.StationType.Campfire)
        {
            if (ProcessInventoryItems(MyParameters.ItemProperties.raw) != null)
            {
                //yield on a new YieldInstruction that waits for 5 seconds
                yield return new WaitForSeconds(5);

                //After we have waited 5 seconds
                Debug.Log("Item is Cooked");


                item.itemProperties.Add(MyParameters.ItemProperties.cooked);
                item.itemProperties.Remove(MyParameters.ItemProperties.raw);

                item.itemName = item.alternativeNames[0];
                item.itemIcon = item.alternativeIcons[0];
            }
        }

        UpdateStationPanel();
        isProcessing = false;
    }

    #endregion

    /// <summary>
    /// Fuel Management
    /// - Add Fuel
    /// - Remove Fuel
    /// - Calculate Time
    /// - Auto Remove Fuel Souce
    /// </summary>
    /// <param name="amout"></param>
    #region fuel Management

    public void AddFuel(float amout)
    {
        fuelAmount += amout;
    }

    public void RemoveFuel(float amount)
    {
        fuelAmount -= amount;
    }

    public void OutOfFuel()
    {
        Debug.Log("Out of Fuel!");
    }

    public Item FuelSourceManager()
    {
        for (int i = stationInventory.Items.Count - 1; i >= 0; i--)
        {
            if (stationInventory.Items[i].itemProperties.Contains(MyParameters.ItemProperties.fuel))
            {
                return stationInventory.Items[i];
            }
        }

        return null;
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        TimeSpan timeSpan = TimeSpan.FromSeconds(timeToDisplay);
        string timeText = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

        fuelCountOutput.text = $"Fuel Time: {timeText}";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(name);

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
                        UsingStation(true);
                    }
                    else
                    {
                        StartCoroutine(Global.Commands.GetMainSelectedCharacterTransform().GetComponent<PlayerCharacterController>().MoveToObject(transform, MyParameters.ObjectCategory.Station));
                    }
                }
            }
        }
    }
    #endregion
}
