using System;
using System.Collections.Generic;

namespace CanteenProject
{
    public class User
    {
        public int UserID { get; set; }
        public string PermanentID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public string InviteCode { get; set; }
        public string InvitedByAdminCode { get; set; }
        public string ProfilePictureUrl { get; set; }
    }

    public class ExtensionRequest
    {
        public int ExtensionID { get; set; }
        public int BorrowID { get; set; }
        public string StudentEmail { get; set; }
        public string StudentName { get; set; }
        public string EquipmentName { get; set; }
        public string CurrentDueDate { get; set; }
        public string RequestedNewDate { get; set; }
        public string RequestDate { get; set; }
        public string Status { get; set; } // Pending, Approved, Denied
        public int DaysRequested { get; set; } // Number of days requested (1-7)
    }

    public class Equipment
    {
        public int EquipmentID { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
    }

    public class BorrowRequest
    {
        public int RequestID { get; set; }
        public string StudentEmail { get; set; }
        public string StudentName { get; set; }
        public int EquipmentID { get; set; }
        public string EquipmentName { get; set; }
        public string RequestDate { get; set; }
        public string Status { get; set; }
    }

    public class BorrowRecord
    {
        public int BorrowID { get; set; }
        public string StudentEmail { get; set; }
        public string StudentName { get; set; }
        public int EquipmentID { get; set; }
        public string EquipmentName { get; set; }
        public string BorrowDate { get; set; }
        public string DueDate { get; set; }
        public string ReturnDate { get; set; }
        public string Status { get; set; }
    }

    public class ActivityLog
    {
        public int LogID { get; set; }
        public string Timestamp { get; set; }
        public string ActorEmail { get; set; }
        public string ActorName { get; set; }
        public string ActorRole { get; set; }
        public string Action { get; set; }
        public string Details { get; set; }
    }
}