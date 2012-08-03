using System.Diagnostics;
using System.Management;

namespace Karpach.DebugAttachManager
{
    public class ProcessExt
    {
        private readonly Process _process;
        private string _title;

        public ProcessExt(Process process)
        {
            _process = process;
        }

        public string ProcessName
        {
            get { return _process.ProcessName; }
        }

        public string Title
        {
            get { return _title ?? (_title = GetAppPoolName()); }
        }

        public int Hash
        {
            get { return string.Concat(_process.ProcessName, Title).GetHashCode(); }
        }

        private string GetAppPoolName()
        {
            if (string.Compare(_process.ProcessName, "w3wp", true) == 0)
            {                            
                ObjectQuery sq = new ObjectQuery("Select CommandLine from Win32_Process Where ProcessID = '" + _process.Id + "'");
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(sq))
                {
                    ManagementObjectCollection objectCollection = searcher.Get();
                    foreach (ManagementObject oReturn in objectCollection)
                    {
                        var startIndex = oReturn["CommandLine"].ToString().IndexOf("-ap ") + 5; //remove the -ap as well as the space and the "
                        var endIndex = oReturn["CommandLine"].ToString().IndexOf("-", startIndex) - 2; //remove the closing "                        
                        return oReturn["CommandLine"].ToString().Substring(startIndex, endIndex - startIndex);                    
                    }                
                }
            }
            return _process.MainWindowTitle;
        }

    }
}