using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class RobotGripperController : MonoBehaviour
{
    public List<GameObject> Grippers = new List<GameObject>();

    [Header("Object to pick")]
    [SerializeField] private GameObject target;
    [SerializeField] private float GripZoneLength = 0.05f;
    [SerializeField] private Transform grapPoint;

    [Header("Open Close Control")]

    [Range(0, 1)]
    [SerializeField] private float amountOpening;
    [Range(0, 1)]
    [SerializeField] private float goalClose;
    [SerializeField] private float timeBtwOpenClose = 2.0f;

    [HideInInspector]public List<RobotGripper> robotGrippers = new List<RobotGripper>();

    #region rotation state close open
    private Quaternion openInternalBoneR;
    private Quaternion closeInternalBonR;

    private Quaternion openFinalBoneR;
    private Quaternion closeFinalBoneR;

    private Quaternion openExternalBoneR;
    private Quaternion closeExternalBoneR;


    private Quaternion openInternalBoneL;
    private Quaternion closeInternalBoneL;

    private Quaternion openFinalBoneL;
    private Quaternion closeFinalBoneL;

    private Quaternion openExternalBoneL;
    private Quaternion closeExternalBoneL;
    #endregion
    [Header("Gripper States")]

    [SerializeField] private GripperStates currentGripperState = GripperStates.Open;

    [SerializeField]
    public GripperStates GripperState
    {
        set
        {
            currentGripperState = value;
            isChangingState = true;
        }
    }

    private bool isChangingState;
    private float timeCounter;
    private float timeProggres;
    private float horizontalGripLimit = 0.085f;
    //private float verticalGripLimit = 0.040f;


    private void Start()
    {
        foreach (var gripper in Grippers)
        {
            RobotGripper tempGripper = new RobotGripper(gripper);
            robotGrippers.Add(tempGripper);
        }

        InitBonePositions();
    }

    private void Update()
    {
        if (isChangingState)
        {
            if (currentGripperState == GripperStates.Close)
            {
                timeProggres += Time.deltaTime;
            }
            else
            {
                timeProggres -= Time.deltaTime;
            }

            AddProggres(timeProggres);
        }
    }
    private void InitBonePositions()
    {
        openInternalBoneR = robotGrippers[0].InternalBoneR.transform.localRotation;
        closeInternalBonR = Quaternion.Euler(openInternalBoneR.eulerAngles.x, openInternalBoneR.eulerAngles.y, openInternalBoneR.eulerAngles.z + 45);

        openFinalBoneR = robotGrippers[0].FinalBoneR.transform.localRotation;
        closeFinalBoneR = Quaternion.Euler(openFinalBoneR.eulerAngles.x, openFinalBoneR.eulerAngles.y, openFinalBoneR.eulerAngles.z - 45);

        openExternalBoneR = robotGrippers[0].ExternalBoneR.transform.localRotation;
        closeExternalBoneR = Quaternion.Euler(openExternalBoneR.eulerAngles.x, openExternalBoneR.eulerAngles.y, openExternalBoneR.eulerAngles.z + 45);

        openInternalBoneL = robotGrippers[0].InternalBoneL.transform.localRotation;
        closeInternalBoneL = Quaternion.Euler(openInternalBoneL.eulerAngles.x, openInternalBoneL.eulerAngles.y, openInternalBoneL.eulerAngles.z + 45);

        openFinalBoneL = robotGrippers[0].FinalBoneL.transform.localRotation;
        closeFinalBoneL = Quaternion.Euler(openFinalBoneL.eulerAngles.x, openFinalBoneL.eulerAngles.y, openFinalBoneL.eulerAngles.z - 45);

        openExternalBoneL = robotGrippers[0].ExternalBoneL.transform.localRotation;
        closeExternalBoneL = Quaternion.Euler(openExternalBoneL.eulerAngles.x, openExternalBoneL.eulerAngles.y, openExternalBoneL.eulerAngles.z + 45);
    }
    private void AddProggres(float val)
    {
        timeCounter = val;

        if (currentGripperState == GripperStates.Close)
        {
            if (timeCounter >= timeBtwOpenClose * goalClose)
            {
                isChangingState = false;
                timeCounter = timeBtwOpenClose * goalClose;

                Debug.Log(goalClose);
                grapPoint.localPosition = new Vector3(-(GripZoneLength / 2) / 100, 0, 0);
                GrapTarget();

            }
        }
        else
        {
            if (timeCounter <= 0)
            {
                isChangingState = false;
                timeCounter = 0;
            }
        }

        amountOpening = (timeCounter / timeBtwOpenClose);
        OpenCloseGripper(amountOpening);
    }

    private void OpenCloseGripper(float amount)
    {
        foreach (RobotGripper gripper in robotGrippers)
        {
            gripper.InternalBoneR.localRotation = Quaternion.Lerp(openInternalBoneR, closeInternalBonR, amount);
            gripper.FinalBoneR.localRotation = Quaternion.Lerp(openFinalBoneR, closeFinalBoneR, amount);
            gripper.ExternalBoneR.localRotation = Quaternion.Lerp(openExternalBoneR, closeExternalBoneR, amount);
            gripper.InternalBoneL.localRotation = Quaternion.Lerp(openInternalBoneL, closeInternalBoneL, amount);
            gripper.FinalBoneL.localRotation = Quaternion.Lerp(openFinalBoneL, closeFinalBoneL, amount);
            gripper.ExternalBoneL.localRotation = Quaternion.Lerp(openExternalBoneL, closeExternalBoneL, amount);
        }
    }


    public void OpenCloseGripper()
    {
        if (isChangingState)
            return;

        switch (currentGripperState)
        {
            case GripperStates.Open:
                CloseGripper();
                break;
            case GripperStates.Close:
                OpenGripper();
                break;
            default:
                break;
        }
    }

    public void OpenGripper()
    {
        GripperState = GripperStates.Open;
        RealeseTarget();
    }
    public void CloseGripper()
    {
        if (GripZoneLength > horizontalGripLimit)
        {
            Debug.LogError("The grip zone is very large, the maximum opening of the grip is: " + horizontalGripLimit);
            return;
        }
        goalClose = FindGoal();
        GripperState = GripperStates.Close;
    }

    private float FindGoal()
    {
        float result = (((GripZoneLength - 0.0879f))) / (-0.0848f);
        if (result > 1)
        {
            return 1;
        }
        return result;
    }

    private void GrapTarget()
    {

        float xOffSet = Mathf.Abs(grapPoint.position.x - target.transform.position.x);
        float xMin = (target.transform.localScale.x / 2);


        float yOffSet = Mathf.Abs(grapPoint.position.y - target.transform.position.y);
        float yMin = (target.transform.localScale.y);


        float zOffSet = Mathf.Abs(grapPoint.position.z - target.transform.position.z);
        float zMin = (target.transform.localScale.z / 2);

        if (xOffSet < xMin && yOffSet < yMin && zOffSet < zMin)
        {
            Debug.Log("I can grap");
            target.transform.parent = grapPoint;
            return;
        }

        Debug.Log("I Can nnot grap");

    }

    private void RealeseTarget()
    {
        if (target.transform.parent == null)
        {
            return;
        }

        target.transform.parent = null;
    }

}

[Serializable]
public class RobotGripper
{

    private readonly string[] bonnesNames = { "Bone.001 3", "Bone.002 1", "Bone 1", "Bone.001 1", "Bone.002", "Bone 3" };

    public List<Transform> bonnesTransform = new List<Transform>();
    public RobotGripper(GameObject gripper)
    {
        GetBonnes(gripper);
    }

    public Transform InternalBoneR { get => bonnesTransform[0]; set => bonnesTransform[0] = value; }
    public Transform FinalBoneR { get => bonnesTransform[1]; set => bonnesTransform[1] = value; }
    public Transform ExternalBoneR { get => bonnesTransform[2]; set => bonnesTransform[2] = value; }
    public Transform InternalBoneL { get => bonnesTransform[3]; set => bonnesTransform[3] = value; }
    public Transform FinalBoneL { get => bonnesTransform[4]; set => bonnesTransform[4] = value; }
    public Transform ExternalBoneL { get => bonnesTransform[5]; set => bonnesTransform[5] = value; }

    private void GetBonnes(GameObject gripper)
    {
        foreach (string bonne in bonnesNames)
        {
            GetChilds(gripper.transform, bonne);
        }
    }

    private void GetChilds(Transform Root, string bone)
    {
        foreach (Transform child in Root)
        {
            if (child.name == bone)
            {
                bonnesTransform.Add(child);
                break;
            }
            if (child.childCount > 0)
            {
                GetChilds(child, bone);
            }

        }
    }
}

public enum GripperStates
{
    Open,
    Close
}