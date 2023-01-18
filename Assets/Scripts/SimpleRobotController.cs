using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
public class SimpleRobotController : MonoBehaviour
{
    [Header("Controllers")]
    [SerializeField] private InputActionAsset RobotControllers;
    [SerializeField] private InputController inputController = InputController.Keyboard;
    [SerializeField] private float jointSpeed;
    [SerializeField] private InputActionProperty selectJointInput;
    [SerializeField] private InputActionProperty moveJointInput;
    [Header("Robot Description")]
    [SerializeField] private Transform[] joints = new Transform[6];
    [SerializeField] private RobotJoint[] targetJointAngles = new RobotJoint[6];

    public enum InputController { Keyboard, VRControllers, Gamepad }

    private float[] homePosition = { 90.0f, -90.0f, 90.0f, 180.0f, -90.0f, 0.0f };
    private int selectedIndex;
    private enum MoveDirection
    {
        None,
        Positive,
        Negative
    }

    #region Only for editor
    private void OnValidate()
    {
        if (targetJointAngles[5].Name != "")
            return;

        for (int i = 0; i < targetJointAngles.Length; i++)
        {
            if (joints[i] != null)
            {
                targetJointAngles[i].Name = joints[i].name;
            }

        }
    }
    #endregion

    private void Awake()
    {
        SelectRobotController(inputController);
    }
    void Start()
    {
        MoveRobotToHomePosition();
        selectJointInput.action.Enable();
        moveJointInput.action.Enable();
        SetSelectedJointIndex(0);
    }

    public void Update()
    {
        MoveRobot();
        joints[0].localEulerAngles = new Vector3(0, 0 - targetJointAngles[0].JointAngle, -90);
        joints[1].localEulerAngles = new Vector3(0, -90 - targetJointAngles[1].JointAngle, 0);
        joints[2].localEulerAngles = new Vector3(0, 0 - targetJointAngles[2].JointAngle, 0);
        joints[3].localEulerAngles = new Vector3(0, -90 - targetJointAngles[3].JointAngle, 0);
        joints[4].localEulerAngles = new Vector3(targetJointAngles[4].JointAngle, 0, -0);
        joints[5].localEulerAngles = new Vector3(0, -targetJointAngles[5].JointAngle, 0);
    }


    public void MoveRobotToHomePosition()
    {
        for (int i = 0; i < targetJointAngles.Length; i++)
        {
            targetJointAngles[i].JointAngle = homePosition[i];
        }
        joints[0].localEulerAngles = new Vector3(0, 0 - homePosition[0], -90);
        joints[1].localEulerAngles = new Vector3(0, -90 - homePosition[1], 0);
        joints[2].localEulerAngles = new Vector3(0, 0 - homePosition[2], 0);
        joints[3].localEulerAngles = new Vector3(0, -90 - homePosition[3], 0);
        joints[4].localEulerAngles = new Vector3(homePosition[4], 0, -0);
        joints[5].localEulerAngles = new Vector3(0, -homePosition[5], 0);


    }

    private void MoveRobot()
    {

        if (selectJointInput.action.triggered)
        {
            Vector2 input = selectJointInput.action.ReadValue<Vector2>();

            if (input.y > 0)
            {
                SetSelectedJointIndex(selectedIndex + 1);
            }
            else
            {
                SetSelectedJointIndex(selectedIndex - 1);

            }


        }

        UpdateMoveDirection();


    }

    private void UpdateMoveDirection()
    {
        Vector2 inputValue = moveJointInput.action.ReadValue<Vector2>();
        MoveDirection moveDirection = MoveDirection.None;

        if (inputValue.x > 0)
        {
            moveDirection = MoveDirection.Positive;
        }
        else if (inputValue.x < 0)
        {
            moveDirection = MoveDirection.Negative;
        }

        switch (moveDirection)
        {
            case MoveDirection.None:
                SetJointPosition(0);
                break;
            case MoveDirection.Positive:
                SetJointPosition(1);
                break;
            case MoveDirection.Negative:
                SetJointPosition(-1);
                break;
            default:
                break;
        }
    }

    private void SetJointPosition(int direction)
    {
        targetJointAngles[selectedIndex].JointAngle += direction * jointSpeed;
    }

    private void SetSelectedJointIndex(int index)
    {
        int currentIndex = selectedIndex;
        if (joints.Length > 0)
        {
            selectedIndex = (index + joints.Length) % joints.Length;
        }

        HighLightJoint(selectedIndex, currentIndex);
    }

    private void HighLightJoint(int newIndex, int currentIndex) 
    {
        joints[currentIndex].GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(false);
        joints[newIndex].GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(true);

    }

    private void SelectRobotController(InputController inputController)
    {
        switch (inputController)
        {
            case InputController.Keyboard:
                FindControllerScheme(inputController.ToString());
                break;
            case InputController.VRControllers:
                FindControllerScheme(inputController.ToString());
                break;
            case InputController.Gamepad:
                FindControllerScheme(inputController.ToString());
                break;
            default:
                break;
        }
    }

    private void FindControllerScheme(string scheme)
    {

        RobotControllers.bindingMask = InputBinding.MaskByGroup(scheme);
    }
}

[Serializable]
public class RobotJoint
{
    [HideInInspector] public string Name;
    public float JointAngle;
}