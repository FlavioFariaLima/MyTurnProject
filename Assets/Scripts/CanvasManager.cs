using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [Header("Cursors")]
    [SerializeField] public Texture2D cursorDefault;
    [SerializeField] public Texture2D cursorInteract;
    [SerializeField] public Texture2D cursorMove;
    [SerializeField] public Texture2D cursorMelee;
    [SerializeField] public Texture2D cursorRange;

    private bool mouseIsOver;
    private Texture2D lastCursor;
    public Texture2D LastCursor
    {
        get { return lastCursor; }
    }

    public bool MouseIsOver
    {
        get { return mouseIsOver; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        mouseIsOver = true;
        Global.Canvas.SetCursor(Global.Canvas.cursorDefault, true);
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        mouseIsOver = false;
        Global.Canvas.SetCursor(Global.Canvas.LastCursor, false);
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


    // Menu and Stuff
    public void ShowMatchMenu()
    {
        Global.UI.GetMatchMenu().transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
        Global.UI.GetMatchMenu().SetActive(!Global.UI.GetMatchMenu().activeSelf);

        if (Global.UI.GetMatchMenu().activeSelf)
            Global.PauseGame();
        else
            Global.ResumeGame();
    }

    // Misc
    public void GetGridColumnAndRow(GridLayoutGroup glg, out int column, out int row)
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

public enum PanelPosition
{
    left = 0,
    right = 1,
    top = 2,
    bottom = 3
}
