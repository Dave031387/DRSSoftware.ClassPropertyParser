namespace DRSSoftware.ClassPropertyParser;

/// <summary>
/// The exception that is thrown when the class property parser encounters an error.
/// </summary>
[Serializable]
public class ClassPropertyParserException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClassPropertyParserException" /> class.
    /// </summary>
    public ClassPropertyParserException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassPropertyParserException" /> class with the
    /// given <paramref name="message" />.
    /// </summary>
    /// <param name="message">
    /// The error message that explains the reason for the exception.
    /// </param>
    public ClassPropertyParserException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassPropertyParserException" /> class with the
    /// given <paramref name="message" /> and <paramref name="inner" /> exception.
    /// </summary>
    /// <param name="message">
    /// The error message that explains the reason for the exception.
    /// </param>
    /// <param name="inner">
    /// The exception that is the cause of the current exception.
    /// </param>
    public ClassPropertyParserException(string message, Exception inner) : base(message, inner)
    {
    }
}