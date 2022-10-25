
using System;

namespace NextLabs.RightsManager.Wizard
{
    public class InstallException : ApplicationException
    {
        public InstallException(string message)
          : base(message)
        {
        }

        public InstallException(string message, Exception inner)
          : base(message, inner)
        {
        }
    }
}
