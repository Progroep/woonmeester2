<%@ Page Language="C#" MasterPageFile="~/Default.master" AutoEventWireup="true" CodeFile="Overview.aspx.cs" Inherits="_Overview" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContentLeft" Runat="Server">
    <div class="separate">
        <div class="menu"></div>
        <div class="path">
            <div class="pathname">Home > <%=litPageTitle.Text%></div>
            <a href="/zoeken" class="zoek"><img src="/sites/<%=GeneralFunctions.getSiteDirectory()%>/images/but_DoorZoek.png" alt="Zoek" /></a>
        </div>
    </div>
    
    <div class="overview">
        <h1 class="pageTitle"><asp:Literal ID="litPageTitle" runat="server" /></h1>
        <asp:ListView ID="SearchListview" runat="server" 
        OnPagePropertiesChanged="SearchListview_PagePropertiesChanged" 
        OnPagePropertiesChanging="SearchListview_PagePropertiesChanging" 
        OnSorting="SearchListview_Sort">
            <LayoutTemplate>
                <div class="sortDiv">
                    Sorteer op: <asp:DropDownList ID="ddlSort" runat="server" OnSelectedIndexChanged="Sort" AutoPostBack="true">
                                    <asp:ListItem Text="Prijs aflopend" Value="ListingPrice ASC" />
                                    <asp:ListItem Text="Prijs oplopend" Value="ListingPrice DESC" />
                                    <asp:ListItem Text="Kamers aflopend" Value="ListingRooms ASC" />
                                    <asp:ListItem Text="Kamers oplopend" Value="ListingRooms DESC" />
                                    <asp:ListItem Text="Woonopp aflopend" Value="ListingM2 ASC" />
                                    <asp:ListItem Text="Woonopp oplopend" Value="ListingM2 DESC" />
                                    <asp:ListItem Text="Adres aflopend" Value="ListingAddress ASC" />
                                    <asp:ListItem Text="Adres oplopend" Value="ListingAddress DESC" />
                                    <asp:ListItem Text="Plaats aflopend" Value="CityName ASC" />
                                    <asp:ListItem Text="Plaats oplopend" Value="CityName DESC" />   
                                </asp:DropDownList>
                </div>
                <table class="results" cellpadding="0" cellspacing="0" ID="itemPlaceholderContainer" runat="server">
                    <tr ID="itemPlaceholder" runat="server"></tr>
                </table>
            </LayoutTemplate>
            <ItemTemplate>
                <tr <%# CommonFunctions.GetAltStyle(Container.DataItemIndex, "#FFFFFF", "#F4F4F4") %>>
                    <td class="primary" valign="top" style="width:175px">
                        <a href="/woning/<%# Eval("ListingID") %>/omschrijving/<%# CommonFunctions.GetAddress(Convert.ToInt32(Eval("ListingID")),true) %>.aspx">
                        <img style="width:175px;" src="<%# CommonFunctions.GetPhotoURL(Convert.ToString(Eval("photoName")), Convert.ToString(Eval("photoURL")), "medium") %>" alt="<%# CommonFunctions.GetAddress(Convert.ToInt32(Eval("ListingID")),false) %>" border="0" /></a>
                    </td>
                    <td class="primary" valign="top">
                        <a class="address" href="/woning/<%# Eval("ListingID") %>/omschrijving/<%# CommonFunctions.GetAddress(Convert.ToInt32(Eval("ListingID")),true) %>.aspx">
                            <%# CommonFunctions.GetAddress(Convert.ToInt32(Eval("ListingID")),false) %>
                        </a><span class="sold"><%# !String.IsNullOrEmpty(Eval("ListingStatus").ToString()) ? (CommonFunctions.IsSold(Convert.ToInt32(Eval("ListingStatus")))) : "" %></span><br />
                        <div style="padding-top:5px">
                            <%# CommonFunctions.sText(Eval("ListingDescription").ToString(),350)%>
                        </div>
                    </td>
                    <td class="primary" valign="top" <%# CommonFunctions.GetAltStyle(Container.DataItemIndex, "#F4F4F4",  "#ECECEC") %> style="width:130px">
                        <table cellpadding="0" cellspacing="0" style="width: 100%;">
                            <tr>
                                <td class="addressPrice" colspan="2">
                                    &euro; <%# CommonFunctions.FormatNumber(Eval("ListingPrice").ToString()) %>,-
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" class="txtcenter italic" style="padding-bottom:7px">
                                    <%# Eval("ListingPriceTypeName") %>
                                </td>
                            </tr>
                            <tr>
                                <td class="addressSmall" style="width:90px">
                                    Woonopp:&nbsp;
                                </td>
                                <td class="addressSmall">
                                    <%# CommonFunctions.ReplaceEmpty(Eval("ListingM2").ToString(),"m") %>
                                </td>
                            </tr>
                            <tr>
                                <td class="addressSmall" style="width:90px">
                                    Kamers:&nbsp;
                                </td>
                                <td class="addressSmall">
                                    <%# CommonFunctions.ReplaceEmpty(Eval("ListingRooms").ToString(),"d") %>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" class="txtcenter" style="padding-top:10px">
                                    <a href="/woning/<%# Eval("ListingID") %>/omschrijving/<%# CommonFunctions.GetAddress(Convert.ToInt32(Eval("ListingID")),true) %>.aspx">
                                        <img src="/sites/<%=GeneralFunctions.getSiteDirectory()%>/images/but_bekijk_small.png" alt="Bekijk" />
                                    </a>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </ItemTemplate>
            <EmptyDataTemplate>
                <table id="Table1" runat="server" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td>Geen woningen gevonden.</td>
                    </tr>
                </table>
            </EmptyDataTemplate>
        </asp:ListView>
        
        <asp:DataPager ID="DataPager1" PagedControlID="SearchListview" PageSize="10" runat="server">
            <Fields>
                <asp:TemplatePagerField>
                    <PagerTemplate> 
                    <hr/>           
                    </PagerTemplate>
                </asp:TemplatePagerField>
                <asp:NextPreviousPagerField 
                ButtonType="Link"
                ShowFirstPageButton="false"
                ShowPreviousPageButton="true"
                ShowLastPageButton="false" 
                ShowNextPageButton ="false" 
                PreviousPageText="<img src='/images/ico_prev.gif' border='0' alt='Vorige' />" /> 
                <asp:TemplatePagerField>
                    <PagerTemplate> 
                    &nbsp;           
                    </PagerTemplate>
                </asp:TemplatePagerField>
                <asp:NumericPagerField />
                <asp:TemplatePagerField>
                    <PagerTemplate> 
                    &nbsp;           
                    </PagerTemplate>
                </asp:TemplatePagerField>
                <asp:NextPreviousPagerField 
                ButtonType="Link"
                ShowFirstPageButton="false"
                ShowPreviousPageButton="false"
                ShowLastPageButton="false" 
                ShowNextPageButton ="true" 
                NextPageText="<img src='/images/ico_next.gif' border='0' alt='Volgende' />" />
                <asp:TemplatePagerField>
                    <PagerTemplate>
                    <b>
                    &nbsp;&nbsp;&nbsp;Woning
                    <asp:Label runat="server" ID="CurrentPageLabel" 
                    Text="<%# Container.StartRowIndex+1 %>" />
                    t/m
                    <asp:Label runat="server" ID="TotalPagesLabel" 
                    Text="<%# Container.StartRowIndex+Container.PageSize %>" />
                    van <asp:Label runat="server" ID="TotalItemsLabel" Text="<%# Container.TotalRowCount%>" />
                    </b>
                    </PagerTemplate>
                </asp:TemplatePagerField>
            </Fields>
        </asp:DataPager>
    </div> 
</asp:Content>