namespace OtherModels;

public class OtherModel3
{
    public bool OM3Property1
    {
        get;
        set;
    }

    public List<OtherModel1> OM3Property2
    {
        get;
        set;
    } = [];

    public OtherModel2 OM3Property3
    {
        get;
        set;
    } = new();
}