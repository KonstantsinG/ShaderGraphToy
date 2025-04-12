#pragma once

#include "CommonHeaders.h"
#include "../entities/GLwindow.h"


namespace winPlatform
{
	using WindowProc = LRESULT(*)(HWND, UINT, WPARAM, LPARAM);

	struct WindowInitInfo
	{
		WindowProc callback{ nullptr };
		HWND parent{ nullptr };
		const wchar_t* caption{ nullptr };
		i32	left{ 0 };
		i32	top{ 0 };
		i32	width{ 1920 };
		i32	height{ 1080 };
	};

	struct WindowInfo
	{
		HWND hwnd{ nullptr };
		RECT clientArea{ 0, 0, 1920, 1080 };
		RECT fullscreenArea{};
		POINT topLeft{ 0, 0 };
		DWORD style{ WS_VISIBLE };
		bool isFullscreen{ false };
		bool isClosed{ false };
	};


	GLwindow createWindow(const WindowInitInfo* const initInfo = nullptr);
	void removeWindow(ui32 id);
}