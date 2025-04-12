#pragma once

#include <CommonHeaders.h>
#include <Windows.h>


class GLwindow
{
public:
	// basic functions
	GLwindow(ui32 id = id::invalidId);
	bool isValid() const;

	// winPlatform functions
	void setFullscreen(bool is_fullscreen) const;
	bool isFullscreen() const;
	void* getHandle() const;
	void setCaption(const wchar_t* caption) const;
	ui32v4 getSize() const;
	void resize(ui32 width, ui32 height) const;
	ui32 getWidth() const;
	ui32 getHeight() const;
	bool isClosed() const;

private:
	ui32 _id;
};