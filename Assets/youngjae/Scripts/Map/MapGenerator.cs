using UnityEditor;
using UnityEngine;

namespace Map
{

    public class MapGenerator : MonoBehaviour
    {
        [Header("Map Objects")]
        public GameObject floorPrefab; // Prefab for the tile
        public float floorSize = 4.0f; // Size of each tile
        public GameObject wallPrefab; // Prefab for the wall
        public float wallWidth = 1f;
        public float wallLength = 4f;

        [Header("Map Info")]
        public int mapWidth = 10; // Width of the map
        public int mapHeight = 10; // Height of the map

        GameObject floor = null;
        GameObject wall = null;

        [ContextMenu("Generate Map")]
        public void GenerateMap()
        {
            // 맵 리셋
            while (transform.childCount > 0)
            {
                if (!Application.isPlaying)
                {
                    Undo.DestroyObjectImmediate(transform.GetChild(0).gameObject);
                }
                else
                {
                    DestroyImmediate(transform.GetChild(0).gameObject);
                }
            }

            // 1) Floor 컨테이너 만들고 타일 배치
            floor = new GameObject("Floor");
            floor.transform.SetParent(transform);
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    Vector3 pos = new Vector3(x * floorSize, 0, y * floorSize)
                                  + new Vector3(floorSize / 2f, 0, floorSize / 2f);
                    Instantiate(floorPrefab, pos, Quaternion.identity, floor.transform);
                }
            }

            // 2) Wall 컨테이너 만들기
            wall = new GameObject("Wall");
            wall.transform.SetParent(transform);

            // — 남/북 벽 생성
            for (int x = 0; x < mapWidth; x++)
            {
                float posX = x * floorSize + floorSize / 2f;

                // 남쪽
                Vector3 southPos = new Vector3(posX, 0, 0);
                var southWall = Instantiate(wallPrefab, southPos, Quaternion.identity, wall.transform);
                // southWall.transform.localScale = new Vector3(wallLength, southWall.transform.localScale.y, wallWidth);

                // 북쪽
                Vector3 northPos = new Vector3(posX, 0, mapHeight * floorSize);
                var northWall = Instantiate(wallPrefab, northPos, Quaternion.identity, wall.transform);
                // northWall.transform.localScale = southWall.transform.localScale;
            }

            // — 서/동 벽 생성
            for (int y = 0; y < mapHeight; y++)
            {
                float posZ = y * floorSize + floorSize / 2f;

                // 서쪽
                Vector3 westPos = new Vector3(0, 0, posZ);
                var westWall = Instantiate(wallPrefab, westPos, Quaternion.Euler(0, 90f, 0), wall.transform);
                // westWall.transform.localScale = new Vector3(wallLength, westWall.transform.localScale.y, wallWidth);

                // 동쪽
                Vector3 eastPos = new Vector3(mapWidth * floorSize, 0, posZ);
                var eastWall = Instantiate(wallPrefab, eastPos, Quaternion.Euler(0, 90f, 0), wall.transform);
                // eastWall.transform.localScale = westWall.transform.localScale;
            }
        }
    }
}