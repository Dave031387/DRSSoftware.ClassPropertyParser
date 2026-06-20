namespace DRSSoftware.ClassPropertyParser;

/// <summary>
/// The <see cref="IClassPropertyParser" /> interface defines the methods and properties that must
/// be implemented by a class property parser object.
/// </summary>
public interface IClassPropertyParser
{
    /// <summary>
    /// Gets an <see cref="IEnumerable{T}" /> collection of <see cref="Type" /> objects
    /// corresponding to the class types contained within a single assembly.
    /// </summary>
    public IEnumerable<Type> ClassTypes
    {
        get;
    }

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}" /> collection of <see cref="PropertyInfo" /> objects
    /// corresponding to all collection properties that are defined for a single class object.
    /// <para>
    /// A collection property is any property that returns a collection of objects. The type
    /// returned from the property must be assignable to the <see cref="IEnumerable" /> type.
    /// </para>
    /// </summary>
    public IEnumerable<PropertyInfo> CollectionProperties
    {
        get;
    }

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}" /> collection of <see cref="PropertyInfo" /> objects
    /// corresponding to all complex properties that are defined for a single class object.
    /// <para>
    /// A complex property is any property that returns an object that is not a value type,
    /// <see langword="string" />, or collection.
    /// </para>
    /// </summary>
    public IEnumerable<PropertyInfo> ComplexProperties
    {
        get;
    }

    /// <summary>
    /// Gets the current class type that is being parsed.
    /// </summary>
    public Type? CurrentClassType
    {
        get;
    }

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}" /> collection of <see cref="PropertyInfo" /> objects
    /// corresponding to all simple properties that are defined for a single class object.
    /// <para>
    /// A simple property is any property that returns an object that is either a value type or a
    /// <see langword="string" />.
    /// </para>
    /// </summary>
    public IEnumerable<PropertyInfo> SimpleProperties
    {
        get;
    }

    /// <summary>
    /// Retrieves the <see cref="PropertyInfo" /> details for all simple, complex, and collection
    /// properties that are defined for the given <paramref name="classType" />.
    /// </summary>
    /// <remarks>
    /// Only the <see cref="PropertyInfo" /> for public properties is returned.
    /// </remarks>
    /// <param name="classType">
    /// The class type for which we want to retrieve the property information.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the given <paramref name="classType" /> is <see langword="null" />.
    /// </exception>
    public void GetAllProperties(Type classType);

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}" /> collection of <see cref="PropertyInfo" /> objects
    /// corresponding to all collection properties that are defined for the given
    /// <paramref name="classType" />.
    /// <para>
    /// A collection property is any property that returns a collection of objects. The type
    /// returned from the property must be assignable to the <see cref="IEnumerable" /> type.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Only the <see cref="PropertyInfo" /> for public properties is returned.
    /// </remarks>
    /// <param name="classType">
    /// The class type for which we want to retrieve the collection property information.
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}" /> collection of <see cref="PropertyInfo" /> objects
    /// corresponding to all collection properties found for the given
    /// <paramref name="classType" />.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the given <paramref name="classType" /> is <see langword="null" />.
    /// </exception>
    public IEnumerable<PropertyInfo> GetCollectionProperties(Type classType);

    /// <summary>
    /// Gets the <see cref="IEnumerable{T}" /> collection of <see cref="PropertyInfo" /> objects
    /// corresponding to all complex properties that are defined for the given
    /// <paramref name="classType" />.
    /// <para>
    /// A complex property is any property that returns an object that is not a value type,
    /// <see langword="string" />, or collection.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Only the <see cref="PropertyInfo" /> for public properties is returned.
    /// </remarks>
    /// <param name="classType">
    /// The class type for which we want to retrieve the complex property information.
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}" /> collection of <see cref="PropertyInfo" /> objects
    /// corresponding to all complex properties found for the given <paramref name="classType" />.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the given <paramref name="classType" /> is <see langword="null" />.
    /// </exception>
    public IEnumerable<PropertyInfo> GetComplexProperties(Type classType);

    /// <summary>
    /// Retrieves the <see cref="Type" /> information for all visible (public) class types found in
    /// the same .NET assembly as the given <paramref name="classType" />.
    /// </summary>
    /// <param name="classType">
    /// One of the class types from a .NET assembly containing one or more class types.
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}" /> collection of <see cref="Type" /> objects corresponding to
    /// all public class types that are contained in the same .NET assembly as the given
    /// <paramref name="classType" />.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the given <paramref name="classType" /> is <see langword="null" />.
    /// </exception>
    /// <exception cref="ClassPropertyParserException">
    /// Thrown if any issues are encountered when attempting to get the list of class types within a
    /// .NET assembly.
    /// </exception>
    public IEnumerable<Type> GetPublicClassTypes(Type classType);

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}" /> collection of <see cref="PropertyInfo" /> objects
    /// corresponding to all simple properties that are defined for the given
    /// <paramref name="classType" />.
    /// <para>
    /// A simple property is any property that returns an object that is either a value type or a
    /// <see langword="string" />.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Only the <see cref="PropertyInfo" /> for public properties is returned.
    /// </remarks>
    /// <param name="classType">
    /// The class type for which we want to retrieve the simple property information.
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}" /> collection of <see cref="PropertyInfo" /> objects
    /// corresponding to all simple properties found for the given <paramref name="classType" />.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the given <paramref name="classType" /> is <see langword="null" />.
    /// </exception>
    public IEnumerable<PropertyInfo> GetSimpleProperties(Type classType);
}