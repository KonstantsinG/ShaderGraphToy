#pragma once


// macros
#ifdef _DEBUG
#define DEBUG_OP(x) x
#else
#define DEBUG_OP(x) (void(0))
#endif