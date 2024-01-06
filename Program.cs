using System;

namespace csvGenerator
{
  class Program
  {
    private static readonly string help = "CSVGenerator can manipulate txt file to export in csv format:" +
    "\n Gived a txt file with a list of param tool can merge number of row in one row with custom separate char" +
    "\n Mandatory param: " +
    "\n\t -input: <path to txt file>" +
    "\n\t -output: <path output file>" +
    "\n\t -eR: number of element in row" +
    "\n\t -c: char delimitator";

    static private string inputFilePath = "";
    static private string outputFilePath = "";
    static private int numberOfElementForRow = 0;
    static private char delimitator;
    static void Main(string[] args)
    {
      Console.WriteLine("Welcome on csv generaThor!!!");
      if (args.Count() <= 1)
      {
        Console.WriteLine(help);
        return;
      }
      CheckMandatoryParam(args);
      GenereteFileOutput();
    }

    static void GenereteFileOutput()
    {
      var linesInput = File.ReadAllLines(inputFilePath);
      if (linesInput.Length <= 0)
      {
        throw new ArgumentNullException("Input file haven't any string lines");
      }
      if (linesInput.Length % numberOfElementForRow > 0)
      {
        Console.WriteLine($"There are {linesInput.Length} lines in the input file."
        + $"You asked to merge {numberOfElementForRow} of them into one, but some lines could not be merged."
        + "These lines will be discarded");
      }

      var linesOutputNumber = linesInput.Length / numberOfElementForRow;
      var resultLines = new List<string>();
      for (int iter = 0; iter < linesOutputNumber; iter++)
      {
        var tempList = linesInput.Skip(iter * numberOfElementForRow)
        .Take(numberOfElementForRow)
        .ToList();

        //this is a check to keep program safe
        if (!tempList.Any())
        {
          continue;
        }
        resultLines.Add(MergeLines(tempList, delimitator));
      }
      File.WriteAllLines(outputFilePath, resultLines);
    }

    static string MergeLines(List<string> lines, char delimitator)
    {
      return string.Join(delimitator, lines);
    }

    static void CheckMandatoryParam(string[] args)
    {
      //input file
      var inputIndex = Array.IndexOf(args, "-input");
      if (inputIndex < 0)
      {
        throw new ArgumentNullException("-input arguments is mandatory");
      }
      inputFilePath = args[inputIndex + 1];
      if (!File.Exists(inputFilePath))
      {
        throw new ArgumentNullException($"{inputFilePath} cannot exist");
      }

      //output file
      var outputIndex = Array.IndexOf(args, "-output");
      if (outputIndex < 0)
      {
        throw new ArgumentNullException("-output arguments is mandatory");
      }
      outputFilePath = Path.GetFullPath(args[outputIndex + 1]);
      if (File.Exists(outputFilePath))
      {
        File.Delete(outputFilePath);

      }
      else
      {
        var directory = Path.GetDirectoryName(outputFilePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
          Directory.CreateDirectory(directory);
        }
      }

      //number of row start file to merge im single row in output file
      var nIndex = Array.IndexOf(args, "-eR");
      if (nIndex < 0 || !int.TryParse(args[nIndex + 1], out numberOfElementForRow))
      {
        throw new ArgumentNullException("-eR argument is mandatory and must have valid int number");
      }
      if (numberOfElementForRow <= 0)
      {
        throw new ArgumentException("-eR must have positive value >= 0");
      }

      var delimitatorIndex = Array.IndexOf(args, "-c");
      if (delimitatorIndex < 0 || !char.TryParse(args[delimitatorIndex + 1], out delimitator))
      {
        throw new ArgumentNullException("-c argument is mandatory and must have valid char value");
      }
    }
  }
}

