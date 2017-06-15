using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraScript : MonoBehaviour
{

    public GameObject CameraParent;

    public EventSystem EventSystem;
    private PointerEventData pointerEventData;
    private List<GameObject> oldRaycastedGameObjects;

    public Image FadeMask;
    public float FadeSpeed;

    public Image Reticule;
    public float FillSpeed;

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

            if (raycastResults[i].gameObject.tag == "teleport1")
            {
                Reticule.fillAmount += FillSpeed;
                if (Reticule.fillAmount >= 1f)
                {
                    StartCoroutine(Fade());
                    Reticule.fillAmount = 0f;
                }
            }
        }

        if(Reticule.fillAmount >= 0f)
            Reticule.fillAmount -= FillSpeed /2;

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

    private IEnumerator Fade()
    {
        var doWhile = true;
        while (doWhile)
        {
            FadeMask.color = new Color(FadeMask.color.r, FadeMask.color.g, FadeMask.color.b, FadeMask.color.a +FadeSpeed);
            Reticule.fillAmount = 0f;
            yield return new WaitForEndOfFrame();
            if (FadeMask.color.a >= 1f)
                doWhile = false;
        }

        CameraParent.transform.position = new Vector3(CameraParent.transform.position.x, CameraParent.transform.position.y + 20, CameraParent.transform.position.z);
        doWhile = true;
        while (doWhile)
        {
            FadeMask.color = new Color(FadeMask.color.r, FadeMask.color.g, FadeMask.color.b, FadeMask.color.a - FadeSpeed);
            yield return new WaitForEndOfFrame();
            if (FadeMask.color.a <= 0f)
                doWhile = false;
        }
    }
}
