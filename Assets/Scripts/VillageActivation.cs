using UnityEngine;

public class VillageActivation : MonoBehaviour
{
    [SerializeField] SpriteRenderer _spriteRenderer;

    [SerializeField] Color _inactiveColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    [SerializeField] Color _activeColor = Color.yellow;

    public bool IsActived { get; private set; } = false;


    private void Start()
    {
        UpdateVisual();
    }


    public void Activate()
    {
        IsActived = true;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        _spriteRenderer.color = IsActived ? _activeColor : _inactiveColor;
    }
}
