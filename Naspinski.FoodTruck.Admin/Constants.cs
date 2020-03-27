using System.Collections.Generic;

namespace Naspinski.FoodTruck.AdminWeb
{
    public class Constants
    {
        public static IEnumerable<LinkGroup> HomeLinks
        { 
            get
            {
                return new List<LinkGroup>()
                {
                    new LinkGroup()
                    {
                        IncludeInHeader = true,
                        IncludeInFooter = true,
                        IncludeInHome = true,
                        Link = new Link() { Text = "Menu", Controller = "Menu", Action = "Index", Glyphicon = "cutlery"},
                        Links = new List<Link>()
                        {
                            new Link() { Text = "Menu Items", CssClass = "btn btn-primary btn-lg btn-block", Controller = "Menu", Action = "Index", Glyphicon = "list"},
                            new Link() { Text = "Create Menu Item", CssClass = "btn btn-default btn-lg btn-block", Controller = "Menu", Action = "Edit", Glyphicon = "plus-sign"},
                            new Link() { Text = "Categories", CssClass = "btn btn-primary btn-lg btn-block", Controller = "Category", Action = "Index", Glyphicon = "list"},
                            new Link() { Text = "Create Category", CssClass = "btn btn-default btn-lg btn-block", Controller = "Category", Action = "Edit", Glyphicon = "plus-sign"},
                            new Link() { Text = "Price Types", CssClass = "btn btn-primary btn-lg btn-block", Controller = "PriceType", Action = "Index", Glyphicon = "list"},
                            new Link() { Text = "Create Type", CssClass = "btn btn-default btn-lg btn-block", Controller = "PriceType", Action = "Edit", Glyphicon = "plus-sign"}
                        }
                    },
                    new LinkGroup()
                    {
                        IncludeInHeader = true,
                        IncludeInFooter = true,
                        IncludeInHome = true,
                        Link = new Link() { Text = "Calendar", Controller = "Event", Action = "Index", Glyphicon = "calendar"},
                        Links = new List<Link>()
                        {
                            new Link() { Text = "Specials", CssClass = "btn btn-primary btn-lg btn-block", Controller = "Special", Action = "Index", Glyphicon = "list"},
                            new Link() { Text = "Create Special", CssClass = "btn btn-default btn-lg btn-block", Controller = "Special", Action = "Edit", Glyphicon = "plus-sign"},
                            new Link() { Text = "Events", CssClass = "btn btn-primary btn-lg btn-block", Controller = "Event", Action = "Index", Glyphicon = "list"},
                            new Link() { Text = "Create Event", CssClass = "btn btn-default btn-lg btn-block", Controller = "Event", Action = "Edit", Glyphicon = "plus-sign"},
                            new Link() { Text = "Locations", CssClass = "btn btn-primary btn-lg btn-block", Controller = "Location", Action = "Index", Glyphicon = "list"},
                            new Link() { Text = "Create Location", CssClass = "btn btn-default btn-lg btn-block", Controller = "Location", Action = "Edit", Glyphicon = "plus-sign"},
                            new Link() { Text = "Orders", CssClass = "btn btn-primary btn-lg btn-block", Controller = "Order", Action = "Index", Glyphicon = "shopping-cart"}
                        }
                    },
                    new LinkGroup()
                    {
                        IncludeInHeader = false,
                        IncludeInFooter = true,
                        IncludeInHome = true,
                        Link = new Link() { Text = "Settings", Controller = "Settings", Action = "Index", Glyphicon = "cog"},
                        Links = new List<Link>()
                        {
                            new Link() { Text = "Site Settings", CssClass = "btn btn-primary btn-lg btn-block", Controller = "Settings", Action = "Index", Glyphicon = "cog"},
                            new Link() { Text = "Profile", CssClass = "btn btn-default btn-lg btn-block", Controller = "Manage", Action = "Index", Glyphicon = "user"},
                            //new Link() { Text = "Users", CssClass = "btn btn-primary btn-lg btn-block", Controller = "Users", Action = "Index", Glyphicon = "list"},
                            //new Link() { Text = "Create User", CssClass = "btn btn-default btn-lg btn-block", Controller = "Account", Action = "Register", Glyphicon = "plus-sign"}
                            new Link() { Text = "Create User", CssClass = "btn btn-primary btn-lg btn-block", Controller = "Account", Action = "Register", Glyphicon = "plus-sign"},
                            new Link() { Text = "Images", CssClass = "btn btn-default btn-lg btn-block", Controller = "Images", Action = "Index", Glyphicon = "picture"},
                            new Link() { Text = "Documents", CssClass = "btn btn-primary btn-lg btn-block", Controller = "Documents", Action = "Index", Glyphicon = "paperclip"},
                            new Link() { Text = "Delivery Limits", CssClass = "btn btn-default btn-lg btn-block", Controller = "DeliveryDestinations", Action = "Index", Glyphicon = "send"}
                        }
                    },
                    new LinkGroup()
                    {
                        IncludeInHeader = true,
                        IncludeInFooter = false,
                        IncludeInHome = false,
                        Link = new Link() { Text = "Orders", Controller = "Order", Action = "Index", Glyphicon = "shopping-cart"},
                        Links = new List<Link>()
                    }
                };
            }
        }
    }

    public class Link
    {
        public string Text;
        public string CssClass;
        public string Controller;
        public string Action;
        public string Glyphicon;
    }

    public class LinkGroup
    {
        public Link Link;
        public bool IncludeInHeader;
        public bool IncludeInFooter;
        public bool IncludeInHome;
        public IEnumerable<Link> Links;
    }
}
