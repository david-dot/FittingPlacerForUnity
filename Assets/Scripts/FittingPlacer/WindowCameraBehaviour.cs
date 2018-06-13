using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowCameraBehaviour : MonoBehaviour {

    // Data members

    [SerializeField]
    ///<summary>Reference to FittingPlacerForUnity component's GameObject</summary>
    public GameObject FittingPlacerForUnityReferenceGO;
    private FittingPlacerForUnity fittingPlacerForUnity;

    [SerializeField]
    public float StepSize = 0.05f;

    [SerializeField]
    ///<summary>To not sweep full room width</summary>
    public bool DoMinimalSweep = true;

    [SerializeField]
    ///<summary>To sweep room right-to-left instead</summary>
    public bool ReverseSweep = false;

    [SerializeField]
    public bool DoCapture = true;

    [SerializeField]
    public string ScreenCaptureSavePath = "ScreenCapture";
    private string screenCaptureSaveFolderPath;

    [SerializeField]
    ///<summary>What linear factor to supersample output by</summary>
    public int SuperSamplingFactor = 1;

    private int capturedFrameCount = 0;

    private float initialXOffset;
    private float initialXPos;
    private float endXOffset;
    private float endXPos;
    private int stepsTakenCount = 0;
    private int totalSteps;

    private float roomWidthCentimeters;

    private Camera windowCamera;


    // Methods

    // Use this for initialization
    void Start () {

        // Get fitting placer script
        fittingPlacerForUnity = ((FittingPlacerForUnity)FittingPlacerForUnityReferenceGO.GetComponent<FittingPlacerForUnity>());

        // Get camera
        windowCamera = transform.GetComponent<Camera>();

        if (DoCapture)
        {
            // Set capture save path
            int captureCount = 1;
            do
            {
                screenCaptureSaveFolderPath = ScreenCaptureSavePath + "/Capture " + captureCount;
                captureCount++;
            } while (System.IO.Directory.Exists(screenCaptureSaveFolderPath));
            System.IO.Directory.CreateDirectory(screenCaptureSaveFolderPath);
        }

        // Get room width to sweep
        roomWidthCentimeters = 100 * GameObject.FindGameObjectWithTag("Room").GetComponent<RoomController>().Width;

        if (DoMinimalSweep)
        {
            // Set camera sweep to same as smallest room sweep
            //initialXOffset = -0.75f; // For 4:3 aspect ratio crop
            initialXOffset = -0.41f;  // For 16:9 aspect ratio crop
            endXOffset = -initialXOffset;
        }
        else
        {
            // Set room-appropriate camera offset
            switch ((int)Mathf.Round(roomWidthCentimeters))
            {
                case 350:
                    //initialXOffset = -0.75f; // For 4:3 aspect ratio crop
                    initialXOffset = -0.41f;  // For 16:9 aspect ratio crop
                    endXOffset = -initialXOffset;
                    break;
                case 500:
                    //initialXOffset = -1.44f; // For 4:3 aspect ratio crop
                    initialXOffset = -1.16f;  // For 16:9 aspect ratio crop
                    endXOffset = -initialXOffset;
                    break;
                case 700:
                    //initialXOffset = -2.5f; // For 4:3 aspect ratio crop
                    initialXOffset = -2.16f;  // For 16:9 aspect ratio crop
                    endXOffset = -initialXOffset;
                    break;
                default:
                    // The value is not supported
                    // Setting arbitrary values
                    initialXOffset = -100f;
                    endXOffset = -initialXOffset;
                    break;
            }
        }
        if (ReverseSweep)
        {
            float newInitialXOffset = endXOffset;
            endXOffset = initialXOffset;
            initialXOffset = newInitialXOffset;
        }
        initialXPos = transform.position.x + initialXOffset;
        endXPos = transform.position.x + endXOffset;
        float totalTranslationLength = Mathf.Abs(endXOffset - initialXOffset);
        totalSteps = (int)(totalTranslationLength / StepSize);

        // Offset the camera to starting position
        SetPosX(initialXPos);
    }
	
	// Update is called once per frame
	void LateUpdate () {
        if (windowCamera.enabled && Time.time > 1 && isActiveAndEnabled && fittingPlacerForUnity.AreFittingPlacementsFinished && stepsTakenCount <= totalSteps)
        {
            if (DoCapture)
            {
                // Capture screenshot
                ScreenCapture.CaptureScreenshot(screenCaptureSaveFolderPath + "/frame" + capturedFrameCount.ToString("D4") + ".png", SuperSamplingFactor);
                capturedFrameCount++;
            }

            // Translate camera
            SetPosX(initialXPos * (totalSteps - stepsTakenCount) / totalSteps + endXPos * stepsTakenCount / totalSteps);
            stepsTakenCount++;
        }
	}

    private void SetPosX(float posX)
    {
        transform.position = new Vector3(posX, transform.position.y, transform.position.z);
    }

}
