using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChimpmunkTetris
{
    public class Theme
    {
        public ConsoleColor Foreground { get; set; }
        public ConsoleColor Background { get; set; }
        public ConsoleColor TextColor { get; set; }
        public List<ConsoleColor> Colors { get; set; }

        public Theme(GameThemes theme)
        {
            this.Colors = new List<ConsoleColor>();
            switch (theme)
            {
                case GameThemes.Black:
                    this.Background = ConsoleColor.Black;
                    this.Foreground = ConsoleColor.White;
                    this.TextColor = ConsoleColor.Cyan;
                    for (int i = 1; i <= 15; i++)
                        if (i != (int)ConsoleColor.DarkBlue 
                            && i != (int)ConsoleColor.DarkGray
                            && i != (int)ConsoleColor.DarkMagenta) this.Colors.Add((ConsoleColor)i);                            
                    break;

                case GameThemes.Yellow:
                    this.Background = ConsoleColor.Yellow;
                    this.Foreground = ConsoleColor.Gray;
                    this.TextColor = ConsoleColor.DarkCyan;
                    for (int i = 1; i <= 13; i++) this.Colors.Add((ConsoleColor)i);
                    break;

                case GameThemes.Blue:
                    this.Background = ConsoleColor.Blue;
                    this.Foreground = ConsoleColor.Cyan;
                    this.TextColor = ConsoleColor.White;
                    for (int i = 1; i <= 15; i++) if (i != (int)ConsoleColor.Blue
                        && i != (int)ConsoleColor.DarkBlue
                        && i != (int)ConsoleColor.DarkCyan
                        && i != (int)ConsoleColor.DarkMagenta
                        && i != (int)ConsoleColor.DarkGreen
                        && i != (int)ConsoleColor.DarkRed
                        && i != (int)ConsoleColor.Cyan) this.Colors.Add((ConsoleColor)i);
                    break;

                case GameThemes.Cyan:
                    this.Background = ConsoleColor.Cyan;
                    this.Foreground = ConsoleColor.Blue;
                    this.TextColor = ConsoleColor.White;
                    for (int i = 1; i <= 15; i++) if (i != (int)ConsoleColor.Cyan) this.Colors.Add((ConsoleColor)i);
                    break;

                case GameThemes.WhiteYellow:
                    this.Background = ConsoleColor.White;
                    this.Foreground = ConsoleColor.DarkYellow;
                    this.TextColor = ConsoleColor.DarkMagenta;
                    for (int i = 1; i <= 13; i++) this.Colors.Add((ConsoleColor)i);
                    break;

                case GameThemes.WhiteMagenta:
                    this.Background = ConsoleColor.White;
                    this.Foreground = ConsoleColor.DarkMagenta;
                    this.TextColor = ConsoleColor.DarkYellow;
                    for (int i = 1; i <= 13; i++) this.Colors.Add((ConsoleColor)i);
                    break;

                default:
                    this.Background = ConsoleColor.White;
                    this.Foreground = ConsoleColor.Black;
                    this.TextColor = ConsoleColor.DarkCyan;
                    for (int i = 1; i <= 13; i++) this.Colors.Add((ConsoleColor)i);
                    break;
            }
        }
    }
}
