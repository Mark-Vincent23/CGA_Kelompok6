using UnityEngine;

public class Sc_FreeCam : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public float rotationSpeed = 300.0f;
    public float distance = 10.0f;
    public LayerMask whatIsGround;
    public LayerMask whatIsWall;
    public bool isShiftlock=false;
    public Transform thirdPersonLookAt;

    private float currentVerticalAngle;
    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private Quaternion rotation;

    void Start()
    {
        offset = transform.position - player.position;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift)) isShiftlock = !isShiftlock;
        if(isShiftlock){
            Cursor.lockState = CursorLockMode.Locked;
            ShiftLockCamera();
        }else{
            Cursor.lockState = CursorLockMode.None;
            CameraFollowing();
        }
    }

    void CameraFollowing()
    {
        if (Input.GetMouseButton(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
            yaw += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            pitch -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }

        pitch = Mathf.Clamp(pitch, -40f, 85f);
        rotation = Quaternion.Euler(pitch, yaw, 0.0f);
        CameraScrolling();
    }

    void CameraScrolling(){
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        distance -= scrollInput * 10f;
        distance = Mathf.Clamp(distance, 5f, 15f);

        Vector3 desiredPosition = player.position + rotation * offset.normalized * distance;

        RaycastHit hit;
        Vector3 direction = desiredPosition - player.position;

        if (Physics.Raycast(player.position, direction.normalized, out hit, distance, whatIsGround | whatIsWall)){
            desiredPosition = hit.point;
        }

        transform.position = desiredPosition;
        transform.LookAt(player);
    }

    void ShiftLockCamera()
    {
        // Scroll to adjust camera distance
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        distance -= scrollInput * 10f;
        distance = Mathf.Clamp(distance, 5f, 15f);

        // Rotate the player horizontally based on mouse movement
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        player.Rotate(Vector3.up * mouseX);

        // Rotate the camera vertically (up/down) without affecting the player's rotation
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
        currentVerticalAngle -= mouseY; // Invert Y-axis movement for a natural feel
        currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, -40f, 85f); // Limit the vertical angle

        // Calculate camera position based on distance and rotation
        Vector3 cameraOffset = player.position - player.forward * (distance - 2f);
        cameraOffset.y += 1f;

        // Apply the vertical rotation
        Quaternion rotation = Quaternion.Euler(currentVerticalAngle, player.eulerAngles.y, 0f);

        // Adjust the camera position based on collisions with walls or the ground
        RaycastHit hit;
        if (Physics.Raycast(player.position, cameraOffset - player.position, out hit, distance, whatIsGround | whatIsWall))
            transform.position = hit.point;
        else
            transform.position = cameraOffset;

        // Apply the rotation to the camera and make it look at the player (or another target)
        transform.rotation = rotation;
        transform.LookAt(thirdPersonLookAt.position);
    }
}