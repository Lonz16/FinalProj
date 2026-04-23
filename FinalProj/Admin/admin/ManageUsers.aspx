<%@ Page Title="Manage Users" Language="C#" AutoEventWireup="true" CodeBehind="ManageUsers.aspx.cs" Inherits="CanteenProject.Admin.ManageUsers" %>
<!DOCTYPE html>
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
        .btn-danger {
            background: #ef4444;
            border: none;
            padding: 0.3rem 0.8rem;
            border-radius: 2rem;
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
        .msg-success { color: #34d399; }
        .msg-error { color: #f87171; }
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
                <div class="section-title">👥 All Registered Users</div>
                <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="False" OnRowCommand="gvUsers_RowCommand">
                    <Columns>
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
                                    CssClass="btn btn-warning" style="background:#f59e0b; padding:0.3rem 0.8rem; border-radius:2rem; border:none; color:white; cursor:pointer;" />
                                <asp:Button ID="btnDelete" runat="server" Text="Delete" 
                                    CommandName="DeleteUser" CommandArgument='<%# Eval("Email") %>' 
                                    CssClass="btn btn-danger" style="display:none;" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <tr><td colspan="6" style="text-align:center; padding:2rem;">No users found</td></tr>
                    </EmptyDataTemplate>
                </asp:GridView>
                <asp:Label ID="lblMessage" runat="server" />
            </div>
        </div>
    </form>
</body>
</html>