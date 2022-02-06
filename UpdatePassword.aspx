<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="UpdatePassword.aspx.cs" Inherits="IT2163_Assignment1_202578M.UpdatePassword" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card">
        <h5 class="card-header text-center">Change Password</h5>
        <div class="card-body">
            <h5 class="card-title">
                <asp:Label runat="server" ID="title">Hello! Change your password now!</asp:Label></h5>
            <p class="card-text"></p>

            <div class="form-group" id="show_hide_password">
                <label>
                    <asp:Label ID="PasswordLabel" runat="server" Text="Password"></asp:Label></label>
                <div class="input-group mb-3">
                    <asp:TextBox ID="passwordTB" type="password" class="form-control" runat="server"></asp:TextBox>
                    <div class="input-group-append">
                        <button id="togglepassword" type="button" class="btn btn-outline-secondary"><i class="fa fa-eye-slash" aria-hidden="true"></i></button>
                    </div>
                </div>
                <small id="passwordHelpBlock" class="form-text text-muted">Your password must be 12-64 characters long, contains upper-case letters, lower-case letters numbers, and special characters
                </small>

                <asp:Label ID="LessThan12" runat="server" Text="Password Length less than 12" Style="display: none;"></asp:Label>
                <asp:Label ID="NoNumber" runat="server" Text="Password does not have a number" Style="display: none;"></asp:Label>
                <asp:Label ID="NoSpecial" runat="server" Text="Password does not have a speciacharacterer" Style="display: none;"></asp:Label>
                <asp:Label ID="NoUpper" runat="server" Text="Password does not have a upper-case character" Style="display: none;"></asp:Label>
                <asp:Label ID="NoLower" runat="server" Text="Password does not have a lower-case character" Style="display: none;"></asp:Label>

                <span id="StrengthDisp" class="badge displayBadge"></span>
            </div>
            <div class="form-group">
                <label>
                    <asp:Label ID="ConfirmLabel" runat="server" Text="Confirm Password"></asp:Label></label>
                <div class="input-group mb-3">
                    <asp:TextBox ID="ConfirmPassword" onchange="matchPassword()" type="password" class="form-control" runat="server"></asp:TextBox>
                </div>
                <asp:Label ID="ChangeSuccess" runat="server" Text="Success!" Visible="false"></asp:Label>
                <asp:Label ID="ChangeFail" runat="server" Text="" Visible="false" Style="color: red;"></asp:Label>
                <asp:Label ID="PasswordError" runat="server" Text="Password is not the same" Style="display: none; color: red;"></asp:Label>
            </div>
            <asp:Button class="btn btn-warning" type="submit" OnClick="UpdatePassword" runat="server" Text="Change" ID="Button1" />

        </div>
    </div>


</asp:Content>
