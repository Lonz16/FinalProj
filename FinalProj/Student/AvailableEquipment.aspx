<%@ Page Title="Available Equipment" Language="C#" AutoEventWireup="true" CodeBehind="AvailableEquipment.aspx.cs" Inherits="CanteenProject.Student.AvailableEquipment" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>BorrowBox Pro - Available Equipment</title>
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
        .nav-bar {
            display: flex;
            gap: 1rem;
            margin-bottom: 2rem;
            padding: 1rem;
            background: rgba(15, 25, 45, 0.85);
            border-radius: 1rem;
        }
        .nav-link {
            color: #a78bfa;
            text-decoration: none;
            padding: 0.5rem 1rem;
            border-radius: 0.5rem;
            transition: 0.2s;
        }
        .nav-link:hover {
            background: #334155;
        }
        .dashboard {
            background: rgba(10,20,35,0.7);
            backdrop-filter: blur(12px);
            border-radius: 1.8rem;
            border: 1px solid rgba(255,255,255,0.08);
            padding: 1.8rem;
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
        th, td {
            padding: 0.8rem 1rem;
            text-align: left;
            border-bottom: 1px solid #1e293b;
        }
        th { background: #1e293b; color: #cbd5e1; }
        .btn {
            background: linear-gradient(95deg, #4f46e5, #7c3aed);
            border: none;
            padding: 0.5rem 1rem;
            border-radius: 2rem;
            color: white;
            cursor: pointer;
        }
        .msg-success { color: #34d399; }
        .msg-error { color: #f87171; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="header">
                <h1>🔐 BorrowBox Pro</h1>
                <p>Browse Available Equipment</p>
            </div>
            
            <div class="nav-bar">
                <a href="Dashboard.aspx" class="nav-link">🏠 Dashboard</a>
                <a href="AvailableEquipment.aspx" class="nav-link">📦 Available Equipment</a>
                <a href="MyBorrowings.aspx" class="nav-link">📚 My Borrowings</a>
                <a href="../Account/Login.aspx" class="nav-link">🚪 Logout</a>
            </div>
            
            <div class="dashboard">
                <div class="section-title">📦 All Available Equipment</div>
                <asp:GridView ID="gvAvailableEquipment" runat="server" AutoGenerateColumns="False" OnRowCommand="gvAvailableEquipment_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="Name" HeaderText="Item" />
                        <asp:BoundField DataField="Category" HeaderText="Category" />
                        <asp:BoundField DataField="Quantity" HeaderText="In Stock" />
                        <asp:ButtonField Text="Request Borrow" CommandName="Borrow" ButtonType="Button" ControlStyle-CssClass="btn" />
                    </Columns>
                </asp:GridView>
                <asp:Label ID="lblMessage" runat="server" />
            </div>
        </div>
    </form>
</body>
</html>