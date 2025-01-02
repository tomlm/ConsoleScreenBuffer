using Microsoft.Win32.SafeHandles;
using System.Runtime.Versioning;
using Vanara.PInvoke;
using static Vanara.PInvoke.Kernel32;

namespace System
{
    /// <summary>
    /// ConsoleScreenBuffer - A class for managing console screen buffers on windows
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class ConsoleScreenBuffer : IDisposable
    {
        private SafeHFILE _handle;
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private int _bufferHeight;
        private int _bufferWidth;

        internal ConsoleScreenBuffer(SafeHFILE bufferHandle, bool AutoSize = false)
        {
            _handle = bufferHandle;

            if (AutoSize)
            {
                _ = MonitorSizeAsync(_cts.Token);
            }
        }

        public SafeHFILE Handle => _handle;

        /// <summary>
        /// This will fire an event when the buffer is resized.
        /// </summary>
        public event Action? Resized;

        /// <summary>
        ///     Sets the buffer as the currently active buffer, redirecting all output to the secondary buffer.
        /// </summary>
        /// <returns>Returns true on success, false on error.</returns>
        public bool SetAsActiveBuffer()
        {
            if (!SetConsoleActiveScreenBuffer(_handle))
            {
                return false;
            }

            // change stdout handle to point to the new buffer
            if (!SetStdHandle(StdHandleType.STD_OUTPUT_HANDLE, _handle))
            {
                return false;
            }

            // change stdout stream to point to the new buffer
            var handle = new SafeFileHandle(_handle.DangerousGetHandle(), false);
            var stream = new FileStream(handle, IO.FileAccess.Write);
            try
            {
                var writer = new StreamWriter(stream) { AutoFlush = true };
                Console.SetOut(writer);
                stream = null; // Successfully transferred ownership
            }
            finally
            {
                stream?.Dispose();
            }

            _bufferHeight = Console.BufferHeight;
            _bufferWidth = Console.BufferWidth;
            return true;
        }

        /// <summary>
        ///     Creates a new console screen buffer.
        /// </summary>
        /// <param name="autoSizeBuffer">Auto size buffer height to window height so that there are no scroll bars</param>
        /// <returns>Returns a new ConsoleScreenBuffer on success, throws on error.</returns>
        public static ConsoleScreenBuffer Create(bool autoSizeBuffer = true)
        {
            try
            {
                SafeHFILE buffer = CreateConsoleScreenBuffer(ACCESS_MASK.GENERIC_READ | ACCESS_MASK.GENERIC_WRITE,
                    FileShare.Read | FileShare.Write);
                return new ConsoleScreenBuffer(buffer, autoSizeBuffer);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException("Creation of the Console Buffer was unsuccessful.", ex);
            }
        }

        /// <summary>
        ///     Gets the current console screen buffer.
        /// </summary>
        /// <returns>Returns a new ConsoleBuffer on success, throws on error.</returns>
        public static ConsoleScreenBuffer GetCurrent()
        {
            HFILE buffer = GetStdHandle(StdHandleType.STD_OUTPUT_HANDLE);
            return new ConsoleScreenBuffer(new SafeHFILE(buffer.DangerousGetHandle(), false));
        }

        public void Dispose()
        {
            ((IDisposable)_cts).Dispose();
        }

        private async Task MonitorSizeAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(300, ct);

                    // if this window is the output buffer, resize it to match the window size
                    if (GetStdHandle(StdHandleType.STD_OUTPUT_HANDLE) == _handle)
                    {
                        if (Console.BufferHeight > Console.WindowHeight)
                        {
                            try
                            {
                                Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                // this can happen as we are resizing.
                                continue;
                            }
                            Resized?.Invoke();
                        }
                        else if (Console.BufferHeight != _bufferHeight || Console.BufferWidth != _bufferWidth)
                        {
                            Resized?.Invoke();
                        }
                    }
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }
    }
}
