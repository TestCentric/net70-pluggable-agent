// Load the recipe
#load nuget:?package=TestCentric.Cake.Recipe&version=1.1.1
// Comment out above line and uncomment below for local tests of recipe changes
//#load ../TestCentric.Cake.Recipe/recipe/*.cake

BuildSettings.Initialize
(
	context: Context,
	title: "Net70PluggableAgent",
	solutionFile: "net70-pluggable-agent.sln",
	unitTests: "**/*.tests.exe",
	githubOwner: "TestCentric",
	githubRepository: "net70-pluggable-agent"
);

var MockAssemblyResult = new ExpectedResult("Failed")
{
	Total = 36, Passed = 23, Failed = 5, Warnings = 1, Inconclusive = 1, Skipped = 7,
	Assemblies = new ExpectedAssemblyResult[] { new ExpectedAssemblyResult("mock-assembly.dll") }
};


var AspNetCoreResult = new ExpectedResult("Passed")
{
	Total = 2, Passed = 2, Failed = 0, Warnings = 0, Inconclusive = 0, Skipped = 0,
	Assemblies = new ExpectedAssemblyResult[] { new ExpectedAssemblyResult("aspnetcore-test.dll") }
};

var WindowsFormsResult = new ExpectedResult("Passed")
{
	Total = 2, Passed = 2, Failed = 0, Warnings = 0, Inconclusive = 0, Skipped = 0,
	Assemblies = new ExpectedAssemblyResult[] {	new ExpectedAssemblyResult("windows-forms-test.dll") }
};

var PackageTests = new PackageTest[] {
	new PackageTest(
		1, "NetCore11PackageTest", "Run mock-assembly.dll targeting .NET Core 1.1",
		"tests/netcoreapp1.1/mock-assembly.dll", MockAssemblyResult),
	new PackageTest(
		1, "NetCore21PackageTest", "Run mock-assembly.dll targeting .NET Core 2.1",
		"tests/netcoreapp2.1/mock-assembly.dll", MockAssemblyResult),
	new PackageTest(
		1, "NetCore31PackageTest", "Run mock-assembly.dll targeting .NET Core 3.1",
		"tests/netcoreapp3.1/mock-assembly.dll", MockAssemblyResult),
	new PackageTest(
		1, "Net50PackageTest", "Run mock-assembly.dll targeting .NET 5.0",
		"tests/net5.0/mock-assembly.dll", MockAssemblyResult),
	new PackageTest(
		1, "Net60PackageTest", "Run mock-assembly.dll targeting .NET 6.0",
		"tests/net6.0/mock-assembly.dll", MockAssemblyResult),
	new PackageTest(
		1, "Net70PackageTest", "Run mock-assembly.dll targeting .NET 7.0",
		"tests/net7.0/mock-assembly.dll", MockAssemblyResult),
	new PackageTest(
		1, $"AspNetCore70Test", $"Run test using AspNetCore targeting .NET 7.0",
		$"tests/net7.0/aspnetcore-test.dll", AspNetCoreResult),
// Run Windows test for target framework >= 5.0 (7.0 on AppVeyor)
//if (TargetVersion >= V_6_0 || TargetVersion >= V_5_0 && !BuildSettings.IsRunningOnAppVeyor)
	new PackageTest(
		1, "Net70WindowsFormsTest", $"Run test using windows forms under .NET 7.0",
		"tests/net7.0-windows/windows-forms-test.dll", WindowsFormsResult)
};

// Content of the agent directory is the same for both packages
static readonly FilePath[] AGENT_FILES = {
	"agent/net70-agent.dll", "agent/net70-agent.pdb", "agent/net70-agent.dll.config", "agent/TestCentric.Agent.Core.dll",
	"agent/net70-agent.deps.json", $"agent/net70-agent.runtimeconfig.json",
	"agent/TestCentric.Engine.Api.dll",
	"agent/TestCentric.Metadata.dll", "agent/TestCentric.Extensibility.dll", "agent/TestCentric.InternalTrace.dll",
	"agent/TestCentric.Extensibility.Api.dll", "agent/Microsoft.Extensions.DependencyModel.dll",
	"agent/System.Text.Encodings.Web.dll", "agent/System.Text.Json.dll" };

BuildSettings.Packages.Add(new NuGetPackage(
	"TestCentric.Extension.Net70PluggableAgent",
	title: ".NET 7.0 Pluggable Agent",
	description: "TestCentric engine extension for running tests under .NET 7.0",
	tags: new [] { "testcentric", "pluggable", "agent", "net70" },
	packageContent: new PackageContent()
		.WithRootFiles("../../LICENSE.txt", "../../README.md", "../../testcentric.png")
		.WithDirectories(
			new DirectoryContent("tools").WithFiles(
				"net70-agent-launcher.dll", "net70-agent-launcher.pdb",
				"TestCentric.Extensibility.Api.dll", "Testcentric.Engine.Api.dll" ),
			new DirectoryContent("tools/agent").WithFiles( AGENT_FILES ) ),
	testRunner: new AgentRunner(BuildSettings.NuGetTestDirectory + "TestCentric.Extension.net70PluggableAgent." + BuildSettings.PackageVersion + "/tools/agent/net70-agent.dll"),
	tests: PackageTests) );
	
BuildSettings.Packages.Add(new ChocolateyPackage(
	"testcentric-extension-net70-pluggable-agent",
	title: ".NET 70 Pluggable Agent",
	description: "TestCentric engine extension for running tests under .NET 7.0",
	tags: new [] { "testcentric", "pluggable", "agent", "net70" },
	packageContent: new PackageContent()
		.WithRootFiles("../../testcentric.png")
		.WithDirectories(
			new DirectoryContent("tools").WithFiles( 
				"../../LICENSE.txt", "../../README.md", "../../VERIFICATION.txt",
				"net70-agent-launcher.dll", "net70-agent-launcher.pdb",
				"TestCentric.Extensibility.Api.dll", "TestCentric.Engine.Api.dll" ),
			new DirectoryContent("tools/agent").WithFiles( AGENT_FILES) ),
	testRunner: new AgentRunner(BuildSettings.ChocolateyTestDirectory + "testcentric-extension-net70-pluggable-agent." + BuildSettings.PackageVersion + "/tools/agent/net70-agent.dll"),
	tests: PackageTests) );

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

Build.Run();
