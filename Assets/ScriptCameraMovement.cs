using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptCameraMovement : MonoBehaviour
{

    [Range(0.1f, 9f)] [SerializeField] private float _speedHorizontal;
    [Range(0.1f, 9f)] [SerializeField] private float _speedVertical;
    [Range(0.1f, 5f)] [SerializeField] private float _speedPlayer;
    [Range(0.01f, 1f)] [SerializeField] private float _sphereSize = 0.2f;
    [Range(0f, 20f)] [SerializeField] private float _maxTeleportLength = 10;
    [SerializeField] private float _playerHeight = 0.5f;
    [SerializeField] private Color _sphereColor = Color.red;

    private Vector2 _rotationVect = Vector2.zero;

    //define Sphere
    private GameObject sphere;

    Vector3 target_point;

    // Start is called before the first frame update
    void Start()
    {
        sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere); ;
        sphere.transform.position = new Vector3(0, -100, 0); //put sphere -100 under the ground where it cant be seen
        sphere.layer = 8;
    }

    // Update is called once per frame
    void Update()
    {
        sphere.transform.localScale = new Vector3(_sphereSize, _sphereSize, _sphereSize);
        sphere.GetComponent<MeshRenderer>().material.color = _sphereColor;

        _rotationVect.x += _speedHorizontal * Input.GetAxis("Mouse X");
        _rotationVect.y += _speedVertical   * Input.GetAxis("Mouse Y");
        _rotationVect.y  = Mathf.Clamp(_rotationVect.y, -50f, 90f);

        Quaternion xQuart = Quaternion.AngleAxis(_rotationVect.x, Vector3.up);
        Quaternion yQuart = Quaternion.AngleAxis(_rotationVect.y, Vector3.left);

        // Same as transform.localEulerAngles = new Vector3(-rotation.y, rotation.x, 0);
        transform.localRotation = xQuart * yQuart;


        //need to be called in Update not in FixedUpdate
        if (Input.GetMouseButtonDown(0))
        {
            //teleport
            Camera.main.transform.position = new Vector3(target_point.x, _playerHeight + target_point.y, target_point.z);
        }
    }

    // See Order of Execution for Event Functions for information on FixedUpdate() and Update() related to physics queries
    void FixedUpdate()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        Ray ray = new Ray(transform.position, transform.TransformDirection(Vector3.forward));
        if (Physics.Raycast(ray, out hit, _maxTeleportLength, ~(1<<8)))
        {
            //if raycast hits something draw the sphere so the player knows where you teleports to
            target_point = ray.GetPoint(hit.distance);
            sphere.transform.position = target_point;
        }
        else
        {
            sphere.transform.position = new Vector3(0, -100, 0); //move sphere under the ground where it cant be seen
        }

    }

}
