using UnityEngine;
using System.Collections;

public class LaserPointer : MonoBehaviour
{
    public int Mask = 1 << 8;
    private GameObject Laser;
    public Material HitMaterial;
    public Material MissMaterial;
    public float LaserDistance = 10;
    public GameObject Target = null;
    public Vector3 TargetPoint;
    private bool IsActive = true;

    void Awake()
    {
        Laser = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Laser.name = "LaserPointer";
        Laser.transform.SetParent(gameObject.transform);
        Laser.transform.localPosition = new Vector3(0f, 0f, LaserDistance / 2);
        Laser.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
        Laser.transform.localScale = new Vector3(0.002f, LaserDistance / 2, 0.002f);
        Laser.GetComponent<Collider>().enabled = false;
        Laser.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        Laser.GetComponent<MeshRenderer>().lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        Laser.GetComponent<MeshRenderer>().reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        Laser.SetActive(IsActive);
    }

    public void SetEnabled(bool isenabled)
    {
        IsActive = isenabled;
        if (Laser != null)
        {
            Laser.SetActive(isenabled);
        }
        if (!isenabled)
        {
            Target = null;
        }

    }

    // Use this for initialization
    void Start()
    {
        //       Debug.Log(LaserDistance);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsActive)
        {
            float theDistance;
            RaycastHit hit;
            Vector3 forward = transform.TransformDirection(Vector3.forward);// * (LaserDistance);

            if (Physics.Raycast(transform.position, forward, out hit, LaserDistance, Mask))
            {
                theDistance = hit.distance;
                Laser.GetComponent<MeshRenderer>().material.color = Color.green;
                //hit.collider.gameObject;
                Laser.GetComponent<MeshRenderer>().material = HitMaterial;
                Target = hit.transform.gameObject;
                TargetPoint = hit.point;

            }
            else
            {
                Target = null;
                TargetPoint = Vector3.zero;
                theDistance = LaserDistance;
                Laser.GetComponent<MeshRenderer>().material = MissMaterial;
            }
            Laser.transform.localPosition = new Vector3(0f, 0f, theDistance / 2);
            Laser.transform.localScale = new Vector3(0.002f, theDistance / 2, 0.002f);
        }
    }
}
