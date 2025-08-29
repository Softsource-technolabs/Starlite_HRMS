using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace StarLine.Core.Common
{
    public static class AvtarService
    {
        private static readonly Random _random = new Random();
        public static string GenerateAvatar(string name,string filePath,string userid, int size = 128)
        {
            string initials = GetInitials(name);
            string safeFileName = userid + ".png";
            filePath = Path.Combine(filePath, safeFileName);

            // If already exists, no need to recreate
            if (File.Exists(filePath))
                File.Delete(filePath);

            using var bmp = new Bitmap(size, size);
            bmp.MakeTransparent(); // Ensure transparency
            using var graphics = Graphics.FromImage(bmp);

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.Clear(Color.Transparent); // Transparent background

            // Random background color
            Color backgroundColor = GetRandomColor();

            // Random font color (different from background for visibility)
            Color fontColor;
            do
            {
                fontColor = GetRandomColor();
            } while (AreColorsTooSimilar(backgroundColor, fontColor));

            // Draw background circle
            using (var brush = new SolidBrush(backgroundColor))
            {
                graphics.FillEllipse(brush, 0, 0, size, size);
            }

            // Draw initials
            using (var font = new Font("Arial", size / 2, FontStyle.Bold, GraphicsUnit.Pixel))
            using (var textBrush = new SolidBrush(fontColor))
            {
                var textSize = graphics.MeasureString(initials, font);
                graphics.DrawString(initials, font, textBrush,
                    (size - textSize.Width) / 2,
                    (size - textSize.Height) / 2);
            }

            // Save PNG with transparency
            bmp.Save(filePath, ImageFormat.Png);
            return safeFileName;
        }

        private static Color GetRandomColor()
        {
            return Color.FromArgb(255, _random.Next(0, 256), _random.Next(0, 256), _random.Next(0, 256));
        }

        private static bool AreColorsTooSimilar(Color c1, Color c2)
        {
            int diff = Math.Abs(c1.R - c2.R) + Math.Abs(c1.G - c2.G) + Math.Abs(c1.B - c2.B);
            return diff < 100; // Lower = more strict difference
        }

        private static string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "?";

            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
                return parts[0][0].ToString().ToUpper();

            return $"{parts[0][0]}{parts[1][0]}".ToUpper();
        }
    }
}
