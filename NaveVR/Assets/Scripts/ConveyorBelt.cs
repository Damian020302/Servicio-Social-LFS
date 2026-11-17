using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [Header("Visual (UV Scroll & Rollers)")]
    [SerializeField] private Renderer beltRenderer;
    [SerializeField] private Transform[] rollers;
    [SerializeField] private Vector2 uvDirection = new Vector2(1.0f, 0.0f);
    [SerializeField] private float beltVisualSpeed = 0.5f;
    [SerializeField] private float rollerSpeed = 100.0f;
    [Header("Physics (Movement)")]
    [SerializeField] private Rigidbody beltRigidBody;
    [SerializeField] private float physicalSpeed = 2.0f;
    [SerializeField] private Vector3 pushDirection = Vector3.right;

    private Material beltMaterial;
    private Vector2 uvOffset;
    private static readonly int BaseMapId = Shader.PropertyToID("_BaseMap");

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(beltRenderer != null)
        {
            beltMaterial = beltRenderer.material;
        }
        
        //uvOffset = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if(beltMaterial != null)
        {
            uvOffset += uvDirection * (beltVisualSpeed * Time.deltaTime);
            if(beltMaterial.HasProperty(BaseMapId))
            {
                beltMaterial.SetTextureOffset(BaseMapId, uvOffset);
            }
            else
            {
                beltMaterial.mainTextureOffset = uvOffset;
            }
        }
        foreach (Transform roller in rollers)
        {
            roller.Rotate(0.0f, rollerSpeed * Time.deltaTime, 0.0f, Space.Self);
        }
    }

    private void FixedUpdate()
    {
        if(beltRigidBody != null)
        {
            Vector3 targetPos = beltRigidBody.position;
            Vector3 step = transform.TransformDirection(pushDirection.normalized) * (physicalSpeed * Time.fixedDeltaTime);
            beltRigidBody.position -= step;
            beltRigidBody.MovePosition(targetPos);
        }
    }
}