using Naninovel.Parsing;

namespace Naninovel
{
    public class LabelLineParser : ScriptLineParser<LabelScriptLine, LabelLine>
    {
        protected override LabelScriptLine Parse (LabelLine lineModel)
        {
            return new LabelScriptLine(lineModel.Label.Text, LineIndex, LineHash);
        }
    }
}
