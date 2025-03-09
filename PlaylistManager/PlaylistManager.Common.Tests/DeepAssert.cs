using KellermanSoftware.CompareNetObjects;
using Xunit.Sdk;

namespace PlaylistManager.Common.Tests;

/// <summary>
/// Provides methods for deep comparison of objects.
/// </summary>
public static class DeepAssert
{
    /// <summary>
    /// Compares two objects and throws an exception if they are not equal.
    /// </summary>
    /// <typeparam name="T">The type of the objects being compared.</typeparam>
    /// <param name="expected">The expected object.</param>
    /// <param name="actual">The actual object.</param>
    /// <param name="propertiesToIgnore">Properties to ignore during comparison.</param>
    /// <exception cref="EqualException">Thrown if the objects are not equal.</exception>
    public static void Equal<T>(T? expected, T? actual, params string[] propertiesToIgnore)
    {
        CompareLogic compareLogic = new()
        {
            Config =
            {
                DateTimeKindToUseWhenUnspecified = DateTimeKind.Local,
                MembersToIgnore = propertiesToIgnore.ToList(),
                IgnoreCollectionOrder = true,
                IgnoreObjectTypes = true,
                CompareStaticProperties = false,
                CompareStaticFields = false,
            }
        };

        ComparisonResult comparisonResult = compareLogic.Compare(expected!, actual!);

        if (!comparisonResult.AreEqual)
        {
            throw EqualException.ForMismatchedValues(expected, actual, comparisonResult.DifferencesString);
        }
    }

    /// <summary>
    /// Checks if the collection contains the expected object and throws an exception if it does not.
    /// </summary>
    /// <typeparam name="T">The type of the objects being compared.</typeparam>
    /// <param name="expected">The expected object.</param>
    /// <param name="collection">The collection to search for the expected object.</param>
    /// <param name="propertiesToIgnore">Properties to ignore during comparison.</param>
    /// <exception cref="ArgumentNullException">Thrown if the collection is null.</exception>
    /// <exception cref="ContainsException">Thrown if the collection does not contain the expected object.</exception>
    public static void Contains<T>(T? expected, IEnumerable<T>? collection, params string[] propertiesToIgnore)
    {
        if (collection is null)
            throw new ArgumentNullException(nameof(collection));

        CompareLogic compareLogic = new()
        {
            Config =
            {
                DateTimeKindToUseWhenUnspecified = DateTimeKind.Local,
                MembersToIgnore = propertiesToIgnore.ToList(),
                IgnoreCollectionOrder = true,
                IgnoreObjectTypes = true,
                CompareStaticProperties = false,
                CompareStaticFields = false,
            }
        };

        if (!collection.Any(item => compareLogic.Compare(expected!, item).AreEqual))
        {
            throw ContainsException.ForCollectionItemNotFound(expected!.ToString()!, nameof(collection));
        }
    }
}
