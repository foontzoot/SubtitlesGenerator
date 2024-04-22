using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SubtitlesConverter.Common;

namespace SubtitlesConverter.Domain
{
    class SentencesBreaker : ITextProcessor
    {
        private IEnumerable<(string pattern, string extract, string remove)> Rules { get; } = new[]
        {
            (@"^(?<remove>(?<extract>(\.\.\.|[^\.])+)\.)$", "${extract}", "${remove}"),
            (@"^(?<remove>(?<extract>[^\.]+),)$", "${extract}", "${remove}"),
            (@"^(?<remove>(?<extract>(\.\.\.|[^\.])+)\.)[^\.].*$", "${extract}", "${remove}"),
            (@"^(?<remove>(?<extract>[^:]+):).*$", "${extract}", "${remove}"),
            (@"^(?<extract>.+\?).*$", "${extract}", "${extract}"),
            (@"^(?<extract>.+\!).*$", "${extract}", "${extract}"),
        };

        public IEnumerable<string> Execute(IEnumerable<string> text) =>
            text.SelectMany(BreakSentences);

        private IEnumerable<string> BreakSentences(string text)
        {
            string remaining = text.Trim();
            while (remaining.Length > 0)
            {
                (string extracted, string rest) =
                    FindShortestExtractionRule(Rules, remaining)
                        .Select(tuple => (
                            tuple.extracted,
                            removedLength: tuple.remove.Length))
                        .Select(tuple => (
                            tuple.extracted,
                            remaining: remaining.Substring(tuple.removedLength).Trim()))
                        .DefaultIfEmpty((extracted: remaining, remaining: string.Empty))
                        .First();

                yield return extracted;
                remaining = rest;
            }
        }

        private IEnumerable<(string extracted, string remove)> FindShortestExtractionRule(
            IEnumerable<(string pattern, string extractPattern, string removePattern)> rules,
            string text) =>
            rules
                .Select(rule => (
                    pattern: new Regex(rule.pattern),
                    rule.extractPattern,
                    rule.removePattern))
                .Select(rule => (
                    rule.pattern,
                    match: rule.pattern.Match(text),
                    rule.extractPattern,
                    rule.removePattern))
                .Where(rule => rule.match.Success)
                .Select(rule => (
                    extracted: rule.pattern.Replace(text, rule.extractPattern),
                    remove: rule.pattern.Replace(text, rule.removePattern)))
                .WithMinimumOrEmpty(tuple => tuple.remove.Length);
    }
}
