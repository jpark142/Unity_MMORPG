using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawningPool : MonoBehaviour
{
    [SerializeField]
    int _monsterCount = 0;
    int _reserveCount = 0;

    [SerializeField]
    int _keepMonsterCount = 0;

    [SerializeField]
    Vector3 _spawnPos;

    [SerializeField]
    float _spawnRadius = 15.0f;

    [SerializeField]
    float _spawnTime = 5.0f;

    public void AddMonsterCount(int value)
    {
        _monsterCount += value;
    }
    public void SetKeepMonsterCount(int count)
    {
        _keepMonsterCount = count;
    }

    void Start()
    {
        Managers.Game.OnSpawnEvent += AddMonsterCount;
    }

    void Update()
    {
        while(_monsterCount + _reserveCount < _keepMonsterCount)
        {
            StartCoroutine("ReserveSpawn");
        }

    }
    IEnumerator ReserveSpawn()
    {
        _reserveCount++;
        yield return new WaitForSeconds(Random.Range(1, _spawnTime));
        GameObject go = Managers.Game.Spawn(Define.WorldObject.Monster, "Knight");
        NavMeshAgent nma = go.GetOrAddComponent<NavMeshAgent>();

        Vector3 randPos;
        while(true) // 스폰 가능 위치가 나올 때까지 돌림
        {
            Vector3 randDir = Random.insideUnitSphere * Random.Range(0, _spawnRadius);
            randDir.y = 0;
            randPos = _spawnPos + randDir;

            // 갈 수 있나
            NavMeshPath path = new NavMeshPath();
            if (nma.CalculatePath(randPos, path))
                break;
        }

        go.transform.position = randPos;
        _reserveCount--;
    }
}
