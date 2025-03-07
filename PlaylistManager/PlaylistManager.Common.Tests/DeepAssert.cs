using KellermanSoftware.CompareNetObjects;
using Xunit.Sdk;

namespace PlaylistManager.Common.Tests;

/// <summary>
/// Poskytuje metody pro hluboké porovnání objektů.
/// </summary>
public static class DeepAssert
{
    /// <summary>
    /// Porovná dva objekty a vyhodí výjimku, pokud nejsou stejné.
    /// </summary>
    /// <typeparam name="T">Typ porovnávaných objektů.</typeparam>
    /// <param name="expected">Očekávaný objekt.</param>
    /// <param name="actual">Skutečný objekt.</param>
    /// <param name="propertiesToIgnore">Vlastnosti, které mají být ignorovány při porovnání.</param>
    /// <exception cref="EqualException">Vyhozena, pokud objekty nejsou stejné.</exception>
    public static void Equal<T>(T? expected, T? actual, params string[] propertiesToIgnore)
    {
        CompareLogic compareLogic = new()
        {
            Config =
            {
                MembersToIgnore = propertiesToIgnore.ToList(),
                IgnoreCollectionOrder = true,
                IgnoreObjectTypes = true,
                CompareStaticProperties = false,
                CompareStaticFields = false
            }
        };

        ComparisonResult comparisonResult = compareLogic.Compare(expected!, actual!);

        if (!comparisonResult.AreEqual)
        {
            throw EqualException.ForMismatchedValues(expected, actual, comparisonResult.DifferencesString);
        }
    }

    /// <summary>
    /// Zkontroluje, zda kolekce obsahuje očekávaný objekt a vyhodí výjimku, pokud ne.
    /// </summary>
    /// <typeparam name="T">Typ porovnávaných objektů.</typeparam>
    /// <param name="expected">Očekávaný objekt.</param>
    /// <param name="collection">Kolekce, ve které se má hledat očekávaný objekt.</param>
    /// <param name="propertiesToIgnore">Vlastnosti, které mají být ignorovány při porovnání.</param>
    /// <exception cref="ArgumentNullException">Vyhozena, pokud je kolekce null.</exception>
    /// <exception cref="ContainsException">Vyhozena, pokud kolekce neobsahuje očekávaný objekt.</exception>
    public static void Contains<T>(T? expected, IEnumerable<T>? collection, params string[] propertiesToIgnore)
    {
        if (collection is null)
            throw new ArgumentNullException(nameof(collection));

        CompareLogic compareLogic = new()
        {
            Config =
            {
                MembersToIgnore = propertiesToIgnore.ToList(),
                IgnoreCollectionOrder = true,
                IgnoreObjectTypes = true,
                CompareStaticProperties = false,
                CompareStaticFields = false
            }
        };

        if (!collection.Any(item => compareLogic.Compare(expected!, item).AreEqual))
        {
            throw ContainsException.ForCollectionItemNotFound(expected!.ToString()!, nameof(collection));
        }
    }
}
