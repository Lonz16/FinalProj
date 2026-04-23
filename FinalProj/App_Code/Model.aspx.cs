using System;
using System.Collections.Generic;

namespace CanteenProject
{
    // In App_Code/Models.cs - Add ProfilePicture property to User class

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

        // NEW: Profile picture property
        public string ProfilePictureUrl { get; set; }  // Stores the image path
    }

    public class Equipment
    {
        public int EquipmentID { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
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