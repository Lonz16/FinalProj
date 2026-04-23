<%@ Page Title="Active Borrows" Language="C#" AutoEventWireup="true" CodeBehind="ActiveBorrows.aspx.cs" Inherits="CanteenProject.Teacher.ActiveBorrows" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>BorrowBox Pro - Active Borrows</title>
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
            transition: 0.2s;
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
        .btn-warning {
            background: #f59e0b;
            border: none;
            padding: 0.5rem 1rem;
            border-radius: 2rem;
            color: white;
            cursor: pointer;
        }
        .overdue { color: #ef4444; font-weight: bold; }
        .due-soon { color: #f59e0b; }
        .msg-success { color: #34d399; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="header">
                <h1>🔐 BorrowBox Pro</h1>
                <p>Track Currently Borrowed Equipment</p>
            </div>
            
            <div class="nav-links">
                <a href="Dashboard.aspx" class="nav-link">📊 Dashboard</a>
                <a href="PendingRequests.aspx" class="nav-link">⏳ Pending Requests</a>
                <a href="ActiveBorrows.aspx" class="nav-link">🔄 Active Borrows</a>
                <a href="../Account/Login.aspx" class="nav-link">🚪 Logout</a>
            </div>
            
            <div class="dashboard">
                <div class="section-title">🔄 Currently Borrowed Equipment</div>
                <asp:GridView ID="gvActiveBorrows" runat="server" AutoGenerateColumns="False" OnRowCommand="gvActiveBorrows_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="StudentName" HeaderText="Student" />
                        <asp:BoundField DataField="StudentEmail" HeaderText="Email" />
                        <asp:BoundField DataField="EquipmentName" HeaderText="Equipment" />
                        <asp:BoundField DataField="BorrowDate" HeaderText="Borrow Date" />
                        <asp:TemplateField HeaderText="Due Date">
                            <ItemTemplate>
                                <asp:Label ID="lblDueDate" runat="server" Text='<%# Eval("DueDate") %>' 
                                    CssClass='<%# GetDueDateClass(Eval("DueDate").ToString()) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:ButtonField Text="Mark Returned" CommandName="MarkReturned" ButtonType="Button" ControlStyle-CssClass="btn-warning" />
                    </Columns>
                    <EmptyDataTemplate>
                        <tr><td colspan="6" style="text-align:center; padding:2rem;">📭 No active borrows</td></tr>
                    </EmptyDataTemplate>
                </asp:GridView>
                
                <div class="section-title" style="margin-top: 2rem;">📜 Recently Returned</div>
                <asp:GridView ID="gvReturnedHistory" runat="server" AutoGenerateColumns="True">
                    <Columns>
                        <asp:BoundField DataField="StudentName" HeaderText="Student" />
                        <asp:BoundField DataField="EquipmentName" HeaderText="Equipment" />
                        <asp:BoundField DataField="ReturnDate" HeaderText="Return Date" />
                    </Columns>
                    <EmptyDataTemplate>
                        <tr><td colspan="3" style="text-align:center; padding:2rem;">No recent returns</td></tr>
                    </EmptyDataTemplate>
                </asp:GridView>
                
                <asp:Label ID="lblMessage" runat="server" />
            </div>
        </div>
    </form>
</body>
</html>