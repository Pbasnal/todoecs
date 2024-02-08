namespace StartUp;

public interface Tests
{
    void RunTests();
}

public class DocTableTests : Tests
{
    public List<Action> testsToRun;

    public DocTableTests()
    {
        testsToRun = new List<Action>{
            InitialiseDocTable
        };
    }

    public void RunTests()
    {
        foreach (var test in testsToRun)
        {
            test?.Invoke();   
        }
    }

    private void InitialiseDocTable()
    {
        DocTable docTable = new(100);
    }
}
