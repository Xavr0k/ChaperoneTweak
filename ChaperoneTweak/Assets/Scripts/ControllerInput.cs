using UnityEngine;
using System.Collections;
using Valve.VR;

enum ChaperoneEditMode { wall, playSpace };
//enum TweakActionType { wHeight, wCornerPosition, wAdd, wRemove, psEdge, psHeight, psCorner, psMove, save}

public class ControllerInput : MonoBehaviour
{
    public GameObject HandCamera;

    public GameObject SelectionCircle;

    private bool IsEnabled = true;

    public Camera UICam;
    public GameObject Menu;
    public Transform Head;
    public bool InUse;
    public ControllerInput OtherControllerInput;

    private ChaperoneEditMode EditMode;
    public ChaperoneElements ChapElements;
    public GameObject origin;
    public LaserPointer Laser;

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }

    public GameObject testing;

    private bool GripAction = false;
    private bool TriggerAction = false;
    private bool MenuAction = false;

    public Material  PlaySpaceMatEdit, WallMatEdit, PlaySpaceMatTrans, WallMatTrans;

    public void SetEnabled(bool isenabled)
    {
        IsEnabled = isenabled;
            Laser.SetEnabled(isenabled);
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        EditMode = ChaperoneEditMode.wall;
        Laser.Mask = 1 << 8;
        InUse = false;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Collider[] collisions;
        Vector3 tempv1;
        TweakActionArea hitZone;

        if ((controller == null) || (!IsEnabled))
        {
            return;
        }

        if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            HandCamera.SetActive(!HandCamera.activeInHierarchy);
        }

            if (!MenuAction && !TriggerAction && !GripAction)
        {

            if (Laser.Target != null)
            {
                // Get the hit area
                tempv1 = Laser.Target.transform.InverseTransformPoint(Laser.TargetPoint);
                if (EditMode == ChaperoneEditMode.wall)
                {
                    if (tempv1.y >= 0.75)
                    {
                        if (tempv1.x <= -0.25) { hitZone = TweakActionArea.topLeft; }
                        else if (tempv1.x >= 0.25) { hitZone = TweakActionArea.topRight; }
                        else { hitZone = TweakActionArea.top; }
                    }
                    else if (tempv1.y <= 0.25)
                    {
                        if (tempv1.x <= -0.25) { hitZone = TweakActionArea.bottomLeft; }
                        else if (tempv1.x >= 0.25) { hitZone = TweakActionArea.bottomRight; }
                        else { hitZone = TweakActionArea.bottom; }
                    }
                    else
                    {
                        if (tempv1.x <= -0.25) { hitZone = TweakActionArea.left; }
                        else if (tempv1.x >= 0.25) { hitZone = TweakActionArea.right; }
                        else { hitZone = TweakActionArea.middle; }
                    }
                }
                else
                {
                    if (tempv1.z >= 0.25)
                    {
                        if (tempv1.x <= -0.25) { hitZone = TweakActionArea.topLeft; }
                        else if (tempv1.x >= 0.25) { hitZone = TweakActionArea.topRight; }
                        else { hitZone = TweakActionArea.top; }
                    }
                    else if (tempv1.z <= -0.25)
                    {
                        if (tempv1.x <= -0.25) { hitZone = TweakActionArea.bottomLeft; }
                        else if (tempv1.x >= 0.25) { hitZone = TweakActionArea.bottomRight; }
                        else { hitZone = TweakActionArea.bottom; }
                    }
                    else
                    {
                        if (tempv1.x <= -0.25) { hitZone = TweakActionArea.left; }
                        else if (tempv1.x >= 0.25) { hitZone = TweakActionArea.right; }
                        else { hitZone = TweakActionArea.middle; }
                    }
                } // get the hit area
            }
            else
            {
                hitZone = TweakActionArea.none;
            }
            // grip down
            if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
            {
                if (EditMode == ChaperoneEditMode.wall)
                {
                    if ((hitZone == TweakActionArea.topLeft) || (hitZone == TweakActionArea.top) || (hitZone == TweakActionArea.topRight))
                    {
                        // grip wall toprow
                        if (ChapElements.StartWallHeightAdjust(transform))
                        {
                            GripAction = true;
                            OtherControllerInput.SetEnabled(false);
                        }
                    }
                    else if (hitZone != TweakActionArea.none)
                    {
                        // grip wall toprow/midrow
                        if (ChapElements.StartWallEdgeAdjust(transform, Laser.Target, Laser.TargetPoint))
                        {
                            GripAction = true;
                            OtherControllerInput.SetEnabled(false);
                        }
                    }
                }
                else
                {
                    if ((hitZone == TweakActionArea.topLeft) || (hitZone == TweakActionArea.topRight) || (hitZone == TweakActionArea.bottomLeft) || (hitZone == TweakActionArea.bottomRight))
                    {
                        // grip playspace corner
                        if (ChapElements.StartPlaySpaceCornerPivot(transform, hitZone))
                        {
                            GripAction = true;
                            OtherControllerInput.SetEnabled(false);
                        }
                    }
                    else if ((hitZone == TweakActionArea.top) || (hitZone == TweakActionArea.left) || (hitZone == TweakActionArea.bottom) || (hitZone == TweakActionArea.right))
                    {
                        // grip playspace edge
                        if (ChapElements.StartPlaySpaceEdgeResize(transform, hitZone))
                        {
                            GripAction = true;
                            OtherControllerInput.SetEnabled(false);
                        }
                    }
                    else if (hitZone == TweakActionArea.middle)
                    {
                        // grip playspace middle
                        if (ChapElements.StartPlaySpaceMove(transform))
                        {
                            GripAction = true;
                            OtherControllerInput.SetEnabled(false);
                        }
                    }
                }
            }
            else if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                // trigger pressed
                if (EditMode == ChaperoneEditMode.wall)
                {
                    if ((hitZone == TweakActionArea.top) || (hitZone == TweakActionArea.middle) || (hitZone == TweakActionArea.bottom))
                    {
                        // trigger wall midcolumn
                        if (ChapElements.SplitWallSegment(transform, Laser.Target, Laser.TargetPoint))
                        {
                            TriggerAction = true;
                            OtherControllerInput.SetEnabled(false);
                        }
                    }
                    else if (hitZone != TweakActionArea.none)
                    {
                        // trigger wall leftcolumn/rightcolumn
                        if (ChapElements.DeleteWallSegment(Laser.Target, Laser.TargetPoint))
                        {
                            TriggerAction = true;
                            OtherControllerInput.SetEnabled(false);
                        }
                    }
                }
                else
                {
                    if (hitZone == TweakActionArea.middle)
                    {
                        // trigger playspace middle
                        if (ChapElements.StartPlaySpaceHeightAdjust(transform))
                        {
                            TriggerAction = true;
                            OtherControllerInput.SetEnabled(false);
                        }
                    }
                    else if ((hitZone == TweakActionArea.left) || (hitZone == TweakActionArea.right) || (hitZone == TweakActionArea.bottom))
                    {
                        if (ChapElements.SetFront(hitZone))
                        {
                            TriggerAction = true;
                            OtherControllerInput.SetEnabled(false);
                        }
                    }
                    else if ((hitZone == TweakActionArea.topLeft) || (hitZone == TweakActionArea.topRight) || (hitZone == TweakActionArea.bottomLeft) || (hitZone == TweakActionArea.bottomRight))
                    {
                        if (ChapElements.StartPlaySpaceCentrePivot(transform, hitZone))
                        {
                            TriggerAction = true;
                            OtherControllerInput.SetEnabled(false);
                        }
                    }
                }
            }
            else if (controller.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
            {
                // menu pressed
                OtherControllerInput.SetEnabled(false);
                Laser.SetEnabled(false);
                UICam.enabled = true;
                MenuAction = true;
                Menu.SetActive(true);
                Menu.transform.position = transform.position;
                Menu.transform.rotation = Quaternion.LookRotation(transform.TransformPoint(0, -0.034f, 0.015f) - Head.position, Vector3.up);
            }
        }

        if (controller.GetPressUp(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            if (MenuAction)
            {
                collisions = Physics.OverlapSphere(transform.TransformPoint(0, -0.034f, 0.015f), 0.1f, 1 << 5);
                if (collisions.Length > 0)
                {
                    if (collisions[0].name == "Walls")
                    {
                        EditMode = ChaperoneEditMode.wall;
                        Laser.Mask = 1 << 8;
                        OtherControllerInput.EditMode = ChaperoneEditMode.wall;
                        OtherControllerInput.Laser.Mask = 1 << 8;
                        ChapElements.SetMaterials(WallMatEdit, PlaySpaceMatTrans);
                    }
                    else if (collisions[0].name == "PlaySpace")
                    {
                        EditMode = ChaperoneEditMode.playSpace;
                        Laser.Mask = 1 << 9;
                        OtherControllerInput.EditMode = ChaperoneEditMode.playSpace;
                        OtherControllerInput.Laser.Mask = 1 << 9;
                        ChapElements.SetMaterials(WallMatTrans, PlaySpaceMatEdit);
                    }
                    else if (collisions[0].name == "Reload")
                    {
                        ChapElements.ReloadChaperone();
                    }
                    else if (collisions[0].name == "Save")
                    {
                        ChapElements.SaveChaperone();
                    }
                }



                OtherControllerInput.SetEnabled(true);
                Laser.SetEnabled(true);
                UICam.enabled = false;
                MenuAction = false;
                Menu.SetActive(false);
            }
        }

        if (controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
        {
            // grip released
            if (GripAction)
            {
                ChapElements.EndAction();
                GripAction = false;
                OtherControllerInput.SetEnabled(true);
            }
        }

        if (controller.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            // trigger released
            if (TriggerAction)
            {
                ChapElements.EndAction();
                TriggerAction = false;
                OtherControllerInput.SetEnabled(true);
            }
        }

        //////////////////////////////////////////////
        if (MenuAction)
        {
            collisions = Physics.OverlapSphere(transform.TransformPoint(0, -0.034f, 0.015f), 0.1f, 1 << 5);
            if (collisions.Length > 0)
            {
                SelectionCircle.transform.parent = collisions[0].transform;
                SelectionCircle.transform.localScale = Vector3.one;
                SelectionCircle.transform.localRotation = Quaternion.identity;
                SelectionCircle.transform.localPosition = Vector3.zero;
                SelectionCircle.SetActive(true);
            }
            else
            {
                SelectionCircle.SetActive(false);
            }
        }
    }
}
