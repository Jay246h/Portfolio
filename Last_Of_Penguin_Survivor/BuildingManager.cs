namespace Lop.Survivor.Island
{
    //System
    using System;
    using System.Collections.Generic;

    //Unity
    using UnityEngine;
    using UnityEngine.UI;

    //Project
    using Lop.Survivor.Island.Buildingbase;
    using Structures;
    using static BlockType;
    using System.ComponentModel;


    //건물 방향을 나타내는 이넘
    public enum BuildingDirection
    {
        Top,
        Left,
        Bottom,
        Right
    }

    public class BuildingManager : MonoBehaviour
    {
        public static BuildingManager Instance;

        [Header("BuildingManagerInfo")]
        [SerializeField] private float topDirection = default;
        [SerializeField] private float rightDirection = default;
        [SerializeField] private float bottomDirection = default;
        public BuildingList[] buildingArray;
        [Space(10f)]
        [SerializeField] private Material disPlaceMaterial = null;
        [SerializeField] private Material placeMaterial = null;
        [Header("BuildingManagerUI_Info")]
        [SerializeField] private GameObject buildingUIPlanel = null;
        [SerializeField] private GameObject checkPoint = null;
        public Slider buildingPlaceSlider = null;

        private BuildingDirection buildingDirection;

        private Vector3Int builingPlacePosition;

        private MapSettingManager mapSettingManager;
        private Map map;

        private GameObject checkParentObject;
        private GameObject checkChildObject;

        private string selectBuidling;
        private string builingTypeName;

        public bool isBuildingMode;
        private bool isfirstBuildingCheck;
        //public bool isCanBuilding;
        public bool isCheckBuilding;

        public List<Structure> structureList = new List<Structure>();

        private float buildingRotY;
        private float buildingCoolTime;

        [Header("Network")]
        public BuildingSync buildingSync = null;


        public bool isCheckUnderBuilding = default;

        //프로퍼티
        public string SelectBuildingName => selectBuidling;
        public float BuildingCoolTime => buildingCoolTime;

        public string BuildingTypeName => builingTypeName;

        public Vector3Int BuilingPlacePosition => builingPlacePosition;

        //싱글톤
        private void Awake()
        {
            Instance = this;
        }


        private void Start()
        {
            //변수 초기화
            mapSettingManager = FindObjectOfType<MapSettingManager>();
            buildingSync = GetComponent<BuildingSync>();

            map = mapSettingManager.Map;
            buildingRotY = 180f;

            isfirstBuildingCheck = false;
            //isCanBuilding = false;
            isCheckUnderBuilding = false;



            if (buildingSync != null)
            {
                if (buildingSync.isServer) { mapSettingManager.PlaceTree(); }
            }
            else
            {
                mapSettingManager.PlaceTree();
            }
        }

        private void Update()
        {
            //건축물의 아래있는 블럭을 검사해서 일정 블럭이 없다면 건축물을 파괴시킴
            if (isCheckUnderBuilding)
            {
                for (int i = 0; i < structureList.Count; i++)
                {
                    structureList[i].CheckUnderBuilding();
                }
                isCheckUnderBuilding = false;
            }
        }


        //건축물의 설치를 검사하는 함수
        public void CheckBuilding(float playerDirection, string buildingType, Vector3 position)
        {
            //설치할 건축물의 이름이 null이라면 설치를 검사하지 않음
            if (buildingType == null) return;

            //설치될 건축물 오브젝트를 가지고옴
            GameObject buildingObject = FindBuildingObject(buildingType);

            //오브젝트가 null이라면 설치를 검사하지 않음
            if (buildingObject == null) return;

            int checkDirectionX = 0;
            int checkDirectionZ = 0;

            int posX = Mathf.FloorToInt(position.x);
            int posY = Mathf.FloorToInt(position.y);
            int posZ = Mathf.FloorToInt(position.z);

            builingTypeName = buildingType;
            isBuildingMode = true;

            //건축물 오즈젝트의 데이터를 저장함
            BuildingBase buildingData = buildingObject.GetComponent<BuildingBase>();

            //건축물 오브젝트의 크기를 할당함
            Vector3Int buildingSize = buildingData.BuilingSize;

            //건축물 오브젝트의 설치 속도를 할당함
            buildingCoolTime = buildingData.BuildingCoolTime;

            //설치될 위치
            Vector3Int checkPosition = Vector3Int.right * posX +
                                        Vector3Int.up * posY +
                                        Vector3Int.forward * posZ;

            //플레이어가 바라보고 있는 방향에 따라 건축물의 방향이 바뀜
            if (playerDirection <= 180)
            {
                if (playerDirection <= topDirection) buildingDirection = BuildingDirection.Top;
                else if (playerDirection >= bottomDirection) buildingDirection = BuildingDirection.Bottom;
                else buildingDirection = BuildingDirection.Right;
            }
            else
            {
                if (playerDirection >= 360 - topDirection) buildingDirection = BuildingDirection.Top;
                else if (playerDirection <= rightDirection) buildingDirection = BuildingDirection.Right;
                else buildingDirection = BuildingDirection.Left;
            }

            //건축물의 방향에 따라 검사하는 영역을 바뀌게함
            switch (buildingDirection)
            {
                case BuildingDirection.Top:
                    checkDirectionX = 1;
                    checkDirectionZ = 1;
                    break;
                case BuildingDirection.Right:
                    checkDirectionX = 1;
                    checkDirectionZ = -1;
                    break;
                case BuildingDirection.Bottom:
                    checkDirectionX = -1;
                    checkDirectionZ = -1;
                    break;
                case BuildingDirection.Left:
                    checkDirectionX = -1;
                    checkDirectionZ = 1;
                    break;
            }

            Vector3Int buildingPlacePos = Vector3Int.right * (buildingSize.x * checkDirectionX) +
                                            Vector3Int.up * buildingSize.y +
                                            Vector3Int.forward * (buildingSize.z * checkDirectionZ);

            //건축물의 검사 영역의 최대값을 계산함
            int buildingMaxX = posX + buildingSize.x * checkDirectionX;
            int buildingMaxZ = posZ + buildingSize.z * checkDirectionZ;
            int buildingMaxY = posY + buildingSize.y;

            //건축물의 검사 영역의 최대값을 Vector3Int 형태로 변환함
            Vector3Int buildingCheckArea = Vector3Int.right * buildingMaxX +
                                            Vector3Int.up * buildingMaxY +
                                            Vector3Int.forward * buildingMaxZ;

            if (CheckBuildingPlaceArea(buildingCheckArea, checkPosition, checkDirectionX, checkDirectionZ))
            {
                CheckBuildingPlacePosition(buildingPlacePos, checkPosition);
                ChangeBuildingMaterial(placeMaterial, builingTypeName, builingPlacePosition);
                //isCanBuilding = true;
                buildingUIPlanel.SetActive(true);
            }
            else
            {
                CheckBuildingPlacePosition(buildingPlacePos, checkPosition);
                ChangeBuildingMaterial(disPlaceMaterial, builingTypeName, builingPlacePosition);
                //isCanBuilding = false;
                buildingUIPlanel.SetActive(false);
            }
        }

        //홀로그램 건물의 매터리얼을 바꾸는 함수
        private void ChangeBuildingMaterial(Material setMaterial, string buildingTypeName, Vector3Int placePosition)
        {
            checkParentObject = GameObject.FindGameObjectWithTag(buildingTypeName);
            checkChildObject = checkParentObject.transform.GetChild(0).gameObject;
            checkChildObject.SetActive(true);
            checkChildObject.transform.localPosition = new Vector3(checkChildObject.transform.localPosition.x, mapSettingManager.parentChunkYPos * 2, checkChildObject.transform.localPosition.z);

            //건물 검사 초깃값 초기화
            if (!isfirstBuildingCheck)
            {
                checkParentObject.transform.position = placePosition;
                checkChildObject.transform.rotation = Quaternion.Euler(0, 180, 0);
                isfirstBuildingCheck = true;
            }

            if (checkChildObject.GetComponent<MeshRenderer>() == null)
            {
                checkChildObject.GetComponent<SkinnedMeshRenderer>().material = setMaterial;
            }
            else
            {
                checkChildObject.GetComponent<MeshRenderer>().material = setMaterial;
            }

            checkParentObject.transform.position = placePosition;
        }

        //건축물 설치 영역검사
        private bool CheckBuildingPlaceArea(Vector3Int buildingCheckArea, Vector3Int placePosition, int dirX, int dirZ)
        {

            //맵을 벗어나면 false 반환
            if (buildingCheckArea.x < 0 || buildingCheckArea.x > (mapSettingManager.MapWidthChunkValue * ChunkData.ChunkWidthValue) ||
                buildingCheckArea.z < 0 || buildingCheckArea.z > (mapSettingManager.MapLengthChunkValue * ChunkData.ChunkLengthValue) ||
                buildingCheckArea.y < mapSettingManager.ParentChunkYPos || buildingCheckArea.y > ChunkData.ChunkHeightValue)
            {
                return false;
            }

            //건축물 크기만큼의 영역을 3중 for문으로 검사함
            for (int y = placePosition.y; y <= buildingCheckArea.y; y++)
            {
                for (int x = placePosition.x; (dirX > 0 ? x < buildingCheckArea.x : x > buildingCheckArea.x); x += dirX)
                {
                    for (int z = placePosition.z; (dirZ > 0 ? z < buildingCheckArea.z : z > buildingCheckArea.z); z += dirZ)
                    {
                        if (map.MapBlock[x, y, z].id != Air)
                        {
                            isCheckBuilding = false;
                            return false;
                        }
                        if (map.MapBlock[x, placePosition.y - 1, z].id == Air || map.MapBlock[x, placePosition.y - 1, z].id == Water)
                        {
                            isCheckBuilding = false;
                            return false;
                        }
                    }
                }
            }

            isCheckBuilding = true;
            return true;
        }

        //건축물 설치 위치 검사
        private void CheckBuildingPlacePosition(Vector3Int buildingPlacePos, Vector3Int placePosition)
        {
            int[] buildingPlaceVector = { buildingPlacePos.x + 1, buildingPlacePos.y, buildingPlacePos.z + 1 };
            int[] placePositionVector = { placePosition.x, placePosition.y, placePosition.z };

            for (int i = 0; i < buildingPlaceVector.Length; i++)
            {
                if (buildingPlaceVector[i] < 0)
                {
                    placePositionVector[i] += buildingPlaceVector[i];
                }

                builingPlacePosition = Vector3Int.right * placePositionVector[0] +
                                        Vector3Int.up * placePositionVector[1] +
                                        Vector3Int.forward * placePositionVector[2];
            }
        }

        //건축물의 설치 확인을 멈추는 함수
        public void StopCheckBuilding()
        {
            if (checkChildObject == null) return;

            buildingRotY = 180f;

            checkChildObject.SetActive(false);
            buildingUIPlanel.SetActive(false);

            checkParentObject.transform.position = Vector3.zero;

            checkChildObject = null;
            checkParentObject = null;

            isBuildingMode = false;
            //isCanBuilding = false;
            isCheckBuilding = false;
            isfirstBuildingCheck = false;
        }

        //건축물을 회전시키는 함수
        public void SpinBuilding()
        {
            if (checkChildObject == null) return;
            buildingRotY += 90f;

            if (buildingRotY > 270f)
            {
                buildingRotY = 180;
            }

            checkChildObject.transform.rotation = Quaternion.Euler(0, buildingRotY, 0);
        }

        //건축물을 맵위에 설치하는 함수 (로컬)
        public GameObject PlaceBuildingOnMapLocal(string builingTypeName, Vector3Int builingPlacePosition)
        {
            GameObject building = PlaceBuildingOnMapSync(builingTypeName, builingPlacePosition, buildingRotY);
            StopCheckBuilding();

            return building;
        }

        //건축물을 맵위에 설치하는 함수 (동기화)
        public GameObject PlaceBuildingOnMapSync(string buildingType, Vector3Int position, float rotY)
        {
            if (buildingType == null) return null;

            if (buildingSync.isServer) // 서버일 때
            {
                GameObject builingObject = FindBuildingObject(buildingType);
                GameObject placeBuilding = Instantiate(builingObject, position, Quaternion.identity);

                placeBuilding.transform.GetChild(0).transform.rotation = Quaternion.Euler(0, rotY, 0);
                placeBuilding.transform.GetChild(0).transform.localPosition = new Vector3(placeBuilding.transform.GetChild(0).transform.localPosition.x, mapSettingManager.ParentChunkYPos - 1, placeBuilding.transform.GetChild(0).transform.localPosition.z);
                placeBuilding.GetComponent<BuildingMeshSync>().meshRotY = rotY;
                placeBuilding.GetComponent<BuildingMeshSync>().meshPosY = placeBuilding.transform.GetChild(0).transform.position.y;

                buildingSync.SpawnBuilding(placeBuilding);

                return placeBuilding;
            }
            else // 클라이언트일 때
            {
                buildingSync.SendBuildingSpawnMessage(buildingType, position.x, position.y, position.z, rotY);

                return null;
            }
        }

        //설치되어 있는 건축물을 파괴하는 함수
        public void DestroyBuilding(GameObject building)
        {
            Vector3 buildingPos = building.transform.position;
            buildingPos.y += 1f;

            map.GetChunkFromVector3(buildingPos).EditVoxel(buildingPos, mapSettingManager.FindBLockType(Air));

            if (buildingSync != null)
                buildingSync.DestroyBuilding(building);
        }

        //설치할 건축물의 이름을 찾는함수
        public void FindBuildingName(string Name)
        {
            selectBuidling = Name;
            StopCheckBuilding();
        }

        //건축물 오브젝트를 찾는 함수
        private GameObject FindBuildingObject(string buildingType)
        {
            BuildingList building = Array.Find(buildingArray, x => x.buildingName == buildingType);
            if (building == null)
            {
                return null;
            }

            return building.buildingObject;
        }
    }
}
