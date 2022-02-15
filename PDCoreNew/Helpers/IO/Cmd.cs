using PDCoreNew.Utils;
using System.Threading.Tasks;

namespace PDCoreNew.Helpers.IO
{
    public static class Cmd
    {
        public static Task<(string output, string errorMsg)> ExecuteCommand(string command, string workingDirectory = null)
        {
            return IOUtils.ExecuteCmdCommand(command, workingDirectory);
        }
    }
}
