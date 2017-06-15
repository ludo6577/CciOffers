using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraScript : MonoBehaviour
{

    public Camera UICamera;

    public EventSystem EventSystem;
    private PointerEventData pointerEventData;
    private List<GameObject> oldRaycastedGameObjects;

    public Image FadeMask;
    public float FadeSpeed;

    void Start()
    {
        oldRaycastedGameObjects = new List<GameObject>();
    }

    void Update()
    {
        if (pointerEventData == null)
            pointerEventData = new PointerEventData(EventSystem);

        Vector2 hotspot = new Vector2(0.5f, 0.5f);
        if (UnityEngine.VR.VRSettings.enabled)
            pointerEventData.position = new Vector2(hotspot.x * UnityEngine.VR.VRSettings.eyeTextureWidth, hotspot.y * UnityEngine.VR.VRSettings.eyeTextureHeight);
        else
            pointerEventData.position = new Vector2(hotspot.x * Screen.width, hotspot.y * Screen.height);

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.RaycastAll(pointerEventData, raycastResults);
        for (var i = 0; i < raycastResults.Count; i++)
        {
            var hit = raycastResults[i];
            var other = hit.gameObject;
            var watchableObject = other.GetComponent<WatchableObject>();
            Debug.Log("Enter: " + watchableObject);
            if (watchableObject != null && !oldRaycastedGameObjects.Contains(other))
            {
                oldRaycastedGameObjects.Add(other);
                watchableObject.OnWatch();
            }
        }

        for (var i = oldRaycastedGameObjects.Count - 1; i >= 0; i--)
        {
            if (!raycastResults.Select(x => x.gameObject).Contains(oldRaycastedGameObjects[i]))
            {
                //Debug.Log("Exit: " + oldRaycastedGameObjects[i]);
                var watchableObject = oldRaycastedGameObjects[i].GetComponent<WatchableObject>();
                watchableObject.UnWatch();
                oldRaycastedGameObjects.Remove(oldRaycastedGameObjects[i]);
            }
        }
    }
}
