using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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

    [Header("Tab States")]
    [SerializeField] public bool isTab;
    [SerializeField] public int tabIndex;
    [SerializeField] public bool hasOwnTabPanel;
    [SerializeField] private GameObject myTab;

    [Header("Events")]
    public bool isSelected = false;
    [SerializeField] public UnityEvent Callback;

    [SerializeField]
    public void InvokeCallback()
    {
        Callback?.Invoke();
    }

    private void Awake()
    {
        if (!hasOwnTabPanel)
            myTab = null;
    }

    public void SetSelectedState(bool state)
    {
        isSelected = state;

        if (isSelected)
            GetComponent<Image>().sprite = Selected;
        else
            GetComponent<Image>().sprite = Normal;   
    }

    public void ShowMyTab()
    {
        myTab.SetActive(true);

        foreach (Transform child in transform.parent)
        {
            if (child != transform)
                child.GetComponent<GoodButton>().myTab.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        if (hasGroup)
            GetComponentInParent<GoodButtonsGroup>().SelectButton(transform);
        else
            SetSelectedState(!isSelected);

        InvokeCallback();
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
