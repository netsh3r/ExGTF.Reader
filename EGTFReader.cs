using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ExGTF.Reader
{
    public class ExGTFReader
    {
        private readonly string url;
        private readonly Dictionary<string, object> dictValue;
        public ExGTFReader(string url, Dictionary<string, object> dictValue)
        {
            this.url = url;
            this.dictValue = dictValue;
        }

        public void Create(string createUrl, string fileName = null)
        {
            var fi = new FileInfo(url);
            var newFileName = fi.Name.Substring(0, fi.Name.Length - 6);
            var sb = new StringBuilder();

            using (var sr = new StreamReader(url))
            {
                var header = sr.ReadLine();
                if (!header.Contains("@*"))
                {
                    var rg = new Regex("@(.*)@");
                    newFileName = rg.Match(header).Groups[1].Value;
                }
                var line = string.Empty;
                var rgLine = new Regex(@"(<#(\w*)#>)");
                var rg2 = new Regex(@"(<\$(\w*)\$>)");
                var argLine = new Regex(@"(\[#(.*)#\])");
                var argSpLine = new Regex(@"(\[<(.*)>\])");
                var argSpLineAr = new Regex(@"((\w*):(!{(\d*)..(\d*)}!))(.*)({!(\w*)!})(.*)");
                while ((line = sr.ReadLine()) != null)
                {
                    if (rgLine.IsMatch(line))
                    {
                        var sp = rgLine.Split(line);
                        for (int i = 0; i < sp.Length; i++)
                        {
                            if (rgLine.IsMatch(sp[i]))
                            {
                                sb.AppendLine(dictValue[sp[++i]].ToString());
                                sb.AppendLine("");
                            }
                            else
                            {
                                sb.Append(sp[i]);
                            }
                        }
                    }
                    else if (argSpLine.IsMatch(line))
                    {
                        var m = argSpLine.Match(line).Groups[2];
                        var msp = argSpLine.Split(line);
                        if (argSpLineAr.IsMatch(m.Value))
                        {
                            var mm = argSpLineAr.Match(m.Value);
                            var elName = mm.Groups[2].Value;
                            for (int i = Int32.Parse(mm.Groups[4].Value); i < Int32.Parse(mm.Groups[5].Value); i++)
                            {
                                sb.AppendLine($"{msp[0]}{mm.Groups[6].Value}{i}{mm.Groups[9].Value}");
                            }
                        }
                    }
                    else if (argLine.IsMatch(line))
                    {
                        var m = argLine.Match(line).Groups[2];
                        var msp = argLine.Split(line);
                        if (rg2.IsMatch(m.Value))
                        {
                            var mr2 = rg2.Match(m.Value);
                            var sp = rg2.Split(m.Value);
                            foreach (var val in (Array)dictValue[mr2.Groups[2].Value])
                            {
                                sb.AppendLine();
                                sb.Append($"{msp[0]}");
                                for (int i = 0; i < sp.Length; i++)
                                {
                                    if (rg2.IsMatch(sp[i]))
                                    {
                                        sb.Append(val);
                                        i++;
                                    }
                                    else
                                    {
                                        sb.Append(sp[i]);
                                    }
                                }
                            }
                            
                            sb.AppendLine();

                        }
                    }
                    else if (rg2.IsMatch(line))
                    {
                        sb.AppendLine();
                        var sp = rg2.Split(line);
                        for (int i = 0; i < sp.Length; i++)
                        {
                            if (rg2.IsMatch(sp[i]))
                            {
                                sb.Append(dictValue[sp[++i]]);
                            }
                            else
                            {
                                sb.Append(sp[i]);
                            }
                        }
                        sb.AppendLine();
                    }
                    else
                    {
                        sb.AppendLine(line);
                    }
                }
            }

            //TODO: потом доработать
            using (var sr2 = new StreamWriter($"{createUrl}/{fileName ?? newFileName}.cs"))
            {
                sr2.WriteLine(sb.ToString());
            }
        }
    }
}
