using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
class FittingSelection
{
    static FittingSelection()
    {
        EditorApplication.update += Update;
    }

    // Keep fittings in the floor plane and only rotated around the Y axis
    static void Update()
    {
        if (Selection.transforms != null)
        {
            foreach (Transform trans in Selection.transforms)
            {
                if (trans.tag == "Fitting")
                {
                    // Keep fitting in the XZ floor plane
                    if (trans.position.y != 0)
                    {
                        trans.position = new Vector3(trans.position.x, 0, trans.position.z);
                    }

                    // Keep fitting only rotated around the Y axis
                    trans.localEulerAngles = new Vector3(0, trans.localEulerAngles.y, 0);

                    // Keep fitting sized according to loaded fitting semantic
                    trans.localScale = Vector3.one;
                }
            }
        }
    }

    void OnEnable()
    {
        SceneView.onSceneGUIDelegate += this.OnSceneMouseOver;
    }


    void OnSceneMouseOver(SceneView view)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;
        //And add switch Event.current.type for checking Mouse click and switch tiles
        if (Physics.Raycast(ray, out hit, 100f))
        {
            Debug.DrawRay(ray.origin, hit.transform.position, Color.blue, 5f);
            Debug.Log(hit.transform.name);
            Debug.Log(hit.transform.position);
        }
    }
}
