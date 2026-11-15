
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class GameManager : MonoBehaviour
{
    [SerializeField] FigureSpawner figureSpawner;
    [SerializeField] Camera cam;
    [SerializeField] private TMP_Text ghoulscaptured;
    [SerializeField] private float loseDistance = 1f;
    [SerializeField] private float creepspeed = 0.2f;
    [SerializeField] ScanRoom scanRoom;
 

    [SerializeField] private GameObject StartScreen;
    [SerializeField] private GameObject EndScreen;
    [SerializeField] Button RetryButton;
    [SerializeField] Button StartGameButton;

    private bool _isGameOver;
    private bool _isGameStarted;
    public bool IsPlaying => _isGameStarted;
    private int _ghoulAmount;
    
    private void Awake()
    {
        RetryButton.onClick.AddListener(OnStartGame);
        StartGameButton.onClick.AddListener(OnStartGame);
    }

    private void Start()
    {
        _isGameStarted = false;
    }
    
    private void Update()
    {
        GameObject figure = figureSpawner.CurrentGhoul;
        if(figure == null) return;
      
        Vector3 toCamera = cam.transform.position - figure.transform.position;
        toCamera.y = 0;
        
        if (!IsOnScreen(figure))
        {
            Vector3 direction = toCamera.normalized;
            figure.transform.position += direction * (creepspeed * Time.deltaTime);
            figure.transform.rotation = Quaternion.LookRotation(direction);
            
            Debug.DrawLine(figure.transform.position, cam.transform.position, Color.red);
            Debug.DrawRay(figure.transform.position, direction, Color.green);
        }
        
        float distance = toCamera.magnitude;
        if (distance < loseDistance)
        {
            Destroy(figure);
            GameOver();
        }
    }

    public void TryCapture()
    {
        GameObject figure = figureSpawner.CurrentGhoul;
        Debug.Log("Trying to capture!");

        if (figure == null) return;
        

        if (IsOnScreen(figure))
        {
            Destroy(figure);
            CapturedGhouls();
            Debug.Log("Captured");
        }
        else
        {
            Debug.Log("Not captured");
        }
    }
    
    void CapturedGhouls()
    {
        ghoulscaptured.gameObject.SetActive(true);
        _ghoulAmount += 1;
        ghoulscaptured.text = _ghoulAmount.ToString();
        
        if (_ghoulAmount == 0)
        {
            ghoulscaptured.gameObject.SetActive(false);
        }
    }
    
    bool IsOnScreen(GameObject target)
    {
        Collider col = target.GetComponentInChildren<Collider>();
        if (col == null) return false;

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        return GeometryUtility.TestPlanesAABB(planes, col.bounds);
    }

    void GameOver()
    {
        Debug.unityLogger.Log("Game Over");
        _isGameOver = true;
        _isGameStarted = false;
        EndScreen.SetActive(true);
    }

    private void OnStartGame()
    {
        _isGameStarted = true;
        _isGameOver = false;
        scanRoom.FinishScan();
        
        _ghoulAmount = 0;
        StartScreen.SetActive(false);
        EndScreen.SetActive(false);
    }
}
