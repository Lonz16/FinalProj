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
        }
        .logout-btn-professional:hover {
            background: linear-gradient(135deg, #ef4444, #dc2626);
            transform: translateY(-1px);
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
            width: 20px;
            height: 20px;
            font-size: 0.65rem;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: bold;
        }
        
        .avatar-container {
            position: relative;
            cursor: pointer;
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
            right: 100px;
            width: 350px;
            max-height: 400px;
            overflow-y: auto;
        }
        .show { display: block; }
        
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
        
        /* Search Bar Styles */
        .search-container {
            margin: 1rem 0 1rem 0;
            display: flex;
            gap: 0.5rem;
            flex-wrap: wrap;
            align-items: center;
        }
        .search-box {
            flex: 1;
            position: relative;
        }
        .search-box input {
            width: 100%;
            padding: 0.8rem 2.5rem 0.8rem 1rem;
            background: #0f172a;
            border: 1px solid #334155;
            border-radius: 2rem;
            color: #f1f5f9;
            font-size: 0.9rem;
        }
        .search-box input:focus {
            outline: none;
            border-color: #818cf8;
            box-shadow: 0 0 0 2px rgba(129,140,248,0.3);
        }
        .search-icon {
            position: absolute;
            right: 12px;
            top: 50%;
            transform: translateY(-50%);
            color: #94a3b8;
            font-size: 1.2rem;
        }
        .filter-buttons {
            display: flex;
            gap: 0.5rem;
            flex-wrap: wrap;
        }
        .filter-btn {
            background: #1e293b;
            border: none;
            padding: 0.6rem 1.2rem;
            border-radius: 2rem;
            color: #cbd5e1;
            cursor: pointer;
            transition: 0.2s;
            font-size: 0.85rem;
        }
        .filter-btn:hover {
            background: #334155;
        }
        .filter-btn.active {
            background: linear-gradient(95deg, #4f46e5, #7c3aed);
            color: white;
        }
        .clear-search {
            background: #475569;
            border: none;
            padding: 0.6rem 1.2rem;
            border-radius: 2rem;
            color: white;
            cursor: pointer;
            font-size: 0.85rem;
        }
        .clear-search:hover {
            background: #ef4444;
        }
        .search-results-count {
            font-size: 0.85rem;
            color: #94a3b8;
            margin-bottom: 0.5rem;
        }
        
        table {
            width: 100%;
            border-collapse: collapse;
            background: #0f172a80;
            border-radius: 1rem;
            overflow: hidden;
            margin-top: 0.5rem;
            margin-bottom: 1.5rem;
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
            margin: 0 0.25rem;
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
            padding-bottom: 1rem;
            border-bottom: 1px solid #334155;
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
        .invite-code-box {
            background: #0f172a;
            border: 2px solid #4f46e5;
            border-radius: 1rem;
            padding: 1rem;
            text-align: center;
            margin: 1rem 0;
        }
        .invite-code-box span {
            font-size: 2rem;
            font-weight: bold;
            color: #a78bfa;
            letter-spacing: 2px;
        }
        .stats-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 1rem;
            margin-bottom: 2rem;
        }
        .stat-card {
            background: #1e293b;
            border-radius: 1rem;
            padding: 1rem;
            text-align: center;
        }
        .stat-number {
            font-size: 2rem;
            font-weight: bold;
            color: #a78bfa;
        }
        .stat-label {
            font-size: 0.9rem;
            color: #94a3b8;
        }
        
        /* Modal Styles */
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
        .modal-overlay.show {
            display: flex;
        }
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
        .modal-header .warning-icon {
            font-size: 4rem;
        }
        .modal-header h3 {
            font-size: 1.5rem;
            color: #f87171;
        }
        .modal-body {
            padding: 1.5rem;
            text-align: center;
            color: #cbd5e1;
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
        function showLogoutModal() {
            var modal = document.getElementById('logoutModal');
            if (modal) modal.classList.add('show');
        }
        function hideLogoutModal() {
            var modal = document.getElementById('logoutModal');
            if (modal) modal.classList.remove('show');
        }
        function confirmLogout() {
            hideLogoutModal();
            __doPostBack('btnProfessionalLogout', '');
        }
        document.addEventListener('click', function (event) {
            var accPopup = document.getElementById('accountPopup');
            var notifPopup = document.getElementById('notificationPopup');
            var avatar = document.querySelector('.avatar-container');
            var bell = document.querySelector('.notification-bell');
            var modal = document.getElementById('logoutModal');
            if (accPopup && avatar && !avatar.contains(event.target) && !accPopup.contains(event.target)) {
                accPopup.classList.remove('show');
            }
            if (notifPopup && bell && !bell.contains(event.target) && !notifPopup.contains(event.target)) {
                notifPopup.classList.remove('show');
            }
            if (modal && modal.classList.contains('show') && event.target === modal) {
                hideLogoutModal();
            }
        });
        document.addEventListener('keydown', function (event) {
            if (event.key === 'Escape') hideLogoutModal();
        });
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
                <p>You will need to log in again to access your account.</p>
            </div>
            <div class="modal-footer">
                <button type="button" class="modal-btn modal-btn-cancel" onclick="hideLogoutModal()">Cancel</button>
                <button type="button" class="modal-btn modal-btn-confirm" onclick="confirmLogout()">Yes, Sign Out</button>
            </div>
        </div>
    </div>

    <!-- Account Popup -->
    <div id="accountPopup" class="account-popup">
        <div style="text-align:center; margin-bottom:1rem;">
         
        </div>
        <p><strong>🔑 Permanent ID:</strong> <asp:Label ID="lblPopupPermanentID" runat="server"></asp:Label></p>
        <p><strong>👤 Name:</strong> <asp:Label ID="lblPopupName" runat="server"></asp:Label></p>
        <p><strong>📧 Email:</strong> <asp:Label ID="lblPopupEmail" runat="server"></asp:Label></p>
        <p><strong>🎭 Role:</strong> <asp:Label ID="lblPopupRole" runat="server"></asp:Label></p>
    </div>

    <!-- Notification Popup -->
    <div id="notificationPopup" class="notification-popup">
        <asp:Literal ID="litNotificationContent" runat="server"></asp:Literal>
    </div>

    <!-- Dashboard Content -->
    <div class="dashboard">
        <div class="dashboard-header">
            <h2>Admin Panel - <asp:Label ID="lblAdminName" runat="server" style="color:#a78bfa"></asp:Label></h2>
            <div class="right-icons">
                <div class="notification-bell" onclick="toggleNotificationPopup()">
                    🔔
                    <asp:Label ID="lblNotificationBadge" runat="server" CssClass="badge" Text="0"></asp:Label>
                </div>
                <button type="button" class="logout-btn-professional" onclick="showLogoutModal()">
                    🚪 Sign Out
                </button>
                <div class="avatar-container" onclick="toggleAccountPopup()">
                  
                </div>
            </div>
        </div>

        <div class="nav-links">
            <a href="Dashboard.aspx" class="nav-link">📊 Dashboard</a>
            <a href="ManageEquipment.aspx" class="nav-link">📦 Manage Equipment</a>
            <a href="ManageUsers.aspx" class="nav-link">👥 Manage Users</a>
            <a href="BorrowHistory.aspx" class="nav-link">📜 Borrow History</a>
            <a href="ActivityLog.aspx" class="nav-link">📋 Activity Log</a>
        </div>

        <!-- Stats Overview -->
        <div class="stats-grid">
            <div class="stat-card">
                <div class="stat-number"><asp:Label ID="lblTotalUsers" runat="server" Text="0"></asp:Label></div>
                <div class="stat-label">Total Users</div>
            </div>
            <div class="stat-card">
                <div class="stat-number"><asp:Label ID="lblTotalEquipment" runat="server" Text="0"></asp:Label></div>
                <div class="stat-label">Equipment Items</div>
            </div>
            <div class="stat-card">
                <div class="stat-number"><asp:Label ID="lblActiveBorrows" runat="server" Text="0"></asp:Label></div>
                <div class="stat-label">Active Borrows</div>
            </div>
            <div class="stat-card">
                <div class="stat-number"><asp:Label ID="lblPendingRequests" runat="server" Text="0"></asp:Label></div>
                <div class="stat-label">Pending Requests</div>
            </div>
        </div>

        <!-- Invite Code -->
        <div class="section-title">🔑 Your Invite Code</div>
        <div class="invite-code-box">
            <span><asp:Label ID="lblAdminInviteCode" runat="server"></asp:Label></span>
        </div>
        <p style="font-size:0.8rem; color:#94a3b8;">Share this code with students and teachers so they can register.</p>

        <!-- Quick Actions -->
        <div class="section-title">⚡ Quick Actions</div>
        <div style="display: flex; gap: 1rem; margin: 1rem 0; flex-wrap: wrap;">
            <a href="ManageEquipment.aspx" class="btn btn-primary">➕ Add Equipment</a>
            <a href="ManageUsers.aspx" class="btn">👥 Manage Users</a>
            <a href="BorrowHistory.aspx" class="btn">📜 View All Borrows</a>
        </div>

        <!-- Activity Log with Search -->
        <div class="section-title">📋 Activity Log with Search</div>
        
        <div class="search-container">
            <div class="search-box">
                <asp:TextBox ID="txtActivitySearch" runat="server" placeholder="🔍 Search by user, action, or details..." AutoPostBack="true" OnTextChanged="txtActivitySearch_TextChanged" />
                <span class="search-icon">🔍</span>
            </div>
            <div class="filter-buttons">
                <asp:Button ID="btnAllActions" runat="server" Text="All" CssClass="filter-btn active" OnClick="btnFilterAction_Click" CommandArgument="All" />
                <asp:Button ID="btnLoginActions" runat="server" Text="Login" CssClass="filter-btn" OnClick="btnFilterAction_Click" CommandArgument="Login" />
                <asp:Button ID="btnLogoutActions" runat="server" Text="Logout" CssClass="filter-btn" OnClick="btnFilterAction_Click" CommandArgument="Logout" />
                <asp:Button ID="btnRegisterActions" runat="server" Text="Register" CssClass="filter-btn" OnClick="btnFilterAction_Click" CommandArgument="Register" />
                <asp:Button ID="btnBorrowActions" runat="server" Text="Borrow" CssClass="filter-btn" OnClick="btnFilterAction_Click" CommandArgument="Borrow" />
                <asp:Button ID="btnReturnActions" runat="server" Text="Return" CssClass="filter-btn" OnClick="btnFilterAction_Click" CommandArgument="Return" />
            </div>
            <asp:Button ID="btnClearActivity" runat="server" Text="Clear" CssClass="clear-search" OnClick="btnClearActivity_Click" />
        </div>
        <div class="search-results-count">
            <asp:Label ID="lblActivityCount" runat="server" Text=""></asp:Label>
        </div>

        <asp:GridView ID="gvActivityLog" runat="server" AutoGenerateColumns="True">
            <EmptyDataTemplate>
                <tr><td colspan="3" style="text-align:center; padding:2rem;">No activity records found</td></tr>
            </EmptyDataTemplate>
        </asp:GridView>

        <asp:Label ID="lblAdminMessage" runat="server" Visible="false" />
    </div>
</div>

<!-- Hidden button for server-side postback -->
<asp:Button ID="btnProfessionalLogout" runat="server" style="display:none;" OnClick="btnLogout_Click" />

</form>
</body>
</html>