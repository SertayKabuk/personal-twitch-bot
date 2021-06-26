using StreamlabsLib.Models;
using System;

namespace StreamlabsLib.StreamlabsEvents
{
    public class DonationEventArgs : EventArgs
    {
        public DonationModel Donation { get; }

        public DonationEventArgs(DonationModel donationModel)
        {
            Donation = donationModel;
        }
    }
}
