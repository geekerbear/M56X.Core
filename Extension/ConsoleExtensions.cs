namespace M56X.Core.Extension
{
    public static class ConsoleEx
    {
        /// <summary>
        /// 打印彩色字符
        /// </summary>
        /// <param name="value"></param>
        /// <param name="color"></param>
        public static void WriteColor(string value, ConsoleColor color)
        {
            ConsoleColor currentForeColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(value);
            Console.ForegroundColor = currentForeColor;
        }

        /// <summary>
        /// 打印彩色字符
        /// </summary>
        /// <param name="value"></param>
        /// <param name="color"></param>
        public static void WriteColorLine(string value, ConsoleColor color)
        {
            ConsoleColor currentForeColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(value);
            Console.ForegroundColor = currentForeColor;
        }

        /// <summary>
        /// 打印彩色文本
        /// </summary>
        /// <param name="value"></param>
        /// <param name="color"></param>
        public static void PrintColor(this string value, ConsoleColor color)
        {
            WriteColor(value, color);
        }

        /// <summary>
        /// 打印彩色文本
        /// </summary>
        /// <param name="value"></param>
        /// <param name="color"></param>
        public static void PrintColorLine(this string value, ConsoleColor color)
        {
            WriteColorLine(value, color);
        }

        /// <summary>
        /// 打印信息
        /// </summary>
        /// <param name="value"></param>
        /// <param name="color"></param>
        public static void PrintInfoLine(this string value, ConsoleColor color = ConsoleColor.White)
        {
            WriteColorLine(value, color);
        }

        /// <summary>
        /// 打印成功信息
        /// </summary>
        /// <param name="value"></param>
        /// <param name="color"></param>
        public static void PrintSuccessLine(this string value, ConsoleColor color = ConsoleColor.Green)
        {
            WriteColorLine(value, color);
        }

        /// <summary>
        /// 打印错误信息
        /// </summary>
        /// <param name="value"></param>
        /// <param name="color"></param>
        public static void PrintErrorLine(this string value, ConsoleColor color = ConsoleColor.Red)
        {
            WriteColorLine(value, color);
        }

        /// <summary>
        /// 打印警告信息
        /// </summary>
        /// <param name="value"></param>
        /// <param name="color"></param>
        public static void PrintWarningLine(this string value, ConsoleColor color = ConsoleColor.Yellow)
        {
            WriteColorLine(value, color);
        }
    }
}
