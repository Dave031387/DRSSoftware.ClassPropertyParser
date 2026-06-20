namespace DRSSoftware.ClassPropertyParser;

[ExcludeFromCodeCoverage]
public class ClassPropertyParserTests
{
    private const int TestModelCount = 6;

    private readonly List<string>[] _collectionProperties =
        [
            [],
            [],
            [],
            [],
            [
                nameof(TestModel5.TM5Property2),
                nameof(TestModel5.TM5Property5)
            ],
            [
                nameof(TestModel6.TM6Property2)
            ]
        ];

    private readonly List<string>[] _complexProperties =
        [
            [],
            [],
            [],
            [],
            [
                nameof(TestModel5.TM5Property4),
                nameof(TestModel5.TM5Property6)
            ],
            [
                nameof(TestModel6.TM6Property1)
            ]
        ];

    private readonly List<string>[] _simpleProperties =
        [
            [
                nameof(TestModel1.TM1Property1),
                nameof(TestModel1.TM1Property2),
                nameof(TestModel1.TM1Property3),
                nameof(TestModel1.TM1Property4)
            ],
            [
                nameof(TestModel2.TM2Property1),
                nameof(TestModel2.TM2Property2),
                nameof(TestModel2.TM2Property3)
            ],
            [
                nameof(TestModel3.TM3Property1),
                nameof(TestModel3.TM3Property2)
            ],
            [
                nameof(TestModel4.TM4Property1),
                nameof(TestModel4.TM4Property2),
                nameof(TestModel4.TM4Property3)
            ],
            [
                nameof(TestModel5.TM5Property1),
                nameof(TestModel5.TM5Property3)
            ],
            []
        ];

    private enum PropertyType
    {
        All,
        Collection,
        Complex,
        Simple
    }

    [Fact]
    public void CallConstructorPassingInNullClassType_ShouldThrowException()
    {
        // Arrange
        string parameterName = "classType";

        // Act
        static void action() => _ = new ClassPropertyParser(null!);

        // Assert
        AssertArgumentNullException(parameterName, action);
    }

    [Fact]
    public void CallConstructorPassingInValidClassType_ShouldGetAllPublicClassTypesInSameAssembly()
    {
        // Arrange
        int testModelNumber = 1;
        Type testModelType = GetTestModelType(testModelNumber);

        // Act
        ClassPropertyParser parser = new(testModelType);

        // Assert
        VerifyExpectedClassTypeList(parser);
        VerifyEmptyPropertyLists(PropertyType.All, parser);
    }

    [Fact]
    public void CallDefaultConstructor_ShouldInitializeEmptyLists()
    {
        // Arrange/Act
        ClassPropertyParser parser = new();

        // Assert
        VerifyEmptyClassTypeList(parser);
        VerifyEmptyPropertyLists(PropertyType.All, parser);
    }

    [Fact]
    public void GetAllPropertiesForNullClassType_ShouldThrowException()
    {
        // Arrange
        string parameterName = "classType";
        ClassPropertyParser parser = new();

        // Act
        void action() => parser.GetAllProperties(null!);

        // Assert
        AssertArgumentNullException(parameterName, action);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    public void GetAllPropertiesForValidClassType_ShouldReturnAllProperties(int testModelNumber)
    {
        // Arrange
        Type testModelType = GetTestModelType(testModelNumber);
        ClassPropertyParser parser = new();

        // Act
        parser.GetAllProperties(testModelType);

        // Assert
        VerifyPropertyLists(testModelNumber, PropertyType.All, parser);
    }

    [Fact]
    public void GetAllPropertiesWhenClassTypeDiffersFromCurrentClassType_ShouldReturnAllPropertiesAndUpdateCurrentClassType()
    {
        // Arrange
        int testModelNumber = 1;
        Type testModelType = GetTestModelType(testModelNumber);
        ClassPropertyParser parser = new();
        parser.GetAllProperties(testModelType);
        testModelNumber = 5;
        testModelType = GetTestModelType(testModelNumber);

        // Act
        parser.GetAllProperties(testModelType);

        // Assert
        VerifyPropertyLists(testModelNumber, PropertyType.All, parser);
    }

    [Fact]
    public void GetAllPropertiesWhenCollectionPropertiesAreAlreadyThere_ShouldJustGetComplexAndSimpleProperties()
    {
        // Arrange
        int testModelNumber = 5;
        Type testModelType = GetTestModelType(testModelNumber);
        ClassPropertyParser parser = new();
        _ = parser.GetCollectionProperties(testModelType);

        // Act
        parser.GetAllProperties(testModelType);

        // Assert
        VerifyPropertyLists(testModelNumber, PropertyType.All, parser);
    }

    [Fact]
    public void GetAllPropertiesWhenComplexPropertiesAreAlreadyThere_ShouldJustGetCollectionAndSimpleProperties()
    {
        // Arrange
        int testModelNumber = 5;
        Type testModelType = GetTestModelType(testModelNumber);
        ClassPropertyParser parser = new();
        _ = parser.GetComplexProperties(testModelType);

        // Act
        parser.GetAllProperties(testModelType);

        // Assert
        VerifyPropertyLists(testModelNumber, PropertyType.All, parser);
    }

    [Fact]
    public void GetAllPropertiesWhenSimplePropertiesAreAlreadyThere_ShouldJustGetCollectionAndComplexProperties()
    {
        // Arrange
        int testModelNumber = 5;
        Type testModelType = GetTestModelType(testModelNumber);
        ClassPropertyParser parser = new();
        _ = parser.GetSimpleProperties(testModelType);

        // Act
        parser.GetAllProperties(testModelType);

        // Assert
        VerifyPropertyLists(testModelNumber, PropertyType.All, parser);
    }

    [Fact]
    public void GetCollectionPropertiesForNullClassType_ShouldThrowException()
    {
        // Arrange
        string parameterName = "classType";
        ClassPropertyParser parser = new();

        // Act
        void action() => parser.GetCollectionProperties(null!);

        // Assert
        AssertArgumentNullException(parameterName, action);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    public void GetCollectionPropertiesForValidClassType_ShouldReturnAllCollectionProperties(int testModelNumber)
    {
        // Arrange
        Type testModelType = GetTestModelType(testModelNumber);
        ClassPropertyParser parser = new();

        // Act
        parser.GetCollectionProperties(testModelType);

        // Assert
        VerifyPropertyLists(testModelNumber, PropertyType.Collection, parser);
    }

    [Fact]
    public void GetCollectionPropertiesMoreThanOnceForTheSameClassType_ShouldReturnCollectionPropertiesThatWerePreviouslyFound()
    {
        // Arrange
        int testModelNumber = 5;
        Type testModelType = GetTestModelType(testModelNumber);
        ClassPropertyParser parser = new();
        parser.GetAllProperties(testModelType);

        // Act
        parser.GetCollectionProperties(testModelType);

        // Assert
        VerifyPropertyLists(testModelNumber, PropertyType.All, parser);
    }

    [Fact]
    public void GetCollectionPropertiesWhenClassTypeDiffersFromCurrentClassType_ShouldReturnCollectionPropertiesAndUpdateCurrentClassType()
    {
        // Arrange
        int testModelNumber = 5;
        Type testModelType = GetTestModelType(testModelNumber);
        ClassPropertyParser parser = new();
        parser.GetAllProperties(testModelType);
        testModelNumber = 6;
        testModelType = GetTestModelType(testModelNumber);

        // Act
        parser.GetCollectionProperties(testModelType);

        // Assert
        VerifyPropertyLists(testModelNumber, PropertyType.Collection, parser, true);
    }

    [Fact]
    public void GetComplexPropertiesForNullClassType_ShouldThrowException()
    {
        // Arrange
        string parameterName = "classType";
        ClassPropertyParser parser = new();

        // Act
        void action() => parser.GetComplexProperties(null!);

        // Assert
        AssertArgumentNullException(parameterName, action);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    public void GetComplexPropertiesForValidClassType_ShouldReturnAllComplexProperties(int testModelNumber)
    {
        // Arrange
        Type testModelType = GetTestModelType(testModelNumber);
        ClassPropertyParser parser = new();

        // Act
        parser.GetComplexProperties(testModelType);

        // Assert
        VerifyPropertyLists(testModelNumber, PropertyType.Complex, parser);
    }

    [Fact]
    public void GetComplexPropertiesMoreThanOnceForTheSameClassType_ShouldReturnComplexPropertiesThatWerePreviouslyFound()
    {
        // Arrange
        int testModelNumber = 5;
        Type testModelType = GetTestModelType(testModelNumber);
        ClassPropertyParser parser = new();
        parser.GetAllProperties(testModelType);

        // Act
        parser.GetComplexProperties(testModelType);

        // Assert
        VerifyPropertyLists(testModelNumber, PropertyType.All, parser);
    }

    [Fact]
    public void GetComplexPropertiesWhenClassTypeDiffersFromCurrentClassType_ShouldReturnComplexPropertiesAndUpdateCurrentClassType()
    {
        // Arrange
        int testModelNumber = 5;
        Type testModelType = GetTestModelType(testModelNumber);
        ClassPropertyParser parser = new();
        parser.GetAllProperties(testModelType);
        testModelNumber = 6;
        testModelType = GetTestModelType(testModelNumber);

        // Act
        parser.GetComplexProperties(testModelType);

        // Assert
        VerifyPropertyLists(testModelNumber, PropertyType.Complex, parser, true);
    }

    [Fact]
    public void GetPublicClassTypesForDifferentAssembly_ShouldClearPreviousAssemblyDetailsAndReturnClassTypesForNextAssembly()
    {
        // Arrange
        ClassPropertyParser parser = new(typeof(OtherModel1));
        parser.GetAllProperties(typeof(OtherModel3));
        int testModelNumber = 3;
        Type testModelType = GetTestModelType(testModelNumber);

        // Act
        parser.GetPublicClassTypes(testModelType);

        // Assert
        VerifyExpectedClassTypeList(parser);
        VerifyEmptyPropertyLists(PropertyType.All, parser);
    }

    [Fact]
    public void GetPublicClassTypesWhenNullTypeIsPassedIn_ShouldThrowException()
    {
        // Arrange
        string parameterName = "classType";
        ClassPropertyParser parser = new();

        // Act
        void action() => parser.GetPublicClassTypes(null!);

        // Assert
        AssertArgumentNullException(parameterName, action);
    }

    [Fact]
    public void GetPublicClassTypesWhenValidTypeIsPassedIn_ShouldReturnAllPublicClassTypesInSameAssembly()
    {
        // Arrange
        int testModelNumber = 6;
        Type testModelType = GetTestModelType(testModelNumber);
        ClassPropertyParser parser = new();

        // Act
        parser.GetPublicClassTypes(testModelType);

        // Assert
        VerifyExpectedClassTypeList(parser);
        VerifyEmptyPropertyLists(PropertyType.All, parser);
    }

    [Fact]
    public void GetSimplePropertiesForNullClassType_ShouldThrowException()
    {
        // Arrange
        string parameterName = "classType";
        ClassPropertyParser parser = new();

        // Act
        void action() => parser.GetSimpleProperties(null!);

        // Assert
        AssertArgumentNullException(parameterName, action);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    public void GetSimplePropertiesForValidClassType_ShouldReturnAllSimpleProperties(int testModelNumber)
    {
        // Arrange
        Type testModelType = GetTestModelType(testModelNumber);
        ClassPropertyParser parser = new();

        // Act
        parser.GetSimpleProperties(testModelType);

        // Assert
        VerifyPropertyLists(testModelNumber, PropertyType.Simple, parser);
    }

    [Fact]
    public void GetSimplePropertiesMoreThanOnceForTheSameClassType_ShouldReturnSimplePropertiesThatWerePreviouslyFound()
    {
        // Arrange
        int testModelNumber = 5;
        Type testModelType = GetTestModelType(testModelNumber);
        ClassPropertyParser parser = new();
        parser.GetAllProperties(testModelType);

        // Act
        parser.GetSimpleProperties(testModelType);

        // Assert
        VerifyPropertyLists(testModelNumber, PropertyType.All, parser);
    }

    [Fact]
    public void GetSimplePropertiesWhenClassTypeDiffersFromCurrentClassType_ShouldReturnSimplePropertiesAndUpdateCurrentClassType()
    {
        // Arrange
        int testModelNumber = 6;
        Type testModelType = GetTestModelType(testModelNumber);
        ClassPropertyParser parser = new();
        parser.GetAllProperties(testModelType);
        testModelNumber = 5;
        testModelType = GetTestModelType(testModelNumber);

        // Act
        parser.GetSimpleProperties(testModelType);

        // Assert
        VerifyPropertyLists(testModelNumber, PropertyType.Simple, parser, true);
    }

    [Fact]
    public void GetTypeNameForNullType_ShouldThrowException()
    {
        // Arrange
        string parameterName = "type";

        // Act
        static void action() => _ = ClassPropertyParser.GetTypeName(null!);

        // Assert
        AssertArgumentNullException(parameterName, action);
    }

    [Theory]
    [InlineData(typeof(bool), "System.Boolean")]
    [InlineData(typeof(string), "System.String")]
    [InlineData(typeof(List<string>), "System.Collections.Generic.List<System.String>")]
    [InlineData(typeof(int[]), "System.Int32[]")]
    [InlineData(typeof(Dictionary<int, DateTime>), "System.Collections.Generic.Dictionary<System.Int32,System.DateTime>")]
    public void GetTypeNameForValidType_ShouldReturnFullTypeName(Type type, string expected)
    {
        // Arrange/Act
        string actual = ClassPropertyParser.GetTypeName(type);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    private static void AssertArgumentNullException(string parameterName, Action action)
    {
        string expected = $"Value cannot be null. (Parameter '{parameterName}')";

        action
            .Should()
            .ThrowExactly<ArgumentNullException>()
            .WithMessage(expected);
    }

    private static List<string> GetPropertyNames(IEnumerable<PropertyInfo> propertyInfo)
            => [.. propertyInfo.Select(static t => t.Name)];

    private static Type GetTestModelType(int testModelNumber)
    {
        return testModelNumber switch
        {
            1 => typeof(TestModel1),
            2 => typeof(TestModel2),
            3 => typeof(TestModel3),
            4 => typeof(TestModel4),
            5 => typeof(TestModel5),
            6 => typeof(TestModel6),
            _ => throw new ArgumentException($"Test model number must be between 1 and {TestModelCount}.", nameof(testModelNumber))
        };
    }

    private static void VerifyEmptyClassTypeList(ClassPropertyParser parser)
    {
        parser.ClassTypes
            .Should()
            .NotBeNull()
            .And
            .BeEmpty();
    }

    private static void VerifyEmptyPropertyLists(PropertyType propertyType, ClassPropertyParser parser)
    {
        if (propertyType is PropertyType.All or PropertyType.Collection)
        {
            parser.CollectionProperties
                .Should()
                .NotBeNull()
                .And
                .BeEmpty();
        }

        if (propertyType is PropertyType.All or PropertyType.Complex)
        {
            parser.ComplexProperties
                .Should()
                .NotBeNull()
                .And
                .BeEmpty();
        }

        if (propertyType is PropertyType.All or PropertyType.Simple)
        {
            parser.SimpleProperties
                .Should()
                .NotBeNull()
                .And
                .BeEmpty();
        }

        if (propertyType is PropertyType.All)
        {
            parser.CurrentClassType
                .Should()
                .BeNull();
        }
    }

    private static void VerifyExpectedClassTypeList(ClassPropertyParser parser)
    {
        List<Type> expectedTypes =
        [
            typeof(TestModel1),
            typeof(TestModel2),
            typeof(TestModel3),
            typeof(TestModel4),
            typeof(TestModel5),
            typeof(TestModel6)
        ];

        parser.ClassTypes
            .Should()
            .NotBeNull()
            .And
            .HaveCount(expectedTypes.Count)
            .And
            .Contain(expectedTypes);
    }

    private static void VerifyExpectedPropertyList(PropertyType propertyType, List<string> propertyNames, ClassPropertyParser parser)
    {
        switch (propertyType)
        {
            case PropertyType.Collection:
                parser.CollectionProperties
                    .Should()
                    .HaveCount(propertyNames.Count);
                GetPropertyNames(parser.CollectionProperties)
                    .Should()
                    .Contain(propertyNames);
                break;

            case PropertyType.Complex:
                parser.ComplexProperties
                    .Should()
                    .HaveCount(propertyNames.Count);
                GetPropertyNames(parser.ComplexProperties)
                    .Should()
                    .Contain(propertyNames);
                break;

            case PropertyType.Simple:
                parser.SimpleProperties
                    .Should()
                    .HaveCount(propertyNames.Count);
                GetPropertyNames(parser.SimpleProperties)
                    .Should()
                    .Contain(propertyNames);
                break;

            case PropertyType.All:
            default:
                throw new ArgumentException("Can't verify expected property list for PropertyType.All", nameof(propertyType));
        }
    }

    private static void VerifyOtherPropertyListsAreEmpty(PropertyType propertyType, ClassPropertyParser parser)
    {
        if (propertyType is PropertyType.Collection or PropertyType.Complex)
        {
            VerifyEmptyPropertyLists(PropertyType.Simple, parser);
        }

        if (propertyType is PropertyType.Collection or PropertyType.Simple)
        {
            VerifyEmptyPropertyLists(PropertyType.Complex, parser);
        }

        if (propertyType is PropertyType.Complex or PropertyType.Simple)
        {
            VerifyEmptyPropertyLists(PropertyType.Collection, parser);
        }
    }

    private static void VerifyPropertyList(PropertyType propertyType, List<string> propertyNames, ClassPropertyParser parser, bool otherPropertyListsShouldBeEmpty)
    {
        if (propertyNames.Count is 0)
        {
            VerifyEmptyPropertyLists(propertyType, parser);
        }
        else
        {
            VerifyExpectedPropertyList(propertyType, propertyNames, parser);
        }

        if (otherPropertyListsShouldBeEmpty)
        {
            VerifyOtherPropertyListsAreEmpty(propertyType, parser);
        }
    }

    private void VerifyPropertyLists(int testModelNumber, PropertyType propertyType, ClassPropertyParser parser, bool otherPropertyListsShouldBeEmpty = false)
    {
        testModelNumber
            .Should()
            .BeInRange(1, TestModelCount);

        if (propertyType is PropertyType.All)
        {
            otherPropertyListsShouldBeEmpty = false;
        }

        int modelIndex = testModelNumber - 1;

        if (propertyType is PropertyType.All or PropertyType.Collection)
        {
            List<string> propertyNames = _collectionProperties[modelIndex];
            VerifyPropertyList(PropertyType.Collection, propertyNames, parser, otherPropertyListsShouldBeEmpty);
        }

        if (propertyType is PropertyType.All or PropertyType.Complex)
        {
            List<string> propertyNames = _complexProperties[modelIndex];
            VerifyPropertyList(PropertyType.Complex, propertyNames, parser, otherPropertyListsShouldBeEmpty);
        }

        if (propertyType is PropertyType.All or PropertyType.Simple)
        {
            List<string> propertyNames = _simpleProperties[modelIndex];
            VerifyPropertyList(PropertyType.Simple, propertyNames, parser, otherPropertyListsShouldBeEmpty);
        }

        parser.CurrentClassType
            .Should()
            .BeSameAs(GetTestModelType(testModelNumber));
    }
}