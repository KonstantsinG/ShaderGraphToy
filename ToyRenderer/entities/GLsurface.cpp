#include "GLsurface.h"


GLsurface::GLsurface(ui32 id /* = id::invalidId */)
{
	_id = id;;
}

bool GLsurface::isValid() const
{
	return _id != id::invalidId;
}