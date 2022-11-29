using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static Action<Vector2> movement;
    public static Action<InputAction.CallbackContext> interactStarted;
    public static Action<InputAction.CallbackContext> interactCanceled;
    public static Action<InputAction.CallbackContext> cancelAction;

    private PlayerInputActions _playerInput;
    
    public static string interactKey;

    private void OnEnable()
    {
        InitializeInputs();
        interactKey = _playerInput.GeneralActions.Interact.name;
    }
    
    private void InitializeInputs()
    {
        _playerInput = new PlayerInputActions();
        _playerInput.GeneralActions.Enable();
        _playerInput.GeneralActions.Interact.started += Interact_Started;
        _playerInput.GeneralActions.Interact.canceled += Interact_Canceled;
        _playerInput.GeneralActions.Cancel.performed += Cancel_performed;
    }
    
    private void Update()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        var move = _playerInput.GeneralActions.Movement.ReadValue<Vector2>();
        movement(move);
    }
    
    private void Interact_Started(InputAction.CallbackContext objContext) => interactStarted(objContext);
    private void Interact_Canceled(InputAction.CallbackContext objContext) => interactCanceled(objContext);
    private void Cancel_performed(InputAction.CallbackContext objContext) => cancelAction(objContext);
}


