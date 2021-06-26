using System;
using System.Collections.Generic;

namespace DoberDogBot.UI.Models
{
    public class DonationBaseModel
    {
        public List<string> TotalDonation { get; set; }
        public List<DonationModel> Donations { get; set; }
    }

    public class DonationModel
    {
        public string DonorName { get; set; }
        public string DonationType { get; set; }
        public string DonationAmount { get; set; }
        public string DonationAmountDetail { get; set; }
        public DateTime DonationDate { get; set; }
    }
}
