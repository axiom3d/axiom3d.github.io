#tool nuget:?package=Wyam&version=2.1.1
#addin nuget:?package=Cake.Wyam&version=2.1.1
Task("Build")
    .Does(() =>
    {
        Wyam();        
    });
    
Task("Preview")
    .Does(() =>
    {
        Wyam(new WyamSettings
        {
            Preview = true,
            Watch = true
        });        
    });

Task("Deploy")
    .IsDependentOn("Build")
    .Does(() => 
    {
        if(FileExists("./CNAME"))
            CopyFile("./CNAME", "output/CNAME");

        StartProcess("git", "checkout source");
        StartProcess("git", "add .");
        StartProcess("git", "commit -m \"Checking output in for subtree\"");
        StartProcess("git", "subtree split --prefix output -b master");
        //StartProcess("git", "push -f origin master:master");
        //StartProcess("git", "branch -D master");

    });
