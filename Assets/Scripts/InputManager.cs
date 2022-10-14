using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class InputManager : MonoBehaviourSingleton<InputManager>
{
    #region Events
    public delegate void StartTouch(Vector2 position, float time);
    public event StartTouch OnStartTouch;
    public delegate void EndTouch(Vector2 position, float time);
    public event EndTouch OnEndTouch;
    #endregion

    private PlayerControls playersControls;
    private Camera mainCamera;

    private void Awake()
    {
        playersControls = new PlayerControls();
        mainCamera = Camera.main;
    }
    private void OnEnable()
    {
        playersControls.Enable();
    }
    private void OnDisable()
    {
        playersControls.Disable();
    }

    void Start()
    {
        playersControls.Touch.PrimaryContact.started += ctx => StartTouchPrimary(ctx);
        playersControls.Touch.PrimaryContact.canceled += ctx => EndTouchPrimary(ctx);
    }
    private void StartTouchPrimary(InputAction.CallbackContext context)
    {
        //if (OnStartTouch != null) OnStartTouch(Utils.ScreenToWorld(mainCamera,playersControls.Touch.PrimaryPosition.ReadValue<Vector2>()), (float)context.startTime);
        if (OnStartTouch != null) OnStartTouch(playersControls.Touch.PrimaryPosition.ReadValue<Vector2>(), (float)context.startTime);

    }
    private void EndTouchPrimary(InputAction.CallbackContext context)
    {
        //if (OnEndTouch != null) OnEndTouch(Utils.ScreenToWorld(mainCamera, playersControls.Touch.PrimaryPosition.ReadValue<Vector2>()), (float)context.time);
        if (OnEndTouch != null) OnEndTouch(playersControls.Touch.PrimaryPosition.ReadValue<Vector2>(), (float)context.time);


    }
    public Vector2 PrimaryPosition()
    {
        //return Utils.ScreenToWorld(mainCamera, playersControls.Touch.PrimaryPosition.ReadValue<Vector2>());

        return playersControls.Touch.PrimaryPosition.ReadValue<Vector2>();
    }
}
