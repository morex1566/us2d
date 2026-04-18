using UnityEngine;

public interface IPlayerLook
{
    // 바라보는 방향이 +x -> -x 또는 -x -> +x로 바뀔 때마다 호출
    void OnLookDirectionChange(Vector2 lookDirection);
}
