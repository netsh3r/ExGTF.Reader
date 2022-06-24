namespace ExGTF.Reader.Props
{
    public class DictValue
    {
        public string Name { get; set; }
        public bool IsArray { get; set; } = false;
        public bool IsArrayObjects { get; set; } = false;
        public object Value { get; set; }
    }
}