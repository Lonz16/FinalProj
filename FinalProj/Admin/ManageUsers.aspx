<%@ Page Title="Manage Users" Language="C#" AutoEventWireup="true" CodeBehind="ManageUsers.aspx.cs" Inherits="CanteenProject.Admin.ManageUsers" %><!DOCTYPE html>
<html>
<head runat="server">
    <title>BorrowBox Pro - Manage Users</title>
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
        .container { max-width: 1300px; margin: 0 auto; }
        .header { text-align: center; margin-bottom: 2rem; }
        .header h1 {
            font-size: 2.8rem;
            background: linear-gradient(135deg, #a5f3fc, #c084fc, #f0abfc);
            -webkit-background-clip: text;
            background-clip: text;
            color: transparent;
        }
        .header p { color: #94a3b8; }
        .nav-links {
            display: flex;
            gap: 1rem;
            margin-bottom: 2rem;
            padding: 1rem;
            background: rgba(15, 25, 45, 0.85);
            border-radius: 1rem;
            flex-wrap: wrap;
        }
        .nav-link {
            color: #a78bfa;
            text-decoration: none;
            padding: 0.5rem 1rem;
            border-radius: 0.5rem;
        }
        .nav-link:hover { background: #334155; }
        .dashboard {
            background: rgba(10,20,35,0.7);
            backdrop-filter: blur(12px);
            border-radius: 1.8rem;
            padding: 1.8rem;
        }
        .section-title {
            font-size: 1.3rem;
            font-weight: 600;
            margin-bottom: 1rem;
            color: #a78bfa;
            border-bottom: 2px solid #334155;
            display: inline-block;
        }
        
        /* ===== SEARCH BAR STYLES - ADD THESE ===== */
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
        /* ===== END SEARCH BAR STYLES ===== */
        
        .add-form {
            background: #1e293b;
            padding: 1rem;
            border-radius: 1rem;
            margin: 1rem 0;
            display: flex;
            gap: 1rem;
            flex-wrap: wrap;
            align-items: center;
        }
        .add-form input {
            background: #0f172a;
            border: 1px solid #334155;
            padding: 0.7rem 1rem;
            border-radius: 0.5rem;
            color: white;
            flex: 1;
        }
        
        table {
            width: 100%;
            border-collapse: collapse;
            background: #0f172a80;
            margin-top: 1rem;
        }
        th, td {
            padding: 0.8rem 1rem;
            text-align: left;
            border-bottom: 1px solid #1e293b;
        }
        th { background: #1e293b; }
        
        .btn {
            border: none;
            padding: 0.5rem 1rem;
            border-radius: 2rem;
            cursor: pointer;
            margin: 0 0.25rem;
        }
        .btn-primary {
            background: linear-gradient(95deg, #4f46e5, #7c3aed);
            color: white;
        }
        .btn-danger {
            background: #ef4444;
            border: none;
            padding: 0.3rem 0.8rem;
            border-radius: 2rem;
            color: white;
            cursor: pointer;
        }
        .btn-warning {
            background: #f59e0b;
            padding: 0.3rem 0.8rem;
            border-radius: 2rem;
            border: none;
            color: white;
            cursor: pointer;
        }
        .role-badge {
            display: inline-block;
            padding: 0.2rem 0.6rem;
            border-radius: 1rem;
            font-size: 0.8rem;
        }
        .role-admin { background: #ef4444; }
        .role-teacher { background: #f59e0b; }
        .role-student { background: #10b981; }
        .msg-success { color: #34d399; margin-top: 0.5rem; display: block; }
        .msg-error { color: #f87171; margin-top: 0.5rem; display: block; }
        .section { margin-bottom: 2rem; }
    </style>
</head>

<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="header">
                <h1>🔐 BorrowBox Pro</h1>
                <p>Manage System Users</p>
            </div>
            
            <div class="nav-links">
                <a href="Dashboard.aspx" class="nav-link">📊 Dashboard</a>
                <a href="ManageEquipment.aspx" class="nav-link">📦 Manage Equipment</a>
                <a href="ManageUsers.aspx" class="nav-link">👥 Manage Users</a>
                <a href="BorrowHistory.aspx" class="nav-link">📜 Borrow History</a>
                <a href="ActivityLog.aspx" class="nav-link">📋 Activity Log</a>
                <a href="../Account/Login.aspx" class="nav-link">🚪 Logout</a>
            </div>
            
            <div class="dashboard">
                <!-- Create New Admin Section -->
                <div class="section">
                    <div class="section-title">👑 Create New Admin</div>
                    <div class="add-form">
                        <asp:TextBox ID="txtAdminName" runat="server" placeholder="Admin Full Name" />
                        <asp:TextBox ID="txtAdminEmail" runat="server" placeholder="Admin Email" />
                        <asp:TextBox ID="txtAdminPassword" runat="server" TextMode="Password" placeholder="Temporary Password" />
                        <asp:Button ID="btnCreateAdmin" runat="server" Text="Create Admin Account" CssClass="btn btn-primary" OnClick="btnCreateAdmin_Click" />
                    </div>
                    <asp:Label ID="lblAdminCreateMessage" runat="server" />
                </div>
                
                <!-- All Users Section with Search -->
                <div class="section">
                    <div class="section-title">👥 All Registered Users</div>
                    
                    <!-- Search Bar -->
                    <div class="search-container">
                        <div class="search-box">
                            <asp:TextBox ID="txtUserSearch" runat="server" placeholder="🔍 Search by name, email, or role..." AutoPostBack="true" OnTextChanged="txtUserSearch_TextChanged" />
                            <span class="search-icon">🔍</span>
                        </div>
                        <div class="filter-buttons">
                            <asp:Button ID="btnAllUsers" runat="server" Text="All" CssClass="filter-btn active" OnClick="btnFilterRole_Click" CommandArgument="All" />
                            <asp:Button ID="btnStudents" runat="server" Text="Students" CssClass="filter-btn" OnClick="btnFilterRole_Click" CommandArgument="Student" />
                            <asp:Button ID="btnTeachers" runat="server" Text="Teachers" CssClass="filter-btn" OnClick="btnFilterRole_Click" CommandArgument="Teacher" />
                            <asp:Button ID="btnAdmins" runat="server" Text="Admins" CssClass="filter-btn" OnClick="btnFilterRole_Click" CommandArgument="Admin" />
                        </div>
                        <asp:Button ID="btnClearUsers" runat="server" Text="Clear" CssClass="clear-search" OnClick="btnClearUsers_Click" />
                    </div>
                    <div class="search-results-count">
                        <asp:Label ID="lblUserCount" runat="server" Text=""></asp:Label>
                    </div>
                    
                    <!-- Users GridView -->
                    <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="False" OnRowCommand="gvUsers_RowCommand" DataKeyNames="Email">
                        <Columns>
                            <asp:TemplateField HeaderText="Profile">
                                <ItemTemplate>
                                    <asp:Image ID="imgProfile" runat="server" 
                                        ImageUrl='<%# string.IsNullOrEmpty(Eval("ProfilePictureUrl")?.ToString()) ? "~/Images/default-avatar.png" : Eval("ProfilePictureUrl") %>' 
                                        Style="width: 40px; height: 40px; border-radius: 50%; object-fit: cover;" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="PermanentID" HeaderText="ID" />
                            <asp:BoundField DataField="FullName" HeaderText="Name" />
                            <asp:BoundField DataField="Email" HeaderText="Email" />
                            <asp:TemplateField HeaderText="Role">
                                <ItemTemplate>
                                    <span class='role-badge role-<%# Eval("Role").ToString().ToLower() %>'>
                                        <%# Eval("Role") %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Invited By">
                                <ItemTemplate>
                                    <%# GetInviterName(Eval("InvitedByAdminCode")?.ToString()) %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Actions">
                                <ItemTemplate>
                                    <asp:Button ID="btnResetPwd" runat="server" Text="Reset Password" 
                                        CommandName="ResetPassword" CommandArgument='<%# Eval("Email") %>' 
                                        CssClass="btn-warning" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <tr>
                                <td colspan="7" style="text-align:center; padding:2rem;">No users found</td>
                            </tr>
                        </EmptyDataTemplate>
                    </asp:GridView>
                    
                    <asp:Label ID="lblMessage" runat="server" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>