<%@ Page Title="Forgot Password" Language="C#" AutoEventWireup="true" CodeBehind="ForgotPassword.aspx.cs" Inherits="CanteenProject.Account.ForgotPassword" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>BorrowBox Pro - Reset Password</title>
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
        .card p {
            color: #94a3b8;
            text-align: center;
            margin-bottom: 1.5rem;
            font-size: 0.9rem;
        }
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
            border: none;
            padding: 0.7rem 1.5rem;
            border-radius: 2rem;
            font-weight: 500;
            cursor: pointer;
            font-size: 1rem;
            width: 100%;
        }
        .btn-primary {
            background: linear-gradient(95deg, #4f46e5, #7c3aed);
            color: white;
        }
        .btn-primary:hover { opacity: 0.9; }
        .link-btn {
            background: none;
            border: none;
            color: #a78bfa;
            cursor: pointer;
            text-decoration: underline;
            font-size: 0.9rem;
            width: auto;
            display: inline-block;
        }
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
        .temp-password-box {
            background: #0f172a;
            border: 1px solid #4f46e5;
            border-radius: 1rem;
            padding: 1rem;
            margin-top: 1rem;
            text-align: center;
        }
        .temp-password {
            font-family: monospace;
            font-size: 1.3rem;
            font-weight: bold;
            color: #a78bfa;
            letter-spacing: 2px;
        }
        .debug-info {
            font-size: 0.7rem;
            color: #64748b;
            margin-top: 1rem;
            padding: 0.5rem;
            border-top: 1px solid #334155;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="header">
                <h1>🔐 BorrowBox Pro</h1>
                <p>Reset Your Password</p>
            </div>

            <div class="card">
                <h3>Forgot Password?</h3>
                <p>Enter your email address and we'll send you a temporary password.</p>
                
                <asp:TextBox ID="txtForgotEmail" runat="server" placeholder="Enter your registered email" TextMode="Email"></asp:TextBox>
                <asp:Button ID="btnResetPassword" runat="server" Text="Reset Password" CssClass="btn btn-primary" OnClick="btnResetPassword_Click" />
                
                <div class="text-center">
                    <asp:HyperLink ID="lnkBackLoginFromForgot" runat="server" Text="← Back to Login" NavigateUrl="~/Account/Login.aspx" CssClass="link-btn" />
                </div>
                
                <asp:Label ID="lblForgotMessage" runat="server" Visible="false"></asp:Label>
                
                <!-- Debug info - remove in production -->
                <div class="debug-info" id="debugInfo" runat="server" visible="false">
                    <strong>Debug Info:</strong><br />
                    Total Users: <asp:Label ID="lblDebugUserCount" runat="server" Text="0" /><br />
                    <asp:Label ID="lblDebugEmails" runat="server" Text="" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>