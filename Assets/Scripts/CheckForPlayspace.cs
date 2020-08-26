using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;

public class CheckForPlayspace : MonoBehaviour
{
    public GameObject tower; // main game arena
    public GameObject placementIndicator; // indicator for where you can place the tower
    public GameObject debugFloor; // fake floor for testing placement in editor

    public GameObject ARObjects; // for disabling during testing

    public GameObject light1; // disable in AR
    public GameObject light2;

    ARRaycastManager rayManager;
    Pose placementPose;

    bool placementPoseIsValid = false;
    bool towerPlaced = false;

    public LayerMask layerMask;

    void Start()
    {
        rayManager = FindObjectOfType<ARRaycastManager>();

        if (!Application.isEditor) //hardware testing
        {
            tower.SetActive(false);
            debugFloor.SetActive(false);
            ARObjects.SetActive(true);
            light1.SetActive(false);
            light2.SetActive(false);
        }
        else if (GameOptions.Instance.TestARInEditor) //testing AR placement in editor
        {
            tower.SetActive(false);
            debugFloor.SetActive(true);
            ARObjects.SetActive(true);
        }
        else //test gameplay without AR
        {
            ARObjects.SetActive(false);
        }
    }

    void Update()
    {
        if (!towerPlaced)
        {
            UpdatePlacementPose();
            UpdatePlacementIndicator();
        }

        if (!towerPlaced && placementPoseIsValid)
        {
            if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetKeyDown(KeyCode.Return))
                SetTower();
        }
    }

    // Set tower's position and rotation then activate
    private void SetTower()
    {
        tower.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        tower.SetActive(true);
        towerPlaced = true;
    }

    // Move around placement indicator, set inactive if play area is invalid
    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    // Checks for if you're looking at a valid play area
    private void UpdatePlacementPose()
    {
        if (Application.isEditor)
        {
            Ray r = Camera.main.ScreenPointToRay(new Vector3(Camera.main.pixelWidth * 0.5f, Camera.main.pixelHeight * 0.5f, 0));

            placementPoseIsValid = Physics.Raycast(r, out RaycastHit hit, 500, layerMask);

            if (placementPoseIsValid)
            {
                Vector3 cameraForward = Camera.main.transform.forward;
                Vector3 cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
                placementPose.position = hit.point;
                placementPose.rotation = Quaternion.LookRotation(cameraBearing);
            }
        }
        else
        {
            Vector3 screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            rayManager.Raycast(screenCenter, hits, TrackableType.All);

            placementPoseIsValid = hits.Count > 0;

            if (placementPoseIsValid)
            {
                placementPose = hits[0].pose;

                Vector3 cameraForward = Camera.main.transform.forward;
                Vector3 cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
                placementPose.rotation = Quaternion.LookRotation(cameraBearing);
            }
        }
    }
}