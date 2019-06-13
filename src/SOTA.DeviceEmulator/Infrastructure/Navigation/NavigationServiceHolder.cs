using System;
using Caliburn.Micro;

namespace SOTA.DeviceEmulator.Framework
{
    public class NavigationServiceHolder
    {
        private INavigationService _navigationService;

        public INavigationService NavigationService
        {
            get
            {
                if (_navigationService == null)
                {
                    throw new InvalidOperationException("Navigation service has not been assigned yet.");
                }

                return _navigationService;
            }
            set => _navigationService = value;
        }
    }
}