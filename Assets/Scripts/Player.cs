using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    public float speed;
    public NetworkVariable<Color> playerColorNetVar = new NetworkVariable<Color>(Color.red);
    private Camera playerCamera;
    private GameObject playerFlag;
    
    // Start is called before the first frame update
    void Start()
    {
        playerCamera = transform.Find("Camera").GetComponent<Camera>();
        playerCamera.enabled = IsOwner;
        playerCamera.GetComponent<AudioListener>().enabled = IsOwner;
        playerFlag = transform.GetChild(0).GetChild(0).gameObject;

        ApplyColor();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            OwnerHandleInput();
        }
    }

    private void OwnerHandleInput()
    {
        float adjSpeed = Mathf.Abs(speed) * Time.deltaTime;
        float rotateSpeed = adjSpeed * (float)1.3;
        float rotationSpeed = Input.GetKey(KeyCode.W) ? rotateSpeed * (float)1.5 : rotateSpeed;

        Vector3 translation = Vector3.zero;
        Vector3 rotation = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            translation = new Vector3(0, 0, adjSpeed);
        }

        if (Input.GetKey(KeyCode.D))
        {
            rotation = new Vector3(0, rotationSpeed, 0);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            rotation = new Vector3(0, -rotationSpeed, 0);
        }

        if (translation != Vector3.zero || rotation != Vector3.zero)
        {
            MoveServerRpc(translation, rotation);
        }
    }

    private void ApplyColor()
    {
        playerFlag.GetComponent<MeshRenderer>().material.color = playerColorNetVar.Value;
    }

    [ServerRpc]
    private void MoveServerRpc(Vector3 translation, Vector3 rotation)
    {
        ulong clientId = GetComponent<NetworkObject>().OwnerClientId;

        Transform boatTransform = transform.Find("boat").transform;
        Vector3 boatPosition = boatTransform.position;
        Vector3 nextPosition = boatPosition += boatTransform.TransformVector(translation);

        if (NetworkManager.ServerClientId != clientId && (nextPosition.x > 25 || nextPosition.x < -25 || nextPosition.z > 25 || nextPosition.z < -25))
        {
            return;
        }

        transform.Find("boat").Translate(translation);
        transform.Find("boat").Rotate(rotation);

        Transform cameraTransform = transform.Find("Camera").transform;
        cameraTransform.position = boatPosition += boatTransform.TransformVector(new Vector3(0, 10, -40));
        cameraTransform.rotation = boatTransform.rotation;
    }
}
