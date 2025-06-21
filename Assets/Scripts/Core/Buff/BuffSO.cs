using UnityEngine;

public abstract class BuffSO : ScriptableObject
{
    public string ID;
    public Sprite icon;
    public float value;

    public abstract Coroutine ApplyBuff(PlayerStats stats);
}
