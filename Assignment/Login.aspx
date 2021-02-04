<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Assignment.Login" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 500px;
        }
        .auto-style2 {
            width: 250px;
        }
    </style>
    <script src="https://www.google.com/recaptcha/api.js?render=6LcBaOUZAAAAAJf4I4CGw2OQMPM0lQfUz2oV6x_f"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h3>Login</h3>
            <table class="auto-style1">
                <tbody>
                    <tr>
                        <td class="auto-style2">
                            Email Address
                        </td>
                        <td>
                            <asp:TextBox ID="tb_email" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style2">
                            Password
                        </td>
                        <td>
                            <asp:TextBox ID="tb_pwd" runat="server" onkeyup="javascript:validate()" TextMode="Password"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style2"></td>
                        <td>
                            <asp:Label ID="lbl_chk" runat="server"></asp:Label>
                            <br />
                            <asp:Label ID="lbl_lockout" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style2">
                            <asp:HyperLink href="RegistrationForm.aspx" runat="server">Register</asp:HyperLink>
                        </td>
                        <td>
                            <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response" />
                            <asp:Button ID="btn_Submit" runat="server" Text="Log in" Width="165px" OnClick="btn_Submit_Click"/>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </form>
    <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('6LcBaOUZAAAAAJf4I4CGw2OQMPM0lQfUz2oV6x_f', { action: 'Login' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            });
        });
    </script>
</body>
</html>
