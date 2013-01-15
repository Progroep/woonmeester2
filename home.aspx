<%@ Page Title="" Language="C#" MasterPageFile="~/Default.master" AutoEventWireup="true" CodeFile="home.aspx.cs" Inherits="home" validateRequest="false" enableEventValidation="false"%>

<asp:Content ContentPlaceHolderID="cphHead" runat="server" ID="Content4">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="cphContentLeft" Runat="Server">
    <div class="home content">
        <div class="pages">
            <asp:Repeater ID="rptBlocks" runat="server">
                <ItemTemplate>
                    <div class="content left">
                        <div class="text-block">
                            <h1><%#Eval("pageTitle").ToString()%></h1>
                        </div>
                        <%# Eval("photoName").ToString() != "" && System.IO.File.Exists(Server.MapPath("/photos/page/small/" + Eval("photoName").ToString())) ? "<img src=\"/photos/page/small/" + Eval("photoName").ToString() + "\" alt=\"\" />" : ""%>
                        
                        <div class="text-block">
                            <p><%#Eval("pageHomeText").ToString()%></p>
                            <a href="/<%#Eval("pageURL").ToString()%>" class="button">Lees meer</a>
                        </div>

                        <div class="clear"></div>
                    </div>
                </ItemTemplate>
             
            </asp:Repeater> 
            <div class="clear"></div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphNews" Runat="Server">
    <div class="home bg">
    <div class="home content">
        <div class="left">
            <asp:Repeater ID="rptNews" runat="server">
                <ItemTemplate>
                    <div class="news">
                        <%#CommonFunctions.NewsSummary(CommonFunctions.sText(Eval("pageContent").ToString(), 300), Eval("pageTitle").ToString(), Eval("pageID").ToString(), Eval("photoName").ToString())%>
                    </div>
                </ItemTemplate>
                <SeparatorTemplate>
                    <div class="separator"></div>
                </SeparatorTemplate>         
            </asp:Repeater>     
        </div>
        
        <div class="posts">
            <asp:Repeater ID="rptBlog" runat="server">
                <HeaderTemplate>
                    <h1><a href="/blog">UIT ONZE BLOG</a></h1>
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="post">
                        <span class="date">
                            <%# String.Format("{0:dd MMMM yyyy}", Convert.ToDateTime(Eval("postPublishDate")))%>
                        </span>
                        <h2>
                            <a href="/blog/archieven/<%#Eval("postID")%>/<%#CommonFunctions.PrepTitle(Eval("postTitle").ToString())%>.aspx" class="title">
                                <%#Eval("postTitle")%>
                            </a>
                        </h2>
                        
                        <p>
                            <%#CommonFunctions.sText(Eval("postContent").ToString(), 100)%>
                            <a href="/blog/archieven/<%#Eval("postID")%>/<%#CommonFunctions.PrepTitle(Eval("postTitle").ToString())%>.aspx">Lees meer</a>
                        </p> 
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <div class="clear"></div>
    </div>
    </div>
</asp:Content>