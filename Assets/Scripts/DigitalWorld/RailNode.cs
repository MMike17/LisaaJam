using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SegmentPoint
{
    public Vector3 relativePosition;
    public RailNode left, straight, right;
}

public enum RailMovementDirection
{
    Forward = 0,
    Left = 1,
    Right = 2,
}

public enum RailMovementHeading
{
    North = 0,
    East = 90,
    South = 180,
    West = 270,
}

[SelectionBase]
public class RailNode : MonoBehaviour
{
    [SerializeField] private RailNode north, west, south, east;

    private RailNode left, right, forward;

    private RailMovementHeading prevHeading;
    private RailNode exit;
    private RailPlayer rider;

    public void Advance()
    {
        if (rider == null) return;
        if (exit == null) return;

        var currentPos = rider.transform.position;
        var exitPos = exit.transform.position;
        var closestPoint = currentPos.FindClosestPoint(transform.position, exitPos);

        var targetPos = Vector3.Distance(currentPos, closestPoint) > 0.2f ? closestPoint : exitPos;

        rider.transform.LookAt(targetPos);
        rider.transform.position = Vector3.MoveTowards(currentPos, targetPos, rider.baseSpeed * Time.deltaTime);
    }

    public RailNode Handoff(RailPlayer rider, RailMovementDirection direction)
    {
        if (exit == null) return null;
        var heading = GetHeading(prevHeading, direction);
        var newSegment = GetNode(heading, exit);

        if (!newSegment) return null;
        if (newSegment == this) return this;
        Debug.Log(newSegment.name);


        newSegment.Init(rider, heading, false);
        this.rider = null;

        return newSegment;
    }

    public void Init(RailPlayer rider, RailMovementHeading heading, bool snapToEntrance)
    {
        prevHeading = heading;
        this.rider = rider;
        exit = GetNode(heading, this);
        SetupExitTargets(heading);
        if (snapToEntrance) rider.transform.position = transform.position;
        if (exit != null) rider.transform.LookAt(exit.transform.position);
    }

    private static RailMovementHeading GetHeading(RailMovementHeading heading, RailMovementDirection direction)
    {
        var result = (int) heading;
        switch (direction)
        {
            case RailMovementDirection.Forward:
                // No changes
                break;
            case RailMovementDirection.Left:
                result -= 90;
                break;
            case RailMovementDirection.Right:
                result += 90;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }

        if (result < 0) result += 360;
        else if (result >= 360) result -= 360;

        return (RailMovementHeading) result;
    }

    private RailNode GetNode(RailMovementHeading heading, RailNode parent, int recursiveDepth = 0)
    {
        var result = heading switch
        {
            RailMovementHeading.North => parent.north,
            RailMovementHeading.West => parent.west,
            RailMovementHeading.South => parent.south,
            RailMovementHeading.East => parent.east,
            _ => throw new ArgumentOutOfRangeException(nameof(heading), heading, null)
        };

        if (result == null && recursiveDepth < 4)
            result = GetNode(GetHeading(heading, RailMovementDirection.Right), parent, ++recursiveDepth);

        return result;
    }

    private void SetupExitTargets(RailMovementHeading heading)
    {
        if (exit == null) return;
        switch (heading)
        {
            case RailMovementHeading.North:
                forward = exit.north;
                left = exit.west;
                right = exit.east;
                break;
            case RailMovementHeading.East:
                forward = exit.east;
                left = exit.north;
                right = exit.south;
                break;
            case RailMovementHeading.South:
                forward = exit.south;
                left = exit.east;
                right = exit.west;
                break;
            case RailMovementHeading.West:
                forward = exit.west;
                left = exit.south;
                right = exit.north;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(heading), heading, null);
        }
    }

    public float GetPositionPercent(Vector3 pos)
    {
        if (exit == null) return 1.0f;
        return pos.InverseLerp(transform.position, exit.transform.position);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        if (north != null) Gizmos.DrawSphere(north.transform.position, 0.3f);
        Gizmos.color = Color.red;
        if (south != null) Gizmos.DrawSphere(south.transform.position, 0.3f);
        Gizmos.color = Color.cyan;
        if (west != null) Gizmos.DrawSphere(west.transform.position, 0.3f);
        Gizmos.color = Color.magenta;
        if (east != null) Gizmos.DrawSphere(east.transform.position, 0.3f);
    }
}