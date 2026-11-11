using System.Globalization;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
if (args.Length > 0 && args[0] == "survey")
{
    try { SurveyReport.Run(args); return 0; }
    catch (Exception e) when (e is FormatException or IOException or ArgumentException or OverflowException or UnauthorizedAccessException)
    { Console.Error.WriteLine(e.Message); return 1; }
}

if (args.Length != 1 || args[0] == "--help")
{
    Console.WriteLine("Usage: dotnet run -- survey study.tsv Cohort Answer Eligible  |  original format: <input-file|->");
    return args.Length == 1 ? 0 : 2;
}

try
{
    string[] lines = args[0] == "-" ? Console.In.ReadToEnd().ReplaceLineEndings("\n").TrimEnd('\n').Split('\n') : File.ReadAllLines(args[0]);
    Check(lines.Length >= 1 && lines.Any(line => !string.IsNullOrWhiteSpace(line)), "Input is empty or missing records.");
    var answers = lines.Select(s => Fields(s, 2)).ToArray();
    var choices = answers.Select(a => a[1]).Distinct().Order().ToArray();
    foreach (var group in answers.GroupBy(a => a[0]).OrderBy(g => g.Key))
    {
        Console.WriteLine(group.Key);
        var counts = group.CountBy(a => a[1]).ToDictionary();
        foreach (var choice in choices)
        {
            var n = counts.GetValueOrDefault(choice);
            Console.WriteLine($"  {choice}: {n}/{group.Count()} ({100m * n / group.Count():0.#}%)");
        }
    }

    return 0;
}
catch (Exception e)when (e is IOException or FormatException or OverflowException or ArgumentException or UnauthorizedAccessException or DivideByZeroException)
{
    Console.Error.WriteLine(e.Message);
    return 1;
}

static void Check(bool valid, string message)
{
    if (!valid)
        throw new FormatException(message);
}

static string[] Fields(string line, int count)
{
    var fields = line.Split('\t', StringSplitOptions.TrimEntries);
    Check(fields.Length == count && fields.All(s => s.Length > 0), $"Expected {count} tab-separated values in '{line}'.");
    return fields;
}
