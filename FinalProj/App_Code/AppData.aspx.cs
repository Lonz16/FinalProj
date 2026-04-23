using System.Collections.Generic;

namespace CanteenProject
{
    public static class AppData
    {
        public static List<User> Users = new List<User>();
        public static List<Equipment> EquipmentList = new List<Equipment>();
        public static List<BorrowRequest> BorrowRequests = new List<BorrowRequest>();
        public static List<BorrowRecord> BorrowRecords = new List<BorrowRecord>();
        public static List<ActivityLog> ActivityLogs = new List<ActivityLog>();

        public static int NextUserID = 1001;
        public static int NextEquipID = 1;
        public static int NextRequestID = 1;
        public static int NextBorrowID = 1;

        // Seed flag to prevent multiple seeding
        public static bool IsSeeded = false;
    }
}