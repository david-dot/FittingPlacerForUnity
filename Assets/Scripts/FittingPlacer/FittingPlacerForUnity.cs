using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FittingPlacer;
using System.Threading.Tasks;

public class FittingPlacerForUnity : MonoBehaviour
{
    // Data members

    ///<summary>Instance of furniture layout algorithm solver</summary>
    private Furnisher furnisher;

    ///<summary>Placement information for fittings</summary>
    private FittingPlacement[] fittingPlacements;

    ///<summary>GameObject to place fitting GameObjects relative to, as children</summary>
    [SerializeField]
    public GameObject OriginAnchor;

    ///<summary>Grid cell size for placements in meters</summary>
    [SerializeField]
    public float GridCellSize = 0.1f;

    [SerializeField]
    public bool DoSavePlacementsToFile = false;

    [SerializeField]
    public string PlacementInfoSaveFolderPath = "SavedLayouts";
    private string placementInfoSaveFilePath;

    // Dictionary for saving references to fitting representations

    [System.Serializable]
    public class RepresentationEntry
    {
        // Key constituents
        public string fittingModel;
        public string fittingType;

        // Value
        public GameObject representationPrefab;
    }

    [SerializeField]
    public RepresentationEntry[] FittingRepresentations;

    [HideInInspector]
    public Dictionary<string, GameObject > FittingRepresentationsDic = new Dictionary<string, GameObject >();

    // State booleans
    public bool AreSemanticsLoaded { get; private set; } = false;
    private bool areFittingsPlaced = false;
    [HideInInspector]
    public bool AreFittingPlacementsFinished = false;

    // Test condition variables
    private Room testRoom = new Room(
      6f,
      4f,
      2.6f,
      new List<float[]>()
      {
                new float[] {3f, -1f, 1f, Mathf.PI}
      },
      new List<float[]>()
      {
                new float[] {1f, 2f, 1f, Mathf.PI/2*3}
      }
    );

    private List<string> fittingModelsToBePlacedTest = new List<string>()
    {
        "SVIS burlap two-seater sofa",
        "metal floor lamp with SVIS lamp fixture",
        "SVIS coffee table",
        "SVIS flatscreen TV on TV stand"
    };


    // Methods

    // Use this for initialization
    void Start ()
    {
        // Make sure Origin anchor is set
        if (OriginAnchor == null)
        {
            OriginAnchor = this.gameObject;
        }

        // Add fitting prefab representations from inspector array to sorted dictionary for faster access speed
        foreach (RepresentationEntry entry in FittingRepresentations)
        {
            FittingRepresentationsDic.Add(entry.fittingType + ":" + entry.fittingModel, entry.representationPrefab);
        }

        if (DoSavePlacementsToFile)
        {
            // Set fitting layout save path
            if (!System.IO.Directory.Exists(PlacementInfoSaveFolderPath))
            {
                System.IO.Directory.CreateDirectory(PlacementInfoSaveFolderPath);
            }
            int savesCount = 1;
            do
            {
                placementInfoSaveFilePath = PlacementInfoSaveFolderPath + "/Layout " + savesCount + ".txt";
                savesCount++;
            } while (System.IO.File.Exists(placementInfoSaveFilePath));
        }

        // Load fitting semantics
        furnisher = new Furnisher("FittingDatabase.xml");
        AreSemanticsLoaded = true;
    }
	
	// Update is called once per frame
	void Update ()
    {
        // Place fittings at calculated placements
        if (fittingPlacements != null && fittingPlacements.Length > 0 && !areFittingsPlaced)
        {
            areFittingsPlaced = true;

            // Placement info debug strings
            List<string> debugPlacementsInfo = new List<string>();

            // Place fittings
            foreach (FittingPlacement placement in fittingPlacements)
            {
                // Select Prefab to instatiate 
                GameObject representationPrefab = FittingRepresentationsDic[placement.RepresentationObject.FittingTypeId + ":" + placement.RepresentationObject.FittingModelId];

                // Log instantiation in debugger
                string placementInfo = placement.RepresentationObject.FittingTypeId + ": (" + placement.PositionX + " , " + placement.PositionY + ") and " + Mathf.RoundToInt(placement.Orientation * 180 / Mathf.PI) + " degrees turned. ";
                Debug.Log(placementInfo);
                debugPlacementsInfo.Add(placement.RepresentationObject.FittingModelId + "; " + placementInfo);

                Instantiate(representationPrefab, new Vector3(placement.PositionX, representationPrefab.transform.position.y, placement.PositionY), Quaternion.AngleAxis(placement.Orientation * 180 / Mathf.PI, Vector3.down) * representationPrefab.transform.rotation, OriginAnchor.transform);
            }

            if (DoSavePlacementsToFile)
            {
                // Save placement info string to file
                System.IO.File.WriteAllLines(placementInfoSaveFilePath, debugPlacementsInfo);
            }

            AreFittingPlacementsFinished = true;
        }
	}

    private void CalculateFittingPlacements(Room room, List<string> fittingModelsToBePlaced)
    {
        fittingPlacements = furnisher.GeneratePlacements(room, fittingModelsToBePlaced);
    }

    /// <summary>Asynchronous method for generating placements</summary>
    public async void CalculateFittingPlacementsAsync(Room room, List<string> fittingModelsToBePlaced)
    {
        await Task.Factory.StartNew(() => CalculateFittingPlacements(room, fittingModelsToBePlaced));
    }

    /// <summary>Asynchronous method for generating placements of test fittings in test room</summary>
    private async void CalculateFittingPlacementsAsync()
    {
        await Task.Factory.StartNew(() => CalculateFittingPlacements(testRoom, fittingModelsToBePlacedTest));
    }

    private void CalculateRandomizedFittingPlacements(Room room, List<string> fittingModelsToBePlaced)
    {
        fittingPlacements = furnisher.GenerateRandomizedPlacements(room, fittingModelsToBePlaced);
    }

    /// <summary>Asynchronous method for generating randomized unstructured placements</summary>
    public async void CalculateRandomizedFittingPlacementsAsync(Room room, List<string> fittingModelsToBePlaced)
    {
        await Task.Factory.StartNew(() => CalculateRandomizedFittingPlacements(room, fittingModelsToBePlaced));
    }


    // Accessor methods

    public FittingPlacement[] FittingPlacements
    {
        get
        {
            return fittingPlacements;
        }
    }


}
