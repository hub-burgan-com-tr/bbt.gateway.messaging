

using Newtonsoft.Json;

namespace bbt.gateway.common.Models.v2
{
    public class Notification
    {
        public string notificationId { get; set; }
        public string reminderType { get; set; }
        [JsonProperty("contentHTML")]
        public string contentHtml { get; set; }
        public bool isRead { get; set; }
        public string date { get; set; }
        
    }

    public class NotificationSortByYear : Comparer<Notification>
    {
        public Dictionary<string, int> monthMap = new Dictionary<string, int>()
        {
            {"ocak",1},
            {"subat",2},
            {"şubat",2},
            {"mart",3},
            {"nisan",4},
            {"nısan",4},
            {"mayis",5},
            {"mayıs",5},
            {"haziran",6},
            {"hazıran",6},
            {"temmuz",7},
            {"agustos",8},
            {"ağustos",8},
            {"eylül",9},
            {"eylul",9},
            {"ekim",10},
            {"ekım",10},
            {"kasim",11},
            {"kasım",11},
            {"aralik",12},
            {"aralık",12},
            {"january",1},
            {"february",2},
            {"march",3},
            {"april",4},
            {"aprıl",4},
            {"may",5},
            {"june",6},
            {"july",7},
            {"august",8},
            {"september",9},
            {"october",10},
            {"november",11},
            {"december",12}
        };
        public override int Compare(Notification x, Notification y)
        {
            var firstDateSplitted = x.date.Split(" ");
            var secondDateSplitted = y.date.Split(" ");

            var yearResult = CompareYears(Convert.ToInt32(firstDateSplitted[2]), Convert.ToInt32(secondDateSplitted[2]));
            if (yearResult != 0)
                return yearResult;

            var monthResult = CompareMonths(firstDateSplitted[1].ToLower(), secondDateSplitted[1].ToLower());
            if (monthResult != 0)
                return monthResult;

            return CompareDays(Convert.ToInt32(firstDateSplitted[0]), Convert.ToInt32(secondDateSplitted[0]));
        }

        private int CompareYears(int xYear, int yYear)
        {
            if (xYear > yYear)
                return -1;
            else if (xYear < yYear)
                return 1;
            else
                return 0;
        }

        private int CompareMonths(string xMonth, string yMonth)
        {
            if (monthMap[xMonth] > monthMap[yMonth])
                return -1;
            else if (monthMap[xMonth] < monthMap[yMonth])
                return 1;
            else
                return 0;
        }

        private int CompareDays(int xDay, int yDay)
        {
            if (xDay > yDay)
                return -1;
            else if (xDay < yDay)
                return 1;
            else
                return 0;
        }
    }
}
