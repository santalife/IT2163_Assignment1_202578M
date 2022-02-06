<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Registration.aspx.cs" Inherits="IT2163_Assignment1_202578M.Registration" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <script runat="server">
        void FirstNameValidation(object source, ServerValidateEventArgs args)
        {
            try
            {

                if (Regex.IsMatch(args.Value, @"^[a-zA-Z]+$"))
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

        void LastNameValidation(object source, ServerValidateEventArgs args)
        {
            try
            {

                if (Regex.IsMatch(args.Value, @"^[a-zA-Z]+$"))
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

        void CreditCardValidation(object source, ServerValidateEventArgs args)
        {
            try
            {

                if (Regex.IsMatch(args.Value, @"^(?:(4[0-9]{12}(?:[0-9]{3})?)|(5[1-5][0-9]{14})|(6(?:011|5[0-9]{2})[0-9]{12})|(3[47][0-9]{13})|(3(?:0[0-5]|[68][0-9])[0-9]{11})|((?:2131|1800|35[0-9]{3})[0-9]{11}))$"))
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
        void AgeValidation(object source, ServerValidateEventArgs args)
        {
            try
            {
                DateTime zeroTime = new DateTime(1, 1, 1);
                DateTime now = DateTime.Now;
                DateTime input = DateTime.Parse(args.Value);
                TimeSpan age = now - input;
                int years = (zeroTime + age).Year - 1;
                if (years >= 18)
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

    </script>

    <script language="javascript"> 
        function FirstNameValidation(source, arguments) {
            if (arguments.Value.match('^[a-zA-Z]+$')) {
                arguments.IsValid = true;
            } else {
                arguments.IsValid = false;
            }
        }

        function LastNameValidation(source, arguments) {
            if (arguments.Value.match('^[a-zA-Z]+$')) {
                arguments.IsValid = true;
            } else {
                arguments.IsValid = false;
            }
        }
        function EmailValidation(source, arguments) {
            if (arguments.Value.match('^(\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$)')) {
                arguments.IsValid = true;
            } else {
                arguments.IsValid = false;
            }
        }

        function CreditCardValidation(source, arguments) {
            if (arguments.Value.match('^(?:(4[0-9]{12}(?:[0-9]{3})?)|(5[1-5][0-9]{14})|(6(?:011|5[0-9]{2})[0-9]{12})|(3[47][0-9]{13})|(3(?:0[0-5]|[68][0-9])[0-9]{11})|((?:2131|1800|35[0-9]{3})[0-9]{11}))$')) {
                arguments.IsValid = true;
            } else {
                arguments.IsValid = false;
            }
        }

        function AgeValidation(source, arguments) {
            var age = new Date(new Date() - new Date(arguments.Value)).getFullYear() - 1970
            if (age >= 18) {
                arguments.IsValid = true;
            }
            else {
                arguments.IsValid = false;
            }
        }

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

    <h3>Registration Page</h3>
    <asp:Label ID="ExtraLabel" runat="server"></asp:Label>
    <div class="form-row">
        <div class="form-group col-md-6">
            <asp:Label ID="FirstNameLabel" runat="server" Text="First Name"></asp:Label>
            <asp:TextBox class="form-control" runat="server" ID="FirstNameTB" required></asp:TextBox>
            <asp:CustomValidator ID="FirstNameValidator"
                ControlToValidate="FirstNameTB"
                Display="Dynamic"
                ErrorMessage="Letters Only!"
                ForeColor="red"
                Font-Names="verdana"
                Font-Size="10pt"
                ClientValidationFunction="FirstNameValidation"
                OnServerValidate="FirstNameValidation"
                runat="server" />
        </div>
        <div class="form-group col-md-6">
            <asp:Label ID="LastNameLabel" runat="server" Text="Last Name"></asp:Label>
            <asp:TextBox class="form-control" runat="server" ID="LastNameTB" required></asp:TextBox>
            <asp:CustomValidator ID="LastNameValidator"
                ControlToValidate="LastNameTB"
                Display="Dynamic"
                ErrorMessage="Letters Only!"
                ForeColor="red"
                Font-Names="verdana"
                Font-Size="10pt"
                ClientValidationFunction="LastNameValidation"
                OnServerValidate="LastNameValidation"
                runat="server" />
        </div>
    </div>
    <div class="form-row">

        <div class="form-group col-md-9">
            <asp:Label ID="EmailLabel" runat="server" Text="Email Address"></asp:Label>
            <asp:TextBox type="email" class="form-control" ID="EmailTB" runat="server" required></asp:TextBox>
            <asp:Label ID="emailError" runat="server" ForeColor="Red" Visible="false"></asp:Label>
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
        <div class="form-group col-md-3">
            <asp:Label ID="DOBLabel" runat="server" Text="Date of Birth"></asp:Label>
            <asp:TextBox type="date" class="form-control" ID="DobTB" runat="server" required></asp:TextBox>
            <asp:CustomValidator ID="AgeValidator"
                ControlToValidate="DobTB"
                Display="Dynamic"
                ErrorMessage="You are not able to register if you are not 18 and above!"
                ForeColor="red"
                Font-Names="verdana"
                Font-Size="10pt"
                ClientValidationFunction="AgeValidation"
                OnServerValidate="AgeValidation"
                runat="server" />
        </div>

    </div>

    <div class="form-group">
        <asp:Label ID="CreditLabel" runat="server" Text="Credit Card Number"></asp:Label>
        <asp:TextBox class="form-control" runat="server" ID="CreditCardTB" required></asp:TextBox>
        <asp:CustomValidator ID="CreditCardValidator"
            ControlToValidate="CreditCardTB"
            Display="Dynamic"
            ErrorMessage="Enter a correct Credit Card!"
            ForeColor="red"
            Font-Names="verdana"
            Font-Size="10pt"
            ClientValidationFunction="CreditCardValidation"
            OnServerValidate="CreditCardValidation"
            runat="server" />
    </div>

    <div class="form-group" id="show_hide_password">
        <asp:Label ID="Label1" runat="server" Text="Password"></asp:Label>
        <div class="input-group mb-3">
            <asp:TextBox ID="passwordTB" type="password" class="form-control" runat="server" required></asp:TextBox>
            <div class="input-group-append">
                <button id="togglepassword" type="button" class="btn btn-outline-secondary"><i class="fa fa-eye-slash" aria-hidden="true"></i></button>
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
        </div>
        <small id="passwordHelpBlock" class="form-text text-muted">Your password must be 12-64 characters long, contains upper-case letters, lower-case letters numbers, and special characters
        </small>

        <asp:Label ID="LessThan12" runat="server" Text="Password Length less than 12" Style="display: none;"></asp:Label>
        <asp:Label ID="NoNumber" runat="server" Text="Password does not have a number" Style="display: none;"></asp:Label>
        <asp:Label ID="NoSpecial" runat="server" Text="Password does not have a special characterer" Style="display: none;"></asp:Label>
        <asp:Label ID="NoUpper" runat="server" Text="Password does not have a upper-case character" Style="display: none;"></asp:Label>
        <asp:Label ID="NoLower" runat="server" Text="Password does not have a lower-case character" Style="display: none;"></asp:Label>

        <span id="StrengthDisp" class="badge displayBadge"></span>
    </div>

    <div class="form-group">
    </div>
    <div class="form-group">
        <label for="exampleFormControlFile1">Upload your Photo</label>
        <asp:FileUpload ID="FileUpload1" class="form-control-file" runat="server" />
        <asp:RegularExpressionValidator ID="RegularExpressionValidator7"  
                    runat="server" ControlToValidate="FileUpload1"  
                    ErrorMessage="Only .jpg, .jpeg, .png, .git, .bitmap Image formats are allowed." ForeColor="Red"
                    Font-Names="verdana"
                    Font-Size="10pt"
                    ValidationExpression="/^(([a-zA-Z]:)|(\\{2}\w+)\$?)(\\(\w[\w].*))+(.jpeg|.JPEG|.gif|.GIF|.png|.PNG|.JPG|.jpg|.bitmap|.BITMAP)$/"  
                    ValidationGroup="PartnerProfileUpdate" SetFocusOnError="true"></asp:RegularExpressionValidator> 
    </div>

    <%--    <div class="form-group">
        <div class="form-check">
            <input class="form-check-input" type="checkbox" value="" id="invalidCheck" required>
            <label class="form-check-label" for="invalidCheck">
                Agree to terms and conditions
            </label>
            <div class="invalid-feedback">
                You must agree before submitting.
            </div>

        </div>
    </div>--%>
    <asp:Button class="btn btn-primary" type="submit" OnClick="OnSubmitForm" runat="server" Text="Register" />


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
    <script>


        function AppendErrors(PasswordParameter) {

            if (PasswordParameter.length < 12) {
                document.getElementById('<%=LessThan12.ClientID %>').style.display = "block"
            }

            if (PasswordParameter.search(/^(?=.*[0-9])/)) {
                document.getElementById('<%=NoNumber.ClientID %>').style.display = "block"
            }


            if (PasswordParameter.search(/^(?=.*[A-Za-z0-9])/)) {
                document.getElementById('<%=NoSpecial.ClientID %>').style.display = "block"
            }


            if (PasswordParameter.search(/^(?=.*[a-z])/)) {
                document.getElementById('<%=NoLower.ClientID %>').style.display = "block"
            }


            if (PasswordParameter.search(/^(?=.*[A-Z])/)) {
                document.getElementById('<%=NoUpper.ClientID %>').style.display = "block"
            }

        }

        function EmailValidate(source, arguments) {
            if (/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(arguments.Value)) {
                arguments.IsValid = true;
            }
            else {
                arguments.IsValid = false;
            }
        }
    </script>

</asp:Content>

