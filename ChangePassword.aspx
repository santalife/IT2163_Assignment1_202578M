<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="IT2163_Assignment1_202578M.ChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script runat="server">
        void PasswordValidation(object source, ServerValidateEventArgs args)
        {
            try
            {
                int score = 0;
                if (args.Value.Length < 12)
                {

                }
                else
                {
                    score = 1;
                }

                if (Regex.IsMatch(args.Value, "[a-z]"))
                {
                    score++;
                }


                if (Regex.IsMatch(args.Value, "[A-Z]"))
                {
                    score++;
                }

                if (Regex.IsMatch(args.Value, "[0-9]"))
                {
                    score++;
                }

                if (Regex.IsMatch(args.Value, "[^A-Za-z0-9]"))
                {
                    score++;
                }

                if (score != 5)
                {
                    args.IsValid = false;
                }
                else
                {
                    args.IsValid = true;
                }
            }
            catch (Exception ex)
            {
                args.IsValid = false;
            }
        }

        void ConfirmPasswordValidation(object source, ServerValidateEventArgs args)
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

    <script>
        function PasswordValidation(source, arguments) {
            var score = 0
            if (arguments.Value.length < 12) {

            }
            else {
                score = 1
            }
            if (arguments.Value.match('[a-z]')) {
                score++;
            }
            if (arguments.Value.match('[A-Z]')) {
                score++;
            }
            if (arguments.Value.match('[0-9]')) {
                score++;
            }
            if (arguments.Value.match('[^A-Za-z0-9]')) {
                score++;
            }

            if (score != 5) {
                arguments.IsValid = false

            }
            else {
                arguments.IsValid = true
            }
        }
    </script>

    <style>
        .displayBadge {
            display: none;
            text-align: center;
        }
    </style>
    <div class="card">

        <h5 class="card-header text-center">Change Password</h5>

        <div class="card-body">
            <h5 class="card-title">
                <asp:Label runat="server" ID="title">Hello! Change your password now!</asp:Label></h5>
            <p class="card-text"> 
            </p>

            <div class="form-group" id="show_hide_password">
                <label>
                    <asp:Label ID="PasswordLabel" runat="server" Text="Password"></asp:Label></label>
                <div class="input-group mb-3">
                    <asp:TextBox ID="passwordTB" type="password" class="form-control" runat="server"></asp:TextBox>
                    <div class="input-group-append">
                        <button id="togglepassword" type="button" class="btn btn-outline-secondary"><i class="fa fa-eye-slash" aria-hidden="true"></i></button>
                    </div>
                </div>
                <asp:CustomValidator ID="PasswordValidator"
                        ControlToValidate="passwordTB"
                        Display="Dynamic"
                        ErrorMessage="Please ensure that you follow all of the password policy and achieve a Very Good on the meter"
                        ForeColor="red"
                        Font-Names="verdana"
                        Font-Size="10pt"
                        ClientValidationFunction="PasswordValidation"
                        OnServerValidate="PasswordValidation"
                        runat="server" />
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
                    <div class="input-group-append">
                </div>
            </div>
            <asp:Label ID="ChangeSuccess" runat="server" Text="Success!" visible="false"></asp:Label>
            <asp:Label ID="ChangeFail" runat="server" Text="" visible="false" Style="color:red;"></asp:Label>
            <asp:Label ID="PasswordError" runat="server" Text="Password is not the same" Style="display: none; color:red;"></asp:Label>
            
            <asp:Button class="btn btn-warning" type="submit" OnClick="UpdatePassword" runat="server" Text="Change" ID="Button1" />
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

    <script type="text/javascript">

        let timeout;
        let password = document.getElementById('<%=passwordTB.ClientID %>');
        let strengthBadge = document.getElementById('StrengthDisp');


        function StrengthChecker(PasswordParameter) {

            let score = ScoreChecker(PasswordParameter)

            if (score == 1) {
                strengthBadge.style.backgroundColor = 'red';
                strengthBadge.textContent = 'Very Weak'

            }
            else if (score == 2) {
                strengthBadge.style.backgroundColor = 'red';
                strengthBadge.textContent = 'Weak';
            }
            else if (score == 3) {
                strengthBadge.style.backgroundColor = 'orange';
                strengthBadge.textContent = 'Medium';
            }
            else if (score == 4) {
                strengthBadge.style.backgroundColor = 'green';
                strengthBadge.textContent = 'Strong';
            }
            else if (score == 5) {
                strengthBadge.style.backgroundColor = "green";
                strengthBadge.textContent = 'Very Strong';
            }

        }

        function ScoreChecker(PasswordParameter) {
            var score = 0;
            if (PasswordParameter.length < 12) {
                score = score
            }
            else {
                score += 1
            }

            if (PasswordParameter.search(/^(?=.*[0-9])/)) {
                score = score
            }
            else {
                score += 1
            }

            if (PasswordParameter.search(/^(?=.*[^a-zA-Z0-9])/)) {
                score = score
            }
            else {
                score += 1
            }

            if (PasswordParameter.search(/^(?=.*[a-z])/)) {
                score = score
            }
            else {
                score += 1
            }

            if (PasswordParameter.search(/^(?=.*[A-Z])/)) {
                score = score
            }
            else {
                score += 1
            }
            return score
        }

        password.addEventListener("input", () => {
            strengthBadge.style.display = 'block';
            clearTimeout(timeout);
            timeout = setTimeout(() => StrengthChecker(password.value), 150);
            if (password.value.length !== 0) {
                strengthBadge.style.display != 'block';
            } else {
                strengthBadge.style.display = 'none';
            }
        });

    </script>

    <script>
        function matchPassword() {
            var pw1 = document.getElementById("<%=passwordTB.ClientID %>");
            var pw2 = document.getElementById("<%=ConfirmPassword.ClientID %>");
            var label = document.getElementById("<%=PasswordError.ClientID %>");
            if (pw1.value != pw2.value) {
                label.style.display = 'block';
            }
            else {
                label.style.display = 'none';
            }
        }
    </script>

</asp:Content>
