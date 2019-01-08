-----------------------
IMPORTANT NOTICE ABOUT Reinforced.Typings.settings.xml FILE
-----------------------

Hi!
Congratulations. You have just installed Reinforced.Typings.

If you see that Reinforced.Typings.settings.xml file is included 
into your project - please close this file and continue working. Everything if fine.

Text below is for .NET Core users who dont get Reinforced.Typings.settings.xml 
automatically copied into root poject folder. Yes, you must see 
Reinforced.Typings.settings.xml in the root of your project along 
with other files. if you don't - please, continue reading.

According to new NuGet's restrictions regarding content files in packages, 
unfortunately I am not able to automatically add Reinfroced.Typings.settings.xml 
to your project. So please do that by yourself. Trust me - it is pretty simple. 
And please forgive NuGet for such a strange design. You can read discussions 
about why they followed that way and lots more stuff about transient content 
restore, or adding specific NuGet command for content, etc. here: 
https://github.com/NuGet/Home/wiki/Bringing-back-content-support,-September-24th,-2015

So now please create empty Reinforced.Typings.settings.xml in the root 
folder of your project and put the following text there:

