using System;
using System.Runtime.CompilerServices;
using Game.Scripts.LiveObjects;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static Action<Vector2> movement;
    public static Action<InputAction.CallbackContext> interactStarted;
    public static Action<InputAction.CallbackContext> interactCanceled;
    public static Action<InputAction.CallbackContext> cancelAction;
    public static Action<InputAction.CallbackContext> droneFlyUpStarted;
    public static Action<InputAction.CallbackContext> droneFlyUpCanceled;
    public static Action<InputAction.CallbackContext> droneFlyDownStarted;
    public static Action<InputAction.CallbackContext> droneFlyDownCanceled;
        
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

        _playerInput.Drone.FlyUp.started += FlyUp_started;
        _playerInput.Drone.FlyUp.canceled += FlyUp_canceled;
        _playerInput.Drone.FlyDown.started += FlyDown_started;
        _playerInput.Drone.FlyDown.canceled += FlyDown_canceled;

        Drone.OnEnterFlightMode += EnableDroneActionmap;
        Drone.onExitFlightmode += DisableDroneActionmap;
    }
    
    private void Update()
    {
        PlayerMovement();
    }
    
    private void EnableDroneActionmap() => _playerInput.Drone.Enable();
    private void DisableDroneActionmap() => _playerInput.Drone.Disable();
    private void EnableForkliftActionmap() => _playerInput.Forklift.Enable();
    private void DisableForkliftActionmap() => _playerInput.Forklift.Disable();
    
    //--- General Actions ---
    private void PlayerMovement()
    {
        var move = _playerInput.GeneralActions.Movement.ReadValue<Vector2>();
        movement(move);
    }

    private void Interact_Started(InputAction.CallbackContext objContext)
    {
        if (_playerInput.GeneralActions.enabled)
            interactStarted(objContext);
    }

    private void Interact_Canceled(InputAction.CallbackContext objContext)
    {
        if (_playerInput.GeneralActions.enabled)
            interactCanceled(objContext);
    }

    private void Cancel_performed(InputAction.CallbackContext objContext)
    {
        if (_playerInput.GeneralActions.enabled)
            cancelAction(objContext);
    }

    //--- Drone Actions ---
    private void FlyUp_started(InputAction.CallbackContext objContext)
    {
        if (_playerInput.Drone.enabled)
            Drone.droneFlyUp = true;
    }
    
    private void FlyUp_canceled(InputAction.CallbackContext objContext)
    {
        if (_playerInput.Drone.enabled)
            Drone.droneFlyUp = false;
    }

    private void FlyDown_started(InputAction.CallbackContext objContext)
    {
        if (_playerInput.Drone.enabled)
            Drone.droneFlyDown = true;
    }
    
    private void FlyDown_canceled(InputAction.CallbackContext objContext)
    {
        if (_playerInput.Drone.enabled)
            Drone.droneFlyDown = false;
    }
}


