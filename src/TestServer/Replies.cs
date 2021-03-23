using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServer
{
    internal static class Replies
    {
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

        public const string LoginToken = "<tokens logintoken=\"{0}\" />";

        public const string EditToken = "<tokens csrftoken=\"666+\\\" />";
    }
}
