using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SyncroTeam.Domain.Entities
{
    public class Agent
    {
        public string Name { get; set; }

        public TimeOnly Start { get; set; }
        public TimeOnly End { get; set; }

        public List<DateOnly> AbsentDays { get; set; } = new();
        public bool HasManualSchedule { get; set; } = false;
        public bool IsLockedToTask { get; set; } = false;

        public string ColorHex { get; set; }

        [XmlIgnore]
        public Color Color
        {
            get => ColorTranslator.FromHtml(NormalizeColorHex(ColorHex));
            set => ColorHex = NormalizeColorToHex(value);
        }

        public Agent() { }

        public Agent(string name, TimeOnly start, TimeOnly end, Color color)
        {
            Name = name;
            Start = start;
            End = end;
            Color = color;
        }

        private string NormalizeColorToHex(Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        private string NormalizeColorHex(string hex)
        {
            if(!hex.StartsWith("#") && Color.FromName(hex).IsKnownColor)
                return ColorTranslator.ToHtml(Color.FromName(hex));
            return hex;
        }
    }
}
