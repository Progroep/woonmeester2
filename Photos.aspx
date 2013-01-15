<%@ Page Language="C#" MasterPageFile="~/Default.master" AutoEventWireup="true" CodeFile="photos.aspx.cs" Inherits="_Photos" %>
<%@ Register TagPrefix="UC" TagName="detailsTabs" Src="~/includes/detailsTabs.ascx" %>
<%@ Register TagPrefix="UC" TagName="ListingDetails" Src="~/includes/ListingDetails.ascx" %>
<%@ Register TagPrefix="UC" TagName="officeMenu" Src="~/includes/officeMenu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" Runat="Server">
<script src="/js/jquery-1.4.2.min.js" type="text/javascript"></script>
<script src="/js/galleria/src/galleria.js" type="text/javascript"></script>
<script type="text/javascript">
    Galleria.loadTheme('/js/galleria/src/themes/classic/galleria.classic.js');
    $('.images').galleria({transition: 'fade'});
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContentLeft" Runat="Server">
    <div class="listing" id="conLeft" runat="server">
        <h1 class="pageTitle"><asp:Literal ID="litPageTitle" runat="server" /></h1>
        <asp:Literal ID="Literal1" runat="server"></asp:Literal>
        <UC:detailsTabs ID="detailsTabID" runat="server" />
        <asp:Repeater ID="rptPhotos" runat="server">
        <HeaderTemplate>
            <div class="images">
        </HeaderTemplate>
        <ItemTemplate>
                <img src="/photos/big/<%# Eval("photoName")%>" alt="">
        </ItemTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
        </asp:Repeater>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphContentRight" Runat="Server">
    <div class="listing" id="conRightListing" runat="server">
        <UC:ListingDetails ID="ListingDetails" runat="server" /> 
    </div>
    <div class="office" id="conRightOffice" runat="server">
        <UC:officeMenu ID="officeMenu" runat="server" />
    </div>  
</asp:Content>