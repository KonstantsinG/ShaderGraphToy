using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ShaderGraph.Converters
{
    public class VariantConverter
    {
        public enum DataType
        {
            Variant, // Any
            Number, // Int, Float
            VecN, // Vec(2,3,4)
            MatN, // Mat(2x2,3x3,4x4)
            MatNxM, // Mat(2x2,2x3,2x4,  3x2,3x3,3x4,  4x2,4x3,4x4)
            Calculable, // Number, VecN, MatNxM
            Vec2, Vec3, Vec4,
            Mat2, Mat3, Mat4,
            Mat2x3, Mat2x4, Mat3x2, Mat3x4, Mat4x2, Mat4x3,
            Int, Float,
            Bool
        }
    }
}
