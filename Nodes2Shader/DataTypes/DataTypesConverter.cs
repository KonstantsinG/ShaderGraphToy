using Nodes2Shader.Compilation.MathGraph;

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
        Any, // GenType, GenIType, GenBType, Mat

        Null
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
        public static void RevealTypes(NodeData nd, List<string> variants)
        {
            // 1. check for nd types matches with variants

            // 2. if there is - cast them to variant, otherwise - cast them to greatest type

            // 3. By default, output type is determined by greatest input type
        }

        private static void CastType(ref string val, DataType typeFrom, DataType typeTo)
        {
            if (typeFrom == typeTo) return;

            if (typeFrom == DataType.Float)
            {
                if (typeTo == DataType.Vec2) val = $"({val}, 0.0)";
                else if (typeTo == DataType.Vec3) val = $"({val}, 0.0, 0.0)";
                else if (typeTo == DataType.Vec4) val = $"({val}, 0.0, 0.0, 0.0)";
            }
            else if (typeFrom == DataType.Vec2)
            {
                if (typeTo == DataType.Float) val = val.Split(',')[0].Remove(0, 1);
                else if (typeTo == DataType.Vec3) val = $"{val.Remove(val.Length - 2, 1)}, 0.0)";
                else if (typeTo == DataType.Vec4) val = $"{val.Remove(val.Length - 2, 1)}, 0.0, 0.0)";
            }
            else if (typeFrom == DataType.Vec3)
            {
                if (typeTo == DataType.Float) val = val.Split(',')[0].Remove(0, 1);
                else if (typeTo == DataType.Vec2) val = $"{val.Split(',')[0]}, {val.Split(',')[1]})";
                else if (typeTo == DataType.Vec4) val = $"{val.Remove(val.Length - 2, 1)}, 0.0, 0.0)";
            }
            else if (typeFrom == DataType.Vec4)
            {
                if (typeTo == DataType.Float) val = val.Split(',')[0].Remove(0, 1);
                else if (typeTo == DataType.Vec2) val = $"{val.Split(',')[0]}, {val.Split(',')[1]})";
                else if (typeTo == DataType.Vec3) val = $"{val.Split(',')[0]}, {val.Split(',')[1]}, {val.Split(',')[2]})";
            }

            throw new NotImplementedException($"Cast from {typeFrom} to {typeTo} is currently unavailable...");
        }

        private static DataType GetGreatestType(params DataType[] types)
        {
            return types.OrderBy(t => GetPriority(t)).Last();
        }

        private static int GetPriority(DataType type)
        {
            if (type == DataType.Float || type == DataType.Int || type == DataType.Bool) return 1;
            else if (type == DataType.Vec2 || type == DataType.IVec2 || type == DataType.BVec2) return 2;
            else if (type == DataType.Vec3 || type == DataType.IVec3 || type == DataType.BVec3) return 3;
            else if (type == DataType.Vec4 || type == DataType.IVec4 || type == DataType.BVec4) return 4;

            return -1;
        }

        private static string GetGlslType(DataType type)
        {
            if (type == DataType.Vec || type == DataType.Mat ||
                type == DataType.GenType || type == DataType.GenIType ||
                type == DataType.GenBType || type == DataType.Any ||
                type == DataType.Null)
                return string.Empty;
            else if (type == DataType.Sampler2D)
                return "sampler2D";
            else
                return type.ToString().ToLower();
        }

        private static DataType[] GetInnerTypes(DataType type)
        {
            if (type == DataType.GenType) return [DataType.Float, DataType.Vec2, DataType.Vec3, DataType.Vec4];
            else if (type == DataType.GenIType) return [DataType.Int, DataType.IVec2, DataType.IVec3, DataType.IVec4];
            else if (type == DataType.GenBType) return [DataType.Bool, DataType.BVec2, DataType.BVec3, DataType.BVec4];
            else if (type == DataType.Vec) return [DataType.Vec2, DataType.Vec3, DataType.Vec4, DataType.IVec2, DataType.IVec3, DataType.IVec4, DataType.BVec2, DataType.BVec3, DataType.BVec4];
            else if (type == DataType.Mat) return [DataType.Mat2, DataType.Mat3, DataType.Mat4, DataType.Mat2x3, DataType.Mat2x4, DataType.Mat3x2, DataType.Mat3x4, DataType.Mat4x2, DataType.Mat4x3];
            else if (type == DataType.Any) return [DataType.Float, DataType.Vec2, DataType.Vec3, DataType.Vec4,
                                                   DataType.Int, DataType.IVec2, DataType.IVec3, DataType.IVec4,
                                                   DataType.Bool, DataType.BVec2, DataType.BVec3, DataType.BVec4,
                                                   DataType.Mat2, DataType.Mat3, DataType.Mat4, DataType.Mat2x3, DataType.Mat2x4, DataType.Mat3x2, DataType.Mat3x4, DataType.Mat4x2, DataType.Mat4x3];

            return [];
        }
    }
}
