<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="2FAPage.aspx.cs" Inherits="IT2163_Assignment1_202578M._2FAPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

        <div class="card mx-auto" style="width: 30rem;">
        <div class="card-body">
            <h5 class="card-title text-center">Login</h5>
            <div class="mb-3">
                <label class="form-label">
                    <asp:Label ID="verifyLabel" runat="server" Text="Verification Code"></asp:Label></label>
                <asp:TextBox class="form-control" runat="server" ID="codeTB" required></asp:TextBox>

            </div>
            <asp:Label runat="server" ID="lblMessage"></asp:Label>
            <asp:Button class="btn btn-primary float-right" type="submit" OnClick="VerifyLogin" runat="server" Text="Verify" />
        </div>
    </div>
</asp:Content>
