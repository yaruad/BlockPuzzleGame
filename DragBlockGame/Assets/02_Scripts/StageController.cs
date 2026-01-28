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

    private List<BackGroundBlock> filledBlockList; //줄이 완성된 블록들을 삭제하기 위해 임시 저장하는 리스트

    private void Awake()
    {
        //줄이 완성된 블록들을 삭제하기 위해 임시 저장하는 리스트
        filledBlockList = new List<BackGroundBlock>();

        //뒷 배경으로 사용되는 배경 블록판 생성
        backGroundBlockSpawner.SpawnBlocks(blockCount, blockHalf);

        //드래그 블록을 배치할 때 색상이 변경되는 배경 블록판 생성
        backGroundBlocks = new BackGroundBlock[blockCount.x * blockCount.y];
        backGroundBlocks = foreGroundBlockSpawner.SpawnBlocks(blockCount, blockHalf);

        //블록 배치 시스템
        blockArrangeSystem.SetUp(blockCount, blockHalf, backGroundBlocks, this);

        StartCoroutine(SpawnDragBlocks());
    }

    private IEnumerator SpawnDragBlocks()
    {
        currentDragBlockCount = maxDragBlockCount;
        dragBlockSpawner.SpawnBlock();

        //드래그 블록들의 이동이 완료될 때까지 대기
        yield return new WaitUntil(() => IsCompleteSpawnBlocks());
    }

    //드래그 블록을 생성하고, 등장 애니메이션을 재생할 때
    //모든 드래그 블록의 등장 애니메이션이 종료되었는지 검사
    private bool IsCompleteSpawnBlocks()
    {
        int count = 0;
        for (int i = 0; i < dragBlockSpawner.BlockSpawnPoints.Length; ++i)
        {
            if (dragBlockSpawner.BlockSpawnPoints[i].childCount != 0 &&
                dragBlockSpawner.BlockSpawnPoints[i].GetChild(0).localPosition == Vector3.zero)
            {
                count++;
            }
        }
        return count == dragBlockSpawner.BlockSpawnPoints.Length;
    }

    public void AfterBlockArrangement (DragBlock block)
    {
        StartCoroutine("OnAfterBlockArrangement", block);
    }

    private IEnumerator OnAfterBlockArrangement(DragBlock block)
    {
        //배치가 완료되면 블록 삭제
        Destroy(block.gameObject);

        //완성된 줄이 있는지 검사하고 완성된 줄의 블록을 별도로 저장
        int filledLineCount = CheckFilledLine();

        //줄이 완성된 블록들을 삭제(마지막에 배치한 블록을 기준으로 퍼져나가듯이)
        yield return StartCoroutine(DestroyFilledBlocks(block));

        currentDragBlockCount--;

        if (currentDragBlockCount <= 0)
        {
            yield return StartCoroutine(SpawnDragBlocks());
        }
        yield return new WaitForEndOfFrame();

        if (IsGameOver())
        {
            Debug.Log("GameOver");
        }
    }

    private int CheckFilledLine()
    {
        int filledLineCount = 0;

        filledBlockList.Clear();

        //가로줄 검사
        for (int y = 0; y < blockCount.y; ++y)
        {
            int fillBlockCount = 0;
            for (int x = 0; x < blockCount.x; ++x)
            {
                //해당 블록이 채워져 있으면 fillBlockCount 1증가
                if (backGroundBlocks[y * blockCount.x + x].BlockState == BlockState.Fill) fillBlockCount++;
            }

            //완성된 줄이 있으면 해당 줄의 모든 배경 블록을 filledBlockList에 저장
            if (fillBlockCount == blockCount.x)
            {
                for (int x = 0; x < blockCount.x; ++x)
                {
                    filledBlockList.Add(backGroundBlocks[y*blockCount.x + x]);
                }

                filledLineCount++;
            }
        }

        //세로줄 검사
        for (int x = 0; x < blockCount.x; ++x)
        {
            int fillBlockCount = 0;
            for(int y = 0; y < blockCount.y; ++y)
            {
                //해당 블록이 채워져 있으면 fillBlockCount 1증가
                if (backGroundBlocks[y * blockCount.x + x].BlockState == BlockState.Fill) fillBlockCount++;
            }

            if (fillBlockCount == blockCount.y)
            {
                for (int y = 0;y < blockCount.y; ++y)
                {
                    filledBlockList.Add(backGroundBlocks[y * blockCount.x + x]);
                }
                filledLineCount++;
            }
        }
        return filledLineCount;
    }

    private IEnumerator DestroyFilledBlocks(DragBlock block)
    {
        //마지막에 배치한 블록과 거리가 가까운 거리 순으로 정렬
        filledBlockList.Sort((a, b) => 
        (a.transform.position -block.transform.position).sqrMagnitude.CompareTo((b.transform.position -  block.transform.position).sqrMagnitude));

        //filledBlockList에 저장되어 있는 배경 블록을 순서대로 초기화
        for (int i = 0; i < filledBlockList.Count; ++i)
        {
            filledBlockList[i].EmptyBlock();
            yield return new WaitForSeconds(0.01f);
        }

        filledBlockList.Clear();
    }

    private bool IsGameOver()
    {
        int dragBlockCount = 0;
        //배치 가능한 블록이 남았을 때
        for (int i = 0; i < dragBlockSpawner.BlockSpawnPoints.Length; ++i)
        {
            //dragBlockSpawner.BlockSpawnPoints[i]에 자식이 있으면 (자식 = 드래그 블록)
            if (dragBlockSpawner.BlockSpawnPoints[i].childCount != 0)
            {
                dragBlockCount++;

                if (blockArrangeSystem.IsPossibleArrangement(dragBlockSpawner.BlockSpawnPoints[i].GetComponentInChildren<DragBlock>()))
                {
                    return false;
                }

            }
        }
        //dragBlockCount는 현재 남아있는 드래그 블록 개수인데 맵에 배치할 수 있는 드래그 블록이 있으면
        //if (blockArrangeSystem.IsPossibleArrangement())에서 ture로 메소드를 빠져나가기 때문에
        //이 반환 코드가 실행되고 dragBlockCount가 0이 아니면 게임 오버
        return dragBlockCount != 0;     
    }
}
