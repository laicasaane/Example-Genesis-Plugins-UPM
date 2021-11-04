using UnityEngine;
using Examples.Common.Unions;

namespace Examples
{
    public partial class ExampleBehaviour : MonoBehaviour
    {
        private void Start()
        {
        }

        [Union(typeof((int, float)))]
        public readonly partial struct ExampleUnionA { }
    }
}
