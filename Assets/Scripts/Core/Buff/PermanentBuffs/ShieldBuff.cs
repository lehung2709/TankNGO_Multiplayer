using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "ShieldBuffSO", menuName = "Buff/ShieldBuff")]
public class ShieldBuff : BuffSO
{
    public override Coroutine ApplyBuff(PlayerStats stats)
    {
        stats.shield.Value = true;
        return null;
    }
}
