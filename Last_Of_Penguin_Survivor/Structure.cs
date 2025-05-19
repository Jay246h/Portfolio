namespace Lop.Survivor.Structures
{

    //System
    using System.Collections;
    using System.Collections.Generic;

    //UnityEngine
    using UnityEngine;

    //Project
    using Lop.Survivor.inventroy;
    using Lop.Survivor.Island;
    using static BlockType;

    //구조물 들의 부모 스크립트
    public class Structure : MonoBehaviour
    {
        [Header("StructureInfo")]
        [SerializeField] protected Vector3Int structureSize = default;
        [SerializeField] private bool isPlaceBuilding;
        [SerializeField] private float destroyBuildingBlockCount = default;


        protected Inventory inventory;
        protected List<Vector3Int> underPosList;
        protected int counter = default;

        public DropItemData[] dropItemData;


        protected virtual void Start()
        {
            BuildingManager.Instance.structureList.Add(this);

            if (LopNetworkManager.GetPlayer() != null)
                inventory = LopNetworkManager.GetPlayer().GetComponentInChildren<Inventory>();
            else
                StartCoroutine(Co_GetInventory());

            underPosList = new List<Vector3Int>();

            if (isPlaceBuilding)
            {
                isPlaceBuilding = true;

                for (int x = 0; x < structureSize.x; x++)
                {
                    for (int z = 0; z < structureSize.z; z++)
                    {
                        underPosList.Add(new Vector3Int((int)transform.position.x + x,
                                                        (int)transform.position.y - 2,
                                                        (int)transform.position.z + z));
                    }
                }
            }
        }

        private IEnumerator Co_GetInventory()
        {
            yield return new WaitForSeconds(1f);

            inventory = LopNetworkManager.GetPlayer().GetComponentInChildren<Inventory>();
        }
        public void CheckUnderBuilding()
        {
            if (isPlaceBuilding)
            {
                for (int i = 0; i < underPosList.Count; i++)
                {
                    if (MapSettingManager.Instance.Map.GetVoxelType(underPosList[i]) == Air
                       || MapSettingManager.Instance.Map.GetVoxelType(underPosList[i]) == Water)
                    {
                        counter++;
                    }
                }

                if (counter == destroyBuildingBlockCount)
                {
                    counter = 0;
                    DestroyStructure();
                }
                else
                {
                    counter = 0;
                }
            }
        }

        public void DropItem()
        {

            if (inventory == null)
            {
                Debug.LogError("Inventory 객체를 찾을 수 없습니다.");
                return;
            }

            foreach (var dropItem in dropItemData)
            {
                for (int i = 0; i < dropItem.dropCount; i++)
                {
                    inventory.DropItemGameObject(dropItem.itemID, new Vector3(this.transform.position.x, this.transform.position.y + 0.5f, this.transform.position.z));

                }
                return;
            }
        }

        public void DropFurit()
        {
            if (inventory == null)
            {
                Debug.LogError("Inventory 객체를 찾을 수 없습니다.");
                return;
            }

            // 과일 종류 배열
            string[] fruits = new string[4] { "WaterMelon", "StrawBerry", "Carrot", "PineApple" };

            foreach (var dropItem in dropItemData)
            {
                if (dropItem.itemID == "Fruit") // "Fruit"인 경우에만 드랍
                {
                    for (int i = 0; i < dropItem.dropCount; i++)
                    {
                        // 무작위 과일 드랍
                        inventory.DropItemGameObject(fruits[Random.Range(0, fruits.Length)],
                            new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z));
                    }
                    return; // 과일 드랍 후 함수 종료
                }
            }
        }

        public void DestroyStructure()
        {
            BuildingManager.Instance.structureList.Remove(this);
            BuildingManager.Instance.DestroyBuilding(gameObject);
        }

    }
}
