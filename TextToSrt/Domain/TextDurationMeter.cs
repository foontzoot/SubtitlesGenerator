using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SubtitlesConverter.Domain
{
    internal class TextDurationMeter
    {
        private double FullTextLength { get; }
        private TimeSpan FullTextDuration { get; }

        internal TextDurationMeter(IEnumerable<string> fullText, TimeSpan duration)
        {
            FullTextLength = fullText.Sum(CountReadableLetters);
            FullTextDuration = duration;
        }

        public TimeSpan EstimateDuration(string text) =>
            TimeSpan.FromMilliseconds(EstimateMilliseconds(text));

        private double EstimateMilliseconds(string text) =>
            FullTextDuration.TotalMilliseconds * GetRelativeLength(text);

        private double GetRelativeLength(string text) =>
            CountReadableLetters(text) / FullTextLength;

        private int CountReadableLetters(string text) =>
            Regex.Matches(text, @"\w+")
                .Sum(match => match.Value.Length);
    }
}
