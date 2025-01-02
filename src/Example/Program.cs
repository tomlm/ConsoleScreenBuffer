using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

namespace Example
{
    internal class Program
    {
        [SupportedOSPlatform("windows")]
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("Hello, World!");
            var originalBuffer = ConsoleScreenBuffer.GetCurrent();
            var buffer = ConsoleScreenBuffer.Create(true);

            buffer.Resized += Buffer_Resized;
            buffer.SetAsActiveBuffer();
            Console.Write("Secondary Hello world!");
            Console.SetCursorPosition(10, 2);
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(0, 4);
            bool toggle = true;
            while (true)
            {
                var key = Console.ReadKey(true);
                switch(key.KeyChar)
                {
                    case 'q':
                        originalBuffer.SetAsActiveBuffer();
                        Console.WriteLine("All done");
                        return;
                    case 't':
                        if (toggle)
                            originalBuffer.SetAsActiveBuffer();
                        else
                            buffer.SetAsActiveBuffer();
                        toggle = !toggle;
                        break;
                    case 'c':
                        Console.SetCursorPosition(5, 1);
                        Console.CursorVisible = !Console.CursorVisible;
                        Console.Write($"Cursor Visible: {Console.CursorVisible}  ");
                        break;
                }
            }
        }

        private static void Buffer_Resized()
        {
            var pos = Console.GetCursorPosition();
            Console.SetCursorPosition(5, 2);
            var originalColor = Console.ForegroundColor;
            var originalBackground = Console.BackgroundColor;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Size: {Console.BufferWidth}x{Console.BufferHeight}    ");
            Console.SetCursorPosition(pos.Left, pos.Top);
            Console.ForegroundColor = originalColor;
            Console.BackgroundColor = originalBackground;
        }
    }
}
