namespace ExGTF.Example
{
    using ExGTF.Reader;

    internal class Program
    {
        static void Main(string[] args)
        {
            var rd = new ExGTFReader("C:/Users/dmitr/Documents/Tests/EGTF.Reader/test.exgtf", 
                new Dictionary<string, object>
                {
                    { "userName", "dima" }, 
                    { "password", "123" }, 
                    { "cw", "Console.WriteLine(\"123\")" },
                    { "users", new[]{"1", "2"} }
                });
            rd.Create("C:/Users/dmitr/Documents/Tests");
        }
    }
}