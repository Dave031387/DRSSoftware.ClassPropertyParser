namespace DRSSoftware.ClassPropertyParser;

/// <summary>
/// The <see cref="ClassPropertyParser" /> class can be used for extracting
/// <see cref="PropertyInfo" /> details for all properties of a given class type.
/// </summary>
public class ClassPropertyParser : IClassPropertyParser
{
    /// <summary>
    /// The delimiter character used to separate a generic type name from its list of generic type
    /// parameters.
    /// </summary>
    private const char GenericTypeDelimiter = '`';

    /// <summary>
    /// The separator character used in lists of generic type parameter names.
    /// </summary>
    private const char ParameterSeparator = ',';

    /// <summary>
    /// A <see cref="List{T}" /> of <see cref="Type" /> objects corresponding to class types.
    /// </summary>
    private readonly List<Type> _classTypes;

    /// <summary>
    /// A <see cref="List{T}" /> of <see cref="PropertyInfo" /> objects corresponding to collection
    /// properties contained within a single class type.
    /// </summary>
    private readonly List<PropertyInfo> _collectionProperties;

    /// <summary>
    /// A <see cref="List{T}" /> of <see cref="PropertyInfo" /> objects corresponding to the complex
    /// properties contained within a single class type.
    /// </summary>
    private readonly List<PropertyInfo> _complexProperties;

    /// <summary>
    /// A <see cref="List{T}" /> of <see cref="PropertyInfo" /> objects corresponding to the simple
    /// properties contained within a single class type.
    /// </summary>
    private readonly List<PropertyInfo> _simpleProperties;

    /// <summary>
    /// Creates an empty instance of the <see cref="ClassPropertyParser" /> object.
    /// </summary>
    public ClassPropertyParser()
    {
        _classTypes = [];
        _collectionProperties = [];
        _complexProperties = [];
        _simpleProperties = [];
    }

    /// <summary>
    /// Creates an instance of the <see cref="ClassPropertyParser" /> class and retrieves all public
    /// class types defined in the same assembly as the given <paramref name="classType" />.
    /// </summary>
    /// <param name="classType">
    /// A class type for which we want to retrieve the property information.
    /// </param>
    public ClassPropertyParser(Type classType) : this() => GetPublicClassTypes(classType);

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}" /> collection of <see cref="Type" /> objects
    /// corresponding to the public class types contained within a single assembly.
    /// </summary>
    public IEnumerable<Type> ClassTypes => _classTypes;

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}" /> collection of <see cref="PropertyInfo" /> objects
    /// corresponding to all collection properties that are defined for a single class object.
    /// <para>
    /// A collection property is any property that returns a collection of objects. The type
    /// returned from the property must be assignable to the <see cref="IEnumerable" /> type.
    /// </para>
    /// </summary>
    public IEnumerable<PropertyInfo> CollectionProperties => _collectionProperties;

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}" /> collection of <see cref="PropertyInfo" /> objects
    /// corresponding to all complex properties that are defined for a single class object.
    /// <para>
    /// A complex property is any property that returns an object that is not a value type,
    /// <see langword="string" />, or collection.
    /// </para>
    /// </summary>
    public IEnumerable<PropertyInfo> ComplexProperties => _complexProperties;

    /// <summary>
    /// Gets the current class type that is being parsed.
    /// </summary>
    public Type? CurrentClassType
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}" /> collection of <see cref="PropertyInfo" /> objects
    /// corresponding to all simple properties that are defined for a single class object.
    /// <para>
    /// A simple property is any property that returns an object that is either a value type or a
    /// <see langword="string" />.
    /// </para>
    /// </summary>
    public IEnumerable<PropertyInfo> SimpleProperties => _simpleProperties;

    /// <summary>
    /// A static method that gets the fully qualified type name for the given
    /// <paramref name="type" />.
    /// </summary>
    /// <remarks>
    /// For generic types this method will be called recursively for each of the generic type
    /// parameters.
    /// </remarks>
    /// <param name="type">
    /// The type for which the fully qualified type name is to be retrieved.
    /// </param>
    /// <returns>
    /// The fully qualified type name, including generic type parameters if applicable.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the given <paramref name="type" /> is <see langword="null" />.
    /// </exception>
    /// <exception cref="ClassPropertyParserException">
    /// Thrown if any issues are encountered while trying to determine the full type name for the
    /// given <paramref name="type" />.
    /// </exception>
    public static string GetTypeName(Type type)
    {
        ArgumentNullException.ThrowIfNull(type, nameof(type));
        string? typeFullName;
        string[] genericArguments;

        if (type.IsGenericType)
        {
            try
            {
                typeFullName = type.GetGenericTypeDefinition().FullName;
                ArgumentNullException.ThrowIfNull(typeFullName, nameof(typeFullName));
                genericArguments = [.. type.GetGenericArguments().Select(GetTypeName)];
            }
            catch (Exception ex)
            {
                string message = $"Couldn't determine the generic type details for type \"{type}\"";
                throw new ClassPropertyParserException(message, ex);
            }

            int typeIndex = typeFullName.IndexOf(GenericTypeDelimiter);

            if (typeIndex < 1)
            {
                // Technically, this exception should never be thrown.
                string message = $"Couldn't locate the generic type parameters for generic type \"{typeFullName}\"";
                throw new ClassPropertyParserException(message);
            }

            string genericTypeName = typeFullName[..typeIndex];
            typeFullName = $"{genericTypeName}<{string.Join(ParameterSeparator, genericArguments)}>";
        }
        else
        {
            typeFullName = type.FullName;

            if (typeFullName is null)
            {
                string message = $"Couldn't determine the full type name for unsupported type \"{type}\"";
                throw new ClassPropertyParserException(message);
            }
        }

        return typeFullName;
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
    public void GetAllProperties(Type classType)
    {
        UpdateCurrentClassType(classType);

        PropertyInfo[] propertyInfos = classType.GetProperties();

        if (_collectionProperties.Count is 0)
        {
            _collectionProperties.AddRange(GetCollectionProperties(propertyInfos));
        }

        if (_complexProperties.Count is 0)
        {
            _complexProperties.AddRange(GetComplexProperties(propertyInfos));
        }

        if (_simpleProperties.Count is 0)
        {
            _simpleProperties.AddRange(GetSimpleProperties(propertyInfos));
        }
    }

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
    public IEnumerable<PropertyInfo> GetCollectionProperties(Type classType)
    {
        UpdateCurrentClassType(classType);

        if (_collectionProperties.Count is 0)
        {
            _collectionProperties.AddRange(GetCollectionProperties(classType.GetProperties()));
        }

        return CollectionProperties;
    }

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
    public IEnumerable<PropertyInfo> GetComplexProperties(Type classType)
    {
        UpdateCurrentClassType(classType);

        if (_complexProperties.Count is 0)
        {
            _complexProperties.AddRange(GetComplexProperties(classType.GetProperties()));
        }

        return ComplexProperties;
    }

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
    public IEnumerable<Type> GetPublicClassTypes(Type classType)
    {
        ArgumentNullException.ThrowIfNull(classType, nameof(classType));

        _classTypes.Clear();
        _collectionProperties.Clear();
        _complexProperties.Clear();
        _simpleProperties.Clear();
        CurrentClassType = null;

        try
        {
            _classTypes
                .AddRange(classType.Assembly.GetTypes()
                    .Where(static t => t.IsClass && t.IsVisible));
        }
        catch (Exception ex)
        {
            // Theoretically, the following exception should never occur, but we are including it
            // here just in case.
            string message = $"An unexpected exception was encountered while trying to get the " +
                $"public class types from the assembly containing type \"{classType}\".";
            throw new ClassPropertyParserException(message, ex);
        }

        return ClassTypes;
    }

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
    public IEnumerable<PropertyInfo> GetSimpleProperties(Type classType)
    {
        UpdateCurrentClassType(classType);

        if (_simpleProperties.Count is 0)
        {
            _simpleProperties.AddRange(GetSimpleProperties(classType.GetProperties()));
        }

        return SimpleProperties;
    }

    /// <summary>
    /// Retrieve all of the collection properties that are found in the given array of
    /// <paramref name="propertyInfos" />.
    /// </summary>
    /// <param name="propertyInfos">
    /// An array of <see cref="PropertyInfo" /> objects.
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}" /> collection of <see cref="PropertyInfo" /> objects
    /// representing collection properties that were found in the given array of
    /// <paramref name="propertyInfos" />.
    /// </returns>
    private static IEnumerable<PropertyInfo> GetCollectionProperties(PropertyInfo[] propertyInfos)
        => propertyInfos.Where(static p =>
            p.PropertyType.IsGenericType
            && typeof(IEnumerable).IsAssignableFrom(p.PropertyType));

    /// <summary>
    /// Retrieve all of the complex properties that are found in the given array of
    /// <paramref name="propertyInfos" />.
    /// </summary>
    /// <param name="propertyInfos">
    /// An array of <see cref="PropertyInfo" /> objects.
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}" /> collection of <see cref="PropertyInfo" /> objects
    /// representing complex properties that were found in the given array of
    /// <paramref name="propertyInfos" />.
    /// </returns>
    private static IEnumerable<PropertyInfo> GetComplexProperties(PropertyInfo[] propertyInfos)
        => propertyInfos.Where(static p =>
            !(p.PropertyType.IsValueType || p.PropertyType == typeof(string))
            && p.PropertyType.IsClass
            && !typeof(IEnumerable).IsAssignableFrom(p.PropertyType));

    /// <summary>
    /// Retrieve all of the simple properties that are found in the given array of
    /// <paramref name="propertyInfos" />.
    /// </summary>
    /// <param name="propertyInfos">
    /// An array of <see cref="PropertyInfo" /> objects.
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}" /> collection of <see cref="PropertyInfo" /> objects
    /// representing simple properties that were found in the given array of
    /// <paramref name="propertyInfos" />.
    /// </returns>
    private static IEnumerable<PropertyInfo> GetSimpleProperties(PropertyInfo[] propertyInfos)
        => propertyInfos.Where(static p =>
            p.PropertyType.IsValueType
            || p.PropertyType == typeof(string));

    /// <summary>
    /// Sets the current class type to the given <paramref name="classType" /> if the given type is
    /// different than the current type.
    /// </summary>
    /// <remarks>
    /// This method will also clear all simple, complex, and collection property information if the
    /// given <paramref name="classType" /> differs from the current class type.
    /// </remarks>
    /// <param name="classType">
    /// The class type that the current type will be set to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the given <paramref name="classType" /> is <see langword="null" />.
    /// </exception>
    private void UpdateCurrentClassType(Type classType)
    {
        ArgumentNullException.ThrowIfNull(classType, nameof(classType));

        if (!classType.Equals(CurrentClassType))
        {
            _collectionProperties.Clear();
            _complexProperties.Clear();
            _simpleProperties.Clear();
            CurrentClassType = classType;
        }
    }
}