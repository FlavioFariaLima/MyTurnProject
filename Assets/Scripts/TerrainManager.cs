using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class TerrainManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    LineRenderer line;
    float alpha = 1.0f;
    Gradient gradient;

    // Start is called before the first frame update
    void Start()
    {
        line = this.GetComponent<LineRenderer>();
        gradient = new Gradient();
        gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.green, 0.0f),
                new GradientColorKey(Color.red, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f),
                new GradientAlphaKey(alpha, 1.0f) }
                );
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Global.Commands.GetSelectedCharacters().Count > 0)
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                if (Global.UI.characterSheet.controller.CanMove() && !Global.UI.characterSheet.controller.IsAi())
                {
                    ShowPath(hit.point);
                    Global.UI.distanceInfo.gameObject.SetActive(true);
                    line.enabled = true;
                }
                else
                {
                    if (!Global.Commands.playerIsAttacking)
                        Global.UI.distanceInfo.gameObject.SetActive(false);

                    line.enabled = false;

                    if (Input.GetMouseButtonDown(0))
                    {
                        //Global.Commands.GetSelectedCharacters()[0].SetSelectState(false);
                    }
                }
            }
        }
        else
        {
            Global.UI.distanceInfo.gameObject.SetActive(false);
        }
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

    /// <summary>
    /// Path Line and Path Calculations
    /// </summary>
    void ShowPath(Vector3 dest)
    {
        if (Global.Commands.GetSelectedCharacters()[0].hasMoved >= Global.Commands.GetSelectedCharacters()[0].character.GetMovement())
        {
            return;
        }
        else if (Global.Commands.GetSelectedCharacters()[0].IsAi())
        {
            return;
        }

        // Deal with line
        if (line == null)
        {
            line = this.gameObject.AddComponent<LineRenderer>();
        }

        line.startWidth = 0.05f;
        //line.startColor = new Color(1, 1, 1, 0.2f);
        line.colorGradient = gradient;

        Global.Commands.GetSelectedCharacters()[0].movementSofar = Global.Commands.GetSelectedCharacters()[0].hasMoved;
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(Global.Commands.GetMainSelectedCharacterTransform().position, dest, NavMesh.AllAreas, path);
        float distance = 0;
        float soFar = Global.UI.characterSheet.controller.hasMoved;
        Vector3 finalPoint = new Vector3();

        for (int i = 0; i < path.corners.Length - 1; i++) // Leave room to add 1
        {
            float segmentDistance = (path.corners[i + 1] - path.corners[i]).magnitude;

            if (Global.Commands.GetSelectedCharacters()[0].movementSofar + segmentDistance <= Global.Commands.GetSelectedCharacters()[0].character.GetMovement())
            {
                Global.Commands.GetSelectedCharacters()[0].movementSofar += segmentDistance;
            }
            else // Path length exceeds maxDist
            {
                finalPoint = path.corners[i] + ((path.corners[i + 1] - path.corners[i]).normalized * (Global.Commands.GetSelectedCharacters()[0].character.GetMovement() - Global.Commands.GetSelectedCharacters()[0].movementSofar));
                distance = Vector3.Distance(Global.Commands.GetSelectedCharacters()[0].transform.position, finalPoint);
                NavMesh.CalculatePath(Global.Commands.GetSelectedCharacters()[0].transform.position, finalPoint, NavMesh.AllAreas, path);
                break;
            }
        }

        if (path.corners.Length > 0)
        {
            distance = Vector3.Distance(Global.Commands.GetSelectedCharacters()[0].transform.position, path.corners[path.corners.Length - 1]);

            line.positionCount = path.corners.Length;

            for (int i = 0; i < path.corners.Length; i++)
            {
                line.SetPosition(i, path.corners[i]);

                float dist = Vector3.Distance(line.GetPosition(0), line.GetPosition(i));
                if (dist > 9)
                {
                    line.SetPosition(i, Vector3.MoveTowards(line.GetPosition(0), line.GetPosition(i),
                                    Global.Commands.GetMainSelectedCharacterTransform().GetComponent<CharacterSheet>().GetMovement()));
                    return;
                }
            }

            if (Math.Round(distance, 2) > 0 && !Global.Commands.GetSelectedCharacters()[0].IsAi())
                Global.UI.distanceInfo.text = Math.Round(distance, 1).ToString();

            if (line.positionCount > 0 && !Global.Commands.GetSelectedCharacters()[0].IsAi())
                Global.UI.distanceInfo.rectTransform.position = Camera.main.WorldToScreenPoint(line.GetPosition(line.positionCount - 1));
        }
        else
        {
            Global.UI.distanceInfo.text = string.Empty;
        }
    }
}
