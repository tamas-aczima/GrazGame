using UnityEngine;

public class UIFaceCamera : MonoBehaviour
{

    private Transform camTrans;
    private Transform trans;

    private void Awake()
    {
        camTrans = Camera.main.transform;
        trans = transform;
    }

    void Update()
    {
        transform.LookAt(trans.position + camTrans.rotation * Vector3.forward, camTrans.rotation * Vector3.up);
    }
}
