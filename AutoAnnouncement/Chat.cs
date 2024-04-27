using System;
using System.Linq;

namespace AutoAnnouncement;

record Chat
{
    public int Type { get; set; }

    public int Cycle { get; set; }

    public DateTime Time { get; set; }

    public string Message { get; set; }


    public static Chat GetChat(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;

        string[] x = s.Replace('\t', ' ')
            .Split(' ')
            .Where(y => !string.IsNullOrEmpty(y))
            .ToArray();

        if (x.Length != 4) return null;
        if (!int.TryParse(x[0], out int type)) return null;
        if (!int.TryParse(x[1], out int cycle)) return null;
        if (!TimeOnly.TryParse(x[2], out TimeOnly time)) return null;
        if (string.IsNullOrWhiteSpace(x[3])) return null;


        DateTime dt;

        if (type == 1)
        {
            dt = DateOnly.FromDateTime(DateTime.Now).ToDateTime(time);
        }
        else if (type == 2)
        {
            int weeknow = Convert.ToInt32(DateTime.Now.DayOfWeek);
            weeknow = (weeknow == 0 ? (7 - 1) : (weeknow - 1));
            int daydiff = (-1) * weeknow;
            dt = DateTime.Now.AddDays(daydiff).AddDays(cycle - 1);
        }
        else if (type == 3)
        {
            dt = DateTime.Now.AddDays(-DateTime.Now.Day).AddDays(cycle + 1);
        }
        else
        {
            return null;
        }

        return new Chat
        {
            Type = type,
            Cycle = cycle,
            Time = dt,
            Message = x[3]
        };
    }


    public Chat GetNextTime() => Type switch
    {
        1 => this with { Time = Time.AddDays(1) },
        2 => this with { Time = Time.AddDays(7) },
        3 => this with { Time = Time.AddMonths(1) },
        _ => null,
    };
}