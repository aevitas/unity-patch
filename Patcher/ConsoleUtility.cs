namespace Patcher
{
    using System;
    using System.Collections.Generic;

    public static class ConsoleUtility
    {
        public static T ShowSelectionMenu<T>(List<T> options, Func<T, string> optionLine)
        {
            int selected = 0;
            bool done = false;

            while (!done)
            {
                for (int i = 0; i < options.Count; i++)
                {
                    if (i == selected)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("> ");
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                    Console.WriteLine(optionLine(options[i]));
                    Console.ResetColor();
                }

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        selected = Math.Max(0, selected - 1);
                        break;
                    case ConsoleKey.DownArrow:
                        selected = Math.Min(options.Count - 1, selected + 1);
                        break;
                    case ConsoleKey.Spacebar:
                    case ConsoleKey.Enter:
                        done = true;
                        break;
                }

                if (!done)
                    Console.CursorTop -= options.Count;
            }
            
            return options[selected];
        }
    }
}
