using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundBlockSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject blockPrefab;
    [SerializeField]
    private int orderInLayer;

    //격자 형태로 생성되는 블럭 개수, 블럭 하나의 절반 크기
    //private Vector2Int blockCount = new Vector2Int(10, 10); 
    //private Vector2 blockHalf = new Vector2(0.5f, 0.5f);

    private void Awake()
    {
        
    }

    public BackGroundBlock[] SpawnBlocks(Vector2Int blockCount, Vector2 blockHalf)
    {
        BackGroundBlock[] blocks = new BackGroundBlock[blockCount.x *  blockCount.y];

        for (int y = 0; y < blockCount.y; ++y)
        {
            for (int x = 0; x < blockCount.x; ++x)
            {
                //블록판 중앙이 000이 되도록
                float px = -blockCount.x * 0.5f + blockHalf.x + x;
                float py = blockCount.y * 0.5f - blockHalf.y - y;
                Vector3 position = new Vector3(px, py, 0);
                //블록 생성 (원본 프리팹, 위치, 회전, 부모 Transform)
                GameObject clone = Instantiate(blockPrefab, position, Quaternion.identity, transform);
                //방금 생성한 블록이 생성되는 순서 설정
                clone.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer;
                //생성한 모든 블록의 정보를 반환하기 위해 blocks[] 배열에 저장
                blocks[y * blockCount.x + x] = clone.GetComponent<BackGroundBlock>();
            }
        }
        return blocks;
    }
}
