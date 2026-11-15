using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class FigureSpawner : MonoBehaviour
{
    [SerializeField] GameObject ghoulprefab;
    [SerializeField] GameManager gameManager;
    [SerializeField] Camera cam;
    [SerializeField] ARPlaneManager PlaneManager;
    [SerializeField] private float minSpawnDistance = 2.5f;
    [SerializeField] private float maxSpawnDistance = 3f;
    [SerializeField] private float maxSpawnAngle = 60f;
    [SerializeField] private float minSpawnDealy = 8f;
    [SerializeField] private float maxSpawnDealy = 25f;


    private float spawntimer;
    
   public GameObject CurrentGhoul { get; private set; }


   void Start()
   {
       spawntimer = Random.Range(minSpawnDealy, maxSpawnDealy);
   }
    void SpawnFigure()
    {
        Debug.Log("Attempting to spawn Spawning figure");
        
        if (CurrentGhoul != null)
        {
            Destroy(CurrentGhoul);
        }

        float maxDistance = GetRoomMaxDistance();
        float minDistance = Random.Range(minSpawnDistance, maxSpawnDistance);
        
        for (int attempt = 0; attempt < 10; attempt++)
        {
            var RandomRange = Random.Range(minDistance, maxDistance);
            
            var camForward = cam.transform.forward;
            camForward.y = 0;
            var flatBackWardsDirection = -camForward.normalized;
            float angle = Random.Range(-maxSpawnAngle, maxSpawnAngle);
            Vector3 SpawnDirection = Quaternion.AngleAxis(angle, Vector3.up) * flatBackWardsDirection;
            var lookPos = Quaternion.LookRotation(-SpawnDirection);

            var spawnpoint = cam.transform.position + SpawnDirection * RandomRange;
            var didHit = Physics.Raycast(spawnpoint, Vector3.down, out RaycastHit hit, 10f);
            if (didHit)
            {
                CurrentGhoul = Instantiate(ghoulprefab, hit.point, lookPos);
                var animator = CurrentGhoul.GetComponentInChildren<Animator>();
                int number = Random.Range(0, 6);
                animator.Play("Pose" + number);
                
                Debug.Log("Hit " + hit.collider.gameObject.name);
                return;
            }
            else
            {
                Debug.LogWarning("No floor behind trying again");
            }
        }
    }

    float GetRoomMaxDistance()
    {
        float max = 0;
        
        Vector3 camPos = cam.transform.position;
        camPos.y = 0;
        
        foreach (ARPlane plane in PlaneManager.trackables)
        {
            if (plane.alignment != PlaneAlignment.HorizontalUp) continue;

            foreach (Vector2 p in plane.boundary)
            {
                Vector3 world = plane.transform.TransformPoint(new Vector3(p.x, p.y, 0));
                world.y = 0;
                
                float distance = Vector3.Distance(camPos, world);
                if (distance > max) max = distance;
            }
        }
        return max;
    }

    void Update()
    {
        if (!CurrentGhoul && gameManager.IsPlaying)
        {
            Debug.Log($"Spawning ghoul soon {spawntimer}");
            spawntimer -= Time.deltaTime;
            
            if (spawntimer <= 0f)
            {
                SpawnFigure();
                spawntimer = Random.Range(minSpawnDealy, maxSpawnDealy);
            }
        }
    }
}
