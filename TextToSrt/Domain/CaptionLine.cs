using System;

namespace SubtitlesConverter.Domain
{
    class CaptionLine
    {
        public string Content { get; }
        public TimeSpan Duration { get; }

        public CaptionLine(string content, TimeSpan duration)
        {
            Content = content.Trim();
            Duration = duration;
        }

        public override string ToString() =>
            $"{Duration} --> {Content}";
    }
}
