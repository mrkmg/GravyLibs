using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Gravy.MetaString;

namespace Gravy.ConsoleString;

using ConsoleString = MetaString<ConsoleFormat>;

// TODO: move to Gravy.MetaString
[InterpolatedStringHandler]
public class ConsoleStringInterpolatedStringHandler
{
    private readonly List<ConsoleString> _parts;

    // ReSharper disable twice UnusedParameter.Local
    public ConsoleStringInterpolatedStringHandler(int literalLength, int formattedCount)
        => _parts = new();

    public void AppendLiteral(string s)
        => _parts.Add(new(s));

    public void AppendFormatted(string s)
        => _parts.Add(new(s));
    
    public void AppendFormatted(string s, string format, IFormatProvider? provider = null)
        => _parts.Add(new(string.Format(s, format, provider)));

    public void AppendFormatted<T>(T s)
        => _parts.Add(s as ConsoleString ?? new(s?.ToString() ?? string.Empty));

    public void AppendFormatted<T>(T s, string format, IFormatProvider? provider = null)
    {
        if (s is ConsoleString) throw new InvalidOperationException("Cannot format a ConsoleString");
        if (s is IFormattable f)
        {
            _parts.Add(new(f.ToString(format, provider)));
            return;
        }
        AppendFormatted(s?.ToString() ?? string.Empty, format, provider);
    }
    
    public void AppendFormatted(string str, int alignment, string? format = null, IFormatProvider? provider = null)
    {
        if (format is not null)
            str = string.Format(format, str, provider);
        _parts.Add(alignment switch
        {
            < 0 => new(str.PadLeft(-alignment)),
            > 0 => new(str.PadRight(alignment)),
            _ => new(str)
        });
    }
    
    public void AppendFormatted<T>(T o, int alignment, string? format = null, IFormatProvider? provider = null)
    {
        if (o is ConsoleString cs)
        {
            if (format is not null) throw new InvalidOperationException("Cannot format a ConsoleString");
            _parts.Add(alignment switch
            {
                < 0 => cs.PadLeft(-alignment),
                > 0 => cs.PadRight(alignment),
                _ => cs
            });
            return;
        }
        AppendFormatted(o?.ToString() ?? string.Empty, alignment, format, provider);
    }
    
    public ConsoleString ToConsoleString() 
        => new(_parts.SelectMany(x => x.MetaData).Select(x => x.WithoutPosition()));
    
    
}