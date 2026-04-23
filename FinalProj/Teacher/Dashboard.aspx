<%@ Page Title="Teacher Dashboard" Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="CanteenProject.Teacher.Dashboard" %>
<%@ Register Src="~/Shared/ProfilePicture.ascx" TagName="ProfilePicture" TagPrefix="uc" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>BorrowBox Pro - Teacher Dashboard</title>
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body {
            font-family: 'Inter', 'Segoe UI', system-ui, sans-serif;
            background: radial-gradient(circle at 10% 20%, #0a0f1e, #0a0a1a);
            color: #eef2ff;
            line-height: 1.5;
            min-height: 100vh;
            padding: 2rem 1rem;
        }
        .container { max-width: 1300px; margin: 0 auto; position: relative; }
        .header { text-align: center; margin-bottom: 2rem; }
        .header h1 {
            font-size: 2.8rem;
            background: linear-gradient(135deg, #a5f3fc, #c084fc, #f0abfc);
            -webkit-background-clip: text;
            background-clip: text;
            color: transparent;
        }
        .header p { color: #94a3b8; }
        .dashboard-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 1.5rem;
            padding-bottom: 0.5rem;
            border-bottom: 1px solid #334155;
        }
        .right-icons {
            display: flex;
            gap: 1rem;
            align-items: center;
        }
        .notification-bell {
            position: relative;
            cursor: pointer;
            font-size: 1.8rem;
            background: #1e293b;
            border-radius: 50%;
            width: 48px;
            height: 48px;
            display: flex;
            align-items: center;
            justify-content: center;
            transition: 0.2s;
        }
        .notification-bell:hover { background: #334155; }
        .badge {
            position: absolute;
            top: -5px;
            right: -5px;
            background: #ef4444;
            color: white;
            border-radius: 50%;
            width: 22px;
            height: 22px;
            font-size: 0.7rem;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: bold;
        }
        .avatar {
            background: #4f46e5;
            border-radius: 50%;
            width: 48px;
            height: 48px;
            display: flex;
            align-items: center;
            justify-content: center;
            cursor: pointer;
            font-size: 1.5rem;
            font-weight: bold;
            transition: transform 0.2s, box-shadow 0.2s;
        }
        .avatar:hover {
            transform: scale(1.05);
            box-shadow: 0 0 12px rgba(79, 70, 229, 0.6);
        }
        .account-popup, .notification-popup {
            position: absolute;
            top: 70px;
            right: 20px;
            background: #1e293b;
            border-radius: 1rem;
            padding: 1.2rem;
            width: 320px;
            box-shadow: 0 10px 25px rgba(0,0,0,0.5);
            border: 1px solid #475569;
            z-index: 1000;
            display: none;
        }
        .notification-popup {
            right: 80px;
            width: 350px;
            max-height: 400px;
            overflow-y: auto;
        }
        .show { display: block; }
        .notification-popup h4 {
            margin-bottom: 0.8rem;
            color: #a78bfa;
        }
        .notification-item {
            padding: 0.5rem 0;
            border-bottom: 1px solid #334155;
            font-size: 0.85rem;
        }
        .notification-item .due-soon { color: #f59e0b; }
        .notification-item .overdue {
            color: #ef4444;
            font-weight: bold;
        }
        .no-notifications {
            color: #94a3b8;
            text-align: center;
            padding: 1rem;
        }
        .account-popup p { margin: 0.6rem 0; font-size: 0.9rem; word-break: break-word; }
        .account-popup strong { color: #a78bfa; }
        .account-popup .logout-btn {
            width: 100%;
            margin-top: 1rem;
            background: #ef4444;
            border: none;
            padding: 0.5rem;
            border-radius: 2rem;
            color: white;
            cursor: pointer;
        }
        .dashboard {
            background: rgba(10,20,35,0.7);
            backdrop-filter: blur(12px);
            border-radius: 1.8rem;
            border: 1px solid rgba(255,255,255,0.08);
            padding: 1.8rem;
            margin-top: 1rem;
        }
        .section-title {
            font-size: 1.3rem;
            font-weight: 600;
            margin-bottom: 1rem;
            border-bottom: 2px solid #334155;
            color: #a78bfa;
            display: inline-block;
        }
        table {
            width: 100%;
            border-collapse: collapse;
            background: #0f172a80;
            border-radius: 1rem;
            overflow: hidden;
            margin-top: 1rem;
        }
        th { text-align: left; padding: 0.9rem 1rem; background: #1e293b; color: #cbd5e1; }
        td { padding: 0.8rem 1rem; border-bottom: 1px solid #1e293b; }
        .btn {
            background: #1e293b;
            border: none;
            padding: 0.5rem 1rem;
            border-radius: 2rem;
            font-weight: 500;
            color: #e2e8f0;
            cursor: pointer;
            transition: 0.2s;
            display: inline-block;
            text-align: center;
            margin: 0 0.25rem;
        }
        .btn-success { background: #10b981; color: white; }
        .btn-danger { background: #ef4444; color: white; }
        .btn-warning { background: #f59e0b; color: white; }
        .btn-outline {
            background: transparent;
            border: 1px solid #475569;
        }
        .btn-outline:hover { background: #334155; }
        .msg-success { color: #34d399; margin-top: 0.5rem; display: block; }
        .msg-error { color: #f87171; margin-top: 0.5rem; display: block; }
        .section { margin-bottom: 2rem; }
        .nav-links {
            display: flex;
            gap: 1rem;
            margin-bottom: 1rem;
            flex-wrap: wrap;
        }
        .nav-link {
            color: #a78bfa;
            text-decoration: none;
            padding: 0.5rem 1rem;
            background: #1e293b;
            border-radius: 0.5rem;
            transition: 0.2s;
        }
        .nav-link:hover {
            background: #334155;
        }
    </style>
    <script>
        function toggleAccountPopup() {
            var popup = document.getElementById('accountPopup');
            if (popup) popup.classList.toggle('show');
            var notifPopup = document.getElementById('notificationPopup');
            if (notifPopup) notifPopup.classList.remove('show');
        }
        function toggleNotificationPopup() {
            var popup = document.getElementById('notificationPopup');
            if (popup) popup.classList.toggle('show');
            var accPopup = document.getElementById('accountPopup');
            if (accPopup) accPopup.classList.remove('show');
        }
        document.addEventListener('click', function (event) {
            var accPopup = document.getElementById('accountPopup');
            var notifPopup = document.getElementById('notificationPopup');
            var avatar = document.querySelector('.avatar');
            var bell = document.querySelector('.notification-bell');
            if (accPopup && avatar && !avatar.contains(event.target) && !accPopup.contains(event.target)) {
                accPopup.classList.remove('show');
            }
            if (notifPopup && bell && !bell.contains(event.target) && !notifPopup.contains(event.target)) {
                notifPopup.classList.remove('show');
            }
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="header">
                <h1>🔐 BorrowBox Pro</h1>
                <p>Teacher Management Console</p>
            </div>

            <!-- Account Popup -->
            <div id="accountPopup" class="account-popup">
                <p><strong>🔑 Permanent ID:</strong> <asp:Label ID="lblPopupPermanentID" runat="server"></asp:Label></p>
                <p><strong>👤 Name:</strong> <asp:Label ID="lblPopupName" runat="server"></asp:Label></p>
                <p><strong>📧 Email:</strong> <asp:Label ID="lblPopupEmail" runat="server"></asp:Label></p>
                <p><strong>🎭 Role:</strong> <asp:Label ID="lblPopupRole" runat="server"></asp:Label></p>
                <asp:Button ID="btnPopupLogout" runat="server" Text="Logout" CssClass="logout-btn" OnClick="btnLogout_Click" />
            </div>

            <!-- Notification Popup -->
            <div id="notificationPopup" class="notification-popup">
                <asp:Literal ID="litNotificationContent" runat="server"></asp:Literal>
            </div>

            <!-- Dashboard Content -->
            <div class="dashboard">
                <div class="dashboard-header">
                    <h2>Teacher Console - <asp:Label ID="lblTeacherName" runat="server" style="color:#a78bfa"></asp:Label></h2>
                    <div class="right-icons">
                        <div class="notification-bell" onclick="toggleNotificationPopup()">
                            🔔
                            <asp:Label ID="lblNotificationBadge" runat="server" CssClass="badge" Text="0"></asp:Label>
                        </div>
                        <uc:ProfilePicture ID="profilePicture1" runat="server" UserEmail='<%# Session["LoggedInUser"].ToString() %>' />
                    </div>
                </div>
                
                <div class="nav-links">
                    <a href="Dashboard.aspx" class="nav-link">📊 Dashboard</a>
                    <a href="PendingRequests.aspx" class="nav-link">⏳ Pending Requests</a>
                    <a href="ActiveBorrows.aspx" class="nav-link">🔄 Active Borrows</a>
                </div>
                
                <div class="section">
                    <div class="section-title">⏳ Pending Requests (<asp:Label ID="lblPendingCount" runat="server" Text="0"></asp:Label>)</div>
                    <asp:GridView ID="gvPendingRequests" runat="server" AutoGenerateColumns="False" OnRowCommand="gvPendingRequests_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="StudentName" HeaderText="Student" />
                            <asp:BoundField DataField="EquipmentName" HeaderText="Equipment" />
                            <asp:BoundField DataField="RequestDate" HeaderText="Requested" />
                            <asp:TemplateField HeaderText="Actions">
                                <ItemTemplate>
                                    <asp:Button ID="btnApprove" runat="server" Text="Approve" CommandName="Approve" 
                                        CommandArgument='<%# Eval("RequestID") %>' CssClass="btn btn-success" />
                                    <asp:Button ID="btnDeny" runat="server" Text="Deny" CommandName="Deny" 
                                        CommandArgument='<%# Eval("RequestID") %>' CssClass="btn btn-danger" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <tr><td colspan="4" style="text-align:center; padding:2rem;">✨ No pending requests</td></tr>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
                
                <div class="section">
                    <div class="section-title">🔄 Recent Active Borrows</div>
                    <asp:GridView ID="gvActiveBorrows" runat="server" AutoGenerateColumns="False" OnRowCommand="gvActiveBorrows_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="StudentName" HeaderText="Student" />
                            <asp:BoundField DataField="EquipmentName" HeaderText="Equipment" />
                            <asp:BoundField DataField="BorrowDate" HeaderText="Borrow Date" />
                            <asp:BoundField DataField="DueDate" HeaderText="Due Date" />
                            <asp:ButtonField Text="Mark Returned" CommandName="MarkReturned" ButtonType="Button" ControlStyle-CssClass="btn btn-warning" />
                        </Columns>
                        <EmptyDataTemplate>
                            <tr><td colspan="5" style="text-align:center; padding:2rem;">📭 No active borrows</td></tr>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
                
                <asp:Label ID="lblTeacherMessage" runat="server" Visible="false" />
            </div>
        </div>
    </form>
</body>
</html>