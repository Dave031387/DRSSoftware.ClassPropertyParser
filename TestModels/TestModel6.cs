namespace TestModels;

using System.Collections.Generic;

public class TestModel6
{
    public TestModel5 TM6Property1
    {
        get;
        set;
    } = new();

    public List<TestModel4> TM6Property2
    {
        get;
        set;
    } = [];
}