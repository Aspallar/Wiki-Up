namespace TestServer
{
    internal static class Replies
    {
        public const string UploadSuccess
            = "<upload result=\"Success\"></upload>";

        public const string LpginSuccess
            = "<login result=\"Success\" />";

        public const string LoginFail
            = "<login result=\"WrongPass\" />";

        public const string LoginNeedToken
            = "<login result=\"NeedToken\" token=\"{0}\" />";



        public const string MaxLag = @"
<error 
   code=""maxlag""
   info=""Waiting for a database server: 6 seconds lagged.""
   host=""foobar""
   lag=""6""
   type=""db"">
</error>";

        public const string UserGroups = @"
<users>
  <user>
    <groups>
      <g>autoconfirmed</g>
    </groups>
  </user>
</users>";


        public const string AuthorizedUsers = @"
<pages>
  <page>
    <revisions>

<rev xml:space=""preserve"">
a
aspallar
nbar
</rev>

    </revisions>
  </page>
</pages>";

        public const string SiteInfo = @"
<general base=""http://localhost:10202/"" scriptpath="""">
</general>
<fileextensions>
  <fe ext=""png"" />
  <fe ext=""jpg"" />
  <fe ext=""foo"" />
</fileextensions>";

        public const string NoMetaTokenSupport = @"
<warnings>
  <query xml:space=""preserve"">Unrecognized value for parameter 'meta': tokens</query>
</warnings>";

        public const string EditTokenPage = @"
<pages>
    <page edittoken=""666+\""></page>
</pages>";

        public const string LoginToken
            = "<tokens logintoken=\"{0}\" />";

        public const string EditToken
            = "<tokens csrftoken=\"666+\\\" />";

        public const string AlreadyExists = @"
<upload result=""Warning"">
    <warnings exists=""""></warnings>
</upload>";

        public const string LongErrorMessasge =@"
<error
  code=""lonng-error""
  info=""This os a long error message. Lorem ipsum dolor sit amet, consectetur adipiscing elit.Vivamus pretium neque et arcu scelerisque, vel accumsan ipsum elementum.Sed in convallis tortor.Morbi mollis nunc et felis pharetra, a pellentesque lectus volutpat.Aliquam eleifend purus purus, nec laoreet mi vestibulum. Once upon a time. And then there wrere none. The end."">
</error>";

        public const string BadToken = @"
<error
  code=""badtoken""
  info=""Invalid token."">
</error>";

        public const string MustBeLoggedIn = @"
<error
  code=""mustbeloggedin""
  info=""You must be logged in ya bam"">
</error>";

    }
}
