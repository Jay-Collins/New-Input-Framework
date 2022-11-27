using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Game.Scripts.UI;
using UnityEngine.InputSystem;

namespace Game.Scripts.LiveObjects
{
    public class Drone : MonoBehaviour
    {
        private enum Tilt
        {
            NoTilt, Forward, Back, Left, Right
        }

        [SerializeField]
        private Rigidbody _rigidbody;
        [SerializeField]
        private float _speed = 5f;
        private bool _inFlightMode = false;
        [SerializeField]
        private Animator _propAnim;
        [SerializeField]
        private CinemachineVirtualCamera _droneCam;
        [SerializeField]
        private InteractableZone _interactableZone;

        private PlayerInputActions _playerInput;
        private bool _thrustingUp;
        private bool _thrustingDown;

        public static event Action OnEnterFlightMode;
        public static event Action onExitFlightmode;

        private void OnEnable()
        {
            _playerInput = new PlayerInputActions();
            InteractableZone.onZoneInteractionComplete += EnterFlightMode;
            
            _playerInput.Drone.ThrustUp.started += ThrustUp_started;
            _playerInput.Drone.ThrustUp.canceled += ThrustUp_canceled;
            
            _playerInput.Drone.ThrustDown.started += ThrustDown_started;
            _playerInput.Drone.ThrustDown.canceled += ThrustDown_canceled;
        }

        private void ThrustUp_started(InputAction.CallbackContext obj) => _thrustingUp = true;
        private void ThrustUp_canceled(InputAction.CallbackContext obj) => _thrustingUp = false;
        private void ThrustDown_started(InputAction.CallbackContext obj) => _thrustingDown = true;
        private void ThrustDown_canceled(InputAction.CallbackContext obj) => _thrustingDown = false;

        private void EnterFlightMode(InteractableZone zone)
        {
            if (_inFlightMode != true && zone.GetZoneID() == 4) // drone Scene
            {
                _playerInput.Drone.Enable();
                _propAnim.SetTrigger("StartProps");
                _droneCam.Priority = 11;
                _inFlightMode = true;
                OnEnterFlightMode?.Invoke();
                UIManager.Instance.DroneView(true);
                _interactableZone.CompleteTask(4);
            }
        }

        private void ExitFlightMode()
        {           
            _playerInput.Drone.Disable();
            _droneCam.Priority = 9;
            _inFlightMode = false;
            UIManager.Instance.DroneView(false);            
        }

        private void Update()
        {
            if (_inFlightMode)
            {
                CalculateTilt();
                CalculateMovementUpdate();

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    _inFlightMode = false;
                    onExitFlightmode?.Invoke();
                    Debug.Log("Made it - Drone pressed escape ");
                    ExitFlightMode();
                }
            }
        }

        private void FixedUpdate()
        {
            _rigidbody.AddForce(transform.up * (9.81f), ForceMode.Acceleration);
            if (_inFlightMode)
                CalculateMovementFixedUpdate();
        }

        private void CalculateMovementUpdate()
        {
            var move = _playerInput.Drone.Movement.ReadValue<Vector2>();
            
            if (move.x < 0)
            {
                var tempRot = transform.localRotation.eulerAngles;
                tempRot.y -= _speed / 3;
                transform.localRotation = Quaternion.Euler(tempRot);
            }
            if (move.x > 0)
            {
                var tempRot = transform.localRotation.eulerAngles;
                tempRot.y += _speed / 3;
                transform.localRotation = Quaternion.Euler(tempRot);
            }
        }

        private void CalculateMovementFixedUpdate()
        {
            if (_thrustingUp)
            {
                _rigidbody.AddForce(transform.up * _speed, ForceMode.Acceleration);
            }
            if (_thrustingDown)
            {
                _rigidbody.AddForce(-transform.up * _speed, ForceMode.Acceleration);
            }
        }

        private void CalculateTilt()
        {
            var move = _playerInput.Drone.Movement.ReadValue<Vector2>();
            
            if (move.x < 0) 
                transform.rotation = Quaternion.Euler(00, transform.localRotation.eulerAngles.y, 30);
            else if (move.x > 0)
                transform.rotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, -30);
            else if (move.y > 0)
                transform.rotation = Quaternion.Euler(30, transform.localRotation.eulerAngles.y, 0);
            else if (move.y < 0)
                transform.rotation = Quaternion.Euler(-30, transform.localRotation.eulerAngles.y, 0);
            else 
                transform.rotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, 0);
        }

        private void OnDisable()
        {
            InteractableZone.onZoneInteractionComplete -= EnterFlightMode;
        }
    }
}
