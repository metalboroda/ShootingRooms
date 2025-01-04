namespace UnityEngine
{
    public static class TransformExtension
    {
        public static Vector3 GetDirectionTo(this Transform from, Transform to)
        {
            return (to.position - from.position).normalized;
        }
        
        public static Vector3 GetDirectionTo(this Transform from, Vector3 to)
        {
            return (to - from.position).normalized;
        }

        public static Vector3 GetOffsetTo(this Transform from, Transform to)
        {
            return to.position - from.position;
        }
        
        public static Vector3 GetOffsetTo(this Transform from, Vector3 to)
        {
            return to - from.position;
        }
    }
}
