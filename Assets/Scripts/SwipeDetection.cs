
using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SwipeDetection : MonoBehaviour
{
    [SerializeField]
    private float minimumDistance = 0.2f;

    [SerializeField]
    private float maximumTime = 1f;

    [SerializeField, Range(0f, 1f)]
    private float directionThreshold = 0.9f;

    [SerializeField]
    private GameObject trail;

    private Camera arCamera;

    private ARRaycastManager aRRaycastManager;

    private InputManager inputManager;

    private Vector2 startPosition;
    private float startTime;
    private Vector2 endPosition;
    private float endTime;

    SlowTurning slowTurning;

    [SerializeField]
    private Hand playerHand;

    private Coroutine coroutine;

    private void Awake()
    {
        inputManager = InputManager.Instance;

        arCamera = Camera.main;

        aRRaycastManager = arCamera.GetComponentInParent<ARRaycastManager>();
    }

    private void OnEnable()
    {
        inputManager.OnStartTouch += SwipeStart;
        inputManager.OnEndTouch += SwipeEnd;
    }

    private void Start()
    {
        slowTurning = Camera.main.transform.GetChild(0).GetComponent<SlowTurning>();
    }

    private void OnDisable()
    {
        inputManager.OnStartTouch -= SwipeStart;
        inputManager.OnEndTouch -= SwipeEnd;
    }
    private void SwipeStart(Vector2 position, float time)
    {
        startPosition = position;
        startTime = time;

        trail.SetActive(true);
        trail.transform.position = position;
        coroutine = StartCoroutine(Trail());
    }

    private IEnumerator Trail()
    {
        while (true)
        {
            trail.transform.position = inputManager.PrimaryPosition();
            yield return null;
        }
    }

    private void SwipeEnd(Vector2 position, float time)
    {
        trail.SetActive(false);
        StopCoroutine(coroutine);

        endPosition = position;
        endTime = time;
        DetectSwipe();
        DetectTap();
    }


    private void DetectSwipe()
    {
        if(Vector3.Distance(startPosition,endPosition) >= minimumDistance && (endTime - startTime) <= maximumTime)
        {
            Debug.Log("Swipe Detected");
            Debug.DrawLine(startPosition, endPosition, Color.red, 5f);

            //put the swipe code here



            Vector3 direction = endPosition - startPosition;
            Vector2 direction2d = new Vector2(direction.x, direction.y).normalized;

            slowTurning.SlowTurn(direction.x);

            SwipeDirection(direction2d);
        }
    }

    private void SwipeDirection(Vector2 direction)
    {
        if(Vector2.Dot(Vector2.up, direction)> directionThreshold)
        {
            Debug.Log("Swipe Up");

            Debug.Log(playerHand.canPlay);
            if (playerHand.canPlay)
            {
                playerHand.PlayCard();
            }
            else
            {
                Debug.Log("not your turn");
            }
            //playerHand.SelectCard();
        }
        else if (Vector2.Dot(Vector2.down, direction) > directionThreshold)
        {
            Debug.Log("Swipe Down");
            //playerHand.SelectCard();
        }
        else if (Vector2.Dot(Vector2.left, direction) > directionThreshold)
        {
            Debug.Log("Swipe Left");
        }
        else if (Vector2.Dot(Vector2.right, direction) > directionThreshold)
        {
            Debug.Log("Swipe Right");
        }
    }

    private void DetectTap()
    {
        Vector2 touchPosition = startPosition;

        if (Vector3.Distance(startPosition, endPosition) < minimumDistance)
        {
            Debug.Log("Tap Detected");

            Ray ray = arCamera.ScreenPointToRay(touchPosition);
            RaycastHit hitObject;
            if(Physics.Raycast(ray, out hitObject))
            {
                Card card = hitObject.transform.GetComponent<Card>();
                Debug.Log("hit this => " + hitObject.transform.gameObject.name);
                if(card != null)
                {
                    playerHand.SelectCard(card);
                }
            }
        }
    }
}
