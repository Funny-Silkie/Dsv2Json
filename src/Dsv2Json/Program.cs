using CuiLib;
using System;

namespace Dsv2Json
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var command = new MainCommand();
            if (args.Length == 0)
            {
                command.WriteHelp(SR.StdOut);
                return;
            }

#if DEBUG
            try
            {
                command.Invoke(args);
            }
            catch (Exception e)
            {
                SR.StdErr.WriteLineColored(e, ConsoleColor.Red);
            }
#else
			try
			{
				command.Invoke(args);
			}
			catch (ArgumentAnalysisException e)
			{
				SR.StdErr.WriteLineColored(e.Message, ConsoleColor.Red);
			}
			catch (Exception e)
			{
				SR.StdErr.WriteLineColored(e, ConsoleColor.Red);
			}
#endif
        }
    }
}
