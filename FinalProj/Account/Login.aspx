<%@ Page Title="Login" Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="CanteenProject.Account.Login" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>BorrowBox Pro - Login</title>
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
        .card {
            max-width: 500px;
            margin: 0 auto;
            background: rgba(15, 25, 45, 0.85);
            backdrop-filter: blur(16px);
            border: 1px solid rgba(255,255,255,0.1);
            border-radius: 2rem;
            padding: 2rem;
            box-shadow: 0 25px 40px rgba(0,0,0,0.4);
        }
        .card h3 { font-size: 1.6rem; margin-bottom: 1.5rem; color: #c4b5fd; text-align: center; }
        input {
            width: 100%;
            padding: 0.8rem 1rem;
            margin-bottom: 1rem;
            background: #0f172a;
            border: 1px solid #334155;
            border-radius: 1rem;
            color: #f1f5f9;
            font-size: 1rem;
        }
        input:focus {
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
        .msg-error { color: #f87171; margin-top: 0.5rem; display: block; text-align: center; }
        .text-center { text-align: center; margin-top: 1rem; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="header">
                <h1>🔐 BorrowBox Pro</h1>
                <p>Invite‑only Access · Admin‑Generated Codes</p>
            </div>

            <div class="card">
                <h3>Login to Your Account</h3>
                <asp:TextBox ID="txtLoginEmail" runat="server" placeholder="Email address"></asp:TextBox>
                <asp:TextBox ID="txtLoginPassword" runat="server" TextMode="Password" placeholder="Password"></asp:TextBox>
                <asp:Button ID="btnLogin" runat="server" Text="Login →" CssClass="btn btn-primary" OnClick="btnLogin_Click" />
                <div class="text-center">
                    <asp:HyperLink ID="lnkForgot" runat="server" Text="Forgot Password?" NavigateUrl="~/Account/ForgotPassword.aspx" CssClass="link-btn" />
                    &nbsp;|&nbsp;
                    <asp:HyperLink ID="lnkSignup" runat="server" Text="Create Account" NavigateUrl="~/Account/Signup.aspx" CssClass="link-btn" />
                </div>
                <asp:Label ID="lblLoginMessage" runat="server" CssClass="msg-error"></asp:Label>
            </div>
        </div>
    </form>
</body>
</html>