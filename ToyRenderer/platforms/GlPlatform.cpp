#include "GlPlatform.h"
#include "../utilities/ConstplacesVector.h"


namespace GlPlatform
{
    ConstplacesVector<SurfaceInfo> surfaces;

    SurfaceInfo& getFromId(ui32 id)
    {
        assert(id < surfaces.totalSize());
        
        return surfaces[id];
    }

	void initOpenGL()
	{
        WNDCLASSEX wc{};
        wc.cbSize = sizeof(WNDCLASSEX);
        wc.lpszClassName = L"initWindow";
        wc.lpfnWndProc = DefWindowProc;
        wc.style = CS_OWNDC;

        ATOM classId = RegisterClassEx(&wc);
        assert(classId);

        HWND handle = CreateWindowEx(NULL, MAKEINTATOM(classId), L"", WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU, CW_DEFAULT, CW_DEFAULT, CW_DEFAULT, CW_DEFAULT, NULL, NULL, NULL, NULL);
        assert(handle);

        HDC dc = GetDC(handle);
        PIXELFORMATDESCRIPTOR pixDesc{};
        pixDesc.nSize = sizeof(PIXELFORMATDESCRIPTOR);
        pixDesc.nVersion = 1;
        pixDesc.iPixelType = PFD_TYPE_RGBA;
        pixDesc.dwFlags = PFD_DRAW_TO_WINDOW | PFD_SUPPORT_OPENGL | PFD_DOUBLEBUFFER;
        pixDesc.cColorBits = 32;
        pixDesc.cAlphaBits = 8;
        pixDesc.cDepthBits = 24;
        pixDesc.cStencilBits = 8;
        pixDesc.iLayerType = PFD_MAIN_PLANE;

        int pixFormat = ChoosePixelFormat(dc, &pixDesc);
        //assert(glGetError() == GL_NO_ERROR);
        SetPixelFormat(dc, pixFormat, &pixDesc);
        //assert(glGetError() == GL_NO_ERROR);
        HGLRC context = wglCreateContext(dc);
        //assert(glGetError() == GL_NO_ERROR);
        assert(context);

        wglMakeCurrent(dc, context);
        //assert(glGetError() == GL_NO_ERROR);

        int ers = 0;
        if (!gladLoadWGL(dc)) ers++;
        if (!gladLoadGL()) ers++;
        assert(ers == 0);

        wglMakeCurrent(dc, 0);
        wglDeleteContext(context);
        ReleaseDC(handle, dc);
        DestroyWindow(handle);
        //assert(glGetError() == GL_NO_ERROR);
	}

    GLsurface createSurface(HWND hwnd)
    {
        SurfaceInfo info{};
        info.dc = GetDC(hwnd);

        int pixelFormatAttributes[] = {
                WGL_DRAW_TO_WINDOW_ARB, GL_TRUE,
                WGL_SUPPORT_OPENGL_ARB, GL_TRUE,
                WGL_DOUBLE_BUFFER_ARB, GL_TRUE,
                WGL_ACCELERATION_ARB, WGL_FULL_ACCELERATION_ARB,
                WGL_PIXEL_TYPE_ARB, WGL_TYPE_RGBA_ARB,
                WGL_COLOR_BITS_ARB, 32,
                WGL_DEPTH_BITS_ARB, 24,
                WGL_STENCIL_BITS_ARB, 8,
                0
        };
        int pixelFormat = 0;
        UINT numFormats = 0;
        wglChoosePixelFormatARB(info.dc, pixelFormatAttributes, nullptr, 1, &pixelFormat, &numFormats);
        assert(numFormats);

        PIXELFORMATDESCRIPTOR pixelFormatDesc = {};
        DescribePixelFormat(info.dc, pixelFormat, sizeof(PIXELFORMATDESCRIPTOR), &pixelFormatDesc);
        SetPixelFormat(info.dc, pixelFormat, &pixelFormatDesc);
        int openGLAttributes[] = {
            WGL_CONTEXT_MAJOR_VERSION_ARB, 4,
            WGL_CONTEXT_MINOR_VERSION_ARB, 6,
            WGL_CONTEXT_PROFILE_MASK_ARB, WGL_CONTEXT_CORE_PROFILE_BIT_ARB,
            0
        };

        info.glrc = wglCreateContextAttribsARB(info.dc, 0, openGLAttributes);
        assert(info.glrc);

        ui32 id{ surfaces.add(info) };

        return GLsurface{ id };
    }

    void removeSurface(ui32 id)
    {
        SurfaceInfo info{ getFromId(id) };

        glDeleteVertexArrays(1, &info.vao);
        glDeleteBuffers(1, &info.vbo);
        glDeleteBuffers(1, &info.ebo);
        surfaces.remove(id);
    }

    namespace // anonymous
    {
        void makeSurfaceContextCurrent(ui32 id)
        {
            SurfaceInfo& info{ getFromId(id) };
            assert(info.glrc);
            assert(info.dc);

            wglMakeCurrent(info.dc, info.glrc);
        }

        void swapSurfaceBuffers(ui32 id, bool vsync)
        {
            SurfaceInfo& info{ getFromId(id) };
            assert(info.dc);

            wglSwapIntervalEXT(vsync);
            wglSwapLayerBuffers(info.dc, WGL_SWAP_MAIN_PLANE);
        }

        void setupSurfaceBuffers(ui32 id, f32 verts[], ui32 idxs[])
        {
            SurfaceInfo& info{ getFromId(id) };

            glGenVertexArrays(1, &info.vao);
            glGenBuffers(1, &info.vbo);

            glBindVertexArray(info.vao);

            glBindBuffer(GL_ARRAY_BUFFER, info.vbo);
            glBufferData(GL_ARRAY_BUFFER, sizeof(verts), verts, GL_STATIC_DRAW);

            glVertexAttribPointer(
                0,                  // vertex attribute location -> layout (location = n)
                3,                  // vertex attribute size (vec3 = 3)
                GL_FLOAT,           // data type
                GL_FALSE,           // normalize data
                3 * sizeof(float),  // stride for vertex attribute data
                (void*)0);          // vertex attribute data offset

            glEnableVertexAttribArray(0);

            glGenBuffers(1, &info.ebo);

            glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, info.ebo);
            glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(idxs), idxs, GL_STATIC_DRAW);
            assert(glGetError() == GL_NO_ERROR);
        }

        void loadSurfaceShader(ui32 id, const char* vertSource, const char* fragSource)
        {
            SurfaceInfo& info{ getFromId(id) };

            info.vertShader = glCreateShader(GL_VERTEX_SHADER);
            glShaderSource(info.vertShader, 1, &vertSource, NULL);
            glCompileShader(info.vertShader);
            assert(glGetError() == GL_NO_ERROR);

            info.fragShader = glCreateShader(GL_FRAGMENT_SHADER);
            glShaderSource(info.fragShader, 1, &fragSource, NULL);
            glCompileShader(info.fragShader);
            assert(glGetError() == GL_NO_ERROR);

            info.shProgram = glCreateProgram();
            glAttachShader(info.shProgram, info.vertShader);
            glAttachShader(info.shProgram, info.fragShader);
            glLinkProgram(info.shProgram);

            glDeleteShader(info.vertShader);
            glDeleteShader(info.fragShader);
            assert(glGetError() == GL_NO_ERROR);
        }

        void  setSurfaceViewport(ui32 id, ui32v4 rect)
        {
            SurfaceInfo& info{ getFromId(id) };
            glViewport(rect.x, rect.y, rect.z, rect.w);
        }

        void doSurfaceUpdateStuff(ui32 id)
        {
            SurfaceInfo& info{ getFromId(id) };

            glClearColor(0.5, 0.15, 1, 1);
            glClear(GL_COLOR_BUFFER_BIT);

            int varLoc = glGetUniformLocation(info.shProgram, "time");
            glUseProgram(info.shProgram);
            glUniform1f(varLoc, 0.0f);

            glBindVertexArray(info.vao);
            glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, 0);
            glBindVertexArray(0);

            swapSurfaceBuffers(id, false);
            assert(glGetError() == GL_NO_ERROR);
        }
    } // anonymous namespace
}


void GLsurface::makeContextCurrent()
{
    assert(isValid());
    GlPlatform::makeSurfaceContextCurrent(_id);
}

void GLsurface::swapBuffers(bool vsync)
{
    assert(isValid());
    GlPlatform::swapSurfaceBuffers(_id, vsync);
}

void GLsurface::setupBuffers(f32 verts[], ui32 idxs[])
{
    assert(isValid());
    GlPlatform::setupSurfaceBuffers(_id, verts, idxs);
}

void GLsurface::loadShader(const char* vertSource, const char* fragSource)
{
    assert(isValid());
    GlPlatform::loadSurfaceShader(_id, vertSource, fragSource);
}

void GLsurface::setViewport(ui32v4 rect)
{
    assert(isValid());
    GlPlatform::setSurfaceViewport(_id, rect);
}

void GLsurface::doUpdateStuff()
{
    assert(isValid());
    GlPlatform::doSurfaceUpdateStuff(_id);
}