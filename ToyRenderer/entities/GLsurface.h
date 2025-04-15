#pragma once
#include "CommonHeaders.h"


class GLsurface
{
public:
	GLsurface(ui32 id = id::invalidId);
	bool isValid() const;

	void makeContextCurrent();
	void swapBuffers(bool vsync);
	void setupBuffers(f32 verts[], ui32 idxs[]);
	void loadShader(const char* vartSource, const char* fragSource);
	void setViewport(ui32v4 rect);
	void doUpdateStuff();

private:
	ui32 _id;
};

