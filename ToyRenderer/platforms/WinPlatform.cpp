#include "WinPlatform.h"
#include "../utilities/ConstplacesVector.h"


namespace winPlatform
{
	namespace // anonymous
	{
		ConstplacesVector<WindowInfo> windows;

		WindowInfo& getFromId(ui32 id)
		{
			assert(id < windows.totalSize());
			assert(windows[id].hwnd);

			return windows[id];
		}


		WindowInfo& getFromHandle(HWND handle)
		{
			const ui32 id{ (ui32)GetWindowLongPtr(handle, GWLP_USERDATA) };
			return getFromId(id);
		}

		LRESULT CALLBACK internalWindowProc(HWND hwnd, UINT msg, WPARAM wparam, LPARAM lparam)
		{
			WindowInfo* info{ nullptr };
			switch (msg)
			{
			case WM_DESTROY: // closing window
				getFromHandle(hwnd).isClosed = true;
				break;
			case WM_EXITSIZEMOVE:
				info = &getFromHandle(hwnd);
				break;
			case WM_SIZE: // changing window size
				if (wparam == SIZE_MAXIMIZED)
				{
					info = &getFromHandle(hwnd);
				}
				break;
			case WM_SYSCOMMAND: // handling window command (maximize, minimize, resore, close)
				if (wparam == SC_RESTORE)
				{
					info = &getFromHandle(hwnd);
				}
				break;
			default:
				break;
			}

			if (info)
			{
				assert(info->hwnd);
				GetClientRect(info->hwnd, info->isFullscreen ? &info->fullscreenArea : &info->clientArea);
			}

			LONG_PTR longPtr{ GetWindowLongPtr(hwnd, 0) };
			return longPtr
				? ((WindowProc)longPtr)(hwnd, msg, wparam, lparam)
				: DefWindowProc(hwnd, msg, wparam, lparam);
		}


		void resizeWindow(const WindowInfo& info, const RECT& area)
		{
			RECT windowRect{ area };
			AdjustWindowRect(&windowRect, info.style, FALSE);

			const i32 width{ windowRect.right - windowRect.left };
			const i32 height{ windowRect.bottom - windowRect.top };
			MoveWindow(info.hwnd, info.topLeft.x, info.topLeft.y, width, height, true);
		}

		void setWindowFullscreen(ui32 id, bool isFullscreen)
		{
			WindowInfo& info{ getFromId(id) };

			if (info.isFullscreen != isFullscreen)
			{
				info.isFullscreen = isFullscreen;

				if (isFullscreen)
				{
					// store current window dimensions for restoring purpose
					GetClientRect(info.hwnd, &info.clientArea);
					RECT rect;
					GetWindowRect(info.hwnd, &rect);
					info.topLeft.x = rect.left;
					info.topLeft.y = rect.top;

					SetWindowLongPtr(info.hwnd, GWL_STYLE, 0);
					ShowWindow(info.hwnd, SW_MAXIMIZE);
				}
				else
				{
					SetWindowLongPtr(info.hwnd, GWL_STYLE, info.style);
					resizeWindow(info, info.clientArea);
					ShowWindow(info.hwnd, SW_SHOWNORMAL);
				}
			}
		}

		bool isWindowFullscreen(ui32 id)
		{
			return getFromId(id).isFullscreen;
		}

		HWND getWindowHandle(ui32 id)
		{
			return getFromId(id).hwnd;
		}

		void setWindowCaption(ui32 id, const wchar_t* caption)
		{
			WindowInfo& info{ getFromId(id) };
			SetWindowText(info.hwnd, caption);
		}

		const ui32v4 getWindowSize(ui32 id)
		{
			WindowInfo& info{ getFromId(id) };
			GetClientRect(info.hwnd, info.isFullscreen ? &info.fullscreenArea : &info.clientArea);
			RECT& area{ info.isFullscreen ? info.fullscreenArea : info.clientArea };

			return { (ui32)area.left, (ui32)area.top, (ui32)area.right, (ui32)area.bottom };
		}

		void resizeWindow(ui32 id, ui32 width, ui32 height)
		{
			WindowInfo& info{ getFromId(id) };
			RECT& area{ info.isFullscreen ? info.fullscreenArea : info.clientArea };
			area.bottom = area.top + height;
			area.right = area.left + width;

			resizeWindow(info, area);
		}

		bool isWindowClosed(ui32 id)
		{
			return getFromId(id).isClosed;
		}
	} // anonymous namespace


	GLwindow createWindow(const WindowInitInfo* const initInfo /* = nullptr */)
	{
		WindowProc callback{ initInfo ? initInfo->callback : nullptr };
		HWND parent{ initInfo ? initInfo->parent : nullptr };

		// define and register new window class
		WNDCLASSEX wc;
		ZeroMemory(&wc, sizeof(wc));
		wc.cbSize = sizeof(WNDCLASSEX);
		wc.style = CS_HREDRAW | CS_VREDRAW;
		wc.lpfnWndProc = internalWindowProc;
		wc.cbClsExtra = 0;
		wc.cbWndExtra = callback ? sizeof(callback) : 0;
		wc.hInstance = 0;
		wc.hIcon = LoadIcon(NULL, IDI_APPLICATION);
		wc.hCursor = LoadCursor(NULL, IDC_ARROW);
		wc.hbrBackground = CreateSolidBrush(RGB(48, 40, 54));
		wc.lpszMenuName = NULL;
		wc.lpszClassName = L"GLwindow";
		wc.hIconSm = LoadIcon(NULL, IDI_APPLICATION);
		RegisterClassEx(&wc);

		// create new window
		WindowInfo info{};
		info.clientArea.right = (initInfo && initInfo->width) ? info.clientArea.left + initInfo->width : info.clientArea.right;
		info.clientArea.bottom = (initInfo && initInfo->height) ? info.clientArea.top + initInfo->height : info.clientArea.bottom;
		info.style |= parent ? WS_CHILD : WS_OVERLAPPEDWINDOW;
		RECT rect{ info.clientArea };
		AdjustWindowRect(&rect, info.style, FALSE);

		const wchar_t* caption{ (initInfo && initInfo->caption) ? initInfo->caption : L"Rendering Viewport" };
		const ui32 left{ initInfo ? initInfo->left : (ui32)info.topLeft.x };
		const ui32 top{ initInfo ? initInfo->top : (ui32)info.topLeft.y };
		const ui32 width{ (ui32)(rect.right - rect.left) };
		const ui32 height{ (ui32)(rect.bottom - rect.top) };

		info.hwnd = CreateWindowEx(
			0,					// extended style
			wc.lpszClassName,	// window class name
			caption,			// window title
			info.style,		// window style
			left, top,			// initial position
			width, height,		// init size
			parent,				// window parent
			NULL,				// handle to menu
			NULL,				// extra creation parameters
			NULL				// instance of this application
		);

		if (info.hwnd)
		{
			DEBUG_OP(SetLastError(0));
			ui32 id{ windows.add(info) };
			SetWindowLongPtr(info.hwnd, GWLP_USERDATA, (LONG_PTR)id);

			// set extra bytes for window callback
			if (callback) SetWindowLongPtr(info.hwnd, 0, (LONG_PTR)callback);
			assert(GetLastError() == 0);

			ShowWindow(info.hwnd, SW_SHOWNORMAL);
			UpdateWindow(info.hwnd);

			return GLwindow{ id };
		}

		return GLwindow{};
	}

	void removeWindow(ui32 id)
	{
		WindowInfo info{ getFromId(id) };
		DestroyWindow(info.hwnd);
		windows.remove(id);
	}
}


void GLwindow::setFullscreen(bool isFullscreen) const
{
	assert(isValid());
	winPlatform::setWindowFullscreen(_id, isFullscreen);
}

bool GLwindow::isFullscreen() const
{
	assert(isValid());
	return winPlatform::isWindowFullscreen(_id);
}

void* GLwindow::getHandle() const
{
	assert(isValid());
	return winPlatform::getWindowHandle(_id);
}

void GLwindow::setCaption(const wchar_t* caption) const
{
	assert(isValid());
	winPlatform::setWindowCaption(_id, caption);
}

ui32v4 GLwindow::getSize() const
{
	assert(isValid());
	return winPlatform::getWindowSize(_id);
}

void GLwindow::resize(ui32 width, ui32 height) const
{
	assert(isValid());
	winPlatform::resizeWindow(_id, width, height);
}

ui32 GLwindow::getWidth() const
{
	ui32v4 s{ getSize() };
	return s.z - s.x;
}

ui32 GLwindow::getHeight() const
{
	ui32v4 s{ getSize() };
	return s.w - s.y;
}

bool GLwindow::isClosed() const
{
	assert(isValid());
	return winPlatform::isWindowClosed(_id);
}