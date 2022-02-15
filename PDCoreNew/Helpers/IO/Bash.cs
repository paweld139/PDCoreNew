using PDCoreNew.Utils;
using System.Threading.Tasks;

namespace PDCoreNew.Helpers.IO
{
    public static class Bash
    {
        public static Task<(string output, string errorMsg)> ExecuteCommand(string command, string workingDirectory = null)
        {
            return IOUtils.ExecuteBashCommand(command, workingDirectory);
        }
    }
}
