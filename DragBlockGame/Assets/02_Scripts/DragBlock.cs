using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DragBlock : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve curveMovement;
    [SerializeField]
    private AnimationCurve curveScale;

    private BlockArrangeSystem blockArrangeSystem;

    private float appeartime = 0.5f;
    private float returntime = 0.1f; //블록이 원래 위치로 돌아갈 때 소요되는 시간

    [field: SerializeField]
    public Vector2Int BlockCount {  get; private set; } 

    public Color Color { get; private set; }
    public Vector3[] ChildBlocks { get; private set; } //자식 블록들의 지역 좌표

    public void SetUp(BlockArrangeSystem blockArrangeSystem, Vector3 parentPosition)
    {
        this.blockArrangeSystem = blockArrangeSystem;

        Color = GetComponentInChildren<SpriteRenderer>().color;

        //블록 모양의 따라 자식 블록 수가 다르기 때문에 자식 수만큼 배열방 생성 후 모든 자식의 지역좌표 저장
        ChildBlocks = new Vector3[transform.childCount];
        for (int i = 0; i < ChildBlocks.Length; ++i)
        {
            ChildBlocks[i] = transform.GetChild(i).localPosition;
        }
        StartCoroutine(OnMoveTo(parentPosition, appeartime));    
    }

    private void OnMouseDown()
    {
        StopCoroutine("OnScaleTo");
        StartCoroutine("OnScaleTo", Vector3.one); //배경 블록의 크기는 1이기 때문에 동일한  1로 확대
    }

    private void OnMouseDrag()
    {
        //현재 모든 블록의 피봇은 x정중앙이므로 x는 그대로 사용
        //y는 y축 블록 개수의 절반 (BlockCount.y 0.5f)에 gap만큼 추가한 위치로 사용
        //Camera.main.ScreenToWorldPoint()로 vector3좌표를 구하면 카메라의 위치인 z -10이 찍히므로 +10을 해야지 블록의 z값이 0이 됨
        Vector3 gap = new Vector3(0, BlockCount.y * 0.5f + 1, 10);
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + gap;
    }

    private void OnMouseUp()
    {
        float x = Mathf.RoundToInt(transform.position.x - BlockCount.x % 2 * 0.5f) + BlockCount.x % 2 * 0.5f;
        float y = Mathf.RoundToInt(transform.position.y - BlockCount.y % 2 * 0.5f) + BlockCount.y % 2 * 0.5f;

        transform.position = new Vector3(x, y, 0);

        //현재 위치에 블록을 배치할 수 있는지 검사
        bool isSuccess = blockArrangeSystem.TryArrangeBlock(this);

        //현재 위치에 블록을 배치할 수 없다면 마지막 위치, 크기로 설정
        if (isSuccess == false)
        {
            StopCoroutine("OnScaleTo");
            StartCoroutine("OnScaleTo", Vector3.one * 0.5f); //현재 크기에서 0.5로 축소
            StartCoroutine(OnMoveTo(transform.parent.position, returntime));
        }

    }

    private IEnumerator OnMoveTo(Vector3 end, float time) //애니메이션
    {
        Vector3 start = transform.position;
        float current = 0f;
        float percent = 0f;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            transform.position = Vector3.Lerp(start, end, curveMovement.Evaluate(percent));

            yield return null;
        }
    }

    private IEnumerator OnScaleTo(Vector3 end)
    {
        Vector3 start = transform.localScale;
        float current = 0f;
        float percent = 0f;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / returntime;

            transform.localScale = Vector3.Lerp(start, end, curveScale.Evaluate(percent));

            yield return null;
        }
    }

}
