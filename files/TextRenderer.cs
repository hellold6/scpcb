// TextRenderer.cs — ports AAText.bb (AASetFont, AAText, AAStringWidth/Height)

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SCPCB360.GameLogic
{
    public class AAFont
    {
        public string Name;
        public int Scale = 2;
        public int CharWidth = 6;
        public int CharHeight = 7;
        public int LineHeight = 8;
        public bool IsAA = true;
        public Dictionary<char, int[]> CharWidths = new();
    }

    public static class TextRenderer
    {
        private static readonly Dictionary<char, string[]> Glyphs = BuildGlyphs();
        private static readonly Dictionary<int, AAFont> Fonts = new();
        private static int _selectedFont = 1;
        private static Texture2D _whitePixel;
        private static Color _color = Color.White;

        public static bool AATextEnabled { get; set; } =
            IniConfig.GetInt(GameState.OptionFile, "options", "antialiased text", 1) != 0;

        public static void Initialize(GraphicsDevice gfx)
        {
            _whitePixel = new Texture2D(gfx, 1, 1);
            _whitePixel.SetData(new[] { Color.White });

            Fonts[1] = CreateFont("Font1", 2, 2);
            Fonts[2] = CreateFont("Font2", 2, 2);
            Fonts[3] = CreateFont("Font3", 2, 2);
            Fonts[4] = CreateFont("Font4", 2, 2);
            Fonts[5] = CreateFont("ConsoleFont", 1, 1);
            AASetFont(1);
        }

        private static AAFont CreateFont(string name, int scale, int spacing)
        {
            var font = new AAFont
            {
                Name = name,
                Scale = scale,
                CharWidth = 6 * scale,
                CharHeight = 7 * scale,
                LineHeight = (7 + spacing) * scale,
                IsAA = AATextEnabled,
            };

            foreach (var pair in Glyphs)
                font.CharWidths[pair.Key] = new[] { pair.Value[0].Length * scale, font.CharHeight };
            return font;
        }

        public static int AALoadFont(string name = "Tahoma", int height = 13, int scaleFactor = 2)
        {
            int id = Fonts.Count + 1;
            Fonts[id] = CreateFont(name, Math.Max(1, scaleFactor), 2);
            return id;
        }

        public static void AASetFont(int fontId)
        {
            if (Fonts.ContainsKey(fontId))
                _selectedFont = fontId;
        }

        public static int AAStringWidth(string text)
        {
            if (string.IsNullOrEmpty(text)) return 0;
            var font = Fonts[_selectedFont];
            int width = 0;
            foreach (char raw in text)
            {
                char c = char.ToUpperInvariant(raw);
                if (font.CharWidths.TryGetValue(c, out var dims))
                    width += dims[0] - (font.Scale > 1 ? font.Scale : 0);
                else
                    width += font.CharWidth;
            }
            return width + font.Scale;
        }

        public static int AAStringHeight(string text)
        {
            if (string.IsNullOrEmpty(text)) return Fonts[_selectedFont].LineHeight;
            int lines = 1;
            foreach (char c in text)
                if (c == '\n') lines++;
            return Fonts[_selectedFont].LineHeight * lines;
        }

        public static void SetColor(Color color) => _color = color;

        public static void AAText(SpriteBatch sb, int x, int y, string text,
            bool centerX = false, bool centerY = false, float alpha = 1f)
        {
            if (string.IsNullOrEmpty(text) || sb == null || _whitePixel == null) return;

            var font = Fonts[_selectedFont];
            int scale = font.Scale;
            var color = _color * alpha;

            if (centerX) x -= AAStringWidth(text) / 2;
            if (centerY) y -= AAStringHeight(text) / 2;

            int cursorX = x;
            int cursorY = y;

            foreach (char raw in text)
            {
                if (raw == '\n')
                {
                    cursorX = x;
                    cursorY += font.LineHeight;
                    continue;
                }

                char c = char.ToUpperInvariant(raw);
                if (!Glyphs.TryGetValue(c, out var glyph))
                    glyph = Glyphs[' '];

                for (int row = 0; row < glyph.Length; row++)
                {
                    for (int col = 0; col < glyph[row].Length; col++)
                    {
                        if (glyph[row][col] != '1') continue;
                        var px = new Rectangle(cursorX + col * scale, cursorY + row * scale, scale, scale);
                        if (font.IsAA && scale > 1)
                        {
                            sb.Draw(_whitePixel, px, color * 0.35f);
                            sb.Draw(_whitePixel, new Rectangle(px.X - 1, px.Y, scale, scale), color * 0.2f);
                            sb.Draw(_whitePixel, new Rectangle(px.X + 1, px.Y, scale, scale), color * 0.2f);
                        }
                        sb.Draw(_whitePixel, px, color);
                    }
                }

                cursorX += (glyph[0].Length + 1) * scale;
            }
        }

        private static Dictionary<char, string[]> BuildGlyphs()
        {
            var g = new Dictionary<char, string[]>
            {
                [' '] = new[] { "000", "000", "000", "000", "000", "000", "000" },
                ['!'] = new[] { "1", "1", "1", "1", "0", "0", "1" },
                ['"'] = new[] { "101", "101", "000", "000", "000", "000", "000" },
                [':'] = new[] { "0", "1", "1", "0", "1", "1", "0" },
                ['.'] = new[] { "0", "0", "0", "0", "0", "1", "1" },
                [','] = new[] { "0", "0", "0", "0", "1", "1", "10" },
                ['-'] = new[] { "000", "000", "000", "111", "000", "000", "000" },
                ['('] = new[] { "01", "10", "10", "10", "10", "10", "01" },
                [')'] = new[] { "10", "01", "01", "01", "01", "01", "10" },
                ['/'] = new[] { "001", "001", "010", "010", "100", "100", "000" },
                ['?'] = new[] { "111", "001", "010", "010", "000", "010", "010" },
                ['0'] = new[] { "111", "101", "101", "101", "101", "101", "111" },
                ['1'] = new[] { "010", "110", "010", "010", "010", "010", "111" },
                ['2'] = new[] { "111", "001", "001", "111", "100", "100", "111" },
                ['3'] = new[] { "111", "001", "001", "111", "001", "001", "111" },
                ['4'] = new[] { "101", "101", "101", "111", "001", "001", "001" },
                ['5'] = new[] { "111", "100", "100", "111", "001", "001", "111" },
                ['6'] = new[] { "111", "100", "100", "111", "101", "101", "111" },
                ['7'] = new[] { "111", "001", "001", "010", "010", "010", "010" },
                ['8'] = new[] { "111", "101", "101", "111", "101", "101", "111" },
                ['9'] = new[] { "111", "101", "101", "111", "001", "001", "111" },
                ['A'] = new[] { "111", "101", "101", "111", "101", "101", "101" },
                ['B'] = new[] { "110", "101", "101", "110", "101", "101", "110" },
                ['C'] = new[] { "111", "100", "100", "100", "100", "100", "111" },
                ['D'] = new[] { "110", "101", "101", "101", "101", "101", "110" },
                ['E'] = new[] { "111", "100", "100", "111", "100", "100", "111" },
                ['F'] = new[] { "111", "100", "100", "111", "100", "100", "100" },
                ['G'] = new[] { "111", "100", "100", "101", "101", "101", "111" },
                ['H'] = new[] { "101", "101", "101", "111", "101", "101", "101" },
                ['I'] = new[] { "111", "010", "010", "010", "010", "010", "111" },
                ['J'] = new[] { "001", "001", "001", "001", "001", "101", "110" },
                ['K'] = new[] { "101", "101", "110", "100", "110", "101", "101" },
                ['L'] = new[] { "100", "100", "100", "100", "100", "100", "111" },
                ['M'] = new[] { "101", "111", "111", "101", "101", "101", "101" },
                ['N'] = new[] { "101", "111", "111", "111", "101", "101", "101" },
                ['O'] = new[] { "111", "101", "101", "101", "101", "101", "111" },
                ['P'] = new[] { "111", "101", "101", "111", "100", "100", "100" },
                ['Q'] = new[] { "111", "101", "101", "101", "111", "010", "001" },
                ['R'] = new[] { "110", "101", "101", "110", "101", "101", "101" },
                ['S'] = new[] { "111", "100", "100", "111", "001", "001", "111" },
                ['T'] = new[] { "111", "010", "010", "010", "010", "010", "010" },
                ['U'] = new[] { "101", "101", "101", "101", "101", "101", "111" },
                ['V'] = new[] { "101", "101", "101", "101", "101", "101", "010" },
                ['W'] = new[] { "101", "101", "101", "101", "111", "111", "101" },
                ['X'] = new[] { "101", "101", "101", "010", "101", "101", "101" },
                ['Y'] = new[] { "101", "101", "101", "010", "010", "010", "010" },
                ['Z'] = new[] { "111", "001", "001", "010", "100", "100", "111" },
            };
            return g;
        }
    }
}