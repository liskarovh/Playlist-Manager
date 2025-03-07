using System.Text;
using Xunit.Abstractions;

namespace PlaylistManager.Common.Tests;

/// <summary>
/// Třída pro převod výstupu testů XUnit na textový výstup.
/// </summary>
public class XUnitTestOutputConverter(ITestOutputHelper output) : TextWriter
{
    /// <summary>
    /// Získává kódování používané textovým zapisovačem.
    /// </summary>
    public override Encoding Encoding => Encoding.UTF8;

    /// <summary>
    /// Zapíše řádek zprávy do výstupu testu.
    /// </summary>
    /// <param name="message">Zpráva k zápisu.</param>
    public override void WriteLine(string? message) => output.WriteLine(message);

    /// <summary>
    /// Zapíše formátovaný řádek zprávy do výstupu testu.
    /// </summary>
    /// <param name="format">Formátovací řetězec.</param>
    /// <param name="args">Argumenty pro formátovací řetězec.</param>
    public override void WriteLine(string format, params object?[] args) => output.WriteLine(format, args);
}
