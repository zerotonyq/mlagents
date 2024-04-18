using System.Threading.Tasks;
using Map.Chunk.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Map.Chunk
{
    public class Chunk
    {
        private GameObject _instance;
        private Vector3 _initPosition;
        
        private Chunk(GameObject instance, Vector3 initPosition)
        {
            _initPosition = initPosition;
            _instance = instance;   
        }
        
        public static async Task<Chunk> Construct(AssetReference ar, Vector3 initPosition)
        {
            Task<ChunkData> t = ar.LoadAssetAsync<ChunkData>().Task;
            await t;
            var obj = new GameObject("o");
            obj.AddComponent<MeshRenderer>();
            obj.AddComponent<MeshFilter>().sharedMesh = t.Result.terrainMesh;
            obj.transform.position = initPosition;
            return new Chunk(obj, initPosition);
        }
        public static async Task<Chunk> ConstructFromPrefab(AssetReference ar, Vector3 initPosition)
        {
            Task<ChunkData> t = ar.LoadAssetAsync<ChunkData>().Task;
            await t;
            var obj = GameObject.Instantiate(t.Result.prefab, initPosition, Quaternion.identity, null);
            return new Chunk(obj, initPosition);
        }
        
        public Vector3 ChunkPosition => _instance.transform.position;

        public void Disable() => _instance.SetActive(false);

        public void Enable() => _instance.SetActive(true);
        
        public Vector3 ChunkInitPosition => _initPosition;
        
        
    }
}