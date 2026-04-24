<%@ Page Title="Sign Up" Language="C#" AutoEventWireup="true" CodeBehind="Signup.aspx.cs" Inherits="CanteenProject.Account.Signup" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>BorrowBox Pro - Create Account</title>
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
        .container { max-width: 500px; margin: 0 auto; }
        .header { text-align: center; margin-bottom: 2rem; }
        .header h1 {
            font-size: 2.8rem;
            background: linear-gradient(135deg, #a5f3fc, #c084fc, #f0abfc);
            -webkit-background-clip: text;
            background-clip: text;
            color: transparent;
        }
        .header p { color: #94a3b8; }
        .card {
            background: rgba(15, 25, 45, 0.85);
            backdrop-filter: blur(16px);
            border: 1px solid rgba(255,255,255,0.1);
            border-radius: 2rem;
            padding: 2rem;
            box-shadow: 0 25px 40px rgba(0,0,0,0.4);
        }
        .card h3 { 
            font-size: 1.6rem; 
            margin-bottom: 1.5rem; 
            color: #c4b5fd; 
            text-align: center; 
        }
        input, select {
            width: 100%;
            padding: 0.8rem 1rem;
            margin-bottom: 1rem;
            background: #0f172a;
            border: 1px solid #334155;
            border-radius: 1rem;
            color: #f1f5f9;
            font-size: 1rem;
        }
        input:focus, select:focus {
            outline: none;
            border-color: #818cf8;
            box-shadow: 0 0 0 2px rgba(129,140,248,0.3);
        }
        .btn {
            background: #1e293b;
            border: none;
            padding: 0.7rem 1.5rem;
            border-radius: 2rem;
            font-weight: 500;
            color: #e2e8f0;
            cursor: pointer;
            transition: 0.2s;
            display: inline-block;
            text-align: center;
            font-size: 1rem;
        }
        .btn-primary {
            background: linear-gradient(95deg, #4f46e5, #7c3aed);
            color: white;
            width: 100%;
        }
        .btn-primary:hover { opacity: 0.9; }
        .link-btn {
            background: none;
            border: none;
            color: #a78bfa;
            cursor: pointer;
            text-decoration: underline;
            font-size: 0.9rem;
        }
        .link-btn:hover { color: #c4b5fd; }
        .msg-success { 
            color: #34d399; 
            margin-top: 1rem; 
            padding: 0.75rem;
            background: rgba(52, 211, 153, 0.1);
            border-radius: 0.5rem;
            text-align: center;
            display: block; 
        }
        .msg-error { 
            color: #f87171; 
            margin-top: 1rem; 
            padding: 0.75rem;
            background: rgba(248, 113, 113, 0.1);
            border-radius: 0.5rem;
            text-align: center;
            display: block; 
        }
        .text-center { text-align: center; margin-top: 1rem; }
        .info-note {
            font-size: 0.8rem;
            color: #94a3b8;
            text-align: center;
            margin-top: -0.5rem;
            margin-bottom: 1rem;
        }
    </style>
    <script>
        function toggleInviteCodeField() {
            var role = document.getElementById('<%= ddlSignupRole.ClientID %>');
            var invitePanel = document.getElementById('<%= pnlInviteCode.ClientID %>');
            if (role && invitePanel) {
                invitePanel.style.display = (role.value === 'Student' || role.value === 'Teacher') ? 'block' : 'none';
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="header">
                <h1>🔐 BorrowBox Pro</h1>
                <p>Create Your Account</p>
            </div>

            <div class="card">
                <h3>Student / Teacher Registration</h3>
                
                <asp:TextBox ID="txtSignupName" runat="server" placeholder="Full Name"></asp:TextBox>
                <asp:TextBox ID="txtSignupEmail" runat="server" placeholder="Email Address"></asp:TextBox>
                <asp:TextBox ID="txtSignupPassword" runat="server" TextMode="Password" placeholder="Password (min 6 characters)"></asp:TextBox>
                
                <asp:DropDownList ID="ddlSignupRole" runat="server">
                    <asp:ListItem>Student</asp:ListItem>
                    <asp:ListItem>Teacher</asp:ListItem>
                </asp:DropDownList>
                
                <asp:Panel ID="pnlInviteCode" runat="server">
                    <asp:TextBox ID="txtInviteCode" runat="server" placeholder="Invite Code (required)"></asp:TextBox>
                    <div class="info-note">
                        ℹ️ Ask an Admin for the invite code to register
                    </div>
                </asp:Panel>
                
                <asp:Button ID="btnRegister" runat="server" Text="Register" CssClass="btn btn-primary" OnClick="btnRegister_Click" />
                
                <div class="text-center">
                    <asp:HyperLink ID="lnkBackToLogin" runat="server" Text="← Back to Login" NavigateUrl="~/Account/Login.aspx" CssClass="link-btn" />
                </div>
                
                <asp:Label ID="lblSignupMessage" runat="server" Visible="false"></asp:Label>
            </div>
        </div>

        <script>
            // Initialize the invite code field visibility
            (function() {
                var role = document.getElementById('<%= ddlSignupRole.ClientID %>');
                if (role) {
                    role.onchange = toggleInviteCodeField;
                    toggleInviteCodeField();
                }
            })();
        </script>
    </form>
</body>
</html>