using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class GoodButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Button States")]
    [SerializeField] private Sprite Normal;
    [SerializeField] private Sprite Highlight;
    [SerializeField] private Sprite Selected;

    [SerializeField] private bool hasGroup;
    [SerializeField] public bool isTab;
    [SerializeField] public int tabIndex;
    public bool isSelected = false;

    public void SetSelectedState(bool state)
    {
        isSelected = state;

        if (isSelected)
            GetComponent<Image>().sprite = Selected;
        else
            GetComponent<Image>().sprite = Normal;

        if (isTab)
        {
            GameObject.Find("Global").GetComponent<CreateCharacter>().SelectTabPanel(tabIndex);
            transform.SetAsLastSibling();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (hasGroup)
            GetComponentInParent<GoodButtonsGroup>().SelectButton(transform);
        else
            SetSelectedState(!isSelected);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Image>().sprite = Highlight;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(isSelected)
            GetComponent<Image>().sprite = Selected;
        else
            GetComponent<Image>().sprite = Normal;
    }
}
