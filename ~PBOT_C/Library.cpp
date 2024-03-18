#pragma region Boilerplate

#include "pch.h"
#include "framework.h"
#include "library.h"
#include <sstream>
#include <stdio.h>
#include <Windows.h>

///<summary> The escape sequence used for virtual terminal sequences. </summary>
#define ESC "\x1b"

//Variables
///<summary> Is the console's altenate buffer currently active? </summary>
bool AlternateBufferActive = false;

#pragma endregion

/// <summary> Modifies the console output mode to handle virtual sequences. Necessary to utilize any virtual sequences. </summary>
/// <returns> True if successful, otherwise returns false. </returns> 
bool EnableVirtual()
	{
		// Set output mode to handle virtual terminal sequences
		HANDLE hOut = GetStdHandle(STD_OUTPUT_HANDLE);
		if (hOut == INVALID_HANDLE_VALUE)
		{
			return false;
		}
		HANDLE hIn = GetStdHandle(STD_INPUT_HANDLE);
		if (hIn == INVALID_HANDLE_VALUE)
		{
			return false;
		}

		DWORD dwOriginalOutMode = 0;
		DWORD dwOriginalInMode = 0;
		if (!GetConsoleMode(hOut, &dwOriginalOutMode))
		{
			return false;
		}
		if (!GetConsoleMode(hIn, &dwOriginalInMode))
		{
			return false;
		}

		DWORD dwRequestedOutModes = ENABLE_VIRTUAL_TERMINAL_PROCESSING;
		DWORD dwRequestedInModes = ENABLE_VIRTUAL_TERMINAL_INPUT;

		DWORD dwOutMode = dwOriginalOutMode | dwRequestedOutModes;
		if (!SetConsoleMode(hOut, dwOutMode))
		{
			// we failed to set both modes, try to step down mode gracefully.
			dwRequestedOutModes = ENABLE_VIRTUAL_TERMINAL_PROCESSING;
			dwOutMode = dwOriginalOutMode | dwRequestedOutModes;
			if (!SetConsoleMode(hOut, dwOutMode))
			{
				// Failed to set any VT mode, can't do anything here.
				return false;
			}
		}

		DWORD dwInMode = dwOriginalInMode | dwRequestedInModes;
		if (!SetConsoleMode(hIn, dwInMode))
		{
			// Failed to set VT input mode, can't do anything here.
			return false;
		}

		return true;
	}

/// <summary> Sets the console window and buffer size to the value specified by the given parameters. </summary>
/// <param name="X:"> The width of the console window in characters. </param>
/// <param name="Y:"> The height of the console window in characters. </param>
void SetScreenSize(int X, int Y)
{
	std::ostringstream command;
	command << ESC << "[8;" << X << ';' << Y << 't';
	printf(command.str().c_str());
}

/// <summary> Switches the active console buffer to the alternate buffer if the main buffer is active, otherwise returns to the main console buffer. </summary>
/// <param name="Clear:"> Optional, clears the switched-from buffer if set to true. </param>
/// <returns> True if the switched-to buffer was the alternate buffer, otherwise returns false. </returns>
bool SwitchScreenBuffer(bool Clear = false)
		{
			if (Clear) printf(ESC "[2J" ESC "[H");
			if (!AlternateBufferActive)
			{
				printf(ESC "[?1049h");
				AlternateBufferActive = true;
				return true;
			}
			else
			{
				printf(ESC "[?1049l");
				AlternateBufferActive = false;
				return false;
			}
		}

/// <summary> Is cursor blinking currently enabled? </summary>
static bool IsBlinking;

/// <summary> Is the cursor currently visible? </summary>
static bool IsVisible;

void RevCursorIndex()
{
	printf(ESC "M");
}

/// <summary> Saves the cursor position to memory if 'Mode' is set to true, otherwise restores it. </summary>
/// <param name="ANSILegacyMode:"> Optional, when set to true, uses ANSI.sys emulation for compatability. </param>
void CursorMemory(bool Mode, bool ANSILegacyMode = false)
{
	if (ANSILegacyMode) { if (Mode) printf(ESC "[s"); else printf(ESC "[u"); }
	else if (Mode) printf(ESC "7"); else printf(ESC "8");
}

/// <summary> Moves the cursor according to the given parameters within the viewport, will not scroll. </summary>
/// <param name="Mode:"> Specifies the type of movement according to the table attached. </param>
/// <param name="Table:"> [ 0: Up by 'n' | 1: Down by 'n' | 2: Right by 'n' | 3: Left by 'n' | 4: Down 'n' lines | 5: Up 'n' lines ].
/// Set Position [ 6: Set X to 'n' | 7: Set Y to 'n' ] </param>
/// <returns> True if the given mode was valid, otherwise returns false. </returns>
bool ModCursorPosition(int Mode, int n)
{
	std::ostringstream command;
	command << ESC << '[' << n << "%c";
	switch (Mode)
	{
		default: return false;
		case 0: printf(command.str().c_str(), 'A'); break; // Cursor Up
		case 1: printf(command.str().c_str(), 'B'); break; // Cursor Down
		case 2: printf(command.str().c_str(), 'C'); break; // Cursor Forward
		case 3: printf(command.str().c_str(), 'D'); break; // Cursor Backward
		case 4: printf(command.str().c_str(), 'E'); break; // Cursor Next Line
		case 5: printf(command.str().c_str(), 'F'); break; // Cursor Previous Line
		case 6: printf(command.str().c_str(), 'G'); break; // Cursor Horizontal Absolute
		case 7: printf(command.str().c_str(), 'd'); break; // Vertical Line Position Absolute
	}
	return true;
}

/// <summary> Sets the X and Y position of the cursor. </summary>
/// <param name="X:"> The horizontal position of the cursor. </param>
/// <param name="Y:"> The vertical position of the cursor. </param>
/// <param name="LegacyMode:"> If set to true, will perform an HVP call rather than a CUP call. </param>
void SetCursorPosition(int X, int Y, bool LegacyMode = false)
{
	std::ostringstream command;
	command << ESC << '[' << Y << ';' << X;
	if (!LegacyMode)  command << 'H'; else command << 'f';
	printf(command.str().c_str());
}

/// <summary> Toggles the cursor blinking behaviour in the console. </summary>
/// <param name="Set:"> Optional, forces the blinking off when 0, and on when 1. </param>
/// <returns> True on a successful call, otherwise returns false. </returns>
bool ToggleCursorBlink(int Set = -1)
{
	switch (Set)
	{
		default: return false;
		case -1: if (IsBlinking) { printf(ESC "[?12l"); IsBlinking = false; }
				else { printf(ESC "[?12h"); IsBlinking = true; } break;
		case  0: printf(ESC "[?12l"); IsBlinking = false; break;
		case  1: printf(ESC "[?12h"); IsBlinking = true; break;
	}
}

/// <summary> Toggles the cursor visibility. </summary>
/// <param name="Set:"> Optional, forces visibility to off when 0, and on when 1. </param>
/// <returns> True on a successful call, otherwise returns false. </returns>
bool ToggleCursorVisibility(int Set = -1)
{
	switch (Set)
	{
		default: return false;
		case -1: if (IsVisible) { printf(ESC "[?25l"); IsVisible = false; }
				else { printf(ESC "[?25h"); IsVisible = true; } break;
		case  0: printf(ESC "[?25l"); IsVisible = false; break;
		case  1: printf(ESC "[?25h"); IsVisible = true; break;
	}
}

/// <summary> Sets the shape of the cursor according to the given 'Choice' parameter. </summary>
/// <param name="Choice:"> The shape to set the cursor to according to the attached table. </param>
/// <param name="Table:"> [ 0: Defaults | 1: Block | 3: Underline | 5: Bar ] Add 1 for the non-blinking version of a shape. </param>
/// <returns> True on a successful call, otherwise returns false. </returns>
bool SetCursorShape(int Choice)
{
	switch (Choice)
	{
		default: return false;
		case  0: printf(ESC "[0 q"); IsBlinking = true;  break; // Default
		case  1: printf(ESC "[1 q"); IsBlinking = true;  break; // Block Blinking
		case  2: printf(ESC "[2 q"); IsBlinking = false; break; // Block Steady
		case  3: printf(ESC "[3 q"); IsBlinking = true;  break; // Underline Blinking
		case  4: printf(ESC "[4 q"); IsBlinking = false; break; // Underline Steady
		case  5: printf(ESC "[5 q"); IsBlinking = true;  break; // Bar Blinking
		case  6: printf(ESC "[6 q"); IsBlinking = false; break; // Bar Steady
	}
	return true;
}

/// <summary> Sets the console foreground and background colors using the parameters called.</summary>
/// <param name="ForegroundColor:"> The color to set the foreground color to according to the color table attached. </param>
/// <param name="BackgroundColor:"> The color to set the background color to according to the color table attached. </param>
/// <param name=""></param>
/// <param name="Table:"> [ 1: Black | 2: Red | 3: Green | 4: Yellow | 5: Blue | 6: Magenta | 7: Cyan | 8: White ]
/// Add 8 for a bright/bold version of any given color, use 0 to reset to the default color, or -1 to leave the color as is.</param>
/// <returns> True if the given color choices were set successfully, otherwise returns false. </returns>
bool SetColor(int ForegroundColor, int BackgroundColor)
{
	switch (ForegroundColor)
	{
		default: return false;
		case -1: break; // Do Nothing
		case  0: printf(ESC "[39m"); break; // Reset
		case  1: printf(ESC "[30m"); break; // Black
		case  2: printf(ESC "[31m"); break; // Red
		case  3: printf(ESC "[32m"); break; // Green
		case  4: printf(ESC "[33m"); break; // Yellow
		case  5: printf(ESC "[34m"); break; // Blue
		case  6: printf(ESC "[35m"); break; // Magenta
		case  7: printf(ESC "[36m"); break; // Cyan
		case  8: printf(ESC "[37m"); break; // White
		case  9: printf(ESC "[90m"); break; // Bright Black
		case 10: printf(ESC "[91m"); break; // Bright Red
		case 11: printf(ESC "[92m"); break; // Bright Green
		case 12: printf(ESC "[93m"); break; // Bright Yellow
		case 13: printf(ESC "[94m"); break; // Bright Blue
		case 14: printf(ESC "[95m"); break; // Bright Magenta
		case 15: printf(ESC "[96m"); break; // Bright Cyan
		case 16: printf(ESC "[97m"); break; // Bright White
	}
	switch (BackgroundColor)
	{
		default: return false;
		case -1: break; // Do Nothing
		case  0: printf(ESC "[49m");  break; // Reset
		case  1: printf(ESC "[40m");  break; // Black
		case  2: printf(ESC "[41m");  break; // Red
		case  3: printf(ESC "[42m");  break; // Green
		case  4: printf(ESC "[43m");  break; // Yellow
		case  5: printf(ESC "[44m");  break; // Blue
		case  6: printf(ESC "[45m");  break; // Magenta
		case  7: printf(ESC "[46m");  break; // Cyan
		case  8: printf(ESC "[47m");  break; // White
		case  9: printf(ESC "[100m"); break; // Bright Black
		case 10: printf(ESC "[101m"); break; // Bright Red
		case 11: printf(ESC "[102m"); break; // Bright Green
		case 12: printf(ESC "[103m"); break; // Bright Yellow
		case 13: printf(ESC "[104m"); break; // Bright Blue
		case 14: printf(ESC "[105m"); break; // Bright Magenta
		case 15: printf(ESC "[106m"); break; // Bright Cyan
		case 16: printf(ESC "[107m"); break; // Bright White
	}

	return true;
}

/// <summary> Sets the console foreground or background color to the specified RGB value. </summary>
/// <param name="LayerSelect:"> Indicates a foreground color if true, and a background color if false. </param>
/// <param name="R, G, B:"> Values between 0 and 255 representing the components of the color. </param>
void SetColorExtended(bool LayerSelect, uint8_t R, uint8_t G, uint8_t B)
{
	std::ostringstream command;
	if (LayerSelect) command << ESC << "[38;2;" << R << ';' << G << ';' << B;
	else command << ESC << "[48;2;" << R << ';' << G << ';' << B;
	printf(command.str().c_str());
}

/// <summary> Sets the console foreground or background color to the specified index in the xterm 88/256 color table. </summary>
/// <param name="LayerSelect:"> Indicates a foreground color if true, and a background color if false. </param>
/// <param name="s:"> The color index to apply from the xterm table. </param>
void SetColorIndexed(bool LayerSelect, uint8_t s)
{
	std::ostringstream command;
	if (LayerSelect) command << ESC << "[38;5;" << s;
	else command << ESC << "[48;5;" << s;
	printf(command.str().c_str());
}

/// <summary> Applies a modifier to the current console colors. Resets all parameters to defaults if the input is -1. </summary>
/// <param name ="Select:"> Applies a modifier from the attached table. </param>
/// <param name="Table:"> [ 0: Negative Colors | 1: Positive Colors | 2: Brighten foreground | 3: Dull foreground | 4: Underline On | 5: Underline Off ] </param>
/// <returns> True on a successful call, otherwise returns false. </returns>
bool SetColorPalette(int Select)
{
	switch (Select)
	{
		default: return false;
		case -1: printf(ESC "[0m"); break;
		case  0: printf(ESC "[7m"); break;
		case  1: printf(ESC "[27m"); break;
		case  2: printf(ESC "[1m"); break;
		case  3: printf(ESC "[22m"); break;
		case  4: printf(ESC "[4m"); break;
		case  5: printf(ESC "[24m"); break;
	}
}

/// <summary> Modifies the color palette value at the given index to match the given RGB value. </summary>
/// <param name="index"> The index of the color to modify. </param>
/// <param name="R, G, B:"> Values between 0 and 255 representing the components of the color. </param>
void ModPalette(int index, uint8_t R, uint8_t G, uint8_t B)
{
	std::ostringstream command;
	command << ESC"]4;" << index << ";rgb:" << R << '/' << G << '/' << B << ESC << '\\';
	printf(command.str().c_str());

}