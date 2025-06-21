using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "FireRateBuffSO", menuName = "Buff/FireRateBuff")]

public class FireRateBuff : DurationBuffSO
{
    protected override IEnumerator BuffCoroutine(PlayerStats stats)
    {
        stats.fireRate.Value = value;
        yield return new WaitForSeconds(duration);
        stats.fireRate.Value = stats.DefautFireRate;
    }
}
