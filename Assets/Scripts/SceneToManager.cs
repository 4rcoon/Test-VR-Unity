using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using Unity.Mathematics;
using GameObject = UnityEngine.GameObject;
using Random = UnityEngine.Random;

public class SceneToManager : MonoBehaviour
{
    public GameObject ParticleEffect;
    private Vector2 touchPos;
    private RaycastHit hit;
    private Camera cam;
    public Transform camTransform;

    
    //public PlayerInput playerInput;
    public ARRaycastManager RaycastManager;
    public TrackableType TypeToTrack = TrackableType.AllTypes;
    public GameObject PrefabToInstantiate;
    public PlayerInput PlayerInput;
    private InputAction touchPressAction;
    private InputAction touchPosAction;
    private List<GameObject> instantiatedCubes;
    public List<Material> Materials;
    private InputAction touchPhaseAction;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private TMP_Text timer;
    [SerializeField] private TMP_Text winning;
    private double _timer;
    [SerializeField] double Timer = 30;
    private int cubeCount;
    private bool GameStarted = false;
    [SerializeField] double SpawnDelay;
    private double _spawnDelay = 0;
    private ARRaycastHit firstHit;
    public float SpawnRange = 3f;

    [SerializeField] public GameObject[] disableOnGame;
    [SerializeField] public GameObject[] enableOnGame;
    [SerializeField] public GameObject[] enableOnEnd;

    private void OnTouch()
    {
        touchPos = touchPosAction.ReadValue<Vector2>();
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        RaycastManager.Raycast(touchPos, hits, TrackableType.PlaneWithinBounds);
        if (hits.Count > 0)
        {
            firstHit = hits[0];
            GameObject ballon = Instantiate(PrefabToInstantiate, firstHit.pose.position, Quaternion.identity);
            int randomIndex = Random.Range(0, Materials.Count);
            Material randomMaterial = Materials[randomIndex];
            ballon.GetComponent<MeshRenderer>().material = randomMaterial;
            instantiatedCubes.Add(ballon);
            GameStarted = true;
            able(enableOnGame,true);
            able(enableOnEnd,false);
            able(disableOnGame,false);
            _timer = Timer;
        }
    }

    private void able(GameObject[] list,bool a)
    {
        foreach (GameObject obj in list)
        {
            obj.SetActive(a);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        able(enableOnGame,false);
        able(enableOnEnd,false);
        able(disableOnGame,true);
        cam = camTransform.GetComponent<Camera>();
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

                Ray ray = cam.ScreenPointToRay(touchPos);
                if (Physics.Raycast(ray, out hit,100))
                {
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
                GameObject ballon = Instantiate(PrefabToInstantiate, spawnPos, Quaternion.identity);
                float size = Random.Range(0.1f, 0.3f);
                ballon.transform.localScale = new Vector3(size, size, size);
                Balloon b = ballon.GetComponent<Balloon>();
                int randomIndex = Random.Range(0, Materials.Count);
                b.speed = 0.3f * (randomIndex+1);
                Material randomMaterial = Materials[randomIndex];
                ballon.GetComponent<MeshRenderer>().material = randomMaterial;
                instantiatedCubes.Add(ballon);
            }

            _spawnDelay -= Time.deltaTime;

            _timer -= Time.deltaTime;
            timer.text = $"{math.round(_timer * 100) / 100}";

            if (_timer < 0)
            {
                restart();
            }
        }
    }

    public void restart()
    {
        foreach (GameObject g in instantiatedCubes)
        {
            Destroy(g);
        }
        able(enableOnGame,false);
        able(disableOnGame,true);
        able(enableOnEnd,true); 
        winning.text = $"Congratulations, you've blown up {cubeCount} balloons";
        countText.text = $"counter : 0";
        GameStarted = false;
        cubeCount = 0;
        instantiatedCubes = new System.Collections.Generic.List<GameObject>();
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
