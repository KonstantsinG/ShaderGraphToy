#include "GLwindow.h"


GLwindow::GLwindow(ui32 id /* = id::invalidId */)
{
	_id = id;
}

bool GLwindow::isValid() const
{
	return _id != id::invalidId;
}