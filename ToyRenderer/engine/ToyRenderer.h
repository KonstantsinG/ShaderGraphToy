#pragma once
#include "CommonHeaders.h"
#include "../platforms/WinPlatform.h"
#include "../platforms/GlPlatform.h"


namespace ToyRenderer
{
	struct RenderingSurface
	{
		GLwindow window{};
		GLsurface surface{};
	};

	bool initialize();
	void update();
	void shutdown();
}