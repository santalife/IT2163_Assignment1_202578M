<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Login.aspx.cs" Inherits="IT2163_Assignment1_202578M.Login" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
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
        void PasswordValidation(object source, ServerValidateEventArgs args)
        {
            try
            {
                if(args.Value.Length > 0)
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


    <div class="card mx-auto" style="width: 30rem;">
        <div class="card-body">
            <h5 class="card-title text-center">Login</h5>
            <div class="mb-3">
                <label class="form-label">
                    <asp:Label ID="EmailLabel" runat="server" Text="Email"></asp:Label></label>
                <asp:TextBox class="form-control" runat="server" ID="EmailTB" required></asp:TextBox>
                <asp:CustomValidator ID="EmailValidator"
                    ControlToValidate="EmailTB"
                    Display="Dynamic"
                    ErrorMessage="Check your Email again!"
                    ForeColor="red"
                    Font-Names="verdana"
                    Font-Size="10pt"
                    OnServerValidate="EmailValidation"
                    runat="server" />
                <%--                <label for="exampleInputEmail1" class="form-label">Email address</label>
                <input type="email" class="form-control" id="exampleInputEmail1" aria-describedby="emailHelp">--%>
                <div id="emailHelp" class="form-text">We'll never share your email with anyone else.</div>
            </div>
            <div class="form-group" id="show_hide_password">
                <label><asp:Label ID="PasswordLabel" runat="server" Text="Password"></asp:Label></label>
                <div class="input-group mb-3">
                    <asp:TextBox ID="passwordTB" type="password" class="form-control" runat="server" required></asp:TextBox>
                    <asp:CustomValidator ID="CustomValidator1"
                        ControlToValidate="passwordTB"
                        Display="Dynamic"
                        ErrorMessage="Please enter your password"
                        ForeColor="red"
                        Font-Names="verdana"
                        Font-Size="10pt"
                        OnServerValidate="PasswordValidation"
                        runat="server" />
                    <div class="input-group-append">
                        <button id="togglepassword" type="button" class="btn btn-outline-secondary"><i class="fa fa-eye-slash" aria-hidden="true"></i></button>
                    </div>
                </div>
            </div>
            <a href="CheckEmail.aspx">[Forget Password]</a>
            <br>
            <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>
            <asp:Label runat="server" ID="lblMessage"></asp:Label>
            <asp:Button class="btn btn-primary float-right" type="submit" OnClick="LoginMe" runat="server" Text="Login" />
        </div>
    </div>
        <script>
        $(document).ready(function () {
            $("#togglepassword").on('click', function (event) {
                event.preventDefault();
                if ($('#show_hide_password input').attr("type") == "text") {
                    $('#show_hide_password input').attr('type', 'password');
                    $('#show_hide_password i').addClass("fa-eye-slash");
                    $('#show_hide_password i').removeClass("fa-eye");
                } else if ($('#show_hide_password input').attr("type") == "password") {
                    $('#show_hide_password input').attr('type', 'text');
                    $('#show_hide_password i').removeClass("fa-eye-slash");
                    $('#show_hide_password i').addClass("fa-eye");
                }
            });
        });
        </script>

</asp:Content>

