using UnityEngine;
using System.Collections;

public class Look : MonoBehaviour
{
    public bool LockCursor = true;
    public bool Fly = true;
    public float Speed = 1.25f;
    public float DampenSmoothTime = 0.02f;
    public float Sensitivity = 5.0f;
    float _DesiredXRotation;
    float _DesiredYRotation;
    float _DampenedXRotation;
    float _DampendedYRotation;
    float _CurrentXRotationVelocity = 0.0f;
    float _CurrentYRotationVelocity = 0.0f;

    void Start()
    {
        if (LockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void Update()
    {
        if (Fly)
        {
            transform.Translate(Time.deltaTime * Input.GetAxis("Horizontal") * Speed, 0, 0, Space.Self);
            transform.Translate(0, 0, Time.deltaTime * Input.GetAxis("Vertical") * Speed, Space.Self);
        }
        else
        {
            float forwardMovement = Input.GetAxis("Vertical");
            transform.Translate(new Vector3(forwardMovement * transform.forward.x, 0, forwardMovement * transform.forward.z).normalized * Time.deltaTime * Speed, Space.World);
            float rightMovement = Input.GetAxis("Horizontal");
            transform.Translate(new Vector3(rightMovement * transform.right.x, 0, rightMovement * transform.right.z).normalized * Time.deltaTime * Speed, Space.World);
        }

        if (transform.position.x > 4)
            transform.position = new Vector3(4, transform.position.y, transform.position.z);
        if (transform.position.x < -4)
            transform.position = new Vector3(-4, transform.position.y, transform.position.z);
        if (transform.position.z > 4)
            transform.position = new Vector3(transform.position.x, transform.position.y, 4);
        if (transform.position.z < -4)
            transform.position = new Vector3(transform.position.x, transform.position.y, -4);
        if (transform.position.y > 9)
            transform.position = new Vector3(transform.position.x, 9, transform.position.z);
        if (transform.position.y < 0.1f)
            transform.position = new Vector3(transform.position.x, 0.1f, transform.position.z);


        _DesiredXRotation -= Input.GetAxis("Mouse Y") * Sensitivity;
        _DesiredYRotation += Input.GetAxis("Mouse X") * Sensitivity;
        _DesiredXRotation = Mathf.Clamp(_DesiredXRotation, -89, 89);
        _DampenedXRotation = Mathf.SmoothDamp(_DampenedXRotation, _DesiredXRotation, ref _CurrentXRotationVelocity, DampenSmoothTime);
        _DampendedYRotation = Mathf.SmoothDamp(_DampendedYRotation, _DesiredYRotation, ref _CurrentYRotationVelocity, DampenSmoothTime);
        transform.rotation = Quaternion.Euler(_DampenedXRotation, _DampendedYRotation, 0);
    }
}