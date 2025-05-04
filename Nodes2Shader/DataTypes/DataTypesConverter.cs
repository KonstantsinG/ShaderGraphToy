using Nodes2Shader.Compilation.MathGraph;
using System;
using System.Text.RegularExpressions;

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

    public static partial class DataTypesConverter
    {
        #region FORMAT
        public static string FormatFloat(float value)
        {
            string str = value.ToString();
            return str.Contains('.') ? str : str + ".0";
        }
        #endregion

        #region CAST
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
                    inputs[i].Type = GetGlslType(greatest);
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
                    inputs[i].Type = GetGlslType(targetTypes[i]);
                }
            }
        }

        public static string CastType(string val, string typeFrom, string typeTo)
        {
            DataType t_typeFrom = (DataType)Enum.Parse(typeof(DataType), typeFrom, true);
            DataType t_typeTo = (DataType)Enum.Parse(typeof(DataType), typeTo, true);

            return CastType(val, t_typeFrom, t_typeTo);
        }

        private static string CastType(string val, DataType typeFrom, DataType typeTo)
        {
            if (IsGeneric(typeTo)) throw new InvalidOperationException("Cast to generic type is invalid.");

            if (IsGeneric(typeFrom)) return CastGenericType(val, typeFrom, typeTo);
            else return CastBasicType(val, typeFrom, typeTo);
        }

        private static string CastBasicType(string val, DataType typeFrom, DataType typeTo)
        {
            if (typeFrom == DataType.Null || typeTo == DataType.Null) return val;
            if (typeFrom == typeTo) return val;

            switch (typeFrom)
            {
                case DataType.Int:
                    // val is actual value
                    if (ValueRegex().Match(val).Success)
                    {
                        if (typeTo == DataType.Float) val = $"{val}.0";
                        else if (typeTo == DataType.Vec2) val = $"vec2({val}.0, {val}.0)";
                        else if (typeTo == DataType.Vec3) val = $"vec3({val}.0, {val}.0, {val}.0)";
                        else if (typeTo == DataType.Vec4) val = $"vec4({val}.0, 0.0, 0.0, 0.0)";
                        return val;
                    }
                    else // val is variable name
                    {
                        if (typeTo == DataType.Float) val = $"float({val})";
                        else if (typeTo == DataType.Vec2) val = $"vec2(float({val}), float({val}))";
                        else if (typeTo == DataType.Vec3) val = $"vec3({val}), float({val}), float({val}))";
                        else if (typeTo == DataType.Vec4) val = $"vec4(float({val}), float({val}), float({val}), float({val}))";
                        return val;
                    }
                    

                case DataType.Float:
                    if (typeTo == DataType.Int) val = $"int({val})";
                    if (typeTo == DataType.Vec2) val = $"vec2({val}, {val})";
                    else if (typeTo == DataType.Vec3) val = $"vec3({val}, {val}, {val})";
                    else if (typeTo == DataType.Vec4) val = $"vec4({val}, {val}, {val}, {val})";
                    return val;

                case DataType.Vec2:
                    return FromVec(val, 2, typeTo);

                case DataType.Vec3:
                    return FromVec(val, 3, typeTo);

                case DataType.Vec4:
                    return FromVec(val, 4, typeTo);
            }

            // for now method supports only GenType casts from GLSL
            throw new NotImplementedException($"Cast from {typeFrom} to {typeTo} is currently unavailable...");
        }

        private static string CastGenericType(string val, DataType typeFrom, DataType typeTo)
        {
            string typeStr = DefineType(val);
            DataType type = (DataType)Enum.Parse(typeof(DataType), typeStr, true);

            return CastBasicType(val, type, typeTo);
        }

        private static string FromVec(string vecStr, int comps, DataType to)
        {
            // vecStr is actual value
            if (ValueRegex().Match(vecStr).Success)
            {
                string[] vecNums = vecStr.Replace($"vec{comps}(", "", StringComparison.CurrentCultureIgnoreCase).Replace(")", "").Split(',');

                switch (to)
                {
                    case DataType.Int:
                        return vecNums[0].Split('.')[0];

                    case DataType.Float:
                        return vecNums[0].Contains('.') ? vecNums[0] : vecNums[0] + ".0";

                    case DataType.Vec2:
                        if (comps == 2) return vecStr;
                        else if (comps == 3 || comps == 4) return $"vec2({vecNums[0]}, {vecNums[1]})";
                        break;

                    case DataType.Vec3:
                        if (comps == 2) return $"vec3({vecNums[0]}, {vecNums[1]}, {vecNums[1]})";
                        else if (comps == 3) return vecStr;
                        else if (comps == 4) return $"vec3({vecNums[0]}, {vecNums[1]}, {vecNums[2]})";
                        break;

                    case DataType.Vec4:
                        if (comps == 2) return $"vec4({vecNums[0]}, {vecNums[1]}, {vecNums[1]}, {vecNums[1]})";
                        else if (comps == 3) return $"vec4({vecNums[0]}, {vecNums[1]}, {vecNums[2]}, {vecNums[2]})";
                        else if (comps == 4) return vecStr;
                        break;
                }
            }
            else // vecStr is variable name
            {
                switch (to)
                {
                    case DataType.Int:
                        return $"int({vecStr}.x)";

                    case DataType.Float:
                        return $"{vecStr}.x"; ;

                    case DataType.Vec2:
                        if (comps == 2) return vecStr;
                        else if (comps == 3 || comps == 4) return $"vec2({vecStr}.x, {vecStr}.y)";
                        break;

                    case DataType.Vec3:
                        if (comps == 2) return $"vec3({vecStr}.x, {vecStr}.y, {vecStr}.y)";
                        else if (comps == 3) return vecStr;
                        else if (comps == 4) return $"vec3({vecStr}.x, {vecStr}.y, {vecStr}.z)";
                        break;

                    case DataType.Vec4:
                        if (comps == 2) return $"vec4({vecStr}.x, {vecStr}.y, {vecStr}.y, {vecStr}.y)";
                        else if (comps == 3) return $"vec4({vecStr}.x, {vecStr}.y, {vecStr}.z, {vecStr}.z)";
                        else if (comps == 4) return vecStr;
                        break;
                }
            }

            throw new NotImplementedException($"Cast from Vec{comps} to {to} is currently unavailable...");
        }
        #endregion

        #region IS_RELEVANT
        public static bool IsAnyValid(string val)
        {
            if (string.IsNullOrEmpty(val)) return false;

            // is Int
            if (IntRegex().IsMatch(val))
                return true;

            // is Float
            if (FloatRegex().IsMatch(val))
                return true;

            // is Vec (vec2, vec3, vec4)
            var vectorMatch = VecRegex().Match(val);
            if (vectorMatch.Success)
            {
                int components = int.Parse(vectorMatch.Groups[1].Value);
                string[] values = vectorMatch.Groups[2].Value.Split(',');

                if (values.Length != components)
                    return false;

                string trm;
                foreach (string value in values)
                {
                    trm = value.Trim();
                    if (!FloatRegex().IsMatch(trm))
                        return false;
                }

                return true;
            }

            return false;
        }

        public static bool IsNumberValid(string val)
        {
            if (string.IsNullOrEmpty(val)) return false;

            // is Int
            if (IntRegex().IsMatch(val))
                return true;

            // is Float
            if (FloatRegex().IsMatch(val))
                return true;

            return false;
        }

        public static bool IsTypesRelevant(string[] typesFrom, string[] typesTo, bool strongTyping = true)
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
                if (!strongTyping || IsGeneric(types1[i]) || IsGeneric(types2[i]))
                {
                    if (!IsCastPossible(types1[i], types2[i]))
                        return false;
                }
                else
                {
                    if (types1[i] != types2[i])
                        return false;
                }
            }

            return true;
        }

        public static bool IsCastPossible(string toCast, string target)
        {
            DataType t_toCats = (DataType)Enum.Parse(typeof(DataType), toCast, true);
            DataType t_target = (DataType)Enum.Parse(typeof(DataType), target, true);

            return IsCastPossible(t_toCats, t_target);
        }

        private static bool IsCastPossible(DataType toCast, DataType target)
        {
            if (toCast == target) return true;
            if (toCast == DataType.Null || target == DataType.Null) return false;

            if (toCast == DataType.Sampler2D || target == DataType.Sampler2D) return false;

            return true;
        }
        #endregion

        #region UTILITIES
        public static string DefineType(string value)
        {
            if (IntRegex().IsMatch(value)) return "Int";
            else if (FloatRegex().IsMatch(value)) return "Float";

            var vectorMatch = VecRegex().Match(value);
            if (vectorMatch.Success)
            {
                int comps = int.Parse(vectorMatch.Groups[1].Value);
                if (comps == 2) return "Vec2";
                else if (comps == 3) return "Vec3";
                else if (comps == 4) return "Vec4";
            }

            return "Any";
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

        private static bool IsGeneric(DataType type)
        {
            return type == DataType.GenType || type == DataType.GenIType || type == DataType.GenBType ||
                   type == DataType.Vec || type == DataType.Mat || type == DataType.Any;
        }

        private static string GetGlslType(DataType type)
        {
            if (IsGeneric(type) || type == DataType.Null)
                throw new InvalidOperationException("Only actual types can be converted to GLSL types");

            if (type == DataType.Sampler2D) return "sampler2D";
            else return type.ToString().ToLower();
        }
        #endregion


        [GeneratedRegex(@"^-?\d+$")]
        private static partial Regex IntRegex();
        [GeneratedRegex(@"^-?\d+\.\d+$")]
        private static partial Regex FloatRegex();
        [GeneratedRegex(@"^vec([2-4])\(([^)]+)\)$")]
        private static partial Regex VecRegex();
        [GeneratedRegex(@"^
                        (?:
                            -?\d+(?:\.\d+)?                     # Int or Float
                        |
                            vec[2-4]                            # Vec2-4
                            \(                                  # (
                                \s*-?\d+(?:\.\d+)?\s*           # first vector comp
                                (?:,\s*-?\d+(?:\.\d+)?\s*)*     # next comps
                            \)                                  # )
                        )
                    $", RegexOptions.IgnorePatternWhitespace)]
        private static partial Regex ValueRegex();
    }
}
