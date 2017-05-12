using HTMLxUnitGenerator.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMLxUnitGenerator
{
    class Program
    {
        private static string USAGE = @"[INFO] Usage 1:  ReportUnit 'path-to-folder'\n
                                        [INFO] Usage 2:  ReportUnit 'input.xml'\n
                                        [INFO] Usage 3:  ReportUnit 'input-folder' 'output-folder'\n
                                        [INFO] Usage 4:  ReportUnit 'input.xml' 'output.html'";


        private static Logger _logger = Logger.GetLogger();

        static void Main(string[] args)
        {
            if (args == null || args.Length == 0 || args.Length > 2)
            {
                _logger.Log(Level.Error, "Invalid argument(s) specified.\n" + USAGE);
            }

            if (args.Length == 1)
            {
                string inputPath = args[0];
                if (IsPathValid(args[0]))
                {
                    PathTypeEnum pathType = GetPathType(inputPath);
                    new ReportUnitService().CreateReport(pathType, inputPath);
                }
            }

            if (args.Length == 2)
            {
                string inputPath = args[0];
                string outputPath = args[1];

                if (IsPathValid(inputPath) && IsPathValid(outputPath))
                {
                    PathTypeEnum pathTypeInput = GetPathType(inputPath);

                    String directoryNameOutput = outputPath;

                    if (pathTypeInput == PathTypeEnum.File)
                        directoryNameOutput = Path.GetDirectoryName(outputPath);

                    Directory.CreateDirectory(directoryNameOutput);
                    new ReportUnitService().CreateReport(pathTypeInput, inputPath, outputPath);
                }
            }
        }

        private static bool IsPathValid(string input)
        {
            try
            {
                Path.GetFullPath(input);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Log(Level.Error, ex.Message + USAGE);
                return false;
            }
        }

        private static PathTypeEnum GetPathType(string input)
        {
            var attributes = File.GetAttributes(input);

            if ((FileAttributes.Directory & attributes) == FileAttributes.Directory)
                return PathTypeEnum.Directory;
            else
                return PathTypeEnum.File;
        }
    }
}
