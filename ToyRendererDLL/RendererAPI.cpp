#ifndef EDITOR_INTERFACE
#define EDITOR_INTERFACE extern "C" __declspec(dllexport) // disable names mangling for imported functions
#endif

#include "CommonHeaders.h"
#include <Windows.h>;
#include "../platforms/WinPlatform.h"
#include "../utilities/ConstplacesVector.h"


ConstplacesVector<GLwindow> surfaces;


EDITOR_INTERFACE
ui32 CreateRenderSurface(HWND host, i32 width, i32 height)
{
	assert(host);
	winPlatform::WindowInitInfo info{ nullptr, host, nullptr, 0, 0, width, height };
	GLwindow win{ winPlatform::createWindow(&info) };
	assert(win.isValid());
	ui32 id = surfaces.add(win);

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