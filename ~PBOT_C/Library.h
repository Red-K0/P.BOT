#pragma once

#include <cstdint>

#define API __declspec(dllexport)

extern "C" API bool EnableVirtual();

extern "C" API void SetScreenSize(int X, int Y);

extern "C" API bool SwitchScreenBuffer(bool Clear);

extern "C" API void RevCursorIndex();

extern "C" API void CursorMemory(bool Mode, bool ANSILegacyMode);

extern "C" API bool ModCursorPosition(int Mode, int n);

extern "C" API void SetCursorPosition(int X, int Y, bool LegacyMode);

extern "C" API bool ToggleCursorBlink(int Set);

extern "C" API bool ToggleCursorVisibility(int Set);

extern "C" API bool SetCursorShape(int Choice);

extern "C" API bool SetColor(int ForegroundColor, int BackgroundColor);

extern "C" API void SetColorExtended(bool LayerSelect, uint8_t R, uint8_t G, uint8_t B);

extern "C" API void SetColorIndexed(bool LayerSelect, uint8_t s);

extern "C" API bool SetColorPalette(int Select);

extern "C" API void ModPalette(int index, uint8_t R, uint8_t G, uint8_t B);
