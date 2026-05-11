using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }
    public float LeftTriggerAxis { get; private set; }
    public float RightTriggerAxis { get; private set; }
    public bool IsAccelPressed { get; private set; }
    public bool IsFlyPressed { get; private set; }
    public bool IsDriftPressed { get; private set; }
    public bool IsDriftPressedDown { get; private set; }
    public bool IsDriftReleased { get; private set; }
    public bool IsItemPressed { get; private set; }

    public void UpdateInput(float horizontal, float vertical, float leftTrigger, float rightTrigger, float accel, float fly, float drift, float item)
    {
        Horizontal = horizontal;
        Vertical = vertical;
        LeftTriggerAxis = leftTrigger;
        RightTriggerAxis = rightTrigger;
        IsAccelPressed = accel != 0f;
        IsFlyPressed = fly != 0f;
        IsItemPressed = item != 0f;

        // ドリフトボタンのDown/Upを判定
        IsDriftPressed = drift != 0f;
        IsDriftPressedDown = drift != 0f;
        IsDriftReleased = !(drift != 0f);
    }
}
