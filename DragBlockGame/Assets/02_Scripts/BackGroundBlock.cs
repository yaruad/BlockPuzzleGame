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
}
