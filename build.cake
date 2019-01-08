#tool nuget:?package=Wyam&version=2.1.1
#addin nuget:?package=Cake.Wyam&version=2.1.1

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var outputDir = Directory("./output");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
    {
        if(DirectoryExists(outputDir))
        {
            DeleteDirectory(outputDir, new DeleteDirectorySettings
            {   
                Force = true,
                Recursive = true
            });
        }
    });

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

Task("Publish")
    .IsDependentOn("Clean")
    .IsDependentOn("Build")
    .Does(() => 
    {
        StartProcess("git", "add .output -f");
        StartProcess("git", "commit -m \"Site Deployment\"");
        StartProcess("git", "subtree split --prefix output -b master");
        StartProcess("git", "push -f origin master:master");
        StartProcess("git", "branch -D master");
        StartProcess("git", "reset HEAD^");
    });

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build");
    
Task("AppVeyor")
    .IsDependentOn("Build")
    .IsDependentOn("Publish");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);