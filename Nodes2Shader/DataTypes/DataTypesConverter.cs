

namespace Nodes2Shader.DataTypes
{
    public enum DataType
    {
        //BASIC
        Float, Int, Bool,

        //VECS
        Vec2, Vec3, Vec4,
        IVec2, IVec3, IVec4,
        BVec2, BVec3, BVec4,

        //MATRICES
        Mat2, Mat3, Mat4,
        Mat2x3, Mat2x4, Mat3x2, Mat3x4, Mat4x2, Mat4x3,

        //IMAGES
        Sampler2D,

        //GENERIC
        Vec, // all vector types
        Mat, // all matrix types

        GenType, // Float, Vec2, Vec3, Vec4,
        GenIType, // Int, IVec2, IVec3, IVec4
        GenBType, // Bool, BVec2, BVec3, BVec4
        Any // GenType, GenIType, GenBType, Mat
    }

    public enum ConversionResult
    {
        OK,
        PrecisionWarning,
        ComponentsAmountWarning,
        BoolToNumberWarning,
        Error
    }

    public static class DataTypesConverter
    {
        public static bool IsInterchangeable(string type1, string type2)
        {
            DataType t1, t2;
            if (!DataType.TryParse(type1, true, out t1)) return false;
            if (!DataType.TryParse(type2, true, out t2)) return false;

            // conversion resolved if -> equal, floating point, generic types, components amount
            
            // Sampler2D cannot be converted to any other type
            if (t1 != t2 && (t1 == DataType.Sampler2D || t2 == DataType.Sampler2D)) return false;

            return true;
        }


        private static bool IsPair(DataType t1, DataType t2, DataType r1, DataType r2, bool countOrder = false)
        {
            if (t1 == r1 && t2 == r2) return true;
            if (!countOrder)
            {
                if (t1 == r2 && t2 == r1) return true;
            }

            return false;
        }

        private static bool IsPair(DataType t1, DataType t2, params DataType[] rs)
        {
            foreach (DataType r1 in rs)
            {
                foreach (DataType r2 in rs)
                {
                    if (r1 == r2) continue;

                    if (IsPair(t1, t2, r1, r2)) return true;
                }
            }

            return false;
        }

        private static bool IsRangePair(DataType t1, DataType t2, DataType r1, DataType[] r2s)
        {
            foreach (DataType r2 in r2s)
            {
                if (IsPair(t1, t2, r1, r2)) return true;
            }

            return false;
        }

        private static DataType[] GetInnerTypes(DataType type)
        {
            if (type == DataType.GenType) return [ DataType.Float, DataType.Vec2, DataType.Vec3, DataType.Vec4 ];
            if (type == DataType.GenIType) return [ DataType.Int, DataType.IVec2, DataType.IVec3, DataType.IVec4 ];
            if (type == DataType.GenBType) return [ DataType.Bool, DataType.BVec2, DataType.BVec3, DataType.BVec4 ];
            if (type == DataType.Vec) return [DataType.Vec2, DataType.Vec3, DataType.Vec4, DataType.IVec2, DataType.IVec3, DataType.IVec4, DataType.BVec2, DataType.BVec3, DataType.BVec4];
            if (type == DataType.Mat) return [ DataType.Mat2, DataType.Mat3, DataType.Mat4, DataType.Mat2x3, DataType.Mat2x4, DataType.Mat3x2, DataType.Mat3x4, DataType.Mat4x2, DataType.Mat4x3 ];

            return [];
        }
    }
}
