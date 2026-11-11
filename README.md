# Survey Cross Tab

Analyze named survey columns while separating skipped answers and ineligible respondents.

## Try it

From this folder with the .NET 10 SDK installed:

```sh
dotnet run -- survey study.tsv Cohort Answer Eligible
```

The first TSV row names columns. Pass group, answer and eligibility column names in that order. Eligibility is yes/no. An empty answer for an eligible respondent counts as skipped. Percentages use answered respondents as the denominator.

The included study.tsv uses tab-separated fields. Comment lines start with `#`.

The original exercise remains available with `dotnet run -- example.tsv`.

## When to use it

A methods student needs to take a real survey export into a report without mixing ineligible respondents with skipped answers.

CSV exports are also supported, including quoted fields containing commas. Try `dotnet run -- survey export.csv Cohort Answer Eligible`.

Example result:

```text
Day: 1 answered, 1 skipped, 1 ineligible; response rate 50%
  Bike: 0/1 0%
  Walk: 1/1 100%
Night: 1 answered, 0 skipped, 0 ineligible; response rate 100%
  Bike: 1/1 100%
  Walk: 0/1 0%
```

Related course or field workflow: [AAPOR survey research practices](https://aapor.org/standards-and-ethics/best-practices/).
