using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SegmentPoint
{
    public Vector3 relativePosition;
    public RailSegment left, straight, right;
}

public enum RailMovementDirection
{
    Straight = 0,
    Left = 1,
    Right = 2,
}

public class RailSegment : MonoBehaviour
{
    [SerializeField] private SegmentPoint[] points = new SegmentPoint[2];

    private Vector3 entrance, exit;
    private SegmentPoint exitPoint;
    private RailPlayer rider;

    private void Start()
    {
    }

    public void Advance()
    {
        if (rider == null) return;
        
        var currentPos = rider.transform.position;
        var closestPoint = currentPos.FindClosestPoint(entrance, exit);

        var targetPos= Vector3.Distance(currentPos, closestPoint) > 0.2f ? closestPoint : exit;

        rider.transform.LookAt(targetPos);
        rider.transform.position = Vector3.MoveTowards(currentPos, targetPos, rider.baseSpeed * Time.deltaTime);
    }

    public RailSegment Handoff(RailMovementDirection direction)
    {
        if (exitPoint == null) return null;
        var newSegment = exitPoint.straight;
        if ((exitPoint.left && (int) direction > 0) || !newSegment) newSegment = exitPoint.left;
        if ((exitPoint.right && direction == RailMovementDirection.Right) || !newSegment) newSegment = exitPoint.right;

        if (!newSegment) return null;
        
        newSegment.Init(rider, false);
        rider = null;

        return newSegment;
    }

    public void Init(RailPlayer rider, bool snapToEntrance)
    {
        this.rider = rider;
        SetMovingDirection();
        if (snapToEntrance) rider.transform.position = entrance;
        rider.transform.LookAt(exit);
    }

    public float GetPositionPercent()
    {
        return rider.transform.position.InverseLerp(entrance, exit);
    }

    private void SetMovingDirection()
    {
        var pos = rider.transform.position;
        entrance = FindPoint(pos, true).relativePosition + transform.position;
        exitPoint = FindPoint(pos, false);
        exit = exitPoint.relativePosition + transform.position;
    }

    private SegmentPoint FindPoint(Vector3 pos, bool closest)
    {
        var point0 = points[0].relativePosition + transform.position;
        var point1 = points[1].relativePosition + transform.position;
        var d0 = Vector3.Distance(pos, point0);
        var d1 = Vector3.Distance(pos, point1);

        var isPoint0Closest = d0 < d1;

        SegmentPoint result;

        if (closest)
        {
            result = isPoint0Closest ? points[0] : points[1];
        }
        else
        {
            result = isPoint0Closest ? points[1] : points[0];
        }

        return result;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        foreach (var point in points)
        {
            Gizmos.DrawSphere(transform.position + point.relativePosition, 0.2f);
        }
    }
}