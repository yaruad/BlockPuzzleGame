using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockState { Empty = 0, Fill = 1 }

public class BackGroundBlock : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public BlockState BlockState { get; private set; } //배경 블록 상태

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        BlockState = BlockState.Empty;
    }

    public void FillBlock(Color color)
    {
        BlockState = BlockState.Fill;
        spriteRenderer.color = color;
    }

    public void EmptyBlock()
    {
        BlockState = BlockState.Empty;
        StartCoroutine("ScaleTo", Vector3.zero);
    }

    private IEnumerator ScaleTo(Vector3 end)
    {
        Vector3 start = transform.localScale;
        float current = 0f;
        float percent = 0f;
        float time = 0.15f;

        while (percent < 1f)
        {
            current += Time.deltaTime;
            percent = current / time;

            transform.localScale = Vector3.Lerp(start, end, percent);

            yield return null;
        }

        //축소 애니메이션이 끝나면 색을 흰색으로 크기를 원래크기로 변경
        spriteRenderer.color = Color.white;
        transform.localScale = Vector3.one;
    }
}
