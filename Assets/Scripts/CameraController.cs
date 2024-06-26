﻿using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [field:SerializeField] public ParticleSystem Confettis { get; set; }
    
    [SerializeField] private float _zoomSpeed;
    [SerializeField] private float _zoomMin;
    [SerializeField] private float _zoomMax;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private Vector2 _maxXPositions;
    [SerializeField] private Vector2 _maxZPositions;

    private float _currentZoom;
    private float _targetZoom;
    private Vector3 _referencePosition;

    private Vector3 _previousMousePosition;

    private void Awake()
    {
        _referencePosition = transform.localPosition;
    }

    private void Update()
    {
        HandleZoom();
        HandleMovement();
    }

    private void HandleZoom()
    {
        float zoomAmount = Input.mouseScrollDelta.y * _zoomSpeed * Time.deltaTime;
        _targetZoom = Mathf.Clamp(_targetZoom + zoomAmount, _zoomMin, _zoomMax);
        _currentZoom = Mathf.Lerp(_currentZoom, _targetZoom, 0.1f);
        transform.localPosition = _referencePosition + transform.forward * _currentZoom;
    }
    
    private void HandleMovement()
    {
        if (Input.GetMouseButton(2))
        {
            Vector3 positionDelta = (Input.mousePosition - _previousMousePosition);
            Vector3 moveDirection = new Vector3(-positionDelta.x, 0, -positionDelta.y);
            float zoomFactor = _currentZoom / _zoomMax;
            if (zoomFactor <= 1) zoomFactor = 1;
            Vector3 desiredPosition = _referencePosition + moveDirection * (_moveSpeed * Time.deltaTime * zoomFactor);

            if (desiredPosition.x < _maxXPositions.x || desiredPosition.x > _maxXPositions.y)
            {
                desiredPosition.x = _referencePosition.x;
            }
            if (desiredPosition.z < _maxZPositions.x || desiredPosition.z > _maxZPositions.y)
            {
                desiredPosition.z = _referencePosition.z;
            }
            
            _referencePosition = desiredPosition;
        }
        _previousMousePosition = Input.mousePosition;
    }
}