using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace PDCoreNew.Helpers.IO
{
    public static class Cli
    {
        public static Task<(string output, string errorMsg)> ExecuteCommand(string command, string workingDirectory = null)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return Bash.ExecuteCommand(command, workingDirectory);
            }
            else
            {
                return Cmd.ExecuteCommand(command, workingDirectory);
            }
        }
    }
}
