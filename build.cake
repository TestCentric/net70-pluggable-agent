#tool NuGet.CommandLine&version=6.0.0

// Load the recipe
#load nuget:?package=TestCentric.Cake.Recipe&version=1.0.1-dev00034
// Comment out above line and uncomment below for local tests of recipe changes
//#load ../TestCentric.Cake.Recipe/recipe/*.cake

var target = Argument("target", Argument("t", "Default"));

BuildSettings.Initialize
(
	context: Context,
	title: "Net70PluggableAgent",
	solutionFile: "net70-pluggable-agent.sln",
	unitTests: "**/*.tests.exe",
	githubOwner: "TestCentric",
	githubRepository: "net70-pluggable-agent"
);

BuildSettings.Packages.AddRange(new PluggableAgentFactory(".NetCoreApp, Version=7.0").Packages);

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Appveyor")
	.IsDependentOn("DumpSettings")
	.IsDependentOn("Build")
	.IsDependentOn("Test")
	.IsDependentOn("Package")
	.IsDependentOn("Publish")
	.IsDependentOn("CreateDraftRelease")
	.IsDependentOn("CreateProductionRelease");

Task("Default")
    .IsDependentOn("Build");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
