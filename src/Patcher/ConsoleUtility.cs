using System;
using System.Collections.Generic;

namespace Patcher
{
    /// <summary>
    /// Utility methods for interacting with the console.
    /// </summary>
    public static class ConsoleUtility
    {
        /// <summary>
        /// Displays a selection menu in the console to the user which can be navigated using the arrow keys and enter
        /// to select the desired option from the list.
        /// </summary>
        /// <param name="options">The list of options from which the user can choose</param>
        /// <param name="optionLine">
        /// A function taking one of the options an returning a string representation of how it should be displayed to the user
        /// </param>
        /// <typeparam name="T">The type of a single option</typeparam>
        /// <returns>The selected option from the list</returns>
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
