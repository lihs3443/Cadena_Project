using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    /// <summary>
    /// 스테이지 정보들의 집합 객체
    /// </summary>
    [System.Serializable]
    public class StageInfo
    {
        [Tooltip("스테이지의 인덱스 번호.\n정보를 가져오고 스테이지를 생성하는데 사용하기때문에 겹치면 안된다.")] public uint stageIndex;
        //추후 스테이지 이름등의 정보는 stageIndex와 csvReader를 활용하여 가져오도록 설계하는 편이 좋아보임
        [ReadOnly]
        [Tooltip("스테이지 이름")] public string stageName;
    }
    /// <summary>
    /// 스테이지 구성 요소들의 집합 객체
    /// </summary>
    [System.Serializable]
    public class StageComponents
    {
        [Tooltip("처음 스테이지에 들어왔을때 플레이어의 스폰 위치")] public Transform firstSpawnPoint;
        [Tooltip("해당 포인트에 들어오면 스테이지 클리어로 판정")] public Transform stageEndPoint;
    }
    [Header("스테이지 정보")]
    public StageInfo stageInfo = new StageInfo();
    [Header("스테이지 구성 요소")]
    public StageComponents stageComponents = new StageComponents();

    public Vector2 currentSpawnPoint { get; private set; }

    private void Awake()
    {
        currentSpawnPoint = stageComponents.firstSpawnPoint.position;
    }

    public void RespawnPlayer(GameObject _playerObj)
    {
        _playerObj.transform.position = currentSpawnPoint;
    }

    /// <summary>
    /// 플레이어 사망 등 이후 돌려보내는 currentSpawnPoint를 변경하는 용도
    /// </summary>
    public void ChangeSpawnPosition(Vector2 _position)
    {
        currentSpawnPoint = _position;
    }
}

///스테이지가 가져야할 요소
///스폰 포인트
///시작점과 끝점
///(부가요소)
///클리어 타임
