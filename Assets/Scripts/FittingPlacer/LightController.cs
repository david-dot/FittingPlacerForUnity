using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    // Data members

    [SerializeField]
    ///<summary>Whether the natural light should be set to night time light</summary>
    public bool SetNightTime;


    // Methods

    // Use this for initialization
    void Start () {
        if (SetNightTime)
        {
            GameObject.FindGameObjectWithTag("Sunlight").GetComponent<Light>().enabled = false;
            RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Custom;
        }
        else
        {
            GameObject.FindGameObjectWithTag("Sunlight").GetComponent<Light>().enabled = true;
            RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Skybox;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
