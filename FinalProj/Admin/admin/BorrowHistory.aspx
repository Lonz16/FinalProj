<%@ Page Title="Borrow History" Language="C#" AutoEventWireup="true" CodeBehind="BorrowHistory.aspx.cs" Inherits="CanteenProject.Admin.BorrowHistory" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>BorrowBox Pro - Borrow History</title>
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
        .filter-bar {
            display: flex;
            gap: 1rem;
            margin-bottom: 1rem;
            flex-wrap: wrap;
            align-items: center;
        }
        .filter-bar select, .filter-bar input {
            background: #0f172a;
            border: 1px solid #334155;
            padding: 0.5rem;
            border-radius: 0.5rem;
            color: white;
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
        .status-borrowed { color: #f59e0b; }
        .status-returned { color: #10b981; }
        .overdue { color: #ef4444; font-weight: bold; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="header">
                <h1>🔐 BorrowBox Pro</h1>
                <p>Complete Borrowing History</p>
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
                <div class="section-title">📜 All Borrow Records</div>
                
                <div class="filter-bar">
                    <asp:DropDownList ID="ddlStatus" runat="server">
                        <asp:ListItem Text="All Status" Value="All" />
                        <asp:ListItem Text="Borrowed" Value="Borrowed" />
                        <asp:ListItem Text="Returned" Value="Returned" />
                    </asp:DropDownList>
                    <asp:TextBox ID="txtSearch" runat="server" placeholder="Search by student or equipment..." />
                    <asp:Button ID="btnFilter" runat="server" Text="Filter" CssClass="btn btn-primary" OnClick="btnFilter_Click" style="padding:0.5rem 1rem;" />
                </div>
                
                <asp:GridView ID="gvBorrowHistory" runat="server" AutoGenerateColumns="True">
                    <EmptyDataTemplate>
                        <tr><td colspan="3" style="text-align:center; padding:2rem;">No borrow records found</td></tr>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
    </form>
</body>
</html>