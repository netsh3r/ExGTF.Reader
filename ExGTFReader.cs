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

        public void Create(string createUrl, bool isNeedCreate = true, string fileName = null)
        {
            var (result, newFileName, extension) = GetResult();
            var path = string.Concat(createUrl, "\\", fileName ?? newFileName, ".", extension);
            if (isNeedCreate)
            {
                CreateNewFile(result, path);
            }
            else
            {
                UpdateExistFile(result, path);
            }
        }

        private void CreateNewFile(string result, string path)
        {
            using var sw = new StreamWriter(path);
            sw.WriteLine(result);
        }

        private void UpdateExistFile(string result, string path)
        {
            var sb = new StringBuilder();
            using (var sr = new StreamReader(path))
            {
                string line = string.Empty;
                while ((line = sr.ReadLine()) != null)
                {
                    sb.AppendLine(line);
                    if (line.Contains("#ExGTF_Compiling%start%#"))
                    {
                        sb.Append(result);
                    }
                }
            }

            using (var sw = new StreamWriter(path))
            {
                sw.WriteLine(sb.ToString());
            }
        }

        private (string result, string fileName, string extension) GetResult()
        {
            var fi = new FileInfo(url);
            var newFileName = fi.Name.Substring(0, fi.Name.Length - 6);
            var extension = string.Empty;
            var sb = new StringBuilder();

            using (var sr = new StreamReader(url))
            {
                var header = sr.ReadLine();
                var rgFileName = ExGTF_Regex.FileName.Match(header);
                newFileName = rgFileName.Groups[1].Value;
                extension = rgFileName.Groups[2].Value;
                var line = string.Empty;
                while ((line = sr.ReadLine()) != null)
                {
                    AppendLine(line, sb, sr);
                }
            }

            return (sb.ToString(), newFileName, extension);
        }

        private void AppendLine(string line, StringBuilder sb, StreamReader sr)
        {
            if (line.Contains("^!"))
            {
                return;
            }
            
            if (line.Contains("[##"))
            {
                if (ExGTF_Regex.ArrayMultiLine.IsMatch(line))
                {
                    AppendMultiArrayLine(line, sb, sr);   
                }
                else if (ExGTF_Regex.ArrayWithObjects.IsMatch(line))
                {
                    AppendMultiArrayObjectsLine(line, sb, sr);
                }
            }
            else if (line.Contains("[??"))
            {
                AppendConditionLine(line, sb, sr);
            }
            else if (ExGTF_Regex.SingleLine.IsMatch(line))
            {
                AppendSingleLine(line, sb);
                sb.AppendLine();
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
                sb.AppendLine();
            }
            else
            {
                sb.AppendLine(line);
            }
        }

        private void AppendConditionLine(string line, StringBuilder sb, StreamReader sr)
        {
            var lSb = new StringBuilder();
            var baseMatch = ExGTF_Regex.BlockCondition.Match(line);
            var res = GetResultCondition(dictValue[baseMatch.Groups[3].Value].ToString(),
                baseMatch.Groups[4].Value,
                baseMatch.Groups[6].Value);

            while (!(line = sr.ReadLine()).Contains("??]"))
            {
                if (res)
                {
                    AppendLine(line, sb, sr);   
                }
            }
        }

        
        private bool GetResultCondition(string value1, string op, string value2)
        {
            return op switch
            {
                "!=" => value1 != value2,
                "==" => value1 == value2,
                _ => throw new Exception($"Невозможно обоработать оператор: {op}")
            };
        }

        private void AppendMultiArrayObjectsLine(string line, StringBuilder sb, StreamReader sr)
        {
            var lSb = new StringBuilder();
            var baseMatch = ExGTF_Regex.ArrayMultiLine.Match(line);
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
                else if (ExGTF_Regex.ArrayProps.IsMatch(line))
                {
                    AppendBase(line, lSb, ExGTF_Regex.ArrayProps, "{0}");
                    lSb.AppendLine();
                }
                else
                {
                    AppendLine(line, lSb, sr);
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
            var msp = ExGTF_Regex.ArrayLine.Split(line);
            var arrayGroups = ExGTF_Regex.ArrayLine.Match(line).Groups;
            var arrayName = arrayGroups[4].Value;
            var arrayLine = arrayGroups[5].Value;
            var lSb = new StringBuilder();
            if (ExGTF_Regex.ArrayProps.IsMatch(arrayLine))
            {
                lSb.Append(msp[0]);
                var sp = ExGTF_Regex.ArrayProps.Split(arrayLine);
                for (int i = 0; i<sp.Length; i++)
                {
                    if(ExGTF_Regex.ArrayProps.IsMatch(sp[i]))
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
            
            foreach (var val in (Array)dictValue[arrayName])
            {
                sb.AppendLine(string.Format(lSb.ToString(), val));
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