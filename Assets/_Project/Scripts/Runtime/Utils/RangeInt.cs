
using System;

namespace Factura.Utils
{
    [Serializable]
    public struct RangeInt
    {
        public int min;
        public int max;

        public int random => UnityEngine.Random.Range(min, max);

        public RangeInt(int min, int max)
        {
            this.min = min; 
            this.max = max;
        }
    }

    [Serializable]
    public struct RangeFloat
    {
        public float min;
        public float max;

        public float random => UnityEngine.Random.Range(min, max);

        public RangeFloat(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }

}
