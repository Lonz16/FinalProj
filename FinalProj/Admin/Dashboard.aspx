<%@ Page Title="Admin Dashboard" Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="CanteenProject.Admin.Dashboard" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>BorrowBox Pro - Admin Dashboard</title>
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
        .container { max-width: 1400px; margin: 0 auto; position: relative; }
        .header { text-align: center; margin-bottom: 2rem; }
        .header h1 {
            font-size: 2.8rem;
            background: linear-gradient(135deg, #a5f3fc, #c084fc, #f0abfc);
            -webkit-background-clip: text;
            background-clip: text;
            color: transparent;
        }
        
        /* Profile Picture Styles */
        .header-left {
            display: flex;
            align-items: center;
            gap: 1rem;
        }
        .large-avatar {
            width:170px;
            height: 170px;
            border-radius: 12px;
            object-fit: cover;
            border: 2px solid #4f46e5;
            cursor: pointer;
        }
        .profile-popup {
            display: none;
            position: fixed;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            background: #1e293b;
            padding: 1.5rem;
            border-radius: 1rem;
            z-index: 10000;
            width: 300px;
            border: 1px solid #475569;
            box-shadow: 0 25px 50px rgba(0,0,0,0.5);
        }
        .profile-popup-image {
            width: 100px;
            height: 100px;
            border-radius: 16px;
            object-fit: cover;
            border: 2px solid #4f46e5;
            margin-bottom: 0.5rem;
        }
        .file-upload-input {
            width: 100%;
            padding: 0.3rem;
            margin: 0.5rem 0;
            background: #0f172a;
            border: 1px solid #334155;
            border-radius: 0.5rem;
            color: white;
        }
        .upload-btn-small {
            background: #4f46e5;
            border: none;
            padding: 0.3rem 0.8rem;
            border-radius: 2rem;
            color: white;
            cursor: pointer;
            font-size: 0.8rem;
        }
        .upload-btn-small:hover {
            background: #7c3aed;
        }
        
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
            gap: 0.8rem;
            align-items: center;
        }
        
        .logout-btn-professional {
            display: flex;
            align-items: center;
            gap: 0.5rem;
            background: linear-gradient(135deg, #dc2626, #b91c1c);
            border: none;
            padding: 0.5rem 1.2rem;
            border-radius: 2rem;
            color: white;
            cursor: pointer;
            font-size: 0.9rem;
            font-weight: 500;
            transition: all 0.3s ease;
            box-shadow: 0 2px 4px rgba(0,0,0,0.2);
        }
        .logout-btn-professional:hover {
            background: linear-gradient(135deg, #ef4444, #dc2626);
            transform: translateY(-1px);
            box-shadow: 0 4px 8px rgba(220, 38, 38, 0.3);
        }
        
        .modal-overlay {
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0, 0, 0, 0.7);
            backdrop-filter: blur(5px);
            z-index: 9999;
            justify-content: center;
            align-items: center;
        }
        .modal-overlay.show { display: flex; }
        .modal-container {
            background: linear-gradient(135deg, #1e293b, #0f172a);
            border-radius: 1.5rem;
            width: 400px;
            max-width: 90%;
            box-shadow: 0 25px 50px -12px rgba(0,0,0,0.5);
            border: 1px solid rgba(255,255,255,0.1);
            overflow: hidden;
        }
        .modal-header {
            padding: 1.5rem;
            text-align: center;
            border-bottom: 1px solid #334155;
        }
        .modal-header .warning-icon { font-size: 4rem; }
        .modal-header h3 {
            font-size: 1.5rem;
            color: #f87171;
            margin-top: 0.5rem;
        }
        .modal-body {
            padding: 1.5rem;
            text-align: center;
            color: #cbd5e1;
        }
        .modal-body .user-info {
            background: #0f172a;
            padding: 0.8rem;
            border-radius: 0.75rem;
            margin-top: 1rem;
        }
        .modal-footer {
            padding: 1rem 1.5rem 1.5rem;
            display: flex;
            gap: 1rem;
            justify-content: center;
        }
        .modal-btn {
            padding: 0.7rem 1.5rem;
            border: none;
            border-radius: 2rem;
            font-size: 0.9rem;
            font-weight: 500;
            cursor: pointer;
        }
        .modal-btn-confirm {
            background: linear-gradient(135deg, #dc2626, #b91c1c);
            color: white;
        }
        .modal-btn-cancel {
            background: #334155;
            color: #e2e8f0;
        }
        
        .notification-bell {
            position: relative;
            cursor: pointer;
            font-size: 1.5rem;
            background: #1e293b;
            border-radius: 50%;
            width: 42px;
            height: 42px;
            display: flex;
            align-items: center;
            justify-content: center;
        }
        .badge {
            position: absolute;
            top: -5px;
            right: -5px;
            background: #ef4444;
            color: white;
            border-radius: 50%;
            width: 20px;
            height: 20px;
            font-size: 0.65rem;
            display: flex;
            align-items: center;
            justify-content: center;
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
        .show { display: block; }
        
        .dashboard {
            background: rgba(10,20,35,0.7);
            backdrop-filter: blur(12px);
            border-radius: 1.8rem;
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
        
        .unified-search-container {
            background: #1e293b;
            border-radius: 3rem;
            padding: 0.2rem;
            margin-bottom: 2rem;
            display: flex;
            gap: 0.5rem;
            align-items: center;
            border: 1px solid #334155;
        }
        .unified-search-box {
            flex: 1;
            position: relative;
        }
        .unified-search-box input {
            width: 100%;
            padding: 1rem 3rem 1rem 1.5rem;
            background: transparent;
            border: none;
            color: #f1f5f9;
            font-size: 1rem;
            border-radius: 3rem;
        }
        .unified-search-box input:focus { outline: none; }
        .search-icon {
            position: absolute;
            right: 15px;
            top: 50%;
            transform: translateY(-50%);
            color: #94a3b8;
        }
        .search-clear {
            background: #475569;
            border: none;
            padding: 0.6rem 1.2rem;
            border-radius: 2rem;
            color: white;
            cursor: pointer;
            margin-right: 0.5rem;
        }
        .search-clear:hover { background: #ef4444; }
        .search-stats {
            font-size: 0.85rem;
            color: #94a3b8;
            margin-top: -1rem;
            margin-bottom: 1rem;
        }
        .highlight {
            background-color: rgba(167, 139, 250, 0.4);
            font-weight: bold;
            padding: 0 2px;
            border-radius: 3px;
        }
        
        table {
            width: 100%;
            border-collapse: collapse;
            background: #0f172a80;
            margin-top: 1rem;
        }
        th, td { padding: 0.8rem 1rem; text-align: left; border-bottom: 1px solid #1e293b; }
        th { background: #1e293b; }
        .btn {
            background: #1e293b;
            border: none;
            padding: 0.5rem 1rem;
            border-radius: 2rem;
            cursor: pointer;
        }
        .btn-primary {
            background: linear-gradient(95deg, #4f46e5, #7c3aed);
            color: white;
        }
        .nav-links {
            display: flex;
            gap: 1rem;
            margin-bottom: 1.5rem;
            flex-wrap: wrap;
        }
        .nav-link {
            color: #a78bfa;
            text-decoration: none;
            padding: 0.5rem 1rem;
            background: #1e293b;
            border-radius: 0.5rem;
        }
        .stats-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 1rem;
            margin-bottom: 2rem;
        }
        .stat-card {
            background: #1e293b;
            padding: 1rem;
            border-radius: 1rem;
            text-align: center;
        }
        .stat-number { font-size: 2rem; color: #a78bfa; }
        .invite-code-box {
            background: #0f172a;
            border: 2px solid #4f46e5;
            padding: 1rem;
            text-align: center;
            border-radius: 1rem;
            margin: 1rem 0;
        }
    </style>
    <script>
        function toggleAccountPopup() {
            var popup = document.getElementById('accountPopup');
            if (popup) popup.classList.toggle('show');
        }
        function toggleNotificationPopup() {
            var popup = document.getElementById('notificationPopup');
            if (popup) popup.classList.toggle('show');
        }
        function showLogoutModal() {
            document.getElementById('logoutModal').classList.add('show');
        }
        function hideLogoutModal() {
            document.getElementById('logoutModal').classList.remove('show');
        }
        function confirmLogout() {
            hideLogoutModal();
            __doPostBack('btnProfessionalLogout', '');
        }
        function showProfilePopup() {
            document.getElementById('profilePopup').style.display = 'block';
        }
        function closeProfilePopup() {
            document.getElementById('profilePopup').style.display = 'none';
        }
    </script>
</head>
<body>
<form id="form1" runat="server" enctype="multipart/form-data">
<div class="container">
    <div class="header">
        <h1>🔐 BorrowBox Pro</h1>
        <p>Admin Control Panel</p>
    </div>

    <!-- Logout Modal -->
    <div id="logoutModal" class="modal-overlay">
        <div class="modal-container">
            <div class="modal-header">
                <div class="warning-icon">⚠️</div>
                <h3>Confirm Logout</h3>
            </div>
            <div class="modal-body">
                <p>Are you sure you want to sign out?</p>
                <div class="user-info"><asp:Label ID="lblLogoutUserName" runat="server"></asp:Label></div>
            </div>
            <div class="modal-footer">
                <button type="button" class="modal-btn modal-btn-cancel" onclick="hideLogoutModal()">Cancel</button>
                <button type="button" class="modal-btn modal-btn-confirm" onclick="confirmLogout()">Yes, Sign Out</button>
            </div>
        </div>
    </div>

    <!-- Account Popup -->
    <div id="accountPopup" class="account-popup">
        <div style="text-align:center; margin-bottom:1rem;"></div>
        <p><strong>Permanent ID:</strong> <asp:Label ID="lblPopupPermanentID" runat="server"></asp:Label></p>
        <p><strong>Name:</strong> <asp:Label ID="lblPopupName" runat="server"></asp:Label></p>
        <p><strong>Email:</strong> <asp:Label ID="lblPopupEmail" runat="server"></asp:Label></p>
        <p><strong>Role:</strong> <asp:Label ID="lblPopupRole" runat="server"></asp:Label></p>
    </div>

    <!-- Notification Popup -->
    <div id="notificationPopup" class="notification-popup">
        <asp:Literal ID="litNotificationContent" runat="server"></asp:Literal>
    </div>

   <!-- Profile Picture Popup -->
<div id="profilePopup" class="profile-popup">
    <div style="text-align:center;">
        <asp:Image ID="imgProfile" runat="server" ImageUrl="~/Images/default-avatar.png" CssClass="profile-popup-image" />
        <asp:FileUpload ID="fileUpload" runat="server" CssClass="file-upload-input" />
        <asp:Button ID="btnUpload" runat="server" Text="Upload Picture" CssClass="upload-btn-small" OnClick="btnUpload_Click" />
        <asp:Label ID="lblUploadMsg" runat="server" ForeColor="#34d399" style="font-size:0.7rem;" />
        <br />
        <button type="button" class="upload-btn-small" style="background:#475569; margin-top:0.5rem;" onclick="closeProfilePopup()">Close</button>
    </div>
</div>
    <!-- Dashboard Content -->
    <div class="dashboard">
        <div class="dashboard-header">
            <div class="header-left">
                <asp:Image ID="imgLargeAvatar" runat="server" 
                    ImageUrl="~/Images/default-avatar.png" 
                    CssClass="large-avatar" 
                    onclick="showProfilePopup()" />
                <h2>Admin Panel - <asp:Label ID="lblAdminName" runat="server" style="color:#a78bfa"></asp:Label></h2>
            </div>
            <div class="right-icons">
                <div class="notification-bell" onclick="toggleNotificationPopup()">🔔<asp:Label ID="lblNotificationBadge" runat="server" CssClass="badge" Text="0"></asp:Label></div>
                <button type="button" class="logout-btn-professional" onclick="showLogoutModal()">🚪 Sign Out</button>
            </div>
        </div>

        <div class="nav-links">
            <a href="Dashboard.aspx" class="nav-link">Dashboard</a>
            <a href="ManageEquipment.aspx" class="nav-link">Equipment</a>
            <a href="ManageUsers.aspx" class="nav-link">Users</a>
            <a href="BorrowHistory.aspx" class="nav-link">History</a>
        </div>

        <div class="stats-grid">
            <div class="stat-card"><div class="stat-number"><asp:Label ID="lblTotalUsers" runat="server" Text="0"></asp:Label></div>Users</div>
            <div class="stat-card"><div class="stat-number"><asp:Label ID="lblTotalEquipment" runat="server" Text="0"></asp:Label></div>Equipment</div>
            <div class="stat-card"><div class="stat-number"><asp:Label ID="lblActiveBorrows" runat="server" Text="0"></asp:Label></div>Active Borrows</div>
        </div>

        <div class="invite-code-box">
            <asp:Label ID="lblAdminInviteCode" runat="server" Font-Size="X-Large" Font-Bold="true" />
        </div>

        <!-- Unified Search -->
        <div class="unified-search-container">
            <div class="unified-search-box">
                <asp:TextBox ID="txtUnifiedSearch" runat="server" placeholder="🔍 Search activity log by user, action, or details..." AutoPostBack="true" OnTextChanged="txtUnifiedSearch_TextChanged" />
                <span class="search-icon">🔍</span>
            </div>
            <asp:Button ID="btnClearSearch" runat="server" Text="Clear" CssClass="search-clear" OnClick="btnClearSearch_Click" />
        </div>
        <div class="search-stats"><asp:Label ID="lblSearchStats" runat="server"></asp:Label></div>

        <div class="section-title">📋 Activity Log</div>
        <asp:GridView ID="gvActivityLog" runat="server" AutoGenerateColumns="True" OnRowDataBound="gvActivityLog_RowDataBound">
            <EmptyDataTemplate>
                <tr><td colspan="3" style="text-align:center; padding:2rem;">No activity records found</td></tr>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>
</div>
<asp:Button ID="btnProfessionalLogout" runat="server" style="display:none;" OnClick="btnLogout_Click" />
</form>
</body>
</html>