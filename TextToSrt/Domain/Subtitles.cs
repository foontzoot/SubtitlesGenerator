using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SubtitlesConverter.Domain.TextProcessing;

namespace SubtitlesConverter.Domain
{
    class Subtitles
    {
        private IEnumerable<SubtitleLine> Lines { get; }

        public Subtitles(IEnumerable<SubtitleLine> lines)
        {
            Lines = lines.ToList();
        }

        public static Subtitles Parse(string[] text, TimeSpan clipDuration)
        {
            ITextProcessor cleanup = new LinesTrimmer();
            ITextProcessor intoSentences = new SentencesBreaker();
            // ITextProcessor intoShortLines = new LinesBreaker(95, 45);
            ITextProcessor intoShortLines = new DoNothing();

            IEnumerable<string> lines = cleanup.Execute(text);
            lines = intoSentences.Execute(lines);
            lines = intoShortLines.Execute(lines).ToList();

            TextDurationMeter durationMeter = new TextDurationMeter(lines, clipDuration);
            IEnumerable<SubtitleLine> subtitles = lines
                .Select(line => (text: line, duration: durationMeter.EstimateDuration(line)))
                .Select(tuple => new SubtitleLine(tuple.text, tuple.duration));
            return new Subtitles(subtitles);
        }

        public void SaveAsSrt(FileInfo destination) =>
            File.WriteAllLines(destination.FullName, GenerateSrtFileContent(), Encoding.UTF8);

        private IEnumerable<string> GenerateSrtFileContent() =>
            GenerateLineBoundaries()
                .SelectMany((tuple, index) =>
                    new[]
                    {
                        $"{index + 1}",
                        $"{tuple.begin:hh\\:mm\\:ss\\,fff} --> {tuple.end:hh\\:mm\\:ss\\,fff}",
                        $"{tuple.content}",
                        string.Empty
                    });

        private IEnumerable<(TimeSpan begin, TimeSpan end, string content)> GenerateLineBoundaries()
        {
            TimeSpan begin = new TimeSpan(0);
            foreach (SubtitleLine line in Lines)
            {
                TimeSpan end = begin + line.Duration;
                yield return (begin, end, line.Content);
                begin = end;
            }
        }
    }
}
