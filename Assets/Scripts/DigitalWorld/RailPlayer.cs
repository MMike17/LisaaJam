using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailPlayer : MonoBehaviour
{
    private RailSegment currentSegment;
    public float baseSpeed = 2.0f;

    private RailMovementDirection cachedInput = RailMovementDirection.Straight;

    private RailSegment CurrentSegment
    {
        get => currentSegment;
        set
        {
            if (value == null) Debug.LogWarning($"Assigning null segment, previous segment: {currentSegment}");
            currentSegment = value;
        }
    }

    private void Start()
    {
        var entrance = FindObjectOfType<Entrance>();
        transform.position = entrance.transform.position;
        CurrentSegment = entrance.startingSegment;
        CurrentSegment.Init(this, true);
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
            cachedInput = RailMovementDirection.Straight;
        }
        
        if (CurrentSegment == null) return;
        CurrentSegment.Advance();
        if (CurrentSegment.GetPositionPercent() > 0.9f) CurrentSegment = CurrentSegment.Handoff(cachedInput);

    }
}
