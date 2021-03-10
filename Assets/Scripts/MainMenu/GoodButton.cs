using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GoodButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Button States")]
    [SerializeField] private Sprite Normal;
    [SerializeField] private Sprite Highlight;
    [SerializeField] private Sprite Selected;

    [SerializeField] private bool hasGroup;
    public bool isSelected = false;


    public void SetSelectedState(bool state)
    {
        isSelected = state;

        if (isSelected)
            GetComponent<Image>().sprite = Selected;
        else
            GetComponent<Image>().sprite = Normal;
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
