using System;
using System.Collections.Generic;

namespace SubtitlesConverter.Domain.TextProcessing
{
    public class DoNothing : ITextProcessor
    {
        public IEnumerable<string> Execute(IEnumerable<string> text) => text;
    }
}
