using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//https://github.com/minhhh/unity-camera-follow-2d

#region Instructions
/*
    Target - The transform object to follow.
    Damp Time - The time it took for the camera to follow the target.Default to 0.15 (seconds).
    Follow Type - Which direction to follow the target: None, Horizontal, Vertical or Both.
    Camera Center X, Camera Center Y - The position in view port coordinates that the camera should track by.Default is 0.5, 0.5, which is the center of the camera.If you want the target to appear at one third on the left of the camera you can use 0.33, 0.5.
    Bound Type - Specify if the camera should be bound or not.It can be None - unbounded camera, Horizontal - Camera bounded left and right, Vertical - Camera bounded top and bottom, and Both - Camera bounded both horizontally and vertically.
    Left Bound, Right Bound - Horizontal bounds of the camera.This is the coordinate which the camera left edge and right edge should not pass, respectively.These are only activated if Bound Type is Horizontal or Both.
    Lower Bound, Upper Bound - Vertical bounds of the camera.This is the coordinate which the camera left edge and right edge should not pass, respectively.These are only activated if Bound Type is Vertical or Both.
    Dead Zone Type - Deadzone as in Mario camera. It can be None - no dead zone.Horizontal - horizontal dead zone which activates when moving left or right. Vertical - vertical deadzone which activates when moving up or down.
    Hard Dead Zone - If Hard Dead Zone is True, the camera moves like Mario, i.e, inside the dead zone it would not track the target's movement, only when going to the edge then the camera follows the target. If Hard Dead Zone is False, then even inside the dead zone the camera still follows the target normally. Default to False.
    Left Dead Bound - The number of unit in world coordinate to the left of the center of the camera that the target should not pass.
    Right Dead Bound - The number of unit in world coordinate to the right of the center of the camera that the target should not pass.
    Upper Dead Bound - The number of unit in world coordinate to the top of the center of the camera that the target should not pass.
    Lower Dead Bound - The number of unit in world coordinate to the bottom of the center of the camera that the target should not pass.
    The follow function itself uses Vector3.SmoothDamp function. This has some easing effect, thus performing better than the Vector3.Lerp linear function.

    Other notes
If your camera jitter then try setting the target's rigidbody Interpolate to Interpolate instead of None
If your camera still jitter then try reducing the speed.
*/
#endregion

public class CameraScript : MonoBehaviour
{
    [Flags]
    public enum Direction
    {
        None = 0,
        Horizontal = 1,
        Vertical = 2,
        Both = 3
    }

    public Transform target;
    public float dampTime = 0.15f;
    [EnumFlag("Follow type")] public Direction followType = Direction.Horizontal;
    [Range(0.0f, 1.0f)]
    public float
        cameraCenterX = 0.5f;
    [Range(0.0f, 1.0f)]
    public float
        cameraCenterY = 0.5f;
    [EnumFlag("Bound type")] public Direction boundType = Direction.None;
    public float leftBound = 0;
    public float rightBound = 0;
    public float upperBound = 0;
    public float lowerBound = 0;
    [EnumFlag("Deadzone type")] public Direction deadZoneType = Direction.None;
    public bool hardDeadZone = false;
    public float leftDeadBound = 0;
    public float rightDeadBound = 0;
    public float upperDeadBound = 0;
    public float lowerDeadBound = 0;

    // private
    Camera thisCamera;
    Vector3 velocity = Vector3.zero;
    float vertExtent;
    float horzExtent;
    Vector3 tempVec = Vector3.one;
    bool isBoundHorizontal;
    bool isBoundVertical;
    bool isFollowHorizontal;
    bool isFollowVertical;
    bool isDeadZoneHorizontal;
    bool isDeadZoneVertical;
    Vector3 deltaCenterVec;

    void Start()
    {
        thisCamera = GetComponent<Camera>();
        vertExtent = thisCamera.orthographicSize;
        horzExtent = vertExtent * Screen.width / Screen.height;
        deltaCenterVec = thisCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0))
            - thisCamera.ViewportToWorldPoint(new Vector3(cameraCenterX, cameraCenterY, 0));


        isFollowHorizontal = (followType & Direction.Horizontal) == Direction.Horizontal;
        isFollowVertical = (followType & Direction.Vertical) == Direction.Vertical;
        isBoundHorizontal = (boundType & Direction.Horizontal) == Direction.Horizontal;
        isBoundVertical = (boundType & Direction.Vertical) == Direction.Vertical;

        isDeadZoneHorizontal = ((deadZoneType & Direction.Horizontal) == Direction.Horizontal) && isFollowHorizontal;
        isDeadZoneVertical = ((deadZoneType & Direction.Vertical) == Direction.Vertical) && isFollowVertical;
        tempVec = Vector3.one;
    }

    void LateUpdate()
    {
        if (target)
        {
            Vector3 delta = target.position - thisCamera.ViewportToWorldPoint(new Vector3(cameraCenterX, cameraCenterY, 0));

            if (!isFollowHorizontal)
            {
                delta.x = 0;
            }
            if (!isFollowVertical)
            {
                delta.y = 0;
            }
            Vector3 destination = transform.position + delta;

            if (!hardDeadZone)
            {
                tempVec = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
            }
            else
            {
                tempVec.Set(transform.position.x, transform.position.y, transform.position.z);
            }

            if (isDeadZoneHorizontal)
            {
                if (delta.x > rightDeadBound)
                {
                    tempVec.x = target.position.x - rightDeadBound + deltaCenterVec.x;
                }
                if (delta.x < -leftDeadBound)
                {
                    tempVec.x = target.position.x + leftDeadBound + deltaCenterVec.x;
                }
            }
            if (isDeadZoneVertical)
            {
                if (delta.y > upperDeadBound)
                {
                    tempVec.y = target.position.y - upperDeadBound + deltaCenterVec.y;
                }
                if (delta.y < -lowerDeadBound)
                {
                    tempVec.y = target.position.y + lowerDeadBound + deltaCenterVec.y;
                }
            }

            if (isBoundHorizontal)
            {
                tempVec.x = Mathf.Clamp(tempVec.x, leftBound + horzExtent, rightBound - horzExtent);
            }

            if (isBoundVertical)
            {
                tempVec.y = Mathf.Clamp(tempVec.y, lowerBound + vertExtent, upperBound - vertExtent);
            }

            tempVec.z = transform.position.z;
            transform.position = tempVec;
        }
    }
}

