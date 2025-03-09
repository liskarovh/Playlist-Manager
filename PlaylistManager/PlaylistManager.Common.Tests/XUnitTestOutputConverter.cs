using System.Text;
using Xunit.Abstractions;

namespace PlaylistManager.Common.Tests;

/// <summary>
/// Class for converting XUnit test output to text output.
/// </summary>
public class XUnitTestOutputConverter(ITestOutputHelper output) : TextWriter
{
    /// <summary>
    /// Gets the encoding used by the text writer.
    /// </summary>
    public override Encoding Encoding => Encoding.UTF8;

    /// <summary>
    /// Writes a line of message to the test output.
    /// </summary>
    /// <param name="message">The message to write.</param>
    public override void WriteLine(string? message) => output.WriteLine(message);

    /// <summary>
    /// Writes a formatted line of message to the test output.
    /// </summary>
    /// <param name="format">The format string.</param>
    /// <param name="args">The arguments for the format string.</param>
    public override void WriteLine(string format, params object?[] args) => output.WriteLine(format, args);
}
