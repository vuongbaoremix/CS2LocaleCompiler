 public class Program
{
    private static void writeHelp()
    {
        Console.WriteLine(@"CS2LocaleCompiler:
  Compile and Decompile Cities Skyline 2 localization
Usage:
  CS2LocaleCompiler [options]
Options:
  --decompile <path>                       Decompile localization format file to json.
  --recompile <path>                       Recompile json to localization format.
  --verify    <original> <translation>     Verify the translation with the original  

ex: CS2LocaleCompiler --decompile ""D://en-US.loc""
");
    }


    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        if (args.Length < 2)
        {
            writeHelp();
            return;
        }

        string action = args[0].ToLower();

        if (action == "--decompile")
        {
            string filePath = args[1];

            if (File.Exists(filePath))
            {
                Console.WriteLine($"Decompile {filePath}");
                 
                var localeInfo = CS2LocaleCompiler.LocalizationCompiler.ReadLocale(filePath);
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var dirName = Path.GetDirectoryName(filePath);
                var outputFile = Path.Combine(dirName, $"{fileName}.json");

                Console.WriteLine($"Output {outputFile}"); 

                CS2LocaleCompiler.LocalizationCompiler.WriteLocaleToJson(outputFile, localeInfo);


                Console.WriteLine("Completed.");
            }
            else
            {
                Console.WriteLine($"File not found: {filePath}");
            }
        }
        else if (action == "--recompile")
        {
            string filePath = args[1];

            if (File.Exists(filePath))
            {
                Console.WriteLine($"Recompile {filePath}");


                var localeInfo = CS2LocaleCompiler.LocalizationCompiler.ReadLocaleFromJson(filePath);
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var dirName = Path.GetDirectoryName(filePath);
                var outputFile = Path.Combine(dirName, $"{fileName}.loc");

                Console.WriteLine($"Output {outputFile}");
                CS2LocaleCompiler.LocalizationCompiler.WriteLocale(outputFile, localeInfo);
            }
            else
            {
                Console.WriteLine($"File not found: {filePath}");
            }
        }
        else if (action == "--verify")
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Insufficient arguments for --verify. Usage: CS2LocaleCompiler --verify <sourceFile> <targetFile>");
                return;
            }

            string sourceFile = args[1];
            string targetFile = args[2];

            if (File.Exists(sourceFile) && File.Exists(targetFile))
            {
                Console.WriteLine($"Verify {sourceFile} against {targetFile}");

                var sourceFileLocaleInfo = CS2LocaleCompiler.LocalizationCompiler.ReadLocale(sourceFile);
                var targetFileLocaleInfo = CS2LocaleCompiler.LocalizationCompiler.ReadLocale(targetFile);

                if(CS2LocaleCompiler.LocalizationCompiler.Verify(sourceFileLocaleInfo, targetFileLocaleInfo))
                {
                    Console.WriteLine("Translation is Valid");    
                }
            }
            else
            {
                Console.WriteLine("Source or target file not found.");
            }
        }
        else
        {
            writeHelp();
        }
    } 
}