using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FittingPlacer;
using System;

public class RoomController : MonoBehaviour {

    // Data members

    [SerializeField]
    ///<summary>Reference to FittingPlacerForUnity component's GameObject</summary>
    public GameObject FittingPlacerForUnityReferenceGO;

    [SerializeField]
    public float Width = 5;

    [SerializeField]
    public float Depth = 4;

    [SerializeField]
    public float Height = 2.6f;

    [SerializeField]
    public bool DoPlaceFittings = true;

    [SerializeField]
    public bool PlaceFittingsUnstructuredRandomly = false;

    [SerializeField]
    [TextArea(3, 10)]
    public string NamesOfFittingModelsToBePlaced = "";
    private List<string> fittingModelsToBePlaced = new List<string>() { };

    private bool hasStartedFillingRoom = false;
    private FittingPlacerForUnity fittingPlacerForUnity;
    private Room roomToFill;


    // Methods

    // Use this for initialization
    void Start () {

        fittingPlacerForUnity = ((FittingPlacerForUnity)FittingPlacerForUnityReferenceGO.GetComponent<FittingPlacerForUnity>());

        // Read door information from door prefab transforms
        List<float[]> doorData = new List<float[]>();
        Transform doorsTransform = transform.Find("Doors");
        foreach (Transform doorTransform in doorsTransform)
        {
            // Door's x position, y position, breadth, inwards normal angle in radians from the x axis, height, & elevation in meters
            doorData.Add(new float[] {
                doorTransform.localPosition.x,
                doorTransform.localPosition.z,
                doorTransform.localScale.x,
                (((-doorTransform.localRotation.eulerAngles.y + 270) % 360) + 360) % 360 * Mathf.PI / 180,
                doorTransform.localScale.y,
                doorTransform.localPosition.y - doorTransform.localScale.y / 2
            });
        }

        // Read window information from window prefab transforms
        List<float[]> windowData = new List<float[]>();
        Transform windowsTransform = transform.Find("Windows");
        foreach (Transform windowTransform in windowsTransform)
        {
            // Window's x position, y position, breadth, inwards normal angle in radians from the x axis, height, & elevation in meters
            windowData.Add(new float[] {
                windowTransform.localPosition.x, 
                windowTransform.localPosition.z, 
                windowTransform.localScale.x, 
                (((-windowTransform.localRotation.eulerAngles.y + 270) % 360) + 360) % 360 * Mathf.PI / 180, 
                windowTransform.localScale.y,
                windowTransform.localPosition.y - windowTransform.localScale.y / 2f
            });
        }

        // Creating the room from:
        // the rectangular room dimensions (width, depth, height), 
        // the list of arrays for creating the doors, 
        // the list of arrays for creating the windows)
        roomToFill = new Room(
          Width,
          Depth,
          Height,
          doorData,
          windowData,
          fittingPlacerForUnity.GridCellSize
        );

        // Get the fittings to be placed
        if (!NamesOfFittingModelsToBePlaced.Equals(""))
        {
            // Split fitting models names string by each new line
            string[] modelsToPlace = NamesOfFittingModelsToBePlaced.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            // Unity adds a carridge return at end of multi-line, public fields strings
            // If that has not been changed in Unity, remove that ending char if found
            if (modelsToPlace[modelsToPlace.Length - 1][modelsToPlace[modelsToPlace.Length - 1].Length - 1] == '\r')
            {
                modelsToPlace[modelsToPlace.Length - 1] = 
                  modelsToPlace[modelsToPlace.Length - 1].Remove(modelsToPlace[modelsToPlace.Length - 1].Length - 1);
            }

            fittingModelsToBePlaced.AddRange(modelsToPlace);
        }
        else
        {
            fittingPlacerForUnity.AreFittingPlacementsFinished = true;
        }
    }

    // Update is called once per frame
    void Update () {
        if (DoPlaceFittings)
        {
            if (!hasStartedFillingRoom && fittingPlacerForUnity.AreSemanticsLoaded)
            {
                hasStartedFillingRoom = true;
                if (!PlaceFittingsUnstructuredRandomly)
                {
                    fittingPlacerForUnity.CalculateFittingPlacementsAsync(roomToFill, fittingModelsToBePlaced);
                }
                else
                {
                    fittingPlacerForUnity.CalculateRandomizedFittingPlacementsAsync(roomToFill, fittingModelsToBePlaced);
                }
            }
        }
        else
        {
            fittingPlacerForUnity.AreFittingPlacementsFinished = true;
        }
    }
}
