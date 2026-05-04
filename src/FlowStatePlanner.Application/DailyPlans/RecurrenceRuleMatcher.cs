namespace FlowStatePlanner.Application.DailyPlans;

public interface IRecurrenceRuleMatcher
{
    bool Matches(string? recurrenceRule, DateOnly date);
}

public sealed class RecurrenceRuleMatcher : IRecurrenceRuleMatcher
{
    public bool Matches(string? recurrenceRule, DateOnly date)
    {
        if (string.IsNullOrWhiteSpace(recurrenceRule)) return false;
        var rule = recurrenceRule.Trim().ToUpperInvariant();
        if (rule == "DAILY") return true;
        if (rule.StartsWith("WEEKLY:"))
        {
            var tokens = rule[7..].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var dayToken = date.DayOfWeek.ToString()[..3].ToUpperInvariant();
            return tokens.Contains(dayToken);
        }

        if (rule.StartsWith("MONTHLY:") && int.TryParse(rule[8..], out var day))
            return day == date.Day;

        return false;
    }
}
