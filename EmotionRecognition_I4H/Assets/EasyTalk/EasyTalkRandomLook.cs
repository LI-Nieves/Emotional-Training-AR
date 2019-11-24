/*
 * This source code is (c)Copyright 2017 Realtime VR, LLC. All rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
 * */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[RequireComponent(typeof(Transform[]))]
[RequireComponent(typeof(Animator[]))]
public class EasyTalkRandomLook : MonoBehaviour {

    public Transform LookAtTransform;

    [Header("> Head:")]
    public Transform HeadBone;
    public Axis HeadAxisRight = Axis.PositiveX;
    public Axis HeadAxisUp = Axis.PositiveY;
    public Axis HeadAxisForward = Axis.PositiveZ;
    public Vector3 HeadRotationDefault = new Vector3(0, 0, 0);

    [Header("> Left Eye:")]
    public Transform LeftEyeBone;
    public Axis LeftEyeAxisRight = Axis.PositiveX;
    public Axis LeftEyeAxisUp = Axis.PositiveY;
    public Axis LeftEyeAxisForward = Axis.PositiveZ;
    public Vector3 LeftEyeRotationDefault = new Vector3(0, 0, 0);
    
    [Header("> Right Eye:")]
    public Transform RightEyeBone;
    public Axis RightEyeAxisRight = Axis.PositiveX;
    public Axis RightEyeAxisUp = Axis.PositiveY;
    public Axis RightEyeAxisForward = Axis.PositiveZ;
    public Vector3 RightEyeRotationDefault = new Vector3(0, 0, 0);

    public enum Axis
    {
        PositiveX, PositiveY, PositiveZ, NegativeX, NegativeY, NegativeZ
    }

    private float _HeadRotationAngleLimit = 70;
    private float _EyeRotationAngleimit = 15;

    private Animator _Animator;

    private float _HeadLookAtTransitionTime = 0.0f;
    private float _HeadLookAtStartTime = 0.0f;
    private float _HeadLookAtEndTime = 0.0f;
    private float _LevelOfLookAtTransition = 0.0f;
    private int _Phase0to3 = 0; // 0 is NeutralIdleTimeLookingAway, 1 is LookAt, 2 is NeutralIdleTimeLookingAt, 3 is LookAway, GoBackTo 0

    public enum HeadMotionEnum
    {
        None,
        RandomLook,
        LookAt,
        LookAway
    }
    public enum HeadTiltEnum
    {
        None,
        Left,
        Right
    }

    void Start()
    {
        _Animator = GetComponent<Animator>();
    }

    private Vector3 GetTrueAxis(Transform transform, Axis axis)
    {
        if (axis == Axis.PositiveX)
            return transform.right;
        if (axis == Axis.NegativeX)
            return -transform.right;
        if (axis == Axis.PositiveY)
            return transform.up;
        if (axis == Axis.NegativeY)
            return -transform.up;
        if (axis == Axis.PositiveZ)
            return transform.forward;
        if (axis == Axis.NegativeZ)
            return -transform.forward;

        return transform.forward;
    }

    private float LinearInterpolation(float x1, float y1, float x2, float y2, float x)
    {
        return y1 + (((x - x1) * (y2 - y1)) / (x2 - x1));
    }

    private Transform LookAt(Transform transformToRotate, Transform transformToLookAt, Axis transformToRotateForwardAxis, float level, float angleLimit)
    {
        Vector3 forwardDirection = GetTrueAxis(transformToRotate, transformToRotateForwardAxis);
        Vector3 desiredDirection = (transformToLookAt.position - transformToRotate.position);
        Vector3 rotationAxis = Vector3.Cross(forwardDirection, desiredDirection);

        //Debug.DrawLine(transformToRotate.position, rotationAxis * 50, Color.cyan);
        //Debug.DrawLine(transformToRotate.position, forwardDirection * 50, Color.blue);
        //Debug.DrawLine(transformToRotate.position, desiredDirection * 50, Color.white);

        // Basic limiter method. Could limit by yaw and pitch in a future version.
        float angle = Mathf.Acos(Vector3.Dot(forwardDirection.normalized, desiredDirection.normalized)) * Mathf.Rad2Deg;
        if (angle > angleLimit)
        {
            angle = LinearInterpolation(angleLimit, angleLimit, 180, 0, angle);
        }
        angle *= level;
        transformToRotate.Rotate(rotationAxis, angle, Space.World);
        
        return transformToRotate;
    }

    private void Play(HeadMotionEnum motion)
    {
        // We will have a half second transition to the new motion
        if (_HeadLookAtEndTime > 0.5f)
        {
            // Cut the time short and start new for sake of elect
            _HeadLookAtEndTime = 0.5f;
        }
        _HeadLookAtEndTime = 0.5f;

        if (motion == HeadMotionEnum.LookAt)
        {
            _Phase0to3 = 1 - 1;
        }
        else if (motion == HeadMotionEnum.LookAway)
        {
            _Phase0to3 = 3 - 1;
        }
    }

    void LateUpdate()
    {
        // We require an animator but if no controller is assigned, we need to zero out the angles of
        // the head and eye's before we rotate them.
        if (_Animator.runtimeAnimatorController == null)
        {
            // Zero out transforms
            HeadBone.localEulerAngles = HeadRotationDefault;
            LeftEyeBone.localEulerAngles = LeftEyeRotationDefault;
            RightEyeBone.localEulerAngles = RightEyeRotationDefault;
        }

        // Need a random amount of time between transitions. We also need a random 
        if (Time.fixedTime >= _HeadLookAtEndTime)
        {
            _Phase0to3++;

            if (_Phase0to3 == 4)
                _Phase0to3 = 0;
            float headTransitionTime = 0.0f;

            // 0 is NeutralIdleTimeLookingAway, 1 is LookAt, 2 is NeutralIdleTimeLookingAt, 3 is LookAway, GoBackTo 0
            if (_Phase0to3 == 0 || _Phase0to3 == 2)
            {
                //headTransitionTime = Random.Range(2, 6);
                headTransitionTime = Random.Range(2, 6);
            }
            else if (_Phase0to3 == 1)
            {
                //headTransitionTime = _HeadLookAtTransitionTime = Random.Range(1.0f, 1.5f);
                headTransitionTime = _HeadLookAtTransitionTime = Random.Range(0.5f, 1.0f);
            }
            else if (_Phase0to3 == 3)
            {
                //headTransitionTime = _HeadLookAtTransitionTime = Random.Range(1.1f, 2.1f);
                headTransitionTime = _HeadLookAtTransitionTime = Random.Range(0.5f, 1.5f);
            }

            _HeadLookAtStartTime = Time.fixedTime;
            _HeadLookAtEndTime = _HeadLookAtStartTime + headTransitionTime;
        }

        // maxHeadMovement is between 0 and 1 and allows for a more realistic head motion with affecting max pitch and yaw.
        // For example, a value of 0.5 will allow 50% original animation head movement, and 50% lookat. LookAt is a 
        // complete lookat head turn while the body moves as the original animation. It's a direct on stare and that's
        // un-natural. A good value seems to be 0.5
        float maxHeadMovement = 0.65f;
        if (_Phase0to3 == 1)
        {
            _LevelOfLookAtTransition = LinearInterpolation(_HeadLookAtStartTime, 0.0f, _HeadLookAtEndTime, maxHeadMovement, Time.fixedTime);
        }
        else if (_Phase0to3 == 3)
        {
            _LevelOfLookAtTransition = LinearInterpolation(_HeadLookAtStartTime, maxHeadMovement, _HeadLookAtEndTime, 0.0f, Time.fixedTime);
        }

        // DEBUG: _LevelOfLookAtTransition = 1;
        HeadBone = LookAtRotate(HeadBone, LookAtTransform, HeadAxisRight, HeadAxisUp, HeadAxisForward, _LevelOfLookAtTransition, _HeadRotationAngleLimit);

        if (_Phase0to3 == 2)
        {

            if ((int)(Time.fixedTime) % 2 == 0)
            {
                RightEyeBone = LookAtRotate(RightEyeBone, LookAtTransform, RightEyeAxisRight, RightEyeAxisUp, RightEyeAxisForward, 1.0f, _EyeRotationAngleimit);
                LeftEyeBone = LookAtRotate(LeftEyeBone, LookAtTransform, LeftEyeAxisRight, LeftEyeAxisUp, LeftEyeAxisForward, 1.0f, _EyeRotationAngleimit);
            }
            else
            {
                // Does this ever get called?
                LeftEyeBone.localEulerAngles = LeftEyeRotationDefault;
                RightEyeBone.localEulerAngles = RightEyeRotationDefault;
            }
        }
    }
    
    private Transform LookAtRotate(Transform bone, Transform lookAtTarget, Axis axisRight, Axis axisUp, Axis axisForward, float level, float angleLimit)
    {
        LookAt(bone, lookAtTarget, axisForward, level, angleLimit);
        return bone;
    }
}
