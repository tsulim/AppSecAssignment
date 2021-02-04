<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RegistrationForm.aspx.cs" Inherits="Assignment.RegistrationForm" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 700px;
        }
        .auto-style2 {
            width: 250px;
        }
        .auto-style3 {
            width: 200px;
        }
    </style>
    <script type="text/javascript">
        function validate() {
            var str = document.getElementById('<%=tb_pwd.ClientID%>').value;
            var submitBtn = document.getElementById("btn_Submit")
            submitBtn.disabled = true;

            var lblmsglength = document.getElementById("lbl_pwdchklength");
            var lblmsgdigit = document.getElementById("lbl_pwdchkdigit");
            var lblmsgupper = document.getElementById("lbl_pwdchkupper");
            var lblmsglower = document.getElementById("lbl_pwdchklower");
            var lblmsgspecial = document.getElementById("lbl_pwdchkspecial");
            var valid = true;

            var tb_fname = document.getElementById("tb_fname").value;
            var tb_lname = document.getElementById("tb_lname").value;
            var tb_cardno = document.getElementById("tb_cardno").value;
            var tb_cardname = document.getElementById("tb_cardname").value;
            var tb_expdate = document.getElementById("tb_expDate").value;
            var tb_cvv = document.getElementById("tb_cvv").value;
            var tb_email = document.getElementById("tb_email").value;
            var tb_dob = document.getElementById("tb_dob").value;

            if (tb_fname.length == 0 || tb_lname.length == 0 || tb_cardno.length == 0 || tb_cardname.length == 0 || tb_expdate.length == 0 || tb_cvv.length == 0 || tb_email.length == 0 || tb_dob.length == 0) {
                valid = false;
            }

            if (str.length < 8) {
                lblmsglength.style.color = "Red";
                lblmsglength.innerHTML = "✖ At least 8 characters";
                valid = false;
            } else {
                lblmsglength.style.color = "Green";
                lblmsglength.innerHTML = "✓ At least 8 characters";
            }
            if (str.search(/[0-9]/) == -1) {
                lblmsgdigit.style.color = "Red";
                lblmsgdigit.innerHTML = "✖ At least 1 number";
                valid = false;
            } else {
                lblmsgdigit.style.color = "Green"
                lblmsgdigit.innerHTML = "✓ At least 1 number";
            }
            if (str.search(/[a-z]/) == -1) {
                lblmsglower.style.color = "Red";
                lblmsglower.innerHTML = "✖ At least 1 lowercase letter";
                valid = false;
            } else {
                lblmsglower.style.color = "Green";
                lblmsglower.innerHTML = "✓ At least 1 lowercase letter";
            }
            if (str.search(/[A-Z]/) == -1) {
                lblmsgupper.style.color = "Red";
                lblmsgupper.innerHTML = "✖ At least 1 uppercase letter";
                valid = false;
            } else {
                lblmsgupper.style.color = "Green";
                lblmsgupper.innerHTML = "✓ At least 1 uppercase letter";
            }

            if (str.search(/[^A-Za-z0-9#]/) == -1) {
                lblmsgspecial.style.color = "Red";
                lblmsgspecial.innerHTML = "✖ At least 1 special character";
                valid = false;
            } else {
                lblmsgspecial.style.color = "Green";
                lblmsgspecial.innerHTML = "✓ At least 1 special character";
            }
            var cfmPwd = document.getElementById('<%=tb_cfpwd.ClientID%>').value;

            if (valid && cfmPwd != "") {
                submitBtn.disabled = false;
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h3>Registration</h3>
            <table class="auto-style1">
                <tbody>
                    <tr>
                        <td class="auto-style2">
                            First Name
                        </td>
                        <td class="auto-style3">
                            <asp:TextBox ID="tb_fname" runat="server" onkeyup="javascript:validate()"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style2">
                            Last Name
                        </td>
                        <td class="auto-style3">
                            <asp:TextBox ID="tb_lname" runat="server" onkeyup="javascript:validate()"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style2">
                            Card number
                        </td>
                        <td class="auto-style3">
                            <asp:TextBox ID="tb_cardno" runat="server" onkeyup="javascript:validate()"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style2">
                            Name on Card
                        </td>
                        <td class="auto-style3">
                            <asp:TextBox ID="tb_cardname" runat="server" onkeyup="javascript:validate()"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style2">Expiry Date</td>
                        <td class="auto-style3">
                            <asp:TextBox ID="tb_expDate" runat="server" onkeyup="javascript:validate()"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style2">
                            Security Code
                        </td>
                        <td class="auto-style3">
                            <asp:TextBox ID="tb_cvv" runat="server" onkeyup="javascript:validate()"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style2">
                            Email Address
                        </td>
                        <td class="auto-style3">
                            <asp:TextBox ID="tb_email" runat="server" onkeyup="javascript:validate()"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="lbl_emailchk" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style2">
                            Date of Birth
                        </td>
                        <td class="auto-style3">
                            <asp:TextBox ID="tb_dob" runat="server" onkeyup="javascript:validate()"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style2">
                            Password
                        </td>
                        <td class="auto-style3">
                            <asp:TextBox ID="tb_pwd" runat="server" onkeyup="javascript:validate()" TextMode="Password"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style2">
                            Confirm Password
                        </td>
                        <td class="auto-style3">
                            <asp:TextBox ID="tb_cfpwd" runat="server" TextMode="Password" onkeyup="javascript:validate()"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style2">
                            <asp:Label runat="server" Font-Bold="True">Password must include:</asp:Label>
                            <br />
                            <asp:Label ID="lbl_pwdchklength" runat="server" ForeColor="Red">✖ At least 8 characters</asp:Label>
                            <br />
                            <asp:Label ID="lbl_pwdchkdigit" runat="server" ForeColor="Red">✖ At least 1 number</asp:Label>
                            <br />
                            <asp:Label ID="lbl_pwdchkupper" runat="server" ForeColor="Red">✖ At least 1 uppercase letter</asp:Label>
                            <br />
                            <asp:Label ID="lbl_pwdchklower" runat="server" ForeColor="Red">✖ At least 1 lowercase letter</asp:Label>
                            <br />
                            <asp:Label ID="lbl_pwdchkspecial" runat="server" ForeColor="Red">✖ At least 1 special character</asp:Label>
                        </td>
                        <td class="auto-style3">
                            <asp:Label ID="lbl_chk" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style2"></td>
                        <td class="auto-style3">
                            <asp:Button ID="btn_Submit" runat="server" Text="Submit" Width="165px" OnClick="btn_Submit_Click" Enabled="false"/>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </form>
</body>
</html>
