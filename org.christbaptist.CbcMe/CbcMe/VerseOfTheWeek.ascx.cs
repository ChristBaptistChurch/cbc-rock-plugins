using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

[DisplayName("Verse of the Week")]
[Category("org_ChristBaptist > Initiatives")]
[Description("The verse of the week quizzer for Christ Baptist Church")]

public partial class Plugins_org_christbaptist_VerseOfTheWeek : Rock.Web.UI.RockBlock
{
    protected void Page_Load(object sender, EventArgs e)
    {
        RockPage.AddCSSLink(ResolveUrl("~/Plugins/org_christbaptist/CbcMe/verseoftheweek.css"));
    }
}