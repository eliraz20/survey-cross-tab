static class SurveyReport
{
    public static void Run(string[] args)
    {
        if (args.Length != 5)
            throw new FormatException("Usage: survey study.tsv Cohort Answer Eligible");
        var rows = Rows(args[1]);
        var header = rows[0];
        Require(header.All(s => s.Length > 0) && header.Distinct().Count() == header.Length, "Invalid header.");
        int Column(string name)
        {
            var index = Array.IndexOf(header, name);
            Require(index >= 0, "Missing column: " + name);
            return index;
        }

        int group = Column(args[2]), answer = Column(args[3]), eligible = Column(args[4]);
        var responses = rows.Skip(1).Select(r =>
        {
            Require(r.Length == header.Length && r[group].Length > 0, "Invalid survey row.");
            var flag = r[eligible].ToLowerInvariant();
            Require(flag is "yes" or "no", "Eligibility is yes or no.");
            return (Group: r[group], Answer: r[answer], Eligible: flag == "yes");
        }).ToArray();
        Require(responses.Length > 0, "No respondents.");
        var choices = responses.Where(r => r.Eligible && r.Answer.Length > 0).Select(r => r.Answer).Distinct().Order().ToArray();
        foreach (var g in responses.GroupBy(r => r.Group))
        {
            var yes = g.Where(r => r.Eligible).ToArray();
            var answered = yes.Where(r => r.Answer.Length > 0).ToArray();
            Console.WriteLine($"{g.Key}: {answered.Length} answered, {yes.Length - answered.Length} skipped, {g.Count() - yes.Length} ineligible; response rate " + (yes.Length == 0 ? "undefined" : $"{100m * answered.Length / yes.Length:0.##}%"));
            foreach (var choice in choices)
            {
                int n = answered.Count(r => r.Answer == choice);
                Console.WriteLine($"  {choice}: {n}/{answered.Length} " + (answered.Length == 0 ? "undefined" : $"{100m * n / answered.Length:0.##}%"));
            }
        }
    }

    static List<string[]> Rows(string file)
    {
        using var reader = file == "-" ? Console.In : File.OpenText(file);
        var rows = new List<string[]>();
        using var parser = new Microsoft.VisualBasic.FileIO.TextFieldParser(reader);
        parser.SetDelimiters(Path.GetExtension(file).Equals(".csv", StringComparison.OrdinalIgnoreCase) ? "," : "\t");
        parser.HasFieldsEnclosedInQuotes = true;
        parser.CommentTokens = ["#"];
        try
        {
            while (!parser.EndOfData)
                if (parser.ReadFields() is { } cells) rows.Add(cells);
        }
        catch (Microsoft.VisualBasic.FileIO.MalformedLineException e)
        {
            throw new FormatException($"Malformed record at line {parser.ErrorLineNumber}.", e);
        }

        if (rows.Count == 0)
            throw new FormatException("Empty input.");
        return rows;
    }

    static void Require(bool condition, string message)
    {
        if (!condition)
            throw new FormatException(message);
    }
}
