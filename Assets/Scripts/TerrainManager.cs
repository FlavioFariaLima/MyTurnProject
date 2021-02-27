using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class TerrainManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Global.Commands.GetSelectedCharacters().Count > 0)
        {
            //Use this to tell when the user left-clicks on the Button
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                RaycastHit hit;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                {
                    StartCoroutine(Global.Commands.GetMainSelectedCharacterTransform().GetComponent<PlayerCharacterController>().MoveToPosition(hit.point));
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // throw new System.NotImplementedException();
    }
}
