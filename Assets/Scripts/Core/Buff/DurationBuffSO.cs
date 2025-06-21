using UnityEngine;
using System.Collections;


public abstract class DurationBuffSO : BuffSO
{
    public float duration;

    public override Coroutine ApplyBuff(PlayerStats stats)
    {
        return stats.StartCoroutine(BuffCoroutine(stats));
     }
    protected abstract IEnumerator BuffCoroutine(PlayerStats stats);
    
}
