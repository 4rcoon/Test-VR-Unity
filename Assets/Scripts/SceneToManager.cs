using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class SceneToManager : MonoBehaviour
{
    public GameObject ParticleEffect;
    private Vector2 touchPos;
    private RaycastHit hit;
    private Camera cam;
    
    //public PlayerInput playerInput;
    public ARRaycastManager RaycastManager;
    public TrackableType TypeToTrack = TrackableType.PlaneWithinBounds;
    public GameObject PrefabToInstantiate;
    public PlayerInput PlayerInput;
    private InputAction touchPressAction;
    private InputAction touchPosAction;
    private List<GameObject> instantiatedCubes;
    public List<Material> Materials;
    private InputAction touchPhaseAction;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private TMP_Text timer;
    [SerializeField] double _timer = 30;
    private int cubeCount;
    private bool GameStarted = false;
    [SerializeField] double SpawnDelay;
    private double _spawnDelay = 0;
    private ARRaycastHit firstHit;
    public float SpawnRange = 3f;

    private void OnTouch()
    {
        touchPos = touchPosAction.ReadValue<Vector2>();
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        RaycastManager.Raycast(touchPos, hits, TypeToTrack);
        if (hits.Count > 0)
        {
            firstHit = hits[0];
            GameObject ballon = Instantiate(PrefabToInstantiate, firstHit.pose.position, firstHit.pose.rotation);
            int randomIndex = Random.Range(0, Materials.Count);
            Material randomMaterial = Materials[randomIndex];
            ballon.GetComponent<MeshRenderer>().material = randomMaterial;
            instantiatedCubes.Add(ballon);
            GameStarted = true;
        }
    }    
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        touchPressAction = PlayerInput.actions["TouchPress"];
        touchPosAction = PlayerInput.actions["TouchPos"];
        touchPhaseAction = PlayerInput.actions["TouchPhase"];
        instantiatedCubes = new List<GameObject>();
        cubeCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (touchPressAction.WasPerformedThisFrame())
        {
            if (GameStarted)
            {
                touchPos = touchPosAction.ReadValue<Vector2>();
                //countText.text = $"test : {cubeCount}";

                Ray ray = cam.ScreenPointToRay(touchPos);
                if (Physics.Raycast(ray, out hit))
                {
                    countText.text = $"test 2 : {cubeCount}";

                    GameObject hitObj = hit.collider.gameObject;
                    if (hitObj.tag == "Enemy")
                    {
                        var clone = Instantiate(ParticleEffect, hitObj.transform.position, Quaternion.identity);
                        clone.transform.localScale = hitObj.transform.localScale;
                        cubeCount++;
                        countText.text = $"counter : {cubeCount}";
                        Destroy(hitObj);
                    }
                }
            }
            else
            {
                var touchPhase = touchPhaseAction.ReadValue<UnityEngine.InputSystem.TouchPhase>();
                if (touchPhase == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    OnTouch();
                }
            }
            
        }

        if (GameStarted)
        {
            if (_spawnDelay < 0)
            {
                _spawnDelay = SpawnDelay;
                float x = firstHit.pose.position.x+ Random.Range(-SpawnRange, SpawnRange);
                float y = firstHit.pose.position.y;
                float z = firstHit.pose.position.z;
                Vector3 spawnPos = new Vector3(x, y, z);
                GameObject ballon = Instantiate(PrefabToInstantiate, spawnPos, firstHit.pose.rotation);
                int randomIndex = Random.Range(0, Materials.Count);
                Material randomMaterial = Materials[randomIndex];
                ballon.GetComponent<MeshRenderer>().material = randomMaterial;
                instantiatedCubes.Add(ballon);
            }

            _spawnDelay -= Time.deltaTime;

            _timer -= Time.deltaTime;
            timer.text = $"{math.round(_timer * 100) / 100}";
        }
    }
    
    public void ChangeColor()
    {
        foreach (GameObject cube in instantiatedCubes)
        {
            int randomIndex = Random.Range(0, Materials.Count);
            Material randomMaterial = Materials[randomIndex];
            cube.GetComponent<MeshRenderer>().material = randomMaterial;
        }
    }
}
