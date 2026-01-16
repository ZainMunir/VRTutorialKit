using UnityEngine;

public class GuidingArrow : MonoBehaviour
{
    [SerializeField] private Renderer arrowRenderer;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float fadeSpeed = 5f;

    private MaterialPropertyBlock _propBlock;
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private float alpha = 0f;
    private bool isActive = false;
    private Vector3 targetDirection;

    void Start()
    {
        if (arrowRenderer == null)
        {
            Debug.LogError("Arrow renderer not assigned in GuidingArrow!");
            enabled = false;
            return;
        }

        _propBlock = new MaterialPropertyBlock();

        SetAlpha(0);
    }

    void Update()
    {
        if (isActive)
        {
            // Fade in
            alpha = Mathf.MoveTowards(alpha, 1f, Time.deltaTime * fadeSpeed);
            SetAlpha(alpha);

            // Rotate towards target
            if (targetDirection != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
            }
        }
        else
        {
            // Fade out
            alpha = Mathf.MoveTowards(alpha, 0f, Time.deltaTime * fadeSpeed);
            SetAlpha(alpha);
        }
    }

    public void Activate(Vector3 direction)
    {
        isActive = true;
        targetDirection = direction.normalized;
    }

    public void Deactivate()
    {
        isActive = false;
    }

    private void SetAlpha(float alpha)
    {
        arrowRenderer.GetPropertyBlock(_propBlock);

        Color c = arrowRenderer.sharedMaterial.GetColor(BaseColorId);
        c.a = alpha;

        _propBlock.SetColor(BaseColorId, c);

        arrowRenderer.SetPropertyBlock(_propBlock);
    }
}
