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
        public String Name { get; set; }

        public TimeOnly Start {  get; set; }

        public TimeOnly End { get; set; }

        public String ColorHex { get; set; }

        [XmlIgnore]
        public Color Color
        {
            get => ColorTranslator.FromHtml(ColorHex);
            set => ColorHex = ColorTranslator.ToHtml(value);
        }

        public Agent() { }

        public Agent(String param_Name, TimeOnly param_Start, TimeOnly param_End, Color param_Color)
        {
            Name = param_Name;
            Start = param_Start;
            End = param_End;
            Color = param_Color;
        }
    }
}
