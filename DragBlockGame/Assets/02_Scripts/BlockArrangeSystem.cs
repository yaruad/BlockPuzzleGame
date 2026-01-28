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

        //블록 배치후처리
        stageController.AfterBlockArrangement(block);

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

    //드래그 블록에 포함된 자식 블록들이 현재 블록에 배치가능한지 검사
    public bool IsPossibleArrangement(DragBlock block)
    {
        for (int y = 0; y < blockCount.y; ++y)
        {
            for (int x = 0;  x < blockCount.x; ++x)
            {
                int count = 0;
                Vector3 position = new Vector3(-blockCount.x * 0.5f + blockHalf.x + x, blockCount.y * 0.5f + blockHalf.y - y, 0);

                //블록 개수가 홀수이면 좌표를 그대로 사용, 짝수면 (가로0.5 세로-0.5)를 더해서 사용
                position.x = block.BlockCount.x%2 == 0 ? position.x + 0.5f : position.x;
                position.y = block.BlockCount.y%2 == 0 ? position.y - 0.5f : position.y;

                //현재 블록에 소속된 모든 자식 블록의 위치를 기준으로 맵 내부인지 다른 블록 없는지 검사
                for (int i = 0; i <block.ChildBlocks.Length; ++i)
                {
                    Vector3 blockPosition = block.ChildBlocks[i] + position;

                    if (!IsBlockInsideMap(blockPosition)) break;
                    if (!IsOtherBlockInThisBlock(blockPosition)) break;

                    count++;
                }

                //위에 for문에서 break가 걸리는 경우 count와 자식 블록의 수가 다르기 때문에 배치 불가능 (return false)
                //개수가 같을 경우 배치 가능(retun true)
                if (count == block.ChildBlocks.Length)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
