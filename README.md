# Wiki-Up

A bulk file uploader for [MediaWiki](https://www.mediawiki.org/wiki/MediaWiki) wikis.

Allows any number of files to be selected and uploaded to a wiki.

It has only been tested on a vanilla install of MediaWiki 1.32.1, [FANDOM](https://www.fandom.com/)
and [Gamepedia](https://www.gamepedia.com/) wikis, but should work with many others

Initially written for use on the fandom [Magic Arena](https://magicarena.fandom.com) wiki.

The current release is a **beta** release.

### Installation

* Click on the releases link above.
* Click on the latest release. 
* Download the zip file.
* Extract the contents of the zip to a folder of your choice.
* Run WikiUp.exe to start it up.

### Logging In

On startup you will be asked to log into a wiki. You should specify the wiki url without 
the <code>https://</code> part (e.g. mywiki.fandom.com).

If you need to login to a wiki that does not use https then
you should precede the wiki url with <code>http://</code>, be warned however that logging into a site via
http will result in your username and password being transmitted in unencrypted plain text.

If the wiki uses bot passwords, as [Gamepedia](https://help.gamepedia.com/Logging_in_to_third-party_tools) does,
then you must use the *alternate format* of the bot password i.e. your username and a password of BotUsername@password.

### Uploading Files

Click on the "Add Files" button to add files to the upload list.

To remove files from the upload list, select files the files to remove an click the "Remove Files"
button or press the *Delete* key. You can select multiple files to remove using the *shift* and *control*
keys.

You can save and load upload lists using the "Save" and "Load" buttons.

When you are ready to upload clock the "Upload" button.

### Upload Summary and Initial Page Content

You can specify an upload summary and the initial wikitext content for all the uploaded
files by clicking on the "Content" tab at the top of the window.

Note that any content supplied will only apply to **new** images/files, if you are overwriting
an existing image/file the contents of the file page will be left untouched.

You can add categories for the uploaded image by specifying the <code>[[Category:My Category]]</code> wikitext
in the content.

### Required Bot Privileges

If you are using a bot password the bot account should be assigned the following privileges
* Basic Rights
* Edit existing pages
* Create, edit, and move pages
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

### System Requirements

* Windows 7 or later
* .NET Framework 4.7.2

### It looks like this

![Wiki-Up](https://aspallar.github.com/Images/Upload.PNG)


