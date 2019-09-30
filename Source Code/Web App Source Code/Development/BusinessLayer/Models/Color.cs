using IID.BusinessLayer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IID.BusinessLayer.Models
{
    public class Color
    {
        private Color() { }

        public Color(int colorId)
        {
            using (Entity db = new Entity())
            {
                var color = db.t_color.Find(colorId);
                SetProperties(color);
            }
        }

        public Color(t_color color)
        {
            SetProperties(color);
        }

        private void SetProperties(t_color color)
        {
            ColorId = color.color_id;
            Name = color.name;
            Hexadecimal = color.hexadecimal;
            R = color.r;
            G = color.g;
            B = color.b;
            Active = color.active;
        }

        public int ColorId { get; set; }
        public string Name { get; set; }
        public string Hexadecimal { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public bool? Active { get; set; }

        public System.Drawing.Color DotNetColor { get { return System.Drawing.Color.FromArgb(R, G, B); } }
    }
}