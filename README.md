# C# .NET Class Property Parser
## Introduction
The ***Class Property Parser*** is used to extract property info from class
types. This information could be used, for example, in a code generator class
that generates versions of simple model classes that are suitable for use in
view models in an MVVM application.

The ***ClassPropertyParser*** class implements the ***IClassPropertyParser***
interface. Three types of properties are handled by the parser:
- Simple properties
- Complex properties
- Collection properties

Each of these property types are described in more detail below.

### Simple Properties
A simple property is any property that returns a value type or a string. The
following are examples of simple properties:

```csharp
public int Age { get; set; }
public string Name { get; set; }
public bool IsModified { get; set; }
public DateTime DateModified { get; set; }
```

### Complex Properties
A complex property is any property that returns a reference type other than
string. The property must not return a collection. However, the type returned by
the property may itself contain collection properties. The following are
examples of complex properties:

```csharp
public Person Manager { get; set; }
public Address BusinessAddress { get; set; }
```

### Collection Properties
As the name implies, a collection property is any property that returns a
collection of objects. The collection class type that is returned must implement
the **IEnumerable** interface. The following are all valid collection
properties:

```csharp
public List<string> Departments { get; set; }
public Dictionary<int, string> { get; set; }
public ObservableCollection<Address> AddressList { get; set; }
```

## Constructors
The ***Class Property Parser*** has two constructors with the following
signatures:

```csharp
public ClassPropertyParser()
public ClassPropertyParser(Type classType)
```

The first constructor (the default constructor) simply initializes all the
internal lists to empty lists. Each ***ClassPropertyParser*** property (with the
exception of the ***CurrentClass*** property) corresponds to one of these lists.
(Refer to the **Properties** section in this document.)

The second constructor takes a **Type** argument. It also initializes the
internal lists to empty lists. After initializing the lists, the second
constructor makes a call to the ***GetPublicClassTypes*** method to retrieve all
public class types that reside in the same .NET assembly as the class type that
was supplied to the constructor. The ***ClassTypes*** property is then populated
with the resulting list of **Type** objects.

Both constructors also initialize the ***CurrentClassType*** property to a
**null** value.

Example:

```csharp
IClassPropertyParser parser = new(typeof(Models.Person));
````

> [!NOTE]
> It may make sense to organize your code such that all class types that you
> will be processing with the ***Class Property Parser*** reside in their own
> assembly separate from all other class types. This is optional, but it may
> make things easier since you can simply iterate over the entire list of class
> types without having to check whether or not a given class type is one that
> should be parsed.

## Properties
### ***ClassTypes***
The ***ClassTypes*** property returns an **IEnumerable** collection of **Type**
objects corresponding to the class types found in a single .NET assembly. This
collection is populated by either calling the ***ClassPropertyParser***
constructor that takes a **Type** argument, or by calling the
***GetPublicClassTypes*** method.

Example:

```csharp
IClassPropertyParser parser = new(typeof(Models.Person));
IEnumerable<Type> classTypes = parser.ClassTypes;
```

The result of the second line of code is a collection of all public class types
residing in the same .NET assembly as the **Person** class.

### ***CurrentClassType***
The ***CurrentClassType*** property returns a **Type** object corresponding to
the class type that was passed into the last call to any of the following
methods:
- ***GetAllProperties***
- ***GetCollectionProperties***
- ***GetComplexProperties***
- ***GetSimpleProperties***

The ***CurrentClassType*** property is set to **null** when a
***ClassPropertyParser*** object is first created or when the
***GetPublicClassTypes*** method is called.

Example:

```csharp
IClassPropertyParser parser = new(typeof(Models.Person));
parser.GetAllProperties(typeof(Models.Address));
Type currentType = parser.CurrentClassType;
```

The last line in the example returns a **Type** object corresponding to the
**Address** class type which is found in the same .NET assembly as the
**Person** class type.

### ***CollectionProperties***
The ***CollectionProperties*** property returns an **IEnumerable** collection of
**PropertyInfo** objects corresponding to the collection properties found in a
single class type. This collection is populated by calling either the
***GetAllProperties*** or the ***GetCollectionProperties*** methods.

Example:

```csharp
IClassPropertyParser parser = new(typeof(Models.Person));
parser.GetAllProperties(typeof(Models.Person));
IEnumerable<PropertyInfo> collectionProperties = parser.CollectionProperties;
```

The last line in the example returns a collection of **PropertyInfo** objects
corresponding to all the public collection properties that are defined for the
**Person** class type. An empty collection will be returned if no public
collection properties are defined for the specified class type.

### ***ComplexProperties***
The ***ComplexProperties*** property returns an **IEnumerable** collection of
**PropertyInfo** objects corresponding to the complex properties found in a
single class type. This collection is populated by calling either the
***GetAllProperties*** or the ***GetComplexProperties*** methods.

Example:

```csharp
IClassPropertyParser parser = new(typeof(Models.Person));
parser.GetAllProperties(typeof(Models.Person));
IEnumerable<PropertyInfo> complexProperties = parser.ComplexProperties;
```

The last line in the example returns a collection of **PropertyInfo** objects
corresponding to all the public complex properties that are defined for the
**Person** class type. An empty collection will be returned if no public complex
properties are defined for the specified class type.

### ***SimpleProperties***
The ***SimpleProperties*** property returns an **IEnumerable** collection of
**PropertyInfo** objects corresponding to the simple properties found in a
single class type. This collection is populated by calling either the
***GetAllProperties*** or the ***GetSimpleProperties*** methods.

Example:

```csharp
IClassPropertyParser parser = new(typeof(Models.Person));
parser.GetAllProperties(typeof(Models.Person));
IEnumerable<PropertyInfo> simpleProperties = parser.SimpleProperties;
```

The last line in the example returns a collection of **PropertyInfo** objects
corresponding to all the public simple properties that are defined for the
**Person** class type. An empty collection will be returned if no public simple
properties are defined for the specified class type.

## Methods
### ***GetPublicClassTypes***
The ***GetPublicClassTypes*** method uses reflection to retrieve **Type**
information for all class types residing in a single .NET assembly. The method
takes a single **Type** argument which supplies a single class type from the
assembly of interest. This method clears the ***ClassTypes*** property and then
populates it with **Type** objects corresponding to each public class type that
is found in the assembly. The method then returns the resulting collection to
the caller.

This method also sets the ***CurrentClassType*** property to **null** and sets
the ***CollectionProperties***, ***ComplexProperties***, and
***SimpleProperties*** properties to empty lists.

> [!NOTE]
> An **ArgumentNullException** is thrown if the class type passed into the
> ***GetPublicClassTypes*** method is **null**.

> [!NOTE]
> A ***ClassPropertyParserException*** is thrown if any issues are encountered
> while attempting to retrieve the class type information from the relevant .NET
> assembly.

Example:

```csharp
IClassPropertyParser parser = new();
IEnumerable<Type> classTypes = parser.GetPublicClassTypes(typeof(Models.Person));
```

The last line in the example above will return a list of all public class types
residing in the same .NET assembly as the **Person** class (including the
**Person** type itself, assuming it is public).

### ***GetAllProperties***
The ***GetAllProperties*** method takes a **Type** argument which specifies the
class type for which you want to extract all property info. The effect of
calling this method is identical to calling in sequence the
***GetCollectionProperties***, ***GetComplexProperties***, and
***GetSimpleProperties*** methods for the given class type, the only difference
being that ***GetAllProperties*** doesn't return anything back to the caller.
This method also sets the ***CurrentClassType*** property to the class type that
was passed into the method.

> [!NOTE]
> An **ArgumentNullException** is thrown if the class type passed into the
> ***GetAllProperties*** method is **null**.

Example:

```csharp
IClassPropertyParser parser = new();
parser.GetAllProperties(typeof(Models.AccountInfo));
```

The above example demonstrates that you don't first need to populate the
***ClassTypes*** property (either through the constructor that takes a **Type**
argument, or by calling ***GetPublicClassTypes***) before calling any of the
***GetAllProperties***, ***GetCollectionProperties***,
***GetComplexProperties***, or ***GetSimpleProperties*** methods.

### ***GetCollectionProperties***
The ***GetCollectionProperties*** method uses reflection to retrieve
**PropertyInfo** objects for all public collection properties defined within a
class type. The method takes a **Type** argument which gives the class type of
interest. The method clears the ***CollectionProperties*** property and then
populates it with **PropertyInfo** objects corresponding to each public
collection property that is defined in the given class type. The method then
returns the resulting collection to the caller and sets the
***CurrentClassType*** property to the type that was passed into the method.

> [!NOTE]
> An **ArgumentNullException** is thrown if the class type passed into the
> ***GetCollectionProperties*** method is **null**.

> [!NOTE]
> If the class type passed into the ***GetCollectionProperties*** method is
> different than the value of the ***CurrentClassType*** property then the
> ***ComplexProperties*** and ***SimpleProperties*** properties are both set to
> empty lists.

Example:

```csharp
IClassPropertyParser parser = new();
IEnumerable<PropertyInfo> collectionProperties = parser.GetCollectionProperties(typeof(Models.CompanyInfo));
```

### ***GetComplexProperties***
The ***GetComplexProperties*** method uses reflection to retrieve
**PropertyInfo** objects for all public complex properties defined within a
class type. The method takes a **Type** argument which gives the class type of
interest. The method clears the ***ComplexProperties*** property and then
populates it with **PropertyInfo** objects corresponding to each public complex
property that is defined in the given class type. The method then returns the
resulting collection to the caller and sets the ***CurrentClassType*** property
to the type that was passed into the method.

> [!NOTE]
> An **ArgumentNullException** is thrown if the class type passed into the
> ***GetComplexProperties*** method is **null**.

> [!NOTE]
> If the class type passed into the ***GetComplexProperties*** method is
> different than the value of the ***CurrentClassType*** property then the
> ***CollectionProperties*** and ***SimpleProperties*** properties are both set
> to empty lists.

Example:

```csharp
IClassPropertyParser parser = new();
IEnumerable<PropertyInfo> complexProperties = parser.GetComplexProperties(typeof(Models.CompanyInfo));
```

### ***GetSimpleProperties***
The ***GetSimpleProperties*** method uses reflection to retrieve
**PropertyInfo** objects for all public simple properties defined within a class
type. The method takes a **Type** argument which gives the class type of
interest. The method clears the ***SimpleProperties*** property and then
populates it with **PropertyInfo** objects corresponding to each public simple
property that is defined in the given class type. The method then returns the
resulting collection to the caller and sets the ***CurrentClassType*** property
to the type that was passed into the method.

> [!NOTE]
> An **ArgumentNullException** is thrown if the class type passed into the
> ***GetSimpleProperties*** method is **null**.

> [!NOTE]
> If the class type passed into the ***GetSimpleProperties*** method is
> different than the value of the ***CurrentClassType*** property then the
> ***CollectionProperties*** and ***ComplexProperties*** properties are both set
> to empty lists.

Example:

```csharp
IClassPropertyParser parser = new();
IEnumerable<PropertyInfo> collectionProperties = parser.GetCollectionProperties(typeof(Models.CompanyInfo));
```

### ***GetTypeName***
The ***GetTypeName*** method is a static method that takes a **Type** as an
argument and then returns the full type name to the caller as a **string**.
Generic types are formatted with the list of generic type parameters appearing
as a comma-separated list between angle brackets after the generic type name.

Example:

```csharp
string fullTypeName = ClassPropertyParser.GetTypeName(typeof(Models.Person));
```

> [!NOTE]
> An **ArgumentNullException** is thrown if the class type that is passed into
> the ***GetTypeName*** method is **null**.

> [!NOTE]
> A ***ClassPropertyParserException*** is thrown if any issues are encountered
> while trying to determine the full type name for the given class type.

## ***IClassPropertyParser*** Interface
The ***IClassPropertyParser*** interface defines all of the methods and
properties (with the exception of the ***GetTypeName*** static property)
described in the **Methods** and **Properties** sections above. The
***ClassPropertyParser*** class implements this interface.
