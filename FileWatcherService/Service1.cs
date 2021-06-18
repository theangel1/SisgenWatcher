using FileWatchingService;
using System.ServiceProcess;

namespace FileWatcherService
{
    public partial class Service1 : ServiceBase
    {
        
        public Service1()
        {           
                InitializeComponent();           
        }

        public void onDebug()
        {
            OnStart(null);
        }
        protected override void OnStart(string[] args)
        {
            FileWatcher f = new FileWatcher();
        }

        protected override void OnStop()
        {
        }
      
    }
}
