using Nodes2Shader.Compilation.MathGraph;
using System.Linq;

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
        public static List<DataType> ExactTypes
        {
            get => GetInnerTypes(DataType.Any).ToList();
        }
        public static List<DataType> GenericTypes
        {
            get => [ DataType.Vec, DataType.Mat, DataType.GenType,
                     DataType.GenIType, DataType.GenBType, DataType.Any ];
        }


        public static string DefineType(string value)
        {
            if (value.StartsWith("vec4")) return "Vec4";
            else if (value.StartsWith("vec3")) return "Vec3";
            else if (value.StartsWith("vec2")) return "Vec2";
            else if (!value.Contains(',')) return "Float";

            return "Null";
        }

        public static void RevealTypes(NodeData nd, List<string> variants)
        {
            List<DataType> types = [];
            List<NodeEntry> entries = nd.GetInputs();

            // 1. check for nd types matches with variants
            if (variants.Count > 0)
            {
                List<bool> revealed = [];
                bool found = false;

                foreach (string v in variants)
                {
                    types = v.Split(',').Select(x => (DataType)Enum.Parse(typeof(DataType), x.Trim(), true)).ToList();
                    for (int i = 0; i < types.Count; i++)
                    {
                        if (CanUnwrap((DataType)Enum.Parse(typeof(DataType), entries[i].Type, true), types[i]))
                            revealed.Add(true);
                        else revealed.Add(false);
                    }

                    if (revealed.All(r => r))
                    {
                        for (int i = 0; i < types.Count; i++)
                        {
                            entries[i].Type = GetGlslType(types[i]);
                            foreach (NodeEntry e in nd.GetOutputs()) e.Type = entries[i].Type;
                        }

                        found = true;
                        break;
                    }
                    revealed.Clear();
                }

                // 2. if there is - cast them to variant, otherwise - cast them to first castable variant
                if (found) return;
                else
                {
                    List<DataType> initTypes = [];
                    foreach (NodeEntry e in entries)
                        initTypes.Add((DataType)Enum.Parse(typeof(DataType), e.Type, true));

                    foreach (string v in variants)
                    {
                        types = v.Split(',').Select(x => (DataType)Enum.Parse(typeof(DataType), x.Trim(), true)).ToList();
                        if (IsCastPossible(initTypes, types))
                        {
                            for (int i = 0; i < types.Count; i++)
                            {
                                entries[i].Type = GetGlslType(types[i]);
                                entries[i].Value = CastType(entries[i].Value, initTypes[i], types[i]);
                                foreach (NodeEntry e in nd.GetOutputs()) e.Type = entries[i].Type;
                            }
                            return;
                        }
                    }
                }
            }

            // 3. By default, output type is determined by greatest input type
            types.Clear();
            foreach (NodeEntry e in entries)
                types.Add((DataType)Enum.Parse(typeof(DataType), e.Type, true));
            DataType greatest = GetGreatestType(types);

            for (int i = 0; i < types.Count; i++)
            {
                entries[i].Type = GetGlslType(greatest);
                entries[i].Value = CastType(entries[i].Value, types[i], greatest);
                foreach (NodeEntry e in nd.GetOutputs()) e.Type = entries[i].Type;
            }
        }

        private static bool IsCastPossible(List<DataType> toCast, List<DataType> target)
        {
            for (int i = 0; i < toCast.Count; i++)
            {
                if (GenericTypes.Contains(target[i])) return false;

                if (target[i] == DataType.Sampler2D && toCast[i] != target[i]) return false;
                if (toCast[i] == DataType.Null || target[i] == DataType.Null) return false;
            }

            return true;
        }

        private static string CastType(string val, DataType typeFrom, DataType typeTo)
        {
            if (val == "Ignore" && typeFrom == DataType.Null) return val;
            if (typeFrom == typeTo) return val;

            if (typeFrom == DataType.Float)
            {
                if (typeTo == DataType.Vec2) val = $"({val}, 0.0)";
                else if (typeTo == DataType.Vec3) val = $"({val}, 0.0, 0.0)";
                else if (typeTo == DataType.Vec4) val = $"({val}, 0.0, 0.0, 0.0)";
                return val;
            }
            else if (typeFrom == DataType.Vec2)
            {
                if (typeTo == DataType.Float) val = $"{val}.x";
                else if (typeTo == DataType.Vec3) val = $"{val.Remove(val.Length - 2, 1)}, 0.0)";
                else if (typeTo == DataType.Vec4) val = $"{val.Remove(val.Length - 2, 1)}, 0.0, 0.0)";
                return val;
            }
            else if (typeFrom == DataType.Vec3)
            {
                if (typeTo == DataType.Float) val = $"{val}.x";
                else if (typeTo == DataType.Vec2) val = $"{val}.xy";
                else if (typeTo == DataType.Vec4) val = $"{val.Remove(val.Length - 2, 1)}, 0.0, 0.0)";
                return val;
            }
            else if (typeFrom == DataType.Vec4)
            {
                if (typeTo == DataType.Float) val = $"{val}.x";
                else if (typeTo == DataType.Vec2) val = $"{val}.xy";
                else if (typeTo == DataType.Vec3) val = $"{val}.xyz";
                return val;
            }

            throw new NotImplementedException($"Cast from {typeFrom} to {typeTo} is currently unavailable...");
        }

        private static bool CanUnwrap(DataType toUnwrap, DataType target)
        {
            DataType[] innerTypes = GetInnerTypes(toUnwrap);
            return innerTypes.Contains(target);
        }

        private static DataType GetGreatestType(List<DataType> types)
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
                type == DataType.GenBType || type == DataType.Any)
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

            return [ type ];
        }
    }
}
