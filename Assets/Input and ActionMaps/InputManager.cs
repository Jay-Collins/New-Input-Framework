using System;
using System.Runtime.CompilerServices;
using Game.Scripts.LiveObjects;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class InputManager : MonoBehaviour
{
    public static Action<Vector2> movement;
    public static Action<InputAction.CallbackContext> interactStarted;
    public static Action<InputAction.CallbackContext> interactCanceled;
    public static Action<InputAction.CallbackContext> interactPerformed;
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
        _playerInput.GeneralActions.Interact.started += Interact_started;
        _playerInput.GeneralActions.Interact.canceled += Interact_canceled;
        _playerInput.GeneralActions.Interact.performed += Interact_performed;
        _playerInput.GeneralActions.Cancel.performed += Cancel_performed;

        _playerInput.Drone.FlyUp.started += FlyUp_started;
        _playerInput.Drone.FlyUp.canceled += FlyUp_canceled;
        _playerInput.Drone.FlyDown.started += FlyDown_started;
        _playerInput.Drone.FlyDown.canceled += FlyDown_canceled;

        _playerInput.Forklift.LiftUp.started += LiftUp_started;
        _playerInput.Forklift.LiftUp.canceled += LiftUp_canceled;
        _playerInput.Forklift.LiftDown.started += LiftDown_started;
        _playerInput.Forklift.LiftDown.canceled += LiftDown_canceled;

        Drone.OnEnterFlightMode += EnableDroneActionmap;
        Drone.onExitFlightmode += DisableDroneActionmap;

        Forklift.onDriveModeEntered += EnableForkliftActionmap;
        Forklift.onDriveModeExited += DisableForkliftActionmap;
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

    private void Interact_started(InputAction.CallbackContext objContext)
    {
        if (_playerInput.GeneralActions.enabled)
            interactStarted(objContext);
    }

    private void Interact_canceled(InputAction.CallbackContext objContext)
    {
        if (_playerInput.GeneralActions.enabled)
            interactCanceled(objContext);
    }

    private void Interact_performed(InputAction.CallbackContext objContext)
    {
        if (_playerInput.GeneralActions.enabled)
            interactPerformed(objContext);
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
    
    //--- Forklift Actions ---

    private void LiftUp_started(InputAction.CallbackContext objContext)
    {
        if (_playerInput.Forklift.enabled)
            Forklift.forkliftRaiseForks = true;
    }

    private void LiftUp_canceled(InputAction.CallbackContext objContext)
    {
        if (_playerInput.Forklift.enabled)
            Forklift.forkliftRaiseForks = false;
    }

    private void LiftDown_started(InputAction.CallbackContext objContext)
    {
        if (_playerInput.Forklift.enabled)
            Forklift.forkliftLowerForks = true;
    }

    private void LiftDown_canceled(InputAction.CallbackContext objContext)
    {
        if (_playerInput.Forklift.enabled)
            Forklift.forkliftLowerForks = false;
    }
}


