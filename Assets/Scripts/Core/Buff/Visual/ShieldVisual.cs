using UnityEngine;

public class ShieldVisual : MonoBehaviour
{
    [SerializeField] private GameObject shieldVisualObj;
    [SerializeField] private PlayerStats stats;
    void Start()
    {
        stats.shield.OnValueChanged += OnShieldChangedValue;
    }

    private void OnShieldChangedValue(bool oldValue, bool newValue)
    {
        AudioManager.Instance.SpawnSoundEmitter(null, "ShieldBreak", transform.position);
        shieldVisualObj.SetActive(newValue);
    }    
}
