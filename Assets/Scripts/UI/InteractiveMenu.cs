using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public static class InteractiveMenu
{
    static Dictionary<int, string> options = new Dictionary<int, string>();

    public static void SetupMenu(MyParameters.ObjectCategory _category, GameObject _obj)
    {
        if (options.Count == 0)
            CreateDefault();

        for (int i = 0; i < options.Count; i++)
        {
            Global.UI.InterectiveMenu.transform.GetChild(0).transform.GetChild(i).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = options[i];
            Global.UI.InterectiveMenu.transform.GetChild(0).transform.GetChild(i).gameObject.SetActive(false);
        }

        ActiveOptions(_category, _obj);
    }


    static void CreateDefault()
    {
        options.Add(0, "Use");
        options.Add(1, "Pickup");
        options.Add(2, "Option 3");
        options.Add(3, "Option 4");
        options.Add(4, "Throw");
        options.Add(5, "Examine");
    }

    static void ActiveOptions(MyParameters.ObjectCategory category, GameObject obj)
    {
        switch (category)
        {
            case MyParameters.ObjectCategory.Item:
                //
                Global.UI.InterectiveMenu.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);

                break;

            case MyParameters.ObjectCategory.Station:

                // Use
                Global.UI.InterectiveMenu.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
                Global.UI.InterectiveMenu.transform.GetChild(0).transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { obj.GetComponent<CraftStation>().UsingStation(true); });

                break;
            default:
                break;
        }

        Global.UI.InterectiveMenu.transform.GetChild(0).transform.GetChild(5).gameObject.SetActive(true);
        Global.UI.InterectiveMenu.transform.GetChild(0).transform.GetChild(5).GetComponent<Button>().onClick.AddListener(delegate { OpenExaminePanel(category, obj); });
    }

    static void OpenExaminePanel(MyParameters.ObjectCategory category, GameObject obj)
    {
        string name = "";
        string type = "";
        string str3 = "";
        string str4 = "";

        if (category == MyParameters.ObjectCategory.Item)
        {
            ItemBlueprint item = obj.GetComponent<DropedItem>().GetItemBlueprint();

            name = item.itemName;
            type = "Item";
            str3 = item.itemType.ToString();
            str4 = item.weight.ToString();
        }
        else if (category == MyParameters.ObjectCategory.Station)
        {
            CraftStation station = obj.GetComponent<CraftStation>();

            name = station.GetStationName();
            type = "Interactive Object";
            str3 = station.GetStationType().ToString();
            str4 = station.GetFuelAmout().ToString();
        }

        // Image
        Global.UI.uiCamera.transform.position = new Vector3(obj.transform.position.x - 1.5f, obj.transform.position.y + 0.5f, obj.transform.position.z - 2 );
        Global.UI.uiCamera.transform.LookAt(obj.transform);

        // Stats
        Global.UI.ExaminePanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
        Global.UI.ExaminePanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = type;
        Global.UI.ExaminePanel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = str3;
        Global.UI.ExaminePanel.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = str4;

        // Setup Panel
        Global.UI.ExaminePanel.transform.position = Input.mousePosition;
        Global.UI.ExaminePanel.gameObject.SetActive(true);
    }
}
