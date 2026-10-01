using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

    public abstract class EntitySpawner : MonoBehaviour
    {
        // SessionManager에서 주입
        [SerializeField] 
        protected DamagePopupManager damagePopupManager;
        [SerializeField] 
        protected KillLogManager killLogManager;
        [SerializeField] 
        protected ScoreBlockSpawner scoreBlockSpawner;

        [Header("스킨 오브젝트")]
        [SerializeField] 
        protected Mesh[] skinnedMesh;
        [SerializeField] 
        protected Material[] material;

        protected bool isInitialized = false;
        [Header("랜덤 위치 스폰")]
        [SerializeField]
        protected bool useRandomSpawnPoint = false;
        private Vector3 standardPosition = new Vector3(-40, 0.5f, -40);

        private float xSize = 20 * 5;
        private float ySize = 20 * 5;

        protected EntityBasicData entityBasicData;

        public virtual void Setup(DamagePopupManager damagePopupManager, KillLogManager killLogManager, ScoreBlockSpawner scoreBlockSpawner,
            EntityBasicData basicData)
        {
            this.damagePopupManager = damagePopupManager;
            this.killLogManager = killLogManager;
            this.scoreBlockSpawner = scoreBlockSpawner;
            isInitialized = true;
            entityBasicData = basicData;
        }

        protected virtual void ValidateSetup()
        {
            if (!isInitialized)
            {
                Debug.LogError($"{GetType().Name}이 초기화되지 않았습니다!");
            }
        }
        protected void ApplyRandomSkin(Entity entity)
        {
            if (skinnedMesh != null && skinnedMesh.Length > 0 &&
                material != null && material.Length > 0)
            {
                entity.SetSkin(skinnedMesh[Random.Range(0, skinnedMesh.Length)],
                              material[Random.Range(0, material.Length)]);
            }
        }

        protected Vector3 GetRandomSpawnPoint()
        {   
            if(! useRandomSpawnPoint)
            {
                return Vector3.zero;
            }

            int maxNum = 10;
            for(int i = 0; i < maxNum; ++i)
            {
            Vector3 spawnPoint = standardPosition +
            new Vector3(Mathf.Clamp(Random.Range(0, (int)xSize) + i * 10,0,xSize), 0, 0) +
            new Vector3(0, 0, Mathf.Clamp(Random.Range(0, (int)ySize),0,ySize));

                Debug.DrawLine(spawnPoint, spawnPoint + Vector3.up * 10, Color.red, 5f);
            int targetLayer = int.MaxValue ^ 1 ^ 512;
                if (Physics.CheckBox(spawnPoint, new Vector3(4f, 0.1f, 4f), Quaternion.identity, targetLayer))
                {
                    continue;
                }
                return spawnPoint;
            }

            return Vector3.zero;
        }
    }