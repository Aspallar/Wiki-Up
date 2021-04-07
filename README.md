# Wiki-Up

A bulk file uploader for [MediaWiki](https://www.mediawiki.org/wiki/MediaWiki) wikis.

Allows any number of files to be selected and uploaded to a wiki.

It has only been tested on a vanilla install of MediaWiki 1.32.1, [FANDOM](https://www.fandom.com/)
and [Gamepedia](https://www.gamepedia.com/) wikis, but should work with many others

Initially written for use on the fandom [Magic Arena](https://magicarena.fandom.com) wiki.

### Installation

* Go to [releases](https://github.com/Aspallar/Wiki-Up/releases).
* Click on the latest release.
* in the assets section download the **WikiUpInstaller.msi** file for the desired language.
* Run the downloaded installer by double clicking on it or right click and choose install.

Alternatively you can install Wiki-Up manually by downloading the WikiUp zip file and extracting its contents.

### Logging In

On startup you will be asked to log into a wiki. You should specify the wiki URL without 
the <code>https://</code> part (e.g. mywiki.fandom.com).

If you need to login to a wiki that does not use https then
you should precede the wiki url with <code>http://</code>, be warned however that logging into a site via
http will result in your username and password being transmitted in unencrypted plain text.

If the wiki uses bot passwords, as [Gamepedia](https://help.gamepedia.com/Logging_in_to_third-party_tools) does,
then you must use the *alternate format* of the bot password i.e. your username and a password of BotUsername@password.

### Uploading Files

Click on the "Add Files" button to add files to the upload list. Alternatively you can drag and drop files from File Manager to the upload list.

To remove files from the upload list, select files the files to remove an click the "Remove Files" button or press the *Delete* key. You can select multiple files to remove using the *shift* and *control* keys.

You can save and load upload lists using the "Save" and "Load" buttons.

When you are ready to upload click the "Upload" button.

### Upload Summary and Initial Page Content

You can specify an upload summary/comment and the initial wikitext content for all the uploaded files by clicking on the "Content" tab at the top of the window.

Note that any content supplied will only apply to **new** images/files, if you are overwriting an existing image/file the contents of the file page will be left untouched.

Both the summary and content can contain variables.


Variable | Expands To 
-------- | ---------
<%0> | The full path of the file being uploaded
<%n> | The nth part of the files path counting from the left and starting at 1.
<%-n> | The nth part of the files path counting from the right.
<%filename> | The file name without the extension.

For example if the path is **c:\one\two\three.png** then the following expansions will be made.

* <%0> = c:\one\two\three.png
* <%1> = c:
* <%2> = one
* <%3> = two
* <%4> = three.png
* <%-1> = three.png
* <%-2> = two
* <%-3> = one
* <%-4> = c:
* <%filename%> = three


### Uploading Videos

Wiki-Ip can "Upload" videos to Fandom wikis. To add videos drag a link to them from your browser to the upload list. If you drag and drop a playlist from Youtube then all videos in the playlist will be added to the upload list. Hold down the Ctrl key while dropping to just add a single video from a Youtube playlist.


### Required Bot Privileges

If you are using a bot password the bot account should be assigned the following privileges
* Basic Rights
* Edit existing pages
* Create, edt, and move pages
* Upload new files
* Upload, replace, and move files

### Controlling Access

Wiki admins can control who can use Wiki-Up on their wiki by creating the page
<code>MediaWiki:Custom-WikiUpUsers</code>. Here's some sample content.
<code>
<pre>
$ Wiki-Up Authorized Users
$ ========================

$ This page lists the users that are authorized to use the Wiki-Up
$ bulk file uploader. If this page exists then only users listed here can
$ log in via the uploader, otherwise any autoconfirmed user can upload.

$ Users are listed one per line.
$ Lines starting with a dollar sign ($) are comments
$ Blank lines are ignored.
$ Any line that contains something that looks like a tag is ignored.

userone
usertwo
</pre>
</code>

### Attribution
Background image used on the login page created by [kjpargeter - www.freepik.com](https://www.freepik.com/vectors/background)


### System Requirements

* Windows 7 SP1 or later
* .NET Framework 4.7.2



