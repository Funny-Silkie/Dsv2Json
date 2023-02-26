using CuiLib.Log;

namespace Dsv2Json
{
	/// <summary>
	/// Store static resource.
	/// </summary>
	internal static class SR
	{
		/// <summary>
		/// Gets the logger redirecting to stdout.
		/// </summary>
		public static Logger StdOut { get; }

		/// <summary>
		/// Gets the logger redirecting to stderror.
		/// </summary>
		public static Logger StdErr { get; }

		static SR()
		{
			StdOut = new Logger()
			{
				ConsoleStdoutLogEnabled = true,
			};
			StdErr = new Logger()
			{
				ConsoleErrorEnabled = true,
			};
		}
	}
}
