using CuiLib.Commands;
using CuiLib.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Dsv2Json
{
    internal class MainCommand : Command
    {
        [StringSyntax(StringSyntaxAttribute.Regex)]
        private const string CsvExpression = "(((?<=(^\")|(,\"))[^\"]*?(?=(\",)|(\"$)))|((?<=(^')|(,'))[^']*?(?=(',)|('$)))|((?<=^|,)[^,'\"]*?['\"]?(?=,|$)))";

        private static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
        };

        #region Options

        private readonly FlagOption optionHelp;
        private readonly SingleValueOption<TextReader?> optionIn;
        private readonly SingleValueOption<TextWriter?> optionOut;
        private readonly SingleValueOption<string> optionDelimiter;
        private readonly FlagOption optionWithTypeAnotation;

        #endregion Options

        public MainCommand() : base("dsv2json")
        {
            Description = "Convert DSV format file to JSON file";

            optionHelp = new FlagOption('h', "help")
            {
                Description = "Displays help",
            };
            optionIn = new SingleValueOption<TextReader?>('i', "in")
            {
                Description = "Input file. If null, stdin is used",
                DefaultValue = null,
            };
            optionOut = new SingleValueOption<TextWriter?>('o', "out")
            {
                Description = "Output file. If null, stdout is used",
                DefaultValue = null,
            };
            optionDelimiter = new SingleValueOption<string>('d', "delimiter")
            {
                Description = "Row delimiter of input file",
                DefaultValue = ",",
                Checker = (ValueChecker.NotEmpty() & ValueChecker.FromDelegate<string>(x => x!.Length == 1 ? ValueCheckState.Success : ValueCheckState.AsError("Delimiter must be a single character"))!)!,
            };
            optionWithTypeAnotation = new FlagOption('t', "type-annotation")
            {
                Description = "Use 2nd row as type information",
            };

            Options.Add(optionHelp);
            Options.Add(optionIn);
            Options.Add(optionOut);
            Options.Add(optionDelimiter);
            Options.Add(optionWithTypeAnotation);
        }

        private static string[] ToStringArray(MatchCollection source)
        {
            var result = new string[source.Count];
            for (int i = 0; i < result.Length; i++) result[i] = source[i].Value;

            return result;
        }

        /// <inheritdoc/>
        protected override void OnExecution()
        {
            if (optionHelp.Value)
            {
                WriteHelp(SR.StdOut);
                return;
            }

            using TextReader inReader = optionIn.Value ?? Console.In;
            using TextWriter outWriter = optionOut.Value ?? Console.Out;
            string delimiter = optionDelimiter.Value;
            bool typeAnnotation = optionWithTypeAnotation.Value;

            string expression = CsvExpression;
            if (delimiter is not ",") expression = expression.Replace(",", delimiter);
            var regex = new Regex(expression);

            string? line;
            line = inReader.ReadLine();
            if (line is null)
            {
                SR.StdErr.WriteLineColored("This file is empty", ConsoleColor.Red);
                return;
            }
            Span<string> header = ToStringArray(regex.Matches(line));
            if (header.Length == 0 || (header.Length == 1 && header[0].Length == 0))
            {
                SR.StdErr.WriteLineColored("Header row is empty", ConsoleColor.Red);
                return;
            }
            if (header[^1].Length == 0) header = header[..^1];
            if (header.ToArray().Distinct(StringComparer.Ordinal).Count() != header.Length)
            {
                SR.StdErr.WriteLineColored("Header has duplicated elements", ConsoleColor.Red);
                return;
            }

            if (typeAnnotation)
            {
                line = inReader.ReadLine();
                if (line is null)
                {
                    SR.StdErr.WriteLineColored("Type annotation is not included", ConsoleColor.Red);
                    return;
                }
                Span<string> _annotations = ToStringArray(regex.Matches(line));
                if (_annotations[^1].Length == 0) _annotations = _annotations[..^1];

                TypeAnnotation[] annotation = Array.ConvertAll(_annotations.ToArray(), TypeAnnotation.Parse);
                if (annotation.Length == 0)
                {
                    SR.StdErr.WriteLineColored("Type annotation row is empty", ConsoleColor.Red);
                    return;
                }
                if (annotation.Length != header.Length)
                {
                    SR.StdErr.WriteLineColored("The amount of header elements is not equal to the amount of type annotations.", ConsoleColor.Red);
                    return;
                }

                outWriter.WriteLine('[');

                int index = 0;
                while ((line = inReader.ReadLine()) is not null)
                {
                    if (index++ > 0) outWriter.WriteLine(',');
                    string[] values = ToStringArray(regex.Matches(line));
                    if (values.Length == 0) continue;

                    var dictionary = new Dictionary<string, object?>(header.Length, StringComparer.Ordinal);
                    if (header.Length >= values.Length)
                    {
                        for (int i = 0; i < values.Length; i++) dictionary.Add(header[i], annotation[i].Convert(values[i]));
                    }
                    else
                        for (int i = 0; i < header.Length; i++)
                            dictionary.Add(header[i], annotation[i].Convert(values[i]));
                    string json = JsonSerializer.Serialize(dictionary, jsonSerializerOptions);
                    outWriter.Write(json);
                }

                outWriter.WriteLine();
                outWriter.WriteLine(']');
            }
            else
            {
                outWriter.WriteLine('[');

                int index = 0;
                while ((line = inReader.ReadLine()) is not null)
                {
                    if (index++ > 0) outWriter.WriteLine(',');
                    string[] values = ToStringArray(regex.Matches(line));
                    if (values.Length == 0) continue;

                    var dictionary = new Dictionary<string, string>(header.Length, StringComparer.Ordinal);
                    if (header.Length >= values.Length)
                    {
                        for (int i = 0; i < values.Length; i++) dictionary.Add(header[i], values[i]);
                    }
                    else
                        for (int i = 0; i < header.Length; i++)
                            dictionary.Add(header[i], values[i]);
                    string json = JsonSerializer.Serialize(dictionary, jsonSerializerOptions);
                    outWriter.Write(json);
                }

                outWriter.WriteLine();
                outWriter.WriteLine(']');
            }
        }
    }
}
