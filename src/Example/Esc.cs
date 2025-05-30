﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
    /// <summary>
    ///     ANSI escape definitions and utility methods.
    /// </summary>
    internal static class Esc
    {
        // Control
        public const string EndOfText = "\u0003";
        public const string EndOfTransmission = "\u0004";

        // style modifiers
        public const string Reset = "\u001b[0m";
        public const string Normal = "\u001b[22m";
        public const string Bold = "\u001b[1m";
        public const string Dim = "\u001b[2m";

        // text decorations
        public const string Italic = "\u001b[3m";
        public const string Underline = "\u001b[4m";
        public const string Strikethrough = "\u001b[9m";

        // screen buffer
        public const string EnableAlternateBuffer = "\u001b[?1049h";
        public const string DisableAlternateBuffer = "\u001b[?1049l";
        public const string ClearScreen = "\u001b[2J";

        // cursor
        public const string HideCursor = "\u001b[?25l";
        public const string ShowCursor = "\u001b[?25h";

        // cursor shape
        public const string BlinkingBlockCursor = "\u001b[1 q";
        public const string SteadyBlockCursor = "\u001b[2 q";
        public const string BlinkingUnderlineCursor = "\u001b[3 q";
        public const string SteadyUnderlineCursor = "\u001b[4 q";
        public const string BlinkingBarCursor = "\u001b[5 q";
        public const string SteadyBarCursor = "\u001b[6 q";

        // bracketed 
        public const string EnableBracketedPasteMode = "\u001b[?2004h";
        public const string DisableBracktedPasteMode = "\u001b[?2004l";

        // mouse tracking
        public const string EnableMouseTracking = "\u001b[?1000h";
        public const string DisableMouseTracking = "\u001b[?1000l";
        
        public const string EnableMouseMotionTracking = "\u001b[?1002h";
        public const string DisableMouseMotionTracking = "\u001b[?1002l";
        
        public const string EnableExtendedMouseTracking = "\u001b[?1006h";
        public const string DisableExtendedMouseTracking = "\u001b[?1006l";

        // Cursor Keys input sequences
        public const string ApplicationCursorKeys = "\u001b[?1h";
        public const string NormalCursorKeys = "\u001b[?1l";

        // Keypad input sequences
        public const string ApplicationKeypad = "\u001b=";
        public const string NumericKeypad = "\u001b>";

        // Save/Restore cursor position
        public const string SaveCursorPosition = "\u001b[s";
        public const string RestoreCursorPosition = "\u001b[u";

        // input ansi key codes
        public const string UpKey = "\u001b[A";
        public const string DownKey = "\u001b[B";
        public const string RightKey = "\u001b[C";
        public const string LeftKey = "\u001b[D";
        public const string HomeKey = "\u001b[H";
        public const string EndKey = "\u001b[F";
        public const string PageUpKey = "\u001b[5~";
        public const string PageDownKey = "\u001b[6~";
        public const string InsertKey = "\u001b[2~";
        public const string DeleteKey = "\u001b[3~";
        public const string F1Key = "\u001bOP";
        public const string F2Key = "\u001bOQ";
        public const string F3Key = "\u001bOR";
        public const string F4Key = "\u001bOS";
        public const string F5Key = "\u001b[15~";
        public const string F6Key = "\u001b[17~";
        public const string F7Key = "\u001b[18~";
        public const string F8Key = "\u001b[19~";
        public const string F9Key = "\u001b[20~";
        public const string F10Key = "\u001b[21~";
        public const string F11Key = "\u001b[23~";
        public const string F12Key = "\u001b[24~";
        public const string ControlUpArrow = "\u001b[1;5A";
        public const string ControlDownArrow = "\u001b[1;5B";
        public const string ControlRightArrow = "\u001b[1;5C";
        public const string ControlLeftArrow = "\u001b[1;5D";

        // input ansi mouse codes
        public const string MouseLeftButton = "\u001b[M";
        public const string MouseMiddleButton = "\u001b[M";
        public const string MouseRightButton = "\u001b[M";
        public const string MouseScrollUp = "\u001b[M";
        public const string MouseScrollDown = "\u001b[M";
        public const string MouseRelease = "\u001b[M";


        // move cursor
        public static string MoveCursorUp(int n)
        {
            return $"\u001b[{n}A";
        }

        public static string MoveCursorDown(int n)
        {
            return $"\u001b[{n}B";
        }

        public static string MoveCursorRight(int n)
        {
            return $"\u001b[{n}C";
        }

        public static string MoveCursorLeft(int n)
        {
            return $"\u001b[{n}D";
        }

        public static string SetCursorPosition(int x, int y)
        {
            return $"\u001b[{y + 1};{x + 1}f";
        }

        public static string Foreground(object color)
        {
            return color switch
            {
                ConsoleColor consoleColor => Foreground(consoleColor),
                Color rgbColor => Foreground(rgbColor),
                _ => throw new ArgumentException("Invalid color type")
            };
        }

        public static string Foreground(ConsoleColor color)
        {
            int ansiCode = GetAnsiCode(color);
            return ansiCode < 8
                ?
                // Standard colors
                $"\x1b[{30 + ansiCode}m"
                :
                // Bright colors
                $"\x1b[{90 + (ansiCode - 8)}m";
        }

        public static string Background(object color)
        {
            return color switch
            {
                ConsoleColor consoleColor => Background(consoleColor),
                Color rgbColor => Background(rgbColor),
                _ => throw new ArgumentException("Invalid color type")
            };
        }

        public static string Background(ConsoleColor color)
        {
            int ansiCode = GetAnsiCode(color);
            return ansiCode < 8
                ?
                // Standard colors
                $"\x1b[{40 + ansiCode}m"
                :
                // Bright colors
                $"\x1b[{100 + (ansiCode - 8)}m";
        }

        public static int GetAnsiCode(ConsoleColor color)
        {
            return color switch
            {
                ConsoleColor.Black => 0,
                ConsoleColor.DarkRed => 1,
                ConsoleColor.DarkGreen => 2,
                ConsoleColor.DarkYellow => 3,
                ConsoleColor.DarkBlue => 4,
                ConsoleColor.DarkMagenta => 5,
                ConsoleColor.DarkCyan => 6,
                ConsoleColor.Gray => 7,
                ConsoleColor.DarkGray => 8,
                ConsoleColor.Red => 9,
                ConsoleColor.Green => 10,
                ConsoleColor.Yellow => 11,
                ConsoleColor.Blue => 12,
                ConsoleColor.Magenta => 13,
                ConsoleColor.Cyan => 14,
                ConsoleColor.White => 15,
                _ => 7 // Default to white if unknown
            };
        }

        public static string Foreground(Color color)
        {
            return Foreground(color.R, color.G, color.B);
        }

        public static string Foreground(byte red, byte green, byte blue)
        {
            return $"\u001b[38;2;{red};{green};{blue}m";
        }

        public static string Background(Color color)
        {
            return Background(color.R, color.G, color.B);
        }

        public static string Background(byte red, byte green, byte blue)
        {
            return $"\u001b[48;2;{red};{green};{blue}m";
        }

        public static string SetWindowTitle(string title)
        {
            return $"\u001b]0;{title}\u0007";
        }
    }
}