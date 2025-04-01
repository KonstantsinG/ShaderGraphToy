namespace Nodes2Shader.DataTypes
{
    // Variant -> Any
    // Calculable -> Number, VecN, MatN, MatNxM
    // Number -> Int, Float
    // VecN -> N (2, 3, 4)
    // MatN -> N (2, 3, 4)
    // MatNxN -> NxM (2x3,2x4,  3x2,3x4,  4x2,4x3)


    public class DataType
    {

    }

    // ----------------------

    public class Variant : DataType
    {

    }

    // ----------------------

    public class Bool : Variant
    {

    }

    public class Calculable : Variant
    {

    }

    // ----------------------

    public class Number : Calculable
    {

    }

    public class Int : Number
    {

    }

    public class Float : Number
    {

    }

    // ----------------------

    public class VecN : Calculable
    {

    }

    public class Vec2 : VecN
    {

    }

    public class Vec3 : VecN
    {

    }

    public class Vec4 : VecN
    {

    }

    // ----------------------

    public class MatN : Calculable
    {

    }

    public class Mat2 : MatN
    {

    }

    public class Mat3 : MatN
    {

    }

    public class Mat4 : MatN
    {

    }

    // ----------------------

    public class MatNxM : Calculable
    {

    }

    public class Mat2x3 : MatNxM
    {

    }

    public class Mat2x4 : MatNxM
    {

    }

    public class Mat3x2 : MatNxM
    {

    }

    public class Mat3x4 : MatNxM
    {

    }

    public class Mat4x2 : MatNxM
    {

    }

    public class Mat4x3 : MatNxM
    {

    }
}
