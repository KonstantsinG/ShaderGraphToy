#ifndef EDITOR_INTERFACE
#define EDITOR_INTERFACE extern "C" __declspec(dllexport) // disable names mangling for imported functions
#endif

#include "CommonHeaders.h"
#include <Windows.h>;
#include "../platforms/WinPlatform.h"
#include "../platforms/GlPlatform.h"
#include "../utilities/ConstplacesVector.h"




const char* vertexShaderSource =
"#version 330 core\n"
"layout (location = 0) in vec3 aPos;\n"
"out vec4 vertColor;\n"
"uniform float time;\n"
"float remap(float val, float low1, float high1, float low2, float high2){\n"
"return low2 + (val - low1) * (high2 - low2) / (high1 - low1);}\n"
"vec3 saturate(vec3 val){\n"
"float maxCh = max(val.r, max(val.g, val.b));\n"
"vec3 adjusted = pow(val / maxCh, vec3(5.0));\n"
"return normalize(adjusted) * length(val);}\n"
"vec2 rotate(vec2 p, float angle){\n"
"return vec2(p.x * cos(angle) - p.y * sin(angle), p.x * sin(angle) + p.y * cos(angle));}\n"
"void main()\n"
"{\n"
"   vec2 newP = rotate(aPos.xy, time);\n"
"   gl_Position = vec4(newP.x, newP.y, aPos.z, 1.0);\n"
"   vec3 saturated = saturate(vec3(remap(newP.x, -1, 1, 0, 1), remap(newP.y, -1, 1, 0, 1), remap(aPos.z, -1, 1, 0, 1)));"
"   vertColor = vec4(saturated.r, saturated.g, saturated.b, 1.0);\n"
"}\0";
const char* fragmentShaderSource =
"#version 330 core\n"
"out vec4 FragColor;\n"
"in vec4 vertColor;\n"
"void main()\n"
"{\n"
"   FragColor = vertColor;\n"
"}\n\0";


GLsurface surf;
LRESULT winProc(HWND hwnd, UINT msg, WPARAM wparam, LPARAM lparam)
{
    switch (msg)
    {



    case WM_SIZE:
    {
        GLwindow win{ (ui32)GetWindowLongPtr(hwnd, GWLP_USERDATA) };
        surf.setViewport(win.getSize());
		surf.doUpdateStuff();
        break;
    }
    }

    return DefWindowProc(hwnd, msg, wparam, lparam);
}


ConstplacesVector<GLwindow> surfaces;


EDITOR_INTERFACE
ui32 CreateRenderSurface(HWND host, i32 width, i32 height)
{
	GlPlatform::initOpenGL();

	assert(host);
	winPlatform::WindowInitInfo info{ &winProc, nullptr, nullptr, 0, 0, width, height };
	GLwindow win{ winPlatform::createWindow(&info) };
	assert(win.isValid());
	ui32 id = surfaces.add(win);

	
	surf={ GlPlatform::createSurface((HWND)win.getHandle()) };
	surf.makeContextCurrent();
	surf.setViewport(win.getSize());
	f32 vertices[] = {
		 -1.0f, -1.0f, 0.0f,    // bottom left
		-1.0f, 1.0f, 0.0f,      // top left
		 1.0f,  1.0f, 0.0f,     // top right
		 1.0f, -1.0f, 0.0f      // bottom right
	};
	ui32 indices[] = {
	0, 1, 3,   // first triangle
	1, 2, 3    // second triangle
	};
	surf.setupBuffers(vertices, indices);
	surf.loadShader(vertexShaderSource, fragmentShaderSource);

	/*MSG msg;
	while (true)
	{
		while (PeekMessage(&msg, NULL, 0, 0, PM_REMOVE))
		{
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}

		surf.doUpdateStuff();
	}*/

	return id;
}

EDITOR_INTERFACE
void RemoveRenderSurface(ui32 id)
{
	winPlatform::removeWindow(id);
	surfaces.remove(id);
}

EDITOR_INTERFACE
HWND GetHandle(ui32 id)
{
	return (HWND)surfaces[id].getHandle();
}