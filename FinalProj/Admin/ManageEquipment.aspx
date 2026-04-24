<%@ Page Title="Manage Equipment" Language="C#" AutoEventWireup="true" CodeBehind="ManageEquipment.aspx.cs" Inherits="CanteenProject.Admin.ManageEquipment" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>BorrowBox Pro - Manage Equipment</title>
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
        .container { max-width: 1400px; margin: 0 auto; }
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
        .add-form {
            background: #1e293b;
            padding: 1.5rem;
            border-radius: 1rem;
            margin-bottom: 2rem;
        }
        .form-row {
            display: flex;
            gap: 1rem;
            margin-bottom: 1rem;
            flex-wrap: wrap;
        }
        .form-row input, .form-row textarea, .form-row select {
            flex: 1;
            background: #0f172a;
            border: 1px solid #334155;
            padding: 0.7rem;
            border-radius: 0.5rem;
            color: white;
        }
        .form-row textarea {
            min-width: 100%;
            resize: vertical;
        }
        .image-preview {
            width: 100px;
            height: 100px;
            object-fit: cover;
            border-radius: 0.5rem;
            margin-top: 0.5rem;
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
            vertical-align: middle;
        }
        th { background: #1e293b; }
        .equipment-img {
            width: 50px;
            height: 50px;
            object-fit: cover;
            border-radius: 0.5rem;
        }
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
        .btn-success { background: #10b981; color: white; }
        .btn-danger { background: #ef4444; color: white; }
        .btn-warning { background: #f59e0b; color: white; }
        .msg-success { color: #34d399; margin-top: 0.5rem; display: block; }
        .msg-error { color: #f87171; margin-top: 0.5rem; display: block; }
    </style>
</head>
<body>
    <form id="form1" runat="server" enctype="multipart/form-data">
        <div class="container">
            <div class="header">
                <h1>🔐 BorrowBox Pro</h1>
                <p>Manage Equipment Inventory</p>
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
                <!-- Add Equipment Form with Image Upload -->
                <div class="section-title">➕ Add New Equipment</div>
                <div class="add-form">
                    <div class="form-row">
                        <asp:TextBox ID="txtEquipName" runat="server" placeholder="Equipment Name" />
                        <asp:TextBox ID="txtCategory" runat="server" placeholder="Category" />
                        <asp:TextBox ID="txtQuantity" runat="server" placeholder="Quantity" TextMode="Number" />
                    </div>
                    <div class="form-row">
                        <asp:TextBox ID="txtDescription" runat="server" placeholder="Description (optional)" TextMode="MultiLine" Rows="2" />
                    </div>
                    <div class="form-row">
                        <asp:FileUpload ID="fileEquipmentImage" runat="server" Accept="image/jpeg,image/png,image/jpg,image/gif" />
                        <asp:Button ID="btnAddEquipment" runat="server" Text="Add Equipment" CssClass="btn btn-primary" OnClick="btnAddEquipment_Click" />
                    </div>
                    <asp:Label ID="lblAddMessage" runat="server" />
                </div>
                
                <!-- Equipment List with Images -->
                <div class="section-title">📦 Equipment Inventory</div>
                <asp:GridView ID="gvEquipment" runat="server" AutoGenerateColumns="False" DataKeyNames="EquipmentID"
                    OnRowEditing="gvEquipment_RowEditing" OnRowCancelingEdit="gvEquipment_RowCancelingEdit"
                    OnRowUpdating="gvEquipment_RowUpdating" OnRowCommand="gvEquipment_RowCommand">
                    <Columns>
                        <asp:TemplateField HeaderText="Image">
                            <ItemTemplate>
                                <asp:Image ID="imgEquip" runat="server" 
                                    ImageUrl='<%# string.IsNullOrEmpty(Eval("ImageUrl").ToString()) ? "~/Images/equipment/default-equipment.png" : Eval("ImageUrl") %>' 
                                    CssClass="equipment-img" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:FileUpload ID="fileEditImage" runat="server" />
                                <asp:HiddenField ID="hdnCurrentImage" runat="server" Value='<%# Eval("ImageUrl") %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Name" HeaderText="Name" />
                        <asp:BoundField DataField="Category" HeaderText="Category" />
                        <asp:BoundField DataField="Quantity" HeaderText="Quantity" />
                        <asp:BoundField DataField="Description" HeaderText="Description" />
                        <asp:CommandField ShowEditButton="True" ShowDeleteButton="True" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </form>
</body>
</html>