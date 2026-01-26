using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    [SerializeField]
    private BackGroundBlockSpawner backGroundBlockSpawner;
    [SerializeField]
    private BackGroundBlockSpawner foreGroundBlockSpawner;
    [SerializeField]
    private DragBlockSpawner dragBlockSpawner;
    [SerializeField]
    private BlockArrangeSystem blockArrangeSystem; //블록 배치

    private BackGroundBlock[] backGroundBlocks;     //생성한 배경 블록 정보 저장
    private int currentDragBlockCount;      //현재 남은 드래그 블록 수

    private readonly Vector2Int blockCount = new Vector2Int(10,10);     //블록 판에 배치되는 블록 수
    private readonly Vector2 blockHalf = new Vector2(0.5f, 0.5f);       
    private readonly int maxDragBlockCount = 3;     //한번에 생성할 수 있는 드래그 블록 수

    private void Awake()
    {
        //뒷 배경으로 사용되는 배경 블록판 생성
        backGroundBlockSpawner.SpawnBlocks(blockCount, blockHalf);

        //드래그 블록을 배치할 때 색상이 변경되는 배경 블록판 생성
        backGroundBlocks = new BackGroundBlock[blockCount.x * blockCount.y];
        backGroundBlocks = foreGroundBlockSpawner.SpawnBlocks(blockCount, blockHalf);

        //블록 배치 시스템
        blockArrangeSystem.SetUp(blockCount, blockHalf, backGroundBlocks, this);

        SpawnDragBlocks();
    }

    private void SpawnDragBlocks()
    {
        currentDragBlockCount = maxDragBlockCount;
        dragBlockSpawner.SpawnBlock();
    }
}
