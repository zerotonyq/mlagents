using TerrainGeneration;
using UnityEngine;

namespace DefaultNamespace.StormFlower.Data
{
    public class StormFlowerData
    {
        public Node Node { get; private set; }
        public readonly StormFlowerView View;
        
        public StormFlowerData(Node node, StormFlowerView view)
        {
            Node = node;
            View = view;
        }

        public void SetNewNode(Node node)
        {
            Node = node;
        }
        public void Deactivate()
        {
            Deactivated = true;
            View.gameObject.SetActive(false);
        }

        public void Activate()
        {
            Deactivated = false;
            View.gameObject.SetActive(true);
        }

        public bool Deactivated { get; private set; }
    }
}