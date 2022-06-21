using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ExGTF_Regex = ExGTF.Reader.ExGTF_Const.ExGTF_Regex;

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
                while ((line = sr.ReadLine()) != null)
                {
                    if (ExGTF_Regex.SingleLine.IsMatch(line))
                    {
                        AppendSingleLine(line, sb);
                    }
                    else if (ExGTF_Regex.LocalArrayLine.IsMatch(line))
                    {
                        AppendLocalArrayLine(line, sb);
                    }
                    else if (ExGTF_Regex.ArrayLine.IsMatch(line))
                    {
                        AppendArrayLine(line, sb);
                    }
                    else if (ExGTF_Regex.Single.IsMatch(line))
                    {
                        AppendSingle(line, sb);
                    }
                    else if (line.Contains("[##"))
                    {
                        AppendMultiArrayLine(line, sb, sr);
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

        private void AppendMultiArrayLine(string line, StringBuilder sb, StreamReader sr)
        {
            var lSb = new StringBuilder();
            var baseMatch = ExGTF_Regex.ArrayMultiLine.Match(line);
            var arrayValueName = baseMatch.Groups[1].Value;
            var arrayName = baseMatch.Groups[2].Value;
            while ((line = sr.ReadLine()) != null && !line.Contains("##]"))
            {
                if (ExGTF_Regex.ArrayProps.IsMatch(line) && ExGTF_Regex.Single.IsMatch(line))
                {
                    var sp = ExGTF_Regex.ArrayProps.Split(line);
                    lSb.AppendLine();
                    for (int i = 0; i<sp.Length; i++)
                    {
                        if (ExGTF_Regex.Single.IsMatch(sp[i]))
                        {
                            AppendSingle(sp[i], lSb);
                        }
                        else if(ExGTF_Regex.ArrayProps.IsMatch(sp[i]))
                        {
                            AppendBase(sp[i], lSb, ExGTF_Regex.ArrayProps, "{0}");
                            i++;
                        }
                        else
                        {
                            lSb.Append(sp[i]);
                        }
                    }
                }
                else if (ExGTF_Regex.Single.IsMatch(line))
                {
                    AppendSingle(line, lSb);
                }
                else if (ExGTF_Regex.ArrayProps.IsMatch(line))
                {
                    AppendBase(line, lSb, ExGTF_Regex.ArrayProps, "{0}");
                }
                else
                {
                    lSb.Append(line);
                }
            }

            foreach (var value in (Array)dictValue[arrayName])
            {
                sb.AppendLine(string.Format(lSb.ToString(), value));
            }
        }

        private void AppendSingleLine(string line, StringBuilder sb)
        {
            var sp = ExGTF_Regex.SingleLine.Split(line);
            for (int i = 0; i < sp.Length; i++)
            {
                if (ExGTF_Regex.SingleLine.IsMatch(sp[i]))
                {
                    sb.AppendLine(dictValue[sp[++i]].ToString());
                }
                else
                {
                    sb.Append(sp[i]);
                }
            }
        }

        private void AppendLocalArrayLine(string line, StringBuilder sb)
        {
            var m = ExGTF_Regex.LocalArrayLine.Match(line).Groups[2];
            var msp = ExGTF_Regex.LocalArrayLine.Split(line);
            if (ExGTF_Regex.LocalArrayLineArg.IsMatch(m.Value))
            {
                var mm = ExGTF_Regex.LocalArrayLineArg.Match(m.Value);
                var elName = mm.Groups[2].Value;
                for (int i = Int32.Parse(mm.Groups[4].Value); i < Int32.Parse(mm.Groups[5].Value); i++)
                {
                    sb.AppendLine($"{msp[0]}{mm.Groups[6].Value}{i}{mm.Groups[9].Value}");
                }
            }
        }

        private void AppendArrayLine(string line, StringBuilder sb)
        {
            var m = ExGTF_Regex.ArrayLine.Match(line).Groups[2];
            var msp = ExGTF_Regex.ArrayLine.Split(line);
            if (ExGTF_Regex.Single.IsMatch(m.Value))
            {
                var mr2 = ExGTF_Regex.Single.Match(m.Value);
                var sp = ExGTF_Regex.Single.Split(m.Value);
                foreach (var val in (Array)dictValue[mr2.Groups[2].Value])
                {
                    sb.AppendLine();
                    sb.Append($"{msp[0]}");
                    AppendSingle(m.Value, sb, val);
                }

                sb.AppendLine();
            }
        }

        private void AppendBase(string line, StringBuilder sb, Regex reg, object value = null)
        {
            var sp = reg.Split(line);
            for (int i = 0; i < sp.Length; i++)
            {
                if (reg.IsMatch(sp[i]))
                {
                    ++i;
                    sb.Append(value ?? dictValue[sp[i]]);
                }
                else
                {
                    sb.Append(sp[i]);
                }
            }
        }

        private void AppendSingle(string line, StringBuilder sb, object value = null)
        {
            AppendBase(line, sb, ExGTF_Regex.Single, value);
        }
    }
}