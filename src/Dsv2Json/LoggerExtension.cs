using CuiLib.Log;
using System;

namespace Dsv2Json
{
    internal static class LoggerExtension
    {
        internal static void WriteColored(this Logger logger, string? value, ConsoleColor color)
        {
            ArgumentNullException.ThrowIfNull(logger);

            ConsoleColor fgColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            logger.Write(value);
            Console.ForegroundColor = fgColor;
        }

        internal static void WriteColored(this Logger logger, object? value, ConsoleColor color)
        {
            ArgumentNullException.ThrowIfNull(logger);

            ConsoleColor fgColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            logger.Write(value);
            Console.ForegroundColor = fgColor;
        }

        internal static void WriteLineColored(this Logger logger, string? value, ConsoleColor color)
        {
            ArgumentNullException.ThrowIfNull(logger);

            ConsoleColor fgColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            logger.WriteLine(value);
            Console.ForegroundColor = fgColor;
        }

        internal static void WriteLineColored(this Logger logger, object? value, ConsoleColor color)
        {
            ArgumentNullException.ThrowIfNull(logger);

            ConsoleColor fgColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            logger.WriteLine(value);
            Console.ForegroundColor = fgColor;
        }
    }
}
