using UnityEngine;
public class CollisionBox : MonoBehaviour
{
    float left, right, top, bottom;
    float leftPos, rightPos, topPos, bottomPos;

    public CollisionBox(float _left, float _right, float _bottom, float _top)
    {
        if (_left >= _right || _bottom >= _top) Debug.LogAssertion("left < right이고 bottom < top이어야 합니다.");
        left = _left;
        right = _right;
        bottom = _bottom;
        top = _top;
    }

    public void SetBoxPosition(Vector2 coord) // 박스 위치 설정
    {
        leftPos = coord.x + left;
        rightPos = coord.x + right;
        bottomPos = coord.y + bottom;
        topPos = coord.y + top;
    }

    public void ChangeBoxPosition(Vector2 change) // 박스 위치 이동
    {
        leftPos += change.x;
        rightPos += change.x;
        bottomPos += change.y;
        topPos += change.y;
    }

    public bool DoesCollideWith(CollisionBox box) // 박스가 겹치는가?
    {
        return !((box.rightPos <= leftPos || box.leftPos >= rightPos) && (box.topPos <= bottomPos || box.bottom >= topPos));
    }
}