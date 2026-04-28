using System;
using UnityEngine;

[Serializable]
public partial class CreatureSpringArm
{
    [SerializeField, Min(0f)] private float length = 2f;

    [SerializeField] private Transform pivotTransform = null;

    [SerializeField] private Transform socketTransform = null;



    public float Length => length;

    public Transform PivotTransform => pivotTransform;

    public Transform SocketTransform => socketTransform;



    // 마우스 방향 벡터의 각도까지 회전
    public void Update(Vector3 lookDirection)
    {
        socketTransform.position = pivotTransform.position + lookDirection * length;
        pivotTransform.rotation = Quaternion.Euler(lookDirection);
    }
}