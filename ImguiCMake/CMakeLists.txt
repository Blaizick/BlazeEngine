cmake_minimum_required(VERSION 4.0)

project(ImGuiCMake LANGUAGES C CXX)

set(CMAKE_CXX_STANDARD 17)

add_library(ImGuiCMake SHARED
	imgui.cpp
	imconfig.h
	imgui.h
	imgui.cpp
	imgui_demo.cpp
	imgui_draw.cpp
	imgui_internal.h
	imgui_tables.cpp
	imgui_widgets.cpp
	imstb_rectpack.h
	imstb_textedit.h
	imstb_truetype.h
	backends/imgui_impl_opengl3.cpp
	backends/imgui_impl_opengl3.h
	backends/imgui_impl_opengl3_loader.h
	backends/imgui_impl_sdl3.cpp
	backends/imgui_impl_sdl3.h
	misc/cpp/imgui_stdlib.cpp
	misc/cpp/imgui_stdlib.h
	imgui_exports.cpp
)

target_include_directories(ImGuiCMake PRIVATE .
	backends
	lib/sdl3/include
	misc/cpp
)

target_link_libraries(ImGuiCMake PRIVATE ${CMAKE_SOURCE_DIR}/lib/sdl3/lib/libSDL3.dll.a
    opengl32
)

set_target_properties(ImGuiCMake PROPERTIES
    WINDOWS_EXPORT_ALL_SYMBOLS ON
    OUTPUT_NAME "ImGui"
)