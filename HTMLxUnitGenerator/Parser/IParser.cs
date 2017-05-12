using HTMLxUnitGenerator.Model;

namespace HTMLxUnitGenerator.Parser
{
    public interface IParser
    {
        Report Parse(string filePath);
    }
}
