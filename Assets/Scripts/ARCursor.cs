using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class ARCursor : MonoBehaviour
{
    public GameObject cursorChildObject;
    public GameObject objectToPlace;
    public ARRaycastManager raycastManager;

    public bool useCursor = true;

    //public TMP_Text positionText;
    private int counter = 0;

    // Start is called before the first frame update
    void Start()
    {
        cursorChildObject.SetActive(useCursor);

        //positionText.SetText("lick my balls");
    }

    // Update is called once per frame
    void Update()
    {
        if (useCursor) 
        {
            UpdateCursor();
        }
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (useCursor)
            {
                GameObject.Instantiate(objectToPlace, transform.position, transform.rotation);
            }
            else
            {
                List<ARRaycastHit> hits = new List<ARRaycastHit>();
                raycastManager.Raycast(Input.GetTouch(0).position, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);
                if (hits.Count > 0)
                {
                    GameObject.Instantiate(objectToPlace, hits[0].pose.position, hits[0].pose.rotation);
                }
            }
        }
    }
    void UpdateCursor()
    {
        Vector2 screenPosition = Camera.main.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

        if (hits.Count > 0) 
        {
            transform.position = hits[0].pose.position;
            transform.rotation = hits[0].pose.rotation;

            //UpdateText(transform.position);
        }
    }
    void UpdateText(Vector3 position)
    {
        //positionText.text = position.ToString();

        counter++;
        //positionText.text = counter.ToString();
    }
}
