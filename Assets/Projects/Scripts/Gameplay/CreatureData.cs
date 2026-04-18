using UnityEngine;

public abstract class CreatureData : ScriptableObject
{
    public Vector2 MaxSpeed = new Vector2(3f, 3f);
    public float MaxHp = 100f;
    public float MaxSp = 100f;
}
