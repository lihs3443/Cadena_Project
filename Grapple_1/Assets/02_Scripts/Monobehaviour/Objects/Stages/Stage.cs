using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    /// <summary>
    /// �������� �������� ���� ��ü
    /// </summary>
    [System.Serializable]
    public class StageInfo
    {
        [Tooltip("���������� �ε��� ��ȣ.\n������ �������� ���������� �����ϴµ� ����ϱ⶧���� ��ġ�� �ȵȴ�.")] public uint stageIndex;
        //���� �������� �̸����� ������ stageIndex�� csvReader�� Ȱ���Ͽ� ���������� �����ϴ� ���� ���ƺ���
        [ReadOnly]
        [Tooltip("�������� �̸�")] public string stageName;
    }
    /// <summary>
    /// �������� ���� ��ҵ��� ���� ��ü
    /// </summary>
    [System.Serializable]
    public class StageComponents
    {
        [Tooltip("ó�� ���������� �������� �÷��̾��� ���� ��ġ")] public Transform firstSpawnPoint;
        [Tooltip("�ش� ����Ʈ�� ������ �������� Ŭ����� ����")] public Transform stageEndPoint;
    }
    [Header("�������� ����")]
    public StageInfo stageInfo = new StageInfo();
    [Header("�������� ���� ���")]
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
    /// �÷��̾� ��� �� ���� ���������� currentSpawnPoint�� �����ϴ� �뵵
    /// </summary>
    public void ChangeSpawnPosition(Vector2 _position)
    {
        currentSpawnPoint = _position;
    }
}

///���������� �������� ���
///���� ����Ʈ
///�������� ����
///(�ΰ����)
///Ŭ���� Ÿ��
