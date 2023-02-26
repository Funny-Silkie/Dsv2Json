using System;

namespace Dsv2Json
{
    /// <summary>
    /// Represents the type information of DSV element.
    /// </summary>
    [Serializable]
    public class TypeAnnotation
    {
        /// <summary>
        /// Gets the instance which converts text to <see cref="int"/>.
        /// </summary>
        public static TypeAnnotation Int32 { get; } = new TypeAnnotation("int", x => x.Length == 0 ? 0 : int.Parse(x));

        /// <summary>
        /// Gets the instance which converts text to <see cref="int"/>?.
        /// </summary>
        public static TypeAnnotation Int32N { get; } = new TypeAnnotation("int?", x => x.Length == 0 ? null : int.Parse(x));

        /// <summary>
        /// Gets the instance which converts text to <see cref="long"/>.
        /// </summary>
        public static TypeAnnotation Int64 { get; } = new TypeAnnotation("long", x => x.Length == 0 ? 0 : long.Parse(x));

        /// <summary>
        /// Gets the instance which converts text to <see cref="long"/>?.
        /// </summary>
        public static TypeAnnotation Int64N { get; } = new TypeAnnotation("long?", x => x.Length == 0 ? null : int.Parse(x));

        /// <summary>
        /// Gets the instance which converts text to <see cref="double"/>.
        /// </summary>
        public static TypeAnnotation Float { get; } = new TypeAnnotation("float", x => x.Length == 0 ? 0 : double.Parse(x));

        /// <summary>
        /// Gets the instance which converts text to <see cref="double"/>?.
        /// </summary>
        public static TypeAnnotation FloatN { get; } = new TypeAnnotation("float", x => x.Length == 0 ? null : double.Parse(x));

        /// <summary>
        /// Gets the instance which converts text to <see cref="bool"/>.
        /// </summary>
        public static TypeAnnotation Bool { get; } = new TypeAnnotation("bool", x => bool.Parse(x));

        /// <summary>
        /// Gets the instance which converts text to <see cref="string"/>.
        /// </summary>
        public static TypeAnnotation String { get; } = new TypeAnnotation("string", x => x);

        private readonly Converter<string, object?> converter;

        /// <summary>
        /// Gets the supported type name.
        /// </summary>
        public string TypeName { get; }

        /// <summary>
        /// Initialize the new instance of <see cref="TypeAnnotation"/>
        /// </summary>
        /// <param name="typeName">supported type name</param>
        /// <param name="converter">conversion function</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="typeName"/> or <paramref name="converter"/> is null
        /// </exception>
        public TypeAnnotation(string typeName, Converter<string, object?> converter)
        {
            ArgumentNullException.ThrowIfNull(typeName);
            ArgumentNullException.ThrowIfNull(converter);

            TypeName = typeName;
            this.converter = converter;
        }

        /// <summary>
        /// Gets all supported instances of <see cref="TypeAnnotation"/>.
        /// </summary>
        /// <returns>All supported instances</returns>
        public static TypeAnnotation[] GetSupportedTypes()
        {
            return new[]
            {
                Int32,
                Int32N,
                Int64,
                Int64N,
                Float,
                FloatN,
                Bool,
                String,
            };
        }

        /// <summary>
        /// Gets instance of <see cref="TypeAnnotation"/> based on specified type.
        /// </summary>
        /// <param name="value">Type name</param>
        /// <returns>
        /// The instance of <see cref="TypeAnnotation"/> whose <see cref="TypeName"/> is equal to
        /// <paramref name="value"/>
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="value"/> is empty or not supported type
        /// </exception>
        public static TypeAnnotation Parse(string value)
        {
            ArgumentException.ThrowIfNullOrEmpty(value);

            foreach (TypeAnnotation current in GetSupportedTypes())
                if (string.Equals(value, current.TypeName, StringComparison.OrdinalIgnoreCase))
                    return current;
            throw new ArgumentException(nameof(value), $"Invalid type name '{value}'");
        }

        /// <summary>
        /// Convert text to value.
        /// </summary>
        /// <param name="value">Text value to convert</param>
        /// <returns>Converted value</returns>
        public object? Convert(string value) => converter.Invoke(value);
    }
}
