namespace HTMLxUnitGenerator.Templates
{
    public class SideNav
    {
        public static string Link
        {
            get
            {
                return @"<li>
                            <a href=""./@(Model.FileName).html"" class=""waves-effect waves-light"">
                                <i class=""material-icons"">assignment</i><span>@Model.FileName</span></a>
		                </li>".Replace("\r\n", "").Replace("\t", "").Replace("    ", ""); ;
            }
        }

        public static string IndexLink
        {
            get
            {
                return @"<li>
                            <a href=""./Index.html"" class=""waves-effect waves-light executive-summary"">
                                <i class=""material-icons"">assignment</i><span>Index</span></a>
		                </li>".Replace("\r\n", "").Replace("\t", "").Replace("    ", ""); ;
            }
        }
    }
}
