using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName ="SpeedBuffSO",menuName ="Buff/SpeedBuff")]
public class SpeedBuffSO : DurationBuffSO
{
   
    protected override IEnumerator BuffCoroutine(PlayerStats stats)
    {
        
        stats.movementSpeed.Value = value;
        yield return new WaitForSeconds(duration);
        stats.movementSpeed.Value = stats.DefautSpeed;
    }
}
