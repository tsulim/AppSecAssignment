<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Success.aspx.cs" Inherits="Assignment.Success" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label runat="server">Yay success! Welcome </asp:Label><asp:Label ID="lbl_fname" runat="server"></asp:Label>
            <asp:GridView ID="gvAccInfo" runat="server" AutoGenerateColumns="False">
                <Columns>
                    <asp:BoundField DataField="Fname" HeaderText="Fname" />
                    <asp:BoundField DataField="Lname" HeaderText="Lname" />
                    <asp:BoundField DataField="CardNo" HeaderText="CardNo" />
                    <asp:BoundField DataField="CardName" HeaderText="CardName" />
                    <asp:BoundField DataField="ExpDate" HeaderText="ExpDate" />
                    <asp:BoundField DataField="Cvv" HeaderText="Cvv" />
                    <asp:BoundField DataField="Email" HeaderText="Email" />
                    <asp:BoundField DataField="Dob" DataFormatString="{0:d}" HeaderText="Date of Birth" />
                    <asp:BoundField DataField="PasswordHash" HeaderText="Hash" />
                    <asp:BoundField DataField="PasswordSalt" HeaderText="Salt" />
                    <asp:BoundField DataField="IV" HeaderText="IV" />
                    <asp:BoundField DataField="Key" HeaderText="Key" />
                </Columns>
            </asp:GridView>
        </div>
        <asp:Button ID="Btn_ChgPwd" runat="server" OnClick="Btn_Change_Password" style="height: 29px" Text="Change Password" />
        <asp:Button ID="Btn_Logout" runat="server" OnClick="Btn_Logout_Click" style="height: 29px" Text="Logout" />
    </form>
</body>
</html>
