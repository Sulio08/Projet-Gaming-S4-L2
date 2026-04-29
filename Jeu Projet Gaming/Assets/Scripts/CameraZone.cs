using UnityEngine;

public class CameraZone : MonoBehaviour
{
    public Transform cameraTarget; // position cible de la caméra
    public float transitionSpeed = 5f;

    private static Camera cam;
    private static Vector3 targetPos;

    void Start()
    {
        if (cam == null) cam = Camera.main;
        targetPos = cam.transform.position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            targetPos = new Vector3(cameraTarget.position.x, 
                                   cameraTarget.position.y, 
                                   cam.transform.position.z);
        }
    }

    void LateUpdate()
    {
        cam.transform.position = Vector3.Lerp(cam.transform.position, 
                                              targetPos, 
                                              Time.deltaTime * transitionSpeed);
    }
}