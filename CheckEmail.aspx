<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CheckEmail.aspx.cs" Inherits="IT2163_Assignment1_202578M.CheckEmail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script runat="server">
        void EmailValidation(object source, ServerValidateEventArgs args)
        {
            try
            {

                if (Regex.IsMatch(args.Value, @"^(\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$)"))
                {
                    args.IsValid = true;
                }
                else
                {
                    args.IsValid = false;
                }

            }

            catch (Exception ex)
            {
                args.IsValid = false;
            }
        }
    </script>

    <div class="card">

        <h5 class="card-header text-center">Forget Password</h5>

        <div class="card-body">
            <h5 class="card-title">
                <asp:Label runat="server" ID="title">Hello! If you have forgotten your password, you can change it here!</asp:Label></h5>
            <p class="card-text">
            </p>

            <div class="form-group">
                <label>
                    <asp:Label ID="EmailLabel" runat="server" Text="Email"></asp:Label></label>
                <div class="input-group mb-3">
                    <asp:TextBox ID="EmailTB" type="email" class="form-control" runat="server"></asp:TextBox>
                    <asp:CustomValidator ID="EmailValidator"
                        ControlToValidate="EmailTB"
                        Display="Dynamic"
                        ErrorMessage="Enter a correct Email!"
                        ForeColor="red"
                        Font-Names="verdana"
                        Font-Size="10pt"
                        OnServerValidate="EmailValidation"
                        runat="server" />
                </div>
            </div>
            <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response" />
            <label>
                <asp:Label ID="ErrorMessage" runat="server" Visible="false"></asp:Label></label>
            <asp:Button class="btn btn-warning float-right" type="submit" OnClick="checkEmail" runat="server" Text="Change" ID="Button1" />
        </div>
    </div>

</asp:Content>
