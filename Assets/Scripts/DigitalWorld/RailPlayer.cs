using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailPlayer : MonoBehaviour
{
    [SerializeField] private RailNode _currentNode;
    public float baseSpeed = 2.0f;

    [SerializeField] private RailMovementDirection cachedInput = RailMovementDirection.Forward;

    private RailNode CurrentNode
    {
        get => _currentNode;
        set
        {
            if (value == null) Debug.LogWarning($"Assigning null segment, previous segment: {_currentNode}");
            _currentNode = value;
        }
    }

    private void Start()
    {
        var entrance = FindObjectOfType<Entrance>();
        transform.position = entrance.transform.position;
        CurrentNode = entrance.startingNode;
        CurrentNode.Init(this, RailMovementHeading.North, true);
    }

    private void Update()
    {
        if (Input.GetAxis("Horizontal") < 0)
        {
            cachedInput = RailMovementDirection.Left;
        }
        if (Input.GetAxis("Horizontal") > 0)
        {
            cachedInput = RailMovementDirection.Right;
        }
        if (Input.GetAxis("Vertical") > 0)
        {
            cachedInput = RailMovementDirection.Forward;
        }
        
        if (CurrentNode == null) return;
        CurrentNode.Advance();
        if (CurrentNode.GetPositionPercent(transform.position) > 0.9f)
        {
            CurrentNode = CurrentNode.Handoff(this, cachedInput);
            cachedInput = RailMovementDirection.Forward;
        }

    }
}
