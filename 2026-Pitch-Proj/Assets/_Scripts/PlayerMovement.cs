using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform camHead;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float rotationSpeed = 2f;

    void Start() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update() {
        Vector3 move = new(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        move = transform.TransformDirection(move);
        move.Normalize();
        characterController.SimpleMove(move * speed);

        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * rotationSpeed);
    }

    void LateUpdate() {
        Vector3 e = camHead.eulerAngles;
        e.x -= Input.GetAxis("Mouse Y") * rotationSpeed;
        e.x = RestrictAngle(e.x, -85f, 85f);
        camHead.eulerAngles = e;
    }

    /// <summary>
    /// Helper method for head rotation, to prevent bending backwards
    /// </summary>
    private float RestrictAngle(float angle, float min, float max) {
        if (angle > 180)
            angle -= 360;
        else if (angle < -180)
            angle += 360;

        if (angle > max)
            angle = max;
        if (angle < min)
            angle = min;

        return angle;
    }
}
