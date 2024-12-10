#include <emscripten.h>

EMSCRIPTEN_KEEPALIVE
void custom_emscripten_set_element_style_size(const char* element_id, int width, int height) {
    EM_ASM({
        const elementId = UTF8ToString($0);
        const width = $1;
        const height = $2;

        const element = document.getElementById(elementId);
        if (element) {
            element.style.width = width + 'px';
            element.style.height = height + 'px';
        }
    }, element_id, width, height);
}

EMSCRIPTEN_KEEPALIVE
void custom_emscripten_get_display_usable_bounds(int* width, int* height) {
    MAIN_THREAD_EM_ASM({
        if ($0) {
            HEAP32[$0 >> 2] = window.innerWidth;
        }
        if ($1) {
            HEAP32[$1 >> 2] = window.innerHeight;
        }
    }, width, height);
}