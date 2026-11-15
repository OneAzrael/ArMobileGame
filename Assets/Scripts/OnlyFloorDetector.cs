using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARPlaneManager))]
public class OnlyFloorDetector : MonoBehaviour
{
    private ARPlaneManager planeManager;
    private Camera arCamera;

    [Tooltip("Minimum width/length in meters before a plane counts as a floor.")]
    public float minimumFloorSize = 0.5f;

    void Awake()
    {
        planeManager = GetComponent<ARPlaneManager>();
        arCamera = Camera.main;
    }

    void OnEnable()
    {
        planeManager.planesChanged += FilterForFloor;
    }

    void OnDisable()
    {
        planeManager.planesChanged -= FilterForFloor;
    }

    void FilterForFloor(ARPlanesChangedEventArgs args)
    {
        ARPlane floorPlane = null;
        float lowestY = float.MaxValue;

        foreach (var plane in planeManager.trackables)
        {
            // 1. Must face upward (ignores ceilings)
            if (plane.alignment != PlaneAlignment.HorizontalUp)
            {
                plane.gameObject.SetActive(false);
                continue;
            }

            // 2. Must be at least 0.5m below the phone (ignores high shelves/noise)
            if (arCamera != null && plane.transform.position.y > arCamera.transform.position.y - 0.5f)
            {
                plane.gameObject.SetActive(false);
                continue;
            }

            // 3. Must be big enough to be a floor (ignores tiny random patches)
            if (plane.size.x < minimumFloorSize || plane.size.y < minimumFloorSize)
            {
                plane.gameObject.SetActive(false);
                continue;
            }

            // 4. Find the lowest surface (ignores tables and chairs)
            if (plane.transform.position.y < lowestY)
            {
                lowestY = plane.transform.position.y;
                floorPlane = plane;
            }
        }

        // Disable all planes except the confirmed floor
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(plane == floorPlane);
        }
    }
}
