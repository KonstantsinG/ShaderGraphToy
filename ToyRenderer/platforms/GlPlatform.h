#pragma once
#include "CommonHeaders.h"
#include "../entities/GLsurface.h"
#include "glad/glad.h"
#include "glad/glad_wgl.h"
#include <Windows.h>
#include <float.h>


namespace GlPlatform
{
	struct SurfaceInfo
	{
		HDC dc{ nullptr };
		HGLRC glrc{ nullptr };
		ui32 vao, vbo, ebo;
		ui32 vertShader, fragShader, shProgram;
	};

	void initOpenGL();

	GLsurface createSurface(HWND hwnd);
	void removeSurface(ui32 id);
}