using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragBlockSpawner : MonoBehaviour
{
    [SerializeField]
    private BlockArrangeSystem blockArrangeSystem;
    [SerializeField]
    private Transform[] blockSpawnPoints;
    [SerializeField]
    private GameObject[] blockPrefabs;
    [SerializeField]
    private Vector3 SpawnGapAmount = new Vector3(10, 0, 0); //처음 생성할 때 부모와 떨어진 거리

    //외부에서 드래그 블록의 부모Transform[] 배열 정보 열람
    public Transform[] BlockSpawnPoints => blockSpawnPoints;



    public void SpawnBlock()
    {
        StartCoroutine("OnSpawnBlocks");
    }

    private IEnumerator OnSpawnBlocks()
    {
        //드래그 블록 3개(blockParent.Length) 생성
        for (int i = 0; i < blockSpawnPoints.Length; ++i)
        {
            yield return new WaitForSeconds(0.1f);

            //생성할 드래그 블록 순번
            int index = Random.Range(0, blockPrefabs.Length);

            Vector3 spawnPosition = blockSpawnPoints[i].position + SpawnGapAmount;

            GameObject clone = Instantiate(blockPrefabs[index], spawnPosition, Quaternion.identity, blockSpawnPoints[i]);

            clone.GetComponent<DragBlock>().SetUp(blockArrangeSystem, blockSpawnPoints[i].position); //드래그 블록 생성하고, 부모의 위치까지 이동하는 애니메이션 재생
            
        }
    }
}
