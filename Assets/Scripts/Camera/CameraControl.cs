using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CameraControl : MonoBehaviour
{
    public float panSpeed = 20f;
    public float rotationSpeed = 10f;
    public float zoomSpeed = 50f;
    public float borderWidth = 10f;
    public bool edgeScrolling = true;

    private float zoomMin = 5;
    private float zoomMax = 20;
    private float mouseX, mouseY;

    private bool focusObject;
    private Transform objToFocus;
    private bool followCharacter;
    private Transform character;
    private float currentDistance = 5;

    void Start()
    {
        mouseY = Mathf.Clamp(mouseY, 30, 90);
        transform.localRotation = Quaternion.Euler(mouseY, mouseX, 0);
    }

    private void Update()
    {
        Movement();
        Rotation();
        Zoom();

    }

    private void FixedUpdate()
    {
        if (focusObject && objToFocus != null)
        {
            LookTo();
        }

        if (followCharacter)
        {
            Follow();
        }
    }

    private void FocusInObject(Transform obj)
    {
        objToFocus = obj;
        focusObject = true;
    }

    private void LookTo()
    {
        //transform.LookAt(objToFocus);
        Vector3 relativePos = objToFocus.position - transform.position;
        Quaternion toRotation = Quaternion.LookRotation(relativePos);
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 2 * Time.deltaTime);
    }

    private void Movement()
    {
        Vector3 pos = transform.position;
        Vector3 forward = transform.forward;

        forward.y = 0;
        forward.Normalize();

        Vector3 right = transform.right;
        right.y = 0;
        right.Normalize();

        if (Input.GetKey("w") || edgeScrolling == true && Input.mousePosition.y >= Screen.height - borderWidth)
        {
            // Cancel any lock
            focusObject = false;
            followCharacter = false;

            pos += forward * panSpeed * Time.deltaTime;
        }

        if (Input.GetKey("s") || edgeScrolling == true && Input.mousePosition.y <= borderWidth)
        {
            // Cancel any lock
            focusObject = false;
            followCharacter = false;

            pos -= forward * panSpeed * Time.deltaTime;
        }

        if (Input.GetKey("d") || edgeScrolling == true && Input.mousePosition.x >= Screen.width - borderWidth)
        {
            // Cancel any lock
            focusObject = false;
            followCharacter = false;

            pos += right * panSpeed * Time.deltaTime;
        }

        if (Input.GetKey("a") || edgeScrolling == true && Input.mousePosition.x <= borderWidth)
        {
            // Cancel any lock
            focusObject = false;
            followCharacter = false;

            pos -= right * panSpeed * Time.deltaTime;
        }

        transform.position = pos;
    }

    private void Rotation()
    {
        if (Input.GetMouseButton(2))
        {
            // Cancel any lock
            focusObject = false;

            mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
            mouseY -= Input.GetAxis("Mouse Y") * rotationSpeed;

            mouseY = Mathf.Clamp(mouseY, 30, 90);
            transform.rotation = Quaternion.Euler(mouseY, mouseX, transform.rotation.z);
        }
    }

    private void Zoom()
    {
        Vector3 camPos = transform.position;

        if (Input.GetAxis("Mouse ScrollWheel") > 0 || Input.GetKeyDown(KeyCode.Minus))
        {
            // Cancel any lock
            followCharacter = false;

            //camPos = Vector3.Lerp(camPos, new Vector3(camPos.x, camPos.y - 10, camPos.z), 100);
            camPos.y -= 300 * Time.deltaTime;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 || Input.GetKeyDown(KeyCode.Plus))
        {
            // Cancel any lock
            followCharacter = false;

            //camPos = Vector3.Lerp(camPos, new Vector3(camPos.x, camPos.y + 10, camPos.z), 100);
            camPos.y += 300 * Time.deltaTime;
        }

        camPos.y = Mathf.Clamp(camPos.y, zoomMin, zoomMax);
        transform.position = camPos;
    }

    public void FollowCharacter(Transform Character)
    {
        FocusInObject(Character);

        character = Character;
        followCharacter = true;
    }

    private void Follow()
    {
        currentDistance = Mathf.Clamp(zoomMin, 0, zoomMax);
        transform.position = Vector3.Lerp(transform.position, character.position + Vector3.up * currentDistance - character.forward * (currentDistance + 2 * 0.5f), panSpeed * (Time.deltaTime/15));        
    }

    public IEnumerator GoToCharacter(Vector3 position)
    {
        while (Vector3.Distance(transform.localPosition, position) > 10f)
        {
            transform.position = Vector3.Lerp(transform.position, position + Vector3.up * 2, panSpeed * (Time.deltaTime / 15));
            transform.LookAt(position);
            yield return null;
        }

        Debug.Log($"Go to Position: {position}");
    }
}
