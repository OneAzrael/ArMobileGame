using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ScanRoom : MonoBehaviour
{
    [SerializeField] ARPlaneManager planeManager;
    [SerializeField] private float requiredArea = 4f;
    
    [SerializeField] private GameObject EndScreen;
    [SerializeField] private GameObject StartScreen;
    [SerializeField] Button StartButton;
    [SerializeField] private GameObject Scan;

    [SerializeField] private Image progressBar;

    void Start()
    {
        Scan.SetActive(true);
        StartScreen.SetActive(false);
        
    }
    
    private void Awake()
    {
        StartButton.onClick.AddListener(FinishScan);
    }

    void Update()
    {
        float area = GetFloorArea();
        progressBar.fillAmount = area / requiredArea;
    }

    float GetFloorArea()
    {
        float total = 0;

        foreach (ARPlane plane in planeManager.trackables)
        {
            if (plane.alignment != PlaneAlignment.HorizontalUp) continue;
            var planearea = plane.size.x * plane.size.y;
            total += planearea;
        }
        return total;
    }

    public void FinishScan()
    {
        var area = GetFloorArea();
        if (area <=  requiredArea) return;
        
        Scan.SetActive(false);
        StartScreen.SetActive(true);
        HidePlanes();
        enabled = false;
    }

    public void HidePlanes()
    {
        foreach (ARPlane plane in planeManager.trackables)
        {
            if (plane.TryGetComponent(out MeshRenderer meshRenderer)) meshRenderer.enabled = false;
            if (plane.TryGetComponent(out LineRenderer lineRenderer)) lineRenderer.enabled = false;
        }
    }
}
