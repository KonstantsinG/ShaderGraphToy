#pragma once
#include <stdint.h>


// signed integers
using i8 = int8_t;
using i16 = int16_t;
using i32 = int32_t;
using i64 = int64_t;

// unsigned integers
using ui8 = uint8_t;
using ui16 = uint16_t;
using ui32 = uint32_t;
using ui64 = uint64_t;

// floating point
using f32 = float;

// vectors
struct ui32v4
{
	ui32v4(ui32 x, ui32 y, ui32 z, ui32 w) : x(x), y(y), z(z), w(w) {}

	ui32 x, y, z, w;
};

struct f32v4
{
	f32v4(f32 x, f32 y, f32 z, f32 w) : x(x), y(y), z(z), w(w) {}

	f32 x, y, z, w;
};

// id's
namespace id
{
	// 1111 1111 1111 1111 - max uint32 value is invalid id
	constexpr ui32 invalidId{ 0xffff'ffffui32 };
}