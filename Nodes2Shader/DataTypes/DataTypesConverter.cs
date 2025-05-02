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
        public static string DefineType(string value)
        {
            int c = value.Count(ch => ch == ',');

            if (c == 0)
            {
                if (value.Count(ch => ch == '.') == 0) return "Int";
                else return "Float";
            }
            else if (c == 1) return "Vec2";
            else if (c == 2) return "Vec3";
            else if (c == 3) return "Vec4";

            return "Any";
        }

        public static bool IsAnyValid(string val)
        {
            if (string.IsNullOrEmpty(val)) return false;

            string[] vals = val.Split(',');
            foreach (string v in vals)
            {
                if (v.Contains('.'))
                {
                    if (!float.TryParse(v.Trim(), null, out _))
                        return false;
                }
                else
                {
                    if (!int.TryParse(v.Trim(), null, out _))
                        return false;
                }
            }

            return true;
        }

        public static bool IsTypesRelevant(string[] typesFrom, string[] typesTo)
        {
            if (typesFrom.Length != typesTo.Length) return false;

            DataType[] types1 = new DataType[typesFrom.Length];
            for (int i = 0; i < typesFrom.Length; i++)
                types1[i] = (DataType)Enum.Parse(typeof(DataType), typesFrom[i], true);

            DataType[] types2 = new DataType[typesTo.Length];
            for (int i = 0; i < typesTo.Length; i++)
                types2[i] = (DataType)Enum.Parse(typeof(DataType), typesTo[i], true);

            for (int i = 0; i < types1.Length; i++)
            {
                if (!IsCastPossible(types1[i], types2[i]))
                    return false;
            }

            return true;
        }

        public static bool IsCastPossible(DataType toCast, DataType target)
        {
            if (toCast == target) return true;
            if (toCast == DataType.Null || target == DataType.Null) return false;

            if (toCast == DataType.Sampler2D || target == DataType.Sampler2D) return false;

            return true;
        }


        public static void CastInputs(NodeData nd)
        {
            List<NodeEntry> inputs = nd.GetInputs();
            if (inputs.Count == 0) return; // if node does not have inputs - it is predefined constant, so there is nothing to cast

            if (nd.VarInput == "Greatest") // cast all inputs to one common type
            {
                List<DataType> initTypes = [];
                foreach (NodeEntry e in inputs)
                    initTypes.Add((DataType)Enum.Parse(typeof(DataType), e.Type, true));

                DataType greatest = GetGreatestType(initTypes);
                for (int i = 0; i < inputs.Count; i++)
                {
                    inputs[i].Value = CastType(inputs[i].Value, initTypes[i], greatest);
                    inputs[i].Type = greatest.ToString();
                }
            }
            else
            {
                List<DataType> targetTypes = [];
                foreach (string t in nd.VarInput.Split(','))
                {
                    if (t.Equals("null", StringComparison.CurrentCultureIgnoreCase)) continue;
                    targetTypes.Add((DataType)Enum.Parse(typeof(DataType), t, true));
                }

                DataType typeFrom;
                if (targetTypes.Count != inputs.Count)
                    throw new InvalidOperationException("Target input types count must be equal to input entries count!");

                for (int i = 0; i < inputs.Count; i++)
                {
                    typeFrom = (DataType)Enum.Parse(typeof(DataType), inputs[i].Type, true);
                    inputs[i].Value = CastType(inputs[i].Value, typeFrom, targetTypes[i]);
                    inputs[i].Type = targetTypes[i].ToString();
                }
            }
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

        private static string CastType(string val, DataType typeFrom, DataType typeTo)
        {
            if (typeFrom == DataType.Null || typeTo == DataType.Null) return val;
            if (typeFrom == typeTo) return val;

            if (typeFrom == DataType.Float || typeFrom == DataType.Int)
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

            // for now method supports only GenType casts from GLSL
            throw new NotImplementedException($"Cast from {typeFrom} to {typeTo} is currently unavailable...");
        }
    }
}
