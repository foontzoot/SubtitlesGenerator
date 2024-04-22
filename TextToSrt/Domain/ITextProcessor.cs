using System;
using System.Collections.Generic;

namespace SubtitlesConverter
{
    public interface ITextProcessor
    {
        IEnumerable<string> Execute(IEnumerable<string> text);
    }
}
