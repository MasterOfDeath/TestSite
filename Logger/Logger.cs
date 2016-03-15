namespace TestSite.Logger
{
    using log4net;
    using log4net.Config;

    public static class Logger
    {
        private static readonly ILog _Log = LogManager.GetLogger(typeof(Logger));

        static Logger()
        {
            InitLogger();
        }

        public static ILog Log
        {
            get { return _Log; }
        }

        public static void InitLogger()
        {
            XmlConfigurator.Configure();
        }
    }
}
