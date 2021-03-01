using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanel : MonoBehaviour
{
    [Header("Panel Status")]
    [SerializeField] private bool isActive;
    [SerializeField] private bool activeAutoSize;
    [SerializeField] private PanelPosition PanelPosition;

    [Header("Panel Button")]
    [SerializeField] private GameObject MainButton;

    private void Start()
    {
        SetMainButton(PanelPosition);

        if (isActive)
        {
            isActive = !isActive;
            MainButton.GetComponent<Button>().onClick.Invoke();
        }
    }

    public void AjustSizeAsGrid()
    {
        if (activeAutoSize)
        {
            int row = 0;
            int column = 0;
            GridLayoutGroup grid = transform.GetChild(0).GetComponent<GridLayoutGroup>();

            Global.GetGridColumnAndRow(grid, out column, out row);

            GetComponent<RectTransform>().sizeDelta = new Vector2(586, (grid.cellSize.y * column) - 100);
            grid.gameObject.GetComponent<RectTransform>().sizeDelta = GetComponent<RectTransform>().sizeDelta;
            //grid.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        }
    }

    public float ScaleFactor()
    {
        return Global.CanvasManager.gameObject.GetComponent<Canvas>().scaleFactor;
    }

    public IEnumerator LerpMoveObject(RectTransform rectTransform, PanelPosition panelPos, bool open)
    {
        float timeOfTravel = 0.4f; //time to object reach a target place 
        float currentTime = 0; // actual floting time 
        float normalizedValue;

        rectTransform.ForceUpdateRectTransforms();

        if (panelPos == PanelPosition.left)
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
            }
        }
        else if (panelPos == PanelPosition.right)
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
            }
        }
        else if (panelPos == PanelPosition.top)
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
            }
        }
        else if (panelPos == PanelPosition.bottom)
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

        isActive = !isActive;
    }

    public void SetMainButton(PanelPosition PanelPosition)
    {
        if(PanelPosition == PanelPosition.left)
        {
            
                MainButton.GetComponent<Button>().onClick.AddListener(delegate { 
                    StartCoroutine(LerpMoveObject(GetComponent<RectTransform>(),
                    PanelPosition, isActive)); });
            
        }
        else if (PanelPosition == PanelPosition.right)
        {
                MainButton.GetComponent<Button>().onClick.AddListener(delegate {
                    StartCoroutine(LerpMoveObject(GetComponent<RectTransform>(),
                    PanelPosition, isActive)); 
                });            
        }
        else if (PanelPosition == PanelPosition.top)
        {
                MainButton.GetComponent<Button>().onClick.AddListener(delegate {
                    StartCoroutine(LerpMoveObject(GetComponent<RectTransform>(),
                    PanelPosition, !isActive)); 
                });            
        }
        else if (PanelPosition == PanelPosition.bottom)
        {
                MainButton.GetComponent<Button>().onClick.AddListener(delegate {
                    StartCoroutine(LerpMoveObject(GetComponent<RectTransform>(),
                    PanelPosition, isActive)); 
                });            
        }
    }
}
