using Shared.Helpers;

namespace Shape;

public class Program
{
    static void Main(string[] args)
    {
        HostHelper.BuildAndRun<ShapeRunner>().Run();
    }
}
