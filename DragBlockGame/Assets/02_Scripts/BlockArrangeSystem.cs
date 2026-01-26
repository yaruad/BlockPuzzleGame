using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockArrangeSystem : MonoBehaviour
{
    private Vector2Int blockCount;
    private Vector2 blockHalf;
    private BackGroundBlock[] backGroundBlocks;
    private StageController stageController;

    public void SetUp(Vector2Int blockCount, Vector2 blockHalf, BackGroundBlock[] backGroundBlocks, StageController stageController)
    {
        this.blockCount = blockCount;
        this.blockHalf = blockHalf;
        this.backGroundBlocks = backGroundBlocks;
        this.stageController = stageController;
    }

    public bool TryArrangeBlock(DragBlock block)
    {
        //블록 배치가 가능한지 검사
        for (int i = 0; i < block.ChildBlocks.Length; ++i)
        {
            //자식 블록의 월드 위치 (부모의 월드 좌표 + 자식의 지역 좌표)
            Vector3 position = block.transform.position + block.ChildBlocks[i];
            
            //블록이 맵 내부에 위치한지
            if (!IsBlockInsideMap(position)) return false;
            //현재 위치에 이미 다른 블록이 위치했는지
            if (!IsOtherBlockInThisBlock(position)) return false;
        }
        //블록 배치
        for (int i = 0; i < block.ChildBlocks.Length;  ++i)
        {
            // 자식 블록의 월드 위치(부모의 월드 좌표 + 자식의 지역 좌표)
            Vector3 position = block.transform.position + block.ChildBlocks[i];

            //해당 위치에 있는 배경 블록의 색을 변경하고  채움(BlockState.Fill)로 변경
            backGroundBlocks[PositionToIndex(position)].FillBlock(block.Color);
        }
        return true;
    }

    private bool IsBlockInsideMap(Vector2 position) //매개변수로 받아온 위치가 배경 블록판의 밖인지 검사
    {
        if (position.x < -blockCount.x * 0.5f + blockHalf.x || position.x > blockCount.x * 0.5f - blockHalf.x || 
            position.y < -blockCount.y * 0.5f + blockHalf.y || position.y > blockCount.y * 0.5f - blockHalf.y)
        {
            return false;
        }

        return true;
    }

    private int PositionToIndex(Vector2 position) //매개변수로 받아온 위치 정보를 바탕으로 맵에 배치된 블록의 순번(Index)을 계산해서 반환
    {
        float x = blockCount.x * 0.5f - blockHalf.x + position.x;
        float y = blockCount.y * 0.5f - blockHalf.y - position.y;
        
        return (int)(y * blockCount.x + x);
    }

    private bool IsOtherBlockInThisBlock(Vector2 position) //현재 위치에 있는 블록이 비어있는지 검사 후 결과 반환
    {
        int index = PositionToIndex(position);

        if (backGroundBlocks[index].BlockState == BlockState.Fill)
        {
            return false;
        }
        return true;
    }
}
