using HTMLxUnitGenerator.Templates;
using System;
using System.IO;
using System.Text;

namespace HTMLxUnitGenerator.Templates
{
    public class TemplateManager
    {
        public static string GetSummaryTemplate()
        {
            return Summary.GetSource();// GetEncodedResource(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\Templates\Summary.cshtml"));
        }

        public static string GetFileTemplate()
        {
            return SingleReport.GetSource(); //GetEncodedResource(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\Templates\NewFile.cshtml"));
        }

        private static string GetEncodedResource(string path)
        {
            return Encoding.UTF8.GetString(System.IO.File.ReadAllBytes(path));
        }
    }
}
