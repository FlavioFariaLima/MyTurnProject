using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CanvasManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool mouseIsOver;
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
        Global.UI.SetCursor(Global.UI.cursorDefault, true);
        Debug.Log("Cursor Entering " + name + " GameObject");
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        mouseIsOver = false;
        Global.UI.SetCursor(Global.UI.LastCursor, false);
        Debug.Log("Cursor Exiting " + name + " GameObject");
    }
}

public enum PanelPosition
{
    left = 0,
    right = 1,
    top = 2,
    bottom = 3
}
