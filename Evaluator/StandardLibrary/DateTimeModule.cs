using Snip.AST;

namespace Snip.Evaluator;

public static class DateTimeModule
{
    public static Module CreateModule()
    {
        var module = new Module("datetime");
        
        // Current date/time
        module.Export("now", Value.NativeFunction(new NativeFunctionValue("now", Now)));
        module.Export("timestamp", Value.NativeFunction(new NativeFunctionValue("timestamp", Timestamp)));
        
        // Date/time construction
        module.Export("date", Value.NativeFunction(new NativeFunctionValue("date", Date)));
        module.Export("time", Value.NativeFunction(new NativeFunctionValue("time", Time)));
        module.Export("datetime", Value.NativeFunction(new NativeFunctionValue("datetime", DateTime)));
        
        // Date/time formatting
        module.Export("format", Value.NativeFunction(new NativeFunctionValue("format", Format)));
        
        // Date/time arithmetic
        module.Export("add_days", Value.NativeFunction(new NativeFunctionValue("add_days", AddDays)));
        module.Export("add_hours", Value.NativeFunction(new NativeFunctionValue("add_hours", AddHours)));
        module.Export("add_minutes", Value.NativeFunction(new NativeFunctionValue("add_minutes", AddMinutes)));
        module.Export("add_seconds", Value.NativeFunction(new NativeFunctionValue("add_seconds", AddSeconds)));
        
        // Date/time differences
        module.Export("diff", Value.NativeFunction(new NativeFunctionValue("diff", Diff)));
        module.Export("days_between", Value.NativeFunction(new NativeFunctionValue("days_between", DaysBetween)));
        module.Export("hours_between", Value.NativeFunction(new NativeFunctionValue("hours_between", HoursBetween)));
        module.Export("minutes_between", Value.NativeFunction(new NativeFunctionValue("minutes_between", MinutesBetween)));
        module.Export("seconds_between", Value.NativeFunction(new NativeFunctionValue("seconds_between", SecondsBetween)));
        
        // Time deltas
        module.Export("timedelta", Value.NativeFunction(new NativeFunctionValue("timedelta", TimeDelta)));
        
        return module;
    }
    
    private static Value Now(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 0, 0, "now");
        var now = System.DateTime.Now;
        return Value.Object(new Dictionary<string, Value>
        {
            ["year"] = Value.Number(now.Year),
            ["month"] = Value.Number(now.Month),
            ["day"] = Value.Number(now.Day),
            ["hour"] = Value.Number(now.Hour),
            ["minute"] = Value.Number(now.Minute),
            ["second"] = Value.Number(now.Second),
            ["weekday"] = Value.Number((int)now.DayOfWeek),
            ["timestamp"] = Value.Number(((DateTimeOffset)now).ToUnixTimeSeconds())
        });
    }
    
    private static Value Timestamp(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 0, 0, "timestamp");
        return Value.Number(((DateTimeOffset)System.DateTime.Now).ToUnixTimeSeconds());
    }
    
    private static Value Date(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 3, 3, "date");
        var year = ValidationHelpers.GetInt(args, 0);
        var month = ValidationHelpers.GetInt(args, 1);
        var day = ValidationHelpers.GetInt(args, 2);
        
        try
        {
            var date = new System.DateTime(year, month, day);
            return Value.Object(new Dictionary<string, Value>
            {
                ["year"] = Value.Number(date.Year),
                ["month"] = Value.Number(date.Month),
                ["day"] = Value.Number(date.Day),
                ["weekday"] = Value.Number((int)date.DayOfWeek),
                ["timestamp"] = Value.Number(((DateTimeOffset)date).ToUnixTimeSeconds())
            });
        }
        catch (System.ArgumentException ex)
        {
            throw new Exception($"Invalid date: {ex.Message}");
        }
    }
    
    private static Value Time(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 3, 4, "time");
        var hour = ValidationHelpers.GetInt(args, 0);
        var minute = ValidationHelpers.GetInt(args, 1);
        var second = ValidationHelpers.GetInt(args, 2);
        var millisecond = args.Count > 3 ? ValidationHelpers.GetInt(args, 3) : 0;
        
        try
        {
            var time = new System.TimeSpan(hour, minute, second).Add(System.TimeSpan.FromMilliseconds(millisecond));
            return Value.Object(new Dictionary<string, Value>
            {
                ["hour"] = Value.Number(time.Hours),
                ["minute"] = Value.Number(time.Minutes),
                ["second"] = Value.Number(time.Seconds),
                ["millisecond"] = Value.Number(time.Milliseconds),
                ["total_seconds"] = Value.Number(time.TotalSeconds)
            });
        }
        catch (System.ArgumentException ex)
        {
            throw new Exception($"Invalid time: {ex.Message}");
        }
    }
    
    private static Value DateTime(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 6, 7, "datetime");
        var year = ValidationHelpers.GetInt(args, 0);
        var month = ValidationHelpers.GetInt(args, 1);
        var day = ValidationHelpers.GetInt(args, 2);
        var hour = ValidationHelpers.GetInt(args, 3);
        var minute = ValidationHelpers.GetInt(args, 4);
        var second = ValidationHelpers.GetInt(args, 5);
        var millisecond = args.Count > 6 ? ValidationHelpers.GetInt(args, 6) : 0;
        
        try
        {
            var datetime = new System.DateTime(year, month, day, hour, minute, second, millisecond);
            return Value.Object(new Dictionary<string, Value>
            {
                ["year"] = Value.Number(datetime.Year),
                ["month"] = Value.Number(datetime.Month),
                ["day"] = Value.Number(datetime.Day),
                ["hour"] = Value.Number(datetime.Hour),
                ["minute"] = Value.Number(datetime.Minute),
                ["second"] = Value.Number(datetime.Second),
                ["millisecond"] = Value.Number(datetime.Millisecond),
                ["weekday"] = Value.Number((int)datetime.DayOfWeek),
                ["timestamp"] = Value.Number(((DateTimeOffset)datetime).ToUnixTimeSeconds())
            });
        }
        catch (System.ArgumentException ex)
        {
            throw new Exception($"Invalid datetime: {ex.Message}");
        }
    }
    
    private static Value Format(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "format");
        var dateTime = ParseDateTimeObject(args, 0);
        var format = ValidationHelpers.GetString(args, 1);
        
        try
        {
            return Value.String(dateTime.ToString(format));
        }
        catch (System.FormatException ex)
        {
            throw new Exception($"Invalid format '{format}': {ex.Message}");
        }
    }
    
    private static Value AddDays(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "add_days");
        var dateTime = ParseDateTimeObject(args, 0);
        var days = ValidationHelpers.GetNumber(args, 1);
        
        var result = dateTime.AddDays(days);
        return DateTimeToObject(result);
    }
    
    private static Value AddHours(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "add_hours");
        var dateTime = ParseDateTimeObject(args, 0);
        var hours = ValidationHelpers.GetNumber(args, 1);
        
        var result = dateTime.AddHours(hours);
        return DateTimeToObject(result);
    }
    
    private static Value AddMinutes(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "add_minutes");
        var dateTime = ParseDateTimeObject(args, 0);
        var minutes = ValidationHelpers.GetNumber(args, 1);
        
        var result = dateTime.AddMinutes(minutes);
        return DateTimeToObject(result);
    }
    
    private static Value AddSeconds(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "add_seconds");
        var dateTime = ParseDateTimeObject(args, 0);
        var seconds = ValidationHelpers.GetNumber(args, 1);
        
        var result = dateTime.AddSeconds(seconds);
        return DateTimeToObject(result);
    }
    
    private static Value Diff(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "diff");
        var start = ParseDateTimeObject(args, 0);
        var end = ParseDateTimeObject(args, 1);
        
        var difference = end - start;
        return Value.Object(new Dictionary<string, Value>
        {
            ["days"] = Value.Number(Math.Abs(difference.TotalDays)),
            ["hours"] = Value.Number(Math.Abs(difference.TotalHours)),
            ["minutes"] = Value.Number(Math.Abs(difference.TotalMinutes)),
            ["seconds"] = Value.Number(Math.Abs(difference.TotalSeconds)),
            ["milliseconds"] = Value.Number(Math.Abs(difference.TotalMilliseconds))
        });
    }
    
    private static Value DaysBetween(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "days_between");
        var start = ParseDateTimeObject(args, 0);
        var end = ParseDateTimeObject(args, 1);
        
        return Value.Number(Math.Abs((end - start).TotalDays));
    }
    
    private static Value HoursBetween(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "hours_between");
        var start = ParseDateTimeObject(args, 0);
        var end = ParseDateTimeObject(args, 1);
        
        return Value.Number(Math.Abs((end - start).TotalHours));
    }
    
    private static Value MinutesBetween(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "minutes_between");
        var start = ParseDateTimeObject(args, 0);
        var end = ParseDateTimeObject(args, 1);
        
        return Value.Number(Math.Abs((end - start).TotalMinutes));
    }
    
    private static Value SecondsBetween(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "seconds_between");
        var start = ParseDateTimeObject(args, 0);
        var end = ParseDateTimeObject(args, 1);
        
        return Value.Number(Math.Abs((end - start).TotalSeconds));
    }
    
    private static Value TimeDelta(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 0, 6, "timedelta");
        var days = args.Count > 0 ? (int)ValidationHelpers.GetNumber(args, 0) : 0;
        var hours = args.Count > 1 ? (int)ValidationHelpers.GetNumber(args, 1) : 0;
        var minutes = args.Count > 2 ? (int)ValidationHelpers.GetNumber(args, 2) : 0;
        var seconds = args.Count > 3 ? (int)ValidationHelpers.GetNumber(args, 3) : 0;
        var milliseconds = args.Count > 4 ? (int)ValidationHelpers.GetNumber(args, 4) : 0;
        
        var timeSpan = new System.TimeSpan(days, hours, minutes, seconds).Add(System.TimeSpan.FromMilliseconds(milliseconds));
        return Value.Object(new Dictionary<string, Value>
        {
            ["days"] = Value.Number(timeSpan.Days),
            ["hours"] = Value.Number(timeSpan.Hours),
            ["minutes"] = Value.Number(timeSpan.Minutes),
            ["seconds"] = Value.Number(timeSpan.Seconds),
            ["milliseconds"] = Value.Number(timeSpan.Milliseconds),
            ["total_seconds"] = Value.Number(timeSpan.TotalSeconds)
        });
    }
    
    private static System.DateTime ParseDateTimeObject(List<Value> args, int index)
    {
        if (args[index].Type == ValueType.String)
        {
            var dateString = ValidationHelpers.GetString(args, index);
            if (System.DateTime.TryParse(dateString, out var result))
                return result;
            else
                throw new Exception($"Invalid date string: {dateString}");
        }
        else if (args[index].Type == ValueType.Object)
        {
            var obj = ValidationHelpers.GetObject(args, index);
            var year = obj.ContainsKey("year") ? ValidationHelpers.GetInt(new List<Value> { obj["year"] }, 0) : 1;
            var month = obj.ContainsKey("month") ? ValidationHelpers.GetInt(new List<Value> { obj["month"] }, 0) : 1;
            var day = obj.ContainsKey("day") ? ValidationHelpers.GetInt(new List<Value> { obj["day"] }, 0) : 1;
            var hour = obj.ContainsKey("hour") ? ValidationHelpers.GetInt(new List<Value> { obj["hour"] }, 0) : 0;
            var minute = obj.ContainsKey("minute") ? ValidationHelpers.GetInt(new List<Value> { obj["minute"] }, 0) : 0;
            var second = obj.ContainsKey("second") ? ValidationHelpers.GetInt(new List<Value> { obj["second"] }, 0) : 0;
            
            try
            {
                return new System.DateTime(year, month, day, hour, minute, second);
            }
            catch (System.ArgumentException ex)
            {
                throw new Exception($"Invalid date components: {ex.Message}");
            }
        }
        else
        {
            throw new Exception("Expected date string or date object");
        }
    }
    
    private static Value DateTimeToObject(System.DateTime dateTime)
    {
        return Value.Object(new Dictionary<string, Value>
        {
            ["year"] = Value.Number(dateTime.Year),
            ["month"] = Value.Number(dateTime.Month),
            ["day"] = Value.Number(dateTime.Day),
            ["hour"] = Value.Number(dateTime.Hour),
            ["minute"] = Value.Number(dateTime.Minute),
            ["second"] = Value.Number(dateTime.Second),
            ["weekday"] = Value.Number((int)dateTime.DayOfWeek),
            ["timestamp"] = Value.Number(((DateTimeOffset)dateTime).ToUnixTimeSeconds())
        });
    }
}