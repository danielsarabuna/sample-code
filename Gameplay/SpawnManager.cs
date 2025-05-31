using UnityEngine;
using View;

namespace Gameplay
{
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField] private Transform _enemySpawnPoint;

        private void Awake()
        {
            if (_enemySpawnPoint.transform.childCount > 0)
                Destroy(_enemySpawnPoint.transform.GetChild(0).gameObject);
        }

        public void SpawnEnemy(EnemyEntityView enemyEntityView)
        {
            enemyEntityView.transform.SetParent(_enemySpawnPoint);
            enemyEntityView.transform.localPosition = Vector3.zero;
        }
    }
}