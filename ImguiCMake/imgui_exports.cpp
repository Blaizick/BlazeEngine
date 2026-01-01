#include <iostream>
#include <memory>
#include <string>

#include "SDL3/SDL.h"
#include "SDL3/SDL_opengl.h"

#include "imgui.h"
#include "backends/imgui_impl_sdl3.h"
#include "backends/imgui_impl_opengl3.h"
#include "misc/cpp/imgui_stdlib.h"

class ImGuiContext{
private:
public:
    bool running = true;
    SDL_Event event;
    ImGuiIO io;
    SDL_Window* window;

    bool wasFileDroppedThisFrame;
    std::string droppedFileSource;
};

static ImGuiContext imguiContext;

#ifdef _WIN32
#define API __declspec(dllexport)
#else
#define API
#endif

#ifdef __cplusplus
extern "C" {
#endif


API void ImGui_Init(){
    SDL_Init(SDL_INIT_VIDEO | SDL_INIT_GAMEPAD);

    const char* glsl_version = "#version 130";
    SDL_GL_SetAttribute(SDL_GL_CONTEXT_FLAGS, 0);
    SDL_GL_SetAttribute(SDL_GL_CONTEXT_PROFILE_MASK, SDL_GL_CONTEXT_PROFILE_CORE);
    SDL_GL_SetAttribute(SDL_GL_CONTEXT_MAJOR_VERSION, 3);
    SDL_GL_SetAttribute(SDL_GL_CONTEXT_MINOR_VERSION, 0);

    SDL_GL_SetAttribute(SDL_GL_DOUBLEBUFFER, 1);
    SDL_GL_SetAttribute(SDL_GL_DEPTH_SIZE, 24);
    SDL_GL_SetAttribute(SDL_GL_STENCIL_SIZE, 8);
    float mainScale = SDL_GetDisplayContentScale(SDL_GetPrimaryDisplay());
    SDL_WindowFlags windowFlags = SDL_WINDOW_OPENGL | SDL_WINDOW_RESIZABLE | SDL_WINDOW_HIGH_PIXEL_DENSITY | SDL_WINDOW_TRANSPARENT;
    SDL_Window* window = SDL_CreateWindow("SDL Window!", (int)(600 * mainScale), (int)(400 * mainScale), windowFlags);
    SDL_GLContext glContext = SDL_GL_CreateContext(window);

    imguiContext.window = window;

    SDL_GL_MakeCurrent(window, glContext);
    SDL_GL_SetSwapInterval(1);
    SDL_SetWindowPosition(window, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED);
    SDL_ShowWindow(window);

    IMGUI_CHECKVERSION();
    ImGui::CreateContext();
    ImGuiIO& io = ImGui::GetIO(); (void)io;
    io.ConfigFlags |= ImGuiConfigFlags_NavEnableKeyboard;
    io.ConfigFlags |= ImGuiConfigFlags_NavEnableGamepad;
    io.ConfigFlags |= ImGuiConfigFlags_DockingEnable;
    imguiContext.io = io;

    ImGui_ImplSDL3_InitForOpenGL(window, glContext);
    ImGui_ImplOpenGL3_Init(glsl_version);
}

API void ImGui_PreDraw(){
    imguiContext.wasFileDroppedThisFrame = false;
    while (SDL_PollEvent(&imguiContext.event)){
        auto& e = imguiContext.event;
        ImGui_ImplSDL3_ProcessEvent(&e);
        if (e.type == SDL_EVENT_QUIT){
            imguiContext.running = false;
        }
        if (e.type == SDL_EVENT_DROP_FILE){
            if (e.drop.data != nullptr){
                imguiContext.droppedFileSource = std::string(e.drop.data);
                imguiContext.wasFileDroppedThisFrame = true;
            }
        }
    }
    ImGui_ImplOpenGL3_NewFrame();
    ImGui_ImplSDL3_NewFrame();
    ImGui::NewFrame();
}
API void ImGui_PostDraw(){
    ImGui::Render();
    glViewport(0, 0, (int)imguiContext.io.DisplaySize.x, (int)imguiContext.io.DisplaySize.y);
    glClearColor(0.0f, 0.0f, 0.0f, 1.0f);
    glClear(GL_COLOR_BUFFER_BIT);
    ImGui_ImplOpenGL3_RenderDrawData(ImGui::GetDrawData());
    SDL_GL_SwapWindow(imguiContext.window);
}

API bool ImGui_IsRunning(){
    return imguiContext.running;
}

API bool ImGui_BeginWithCloseButton(const char* text, bool* p_open, int flags){
    return ImGui::Begin(text, p_open, flags);
}
API bool ImGui_Begin(const char* text, int flags){
    return ImGui::Begin(text, 0, flags);
}

API void ImGui_End(){
    ImGui::End();
} 

API void ImGui_Text(const char* text){
    ImGui::Text(text);
}

API bool ImGui_Button(const char* label){
    return ImGui::Button(label);
}
API bool ImGui_ButtonWithSize(const char* label, float width, float height){
    return ImGui::Button(label, {width, height});
}

API bool ImGui_InputText(const char* label, char* buffer, int bufferSize){
    return ImGui::InputText(label, buffer, bufferSize);
}
API bool ImGui_InputFloat(const char* label, float* value){
    return ImGui::InputFloat(label, value);
}

API bool ImGui_BeginTable(const char* id, int columns, int flags){
    return ImGui::BeginTable(id, columns, flags);
}
API void ImGui_EndTable(){
    ImGui::EndTable();
}

API void ImGui_TableNextRow(){
    ImGui::TableNextRow();
}
API void ImGui_TableNextColumn(){
    ImGui::TableNextColumn();
}

API bool ImGui_TableSetColumnIndex(int index){
    return ImGui::TableSetColumnIndex(index);
}

API void ImGui_PushStringId(const char* id){
    ImGui::PushID(id);
}
API void ImGui_PushIntId(int id){
    ImGui::PushID(id);
}
API void ImGui_PopId(){
    ImGui::PopID();
}

API void ImGui_ShowDemoWindow(){
    ImGui::ShowDemoWindow();
}

API bool ImGui_BeginMainMenuBar(){
    return ImGui::BeginMainMenuBar();
}
API void ImGui_EndMainMenuBar(){
    ImGui::EndMainMenuBar();
}

API bool ImGui_BeginMenu(const char* name, bool enabled){
    return ImGui::BeginMenu(name, enabled);
}
API void ImGui_EndMenu(){
    ImGui::EndMenu();
}

API bool ImGui_MenuItem(const char* label, const char* shortcut, bool p_selected, bool enabled){
    return ImGui::MenuItem(label, shortcut, p_selected, enabled);
}

API int ImGui_GetDroppedFile(char* buffer, int bufferSize)
{
    if (!imguiContext.wasFileDroppedThisFrame)
        return 0;
    const auto& str = imguiContext.droppedFileSource;
    int len = (int)str.size();
    if (buffer && bufferSize > len)
        memcpy(buffer, str.c_str(), len + 1);
    return len;
}

API void ImGui_Separator(){
    ImGui::Separator();
}
API void ImGui_SeparatorText(const char* text){
    ImGui::SeparatorText(text);
}

API void ImGui_SameLine(){
    ImGui::SameLine();
}

API void ImGui_BeginCustomMainDockspace(){
    auto viewport = ImGui::GetMainViewport();

    ImGui::SetNextWindowPos(viewport->Pos);
    ImGui::SetNextWindowSize(viewport->Size);
    ImGui::SetNextWindowViewport(viewport->ID);

    ImGui::PushStyleVar(ImGuiStyleVar_WindowRounding, 0.0f);
    ImGui::PushStyleVar(ImGuiStyleVar_WindowBorderSize, 0.0f);
    ImGui::PushStyleVar(ImGuiStyleVar_WindowPadding, {0,0});

    ImGuiWindowFlags windowFlags =
        ImGuiWindowFlags_NoDocking |
        ImGuiWindowFlags_NoTitleBar |
        ImGuiWindowFlags_NoCollapse |
        ImGuiWindowFlags_NoResize |
        ImGuiWindowFlags_NoMove |
        ImGuiWindowFlags_NoBringToFrontOnFocus |
        ImGuiWindowFlags_NoNavFocus |
        ImGuiWindowFlags_MenuBar;

    ImGui::Begin("MainDockSpace", 0, windowFlags);

    ImGui::PopStyleVar(3);

    auto dockspaceId = ImGui::GetID("MainDockSpaceID");
    ImGui::DockSpace(
        dockspaceId,
        {0, 0},
        ImGuiDockNodeFlags_PassthruCentralNode
    );
}

#ifdef __cplusplus
}
#endif