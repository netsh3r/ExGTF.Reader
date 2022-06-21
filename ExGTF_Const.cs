using System.Text.RegularExpressions;

namespace ExGTF.Reader
{
    public class ExGTF_Const
    {
        public const string EMPTY_FILE_NAME = "@*";
        public const string START_ARRAY_BLOCK = "[##";
        public const string END_ARRAY_BLOCK = "##]";
        
        public class ExGTF_Regex
        {
            public static readonly Regex FileName = new Regex("@(.*)@");
            
            /// <summary> Вставка отдельной строки </summary>
            public static readonly Regex SingleLine = new Regex(@"(<#(\w*)#>)");
            
            /// <summary> Вставка значения, без переноса строки </summary>
            public static readonly Regex Single = new Regex(@"(<\$(\w*)\$>)");
            
            /// <summary> Вставка массива значений построчно </summary>
            public static readonly Regex ArrayLine = new Regex(@"(\[#(.*)#\])");
            
            /// <summary> Массив внутренних значений </summary>
            public static readonly Regex LocalArrayLine = new Regex(@"(\[<(.*)>\])");

            /// <summary> Аргумент массива внутренних значений </summary>
            public static readonly Regex LocalArrayLineArg = new Regex(@"((\w*):(!{(\d*)..(\d*)}!))(.*)({!(\w*)!})(.*)");

            public static readonly Regex ArrayMultiLine = new Regex(@"\[##<(\w*):%\$(\w*)\$%");

            /// <summary СВойство массива </summary>
            public static readonly Regex ArrayProps = new Regex(@"({%(\w*)%})");
        }
    }
}