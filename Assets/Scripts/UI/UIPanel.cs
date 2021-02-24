using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanel : MonoBehaviour
{
    public bool isActive;

    [SerializeField] private bool activeAutoSize;   

    public void AjustSizeAsGrid()
    {
        if (activeAutoSize)
        {
            int row = 0;
            int column = 0;
            GridLayoutGroup grid = transform.GetChild(0).GetComponent<GridLayoutGroup>();

            Global.GetGridColumnAndRow(grid, out column, out row);

            this.GetComponent<RectTransform>().sizeDelta = new Vector2(586, 120 + (grid.cellSize.y * column));
            grid.gameObject.GetComponent<RectTransform>().sizeDelta = this.GetComponent<RectTransform>().sizeDelta;
            //grid.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        }
    }
}
